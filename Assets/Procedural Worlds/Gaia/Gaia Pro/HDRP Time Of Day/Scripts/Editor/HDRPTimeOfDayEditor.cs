#if HDPipeline && UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    [CustomEditor(typeof(HDRPTimeOfDay))]
    public class HDRPTimeOfDayEditor : Editor
    {
        private HDRPTimeOfDay m_tod;
        private GUIStyle m_boxStyle;

        private void OnEnable()
        {
            m_tod = (HDRPTimeOfDay) target;
            if (m_tod != null)
            {
                m_tod.SetHasBeenSetup(m_tod.SetupHDRPTimeOfDay());
            }
        }
        public override void OnInspectorGUI()
        {
            if (m_tod == null)
            {
                m_tod = (HDRPTimeOfDay) target;
            }

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = {textColor = GUI.skin.label.normal.textColor},
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
            }

            EditorGUILayout.HelpBox("This version of HDRP Time Of Day is in BETA, improvements and more settings will be added in the future.", MessageType.Info);

            EditorGUILayout.BeginVertical(m_boxStyle);
            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            m_tod.Player = (Transform)EditorGUILayout.ObjectField("Player/Camera", m_tod.Player, typeof(Transform), true);
            bool enableSync = m_tod.m_enableReflectionProbeSync;
            enableSync = EditorGUILayout.Toggle("Reflection Probe Synchronization", enableSync);
            if (enableSync != m_tod.m_enableReflectionProbeSync)
            {
                m_tod.m_enableReflectionProbeSync = enableSync;
                if (enableSync)
                {
                    if (EditorUtility.DisplayDialog("Probe Sync Enabled",
                        "You have enabled 'Reflection Probe Synchronization' this will refresh any probes that have the script attached to the probes in your scene. Would you like to add the script to all your probes in your scene?",
                        "Yes", "No")) 
                    {
                        ReflectionProbe[] probes = GameObject.FindObjectsOfType<ReflectionProbe>();
                        if (probes.Length > 0)
                        {
                            foreach (ReflectionProbe probe in probes)
                            {
                                if (probe.GetComponent<HDRPTimeOfDayReflectionProbeSync>() == null)
                                {
                                    HDRPTimeOfDayReflectionProbeSync system = probe.gameObject.AddComponent<HDRPTimeOfDayReflectionProbeSync>();
                                    system.Setup();
                                }
                            }
                        }
                        m_tod.UpdateProbeSyncTime(m_tod.ReflectionProbeSyncTime);
                        EditorGUIUtility.ExitGUI();
                    }
                    else
                    {
                        EditorGUIUtility.ExitGUI();
                    }
                }
            }
            if (enableSync)
            {
                EditorGUI.indentLevel++;
                m_tod.ReflectionProbeSyncTime = EditorGUILayout.Slider("Reflection Probe Sync Time", m_tod.ReflectionProbeSyncTime, 0.1f, 20f);
                if (GUILayout.Button("Manual Refresh"))
                {
                    m_tod.RefreshReflectionProbes();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Please note that this will sync probes up every time the time of day or direction value has changed and can be very expensive feature. In the future this will be far more optimized when the 'HDRP Time Of Day' system exits the BETA.", MessageType.Warning);
            }
            m_tod.UseOverrideVolumes = EditorGUILayout.Toggle("Use Override Volumes", m_tod.UseOverrideVolumes);
            if (m_tod.UseOverrideVolumes)
            {
                EditorGUILayout.HelpBox("Override volumes are in ALPHA and you may experience some issues. If you are experiencing issues that causes to many bugs with this system for your project just disable 'Use Override Volumes' to disable this feature.", MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_tod);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(m_boxStyle);
            m_tod.TimeOfDayProfile = (HDRPTimeOfDayProfile)EditorGUILayout.ObjectField("Time Of Day Profile", m_tod.TimeOfDayProfile, typeof(HDRPTimeOfDayProfile), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_tod);
            }
            if (m_tod.TimeOfDayProfile != null)
            {
                EditorGUI.BeginChangeCheck();
                m_tod.TimeOfDay = EditorGUILayout.Slider("Time Of Day", m_tod.TimeOfDay, 0f, 24f);
                string direction = "N";
                if (m_tod.DirectionY < 45)
                {
                    direction = "N";
                }
                else if (m_tod.DirectionY >= 45 && m_tod.DirectionY <= 90)
                {
                    direction = "NE";
                }
                else if (m_tod.DirectionY >= 90 && m_tod.DirectionY <= 135)
                {
                    direction = "E";
                }
                else if (m_tod.DirectionY >= 135 && m_tod.DirectionY <= 180)
                {
                    direction = "SE";
                }
                else if (m_tod.DirectionY >= 180 && m_tod.DirectionY <= 225)
                {
                    direction = "S";
                }
                else if (m_tod.DirectionY >= 225 && m_tod.DirectionY <= 270)
                {
                    direction = "SW";
                }
                else if (m_tod.DirectionY >= 270 && m_tod.DirectionY <= 315)
                {
                    direction = "W";
                }
                else
                {
                    direction = "NW";
                }
                m_tod.DirectionY = EditorGUILayout.Slider("Direction (" + direction + ")", m_tod.DirectionY, 0f, 360f);
                m_tod.m_enableTimeOfDaySystem = EditorGUILayout.Toggle("Auto Update", m_tod.m_enableTimeOfDaySystem);
                if (m_tod.m_enableTimeOfDaySystem)
                {
                    EditorGUI.indentLevel++;
                    m_tod.m_timeOfDayMultiplier = EditorGUILayout.FloatField("Time Of Day Multiplier", m_tod.m_timeOfDayMultiplier);
                    EditorGUI.indentLevel--;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_tod);
                }

                EditorGUI.BeginChangeCheck();

                //Advanced Lighting
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_advancedLightingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_advancedLightingSettings, "Advanced Lighting Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_advancedLightingSettings)
                {
                    AdvancedLightingSettings();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Cloud Settings
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudSettings, "Cloud Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudSettings)
                {
                    GlobalCloudType cloudType = m_tod.TimeOfDayProfile.TimeOfDayData.m_globalCloudType;
                    cloudType = (GlobalCloudType)EditorGUILayout.EnumPopup("Global Cloud Type", cloudType);
                    if (cloudType != m_tod.TimeOfDayProfile.TimeOfDayData.m_globalCloudType)
                    {
                        m_tod.TimeOfDayProfile.TimeOfDayData.m_globalCloudType = cloudType;
                        m_tod.SetupVisualEnvironment();
                    }
                    switch (m_tod.TimeOfDayProfile.TimeOfDayData.m_globalCloudType)
                    {
                        case GlobalCloudType.Volumetric:
                        {
                            VolumetricCloudSettings();
                            break;
                        }
                        case GlobalCloudType.Procedural:
                        {

                            ProceduralCloudSettings();
                            break;
                        }
                        case GlobalCloudType.Both:
                        {
                            VolumetricCloudSettings();
                            ProceduralCloudSettings();
                            break;
                        }
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Duration Settings
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_durationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_durationSettings, "Duration Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_durationSettings)
                {
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_dayDuration = EditorGUILayout.FloatField("Day Duration", m_tod.TimeOfDayProfile.TimeOfDayData.m_dayDuration);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_nightDuration = EditorGUILayout.FloatField("Night Duration", m_tod.TimeOfDayProfile.TimeOfDayData.m_nightDuration);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Fog Settings
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_fogSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_fogSettings, "Fog Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_fogSettings)
                {
                    FogSettings();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Sun Settings
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_sunSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_sunSettings, "Sun/Moon Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_sunSettings)
                {
                    SunSettings();
                    LensFlareSettings();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Sky Settings
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_skySettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.TimeOfDayData.m_skySettings, "Sky Settings");
                if (m_tod.TimeOfDayProfile.TimeOfDayData.m_skySettings)
                {
                    SkySettings();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                //Ray Tracing
                EditorGUILayout.BeginVertical(m_boxStyle);
                m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSettings, "Ray Tracing Settings");
                if (m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSettings)
                {
                    RayTracingSettings();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_tod.TimeOfDayProfile);
                    EditorUtility.SetDirty(m_tod);
                    m_tod.ProcessTimeOfDay();
                }
            }
            EditorGUILayout.EndVertical();

            //Post FX
            EditorGUILayout.BeginVertical(m_boxStyle);
            EditorGUI.BeginChangeCheck();
            m_tod.UsePostFX = EditorGUILayout.Toggle("Use Post Processing", m_tod.UsePostFX);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_tod);
            }
            if (m_tod.UsePostFX)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                HDRPTimeOfDayPostFXProfile postFXProfile = m_tod.TimeOfDayPostFxProfile;
                postFXProfile = (HDRPTimeOfDayPostFXProfile)EditorGUILayout.ObjectField("Post Processing Profile", postFXProfile, typeof(HDRPTimeOfDayPostFXProfile), false);
                if (postFXProfile != m_tod.TimeOfDayPostFxProfile)
                {
                    m_tod.TimeOfDayPostFxProfile = postFXProfile;
                    m_tod.SetHasBeenSetup(m_tod.SetupHDRPTimeOfDay());
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_tod);
                }
                EditorGUI.indentLevel--;
                if (m_tod.TimeOfDayPostFxProfile != null)
                {
                    EditorGUI.BeginChangeCheck();
                    //Ambient Occlusion Settings
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientOcclusionSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientOcclusionSettings, "Ambient Occlusion Settings");
                    if (m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientOcclusionSettings)
                    {
                        AmbientOcclusionSettings();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    //Bloom Settings
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomSettings, "Bloom Settings");
                    if (m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomSettings)
                    {
                        BloomSettings();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    //Color Grading Settings
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_colorGradingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_colorGradingSettings, "Color Grading Settings");
                    if (m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_colorGradingSettings)
                    {
                        ColorGradingSettings();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    //Shadow Toning Settings
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadowToningSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadowToningSettings, "Shadow Toning Settings");
                    if (m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadowToningSettings)
                    {
                        ShadowToningSettings();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    //Vignette Settings
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteSettings, "Vignette Settings");
                    if (m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteSettings)
                    {
                        VignetteSettings();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(m_tod.TimeOfDayPostFxProfile);
                        m_tod.ProcessTimeOfDay();
                    }
                }
            }
            EditorGUILayout.EndVertical();

            //Weather
            EditorGUILayout.BeginVertical(m_boxStyle);
            m_tod.m_showWeatherSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.m_showWeatherSettings, "Weather Settings");
            if (m_tod.m_showWeatherSettings)
            {
                WeatherSettings();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(m_boxStyle);
            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
            m_tod.DebugSettings.m_showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(m_tod.DebugSettings.m_showSettings, "Debug Settings");
            if (m_tod.DebugSettings.m_showSettings)
            {
                DebugSettings();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Debugging
        /// </summary>
        private void DebugSettings()
        {
            EditorGUI.BeginChangeCheck();
            bool roundUp = m_tod.DebugSettings.m_roundUp;
            roundUp = EditorGUILayout.Toggle("Round Up", roundUp);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_tod, "Changed Debug Settings");
                m_tod.DebugSettings.m_roundUp = roundUp;
                EditorUtility.SetDirty(m_tod);
            }
            if (GUILayout.Button("Fetch Debug Information"))
            {
                m_tod.GetDebugInformation();
            }
        }
        /// <summary>
        /// Lighting
        /// </summary>
        private void AdvancedLightingSettings()
        {
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSGI = EditorGUILayout.Toggle("Use SSGI", m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSGI);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSGI)
            {
                EditorGUI.indentLevel++;
                m_tod.TimeOfDayProfile.TimeOfDayData.m_ssgiQuality = (GeneralQuality)EditorGUILayout.EnumPopup("SSGI Quality", m_tod.TimeOfDayProfile.TimeOfDayData.m_ssgiQuality);
                EditorGUILayout.HelpBox("SSGI work best when you use reflection probes in your scene so make sure you setup some reflection probes in your scene.", MessageType.Info);
                EditorGUI.indentLevel--;
            }
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSR = EditorGUILayout.Toggle("Use SSR", m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSR);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_useSSR)
            {
                EditorGUI.indentLevel++;
                m_tod.TimeOfDayProfile.TimeOfDayData.m_ssrQuality = (GeneralQuality)EditorGUILayout.EnumPopup("SSR Quality", m_tod.TimeOfDayProfile.TimeOfDayData.m_ssrQuality);
                EditorGUI.indentLevel--;
            }
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useContactShadows = EditorGUILayout.Toggle("Use Contact Shadows", m_tod.TimeOfDayProfile.TimeOfDayData.m_useContactShadows);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useMicroShadows = EditorGUILayout.Toggle("Use Micro Shadows", m_tod.TimeOfDayProfile.TimeOfDayData.m_useMicroShadows);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_generalExposure = EditorGUILayout.CurveField("General Exposure", m_tod.TimeOfDayProfile.TimeOfDayData.m_generalExposure);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_ambientIntensity = EditorGUILayout.CurveField("Ambient Intensity", m_tod.TimeOfDayProfile.TimeOfDayData.m_ambientIntensity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_ambientReflectionIntensity = EditorGUILayout.CurveField("Ambient Reflection Intensity", m_tod.TimeOfDayProfile.TimeOfDayData.m_ambientReflectionIntensity);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Shadow Settings", EditorStyles.boldLabel);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowCascadeCount = EditorGUILayout.IntSlider("Shadow Cascade Count", m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowCascadeCount, 1, 4);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowDistanceMultiplier = EditorGUILayout.Slider("Shadow Distance Multiplier", m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowDistanceMultiplier, 0.01f, 5f);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowDistance = EditorGUILayout.CurveField("Shadow Distance", m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowDistance);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowTransmissionMultiplier = EditorGUILayout.CurveField("Transmission Multiplier", m_tod.TimeOfDayProfile.TimeOfDayData.m_shadowTransmissionMultiplier);
        }
        private void SkySettings()
        {
            TimeOfDaySkyMode skyMode = m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMode;
            skyMode = (TimeOfDaySkyMode) EditorGUILayout.EnumPopup("Sky Mode", skyMode);
            if (skyMode != m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMode)
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMode = skyMode;
                m_tod.SetupVisualEnvironment();
            }
            switch (m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMode)
            {
                case TimeOfDaySkyMode.Gradient:
                {
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyTopColor = EditorGUILayout.GradientField(new GUIContent("Top Color"), m_tod.TimeOfDayProfile.TimeOfDayData.m_skyTopColor, true);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMiddleColor = EditorGUILayout.GradientField(new GUIContent("Middle Color"), m_tod.TimeOfDayProfile.TimeOfDayData.m_skyMiddleColor, true);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyBottomColor = EditorGUILayout.GradientField(new GUIContent("Bottom Color"), m_tod.TimeOfDayProfile.TimeOfDayData.m_skyBottomColor, true);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyGradientDiffusion = EditorGUILayout.CurveField("Gradient Diffusion", m_tod.TimeOfDayProfile.TimeOfDayData.m_skyGradientDiffusion);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyExposureGradient = EditorGUILayout.CurveField("Exposure", m_tod.TimeOfDayProfile.TimeOfDayData.m_skyExposureGradient);
                    break;
                }
                case TimeOfDaySkyMode.PhysicallyBased:
                {
                    EditorGUI.BeginChangeCheck();
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxExposure = EditorGUILayout.FloatField("Sky Exposure", m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxExposure);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxGroundColor = EditorGUILayout.ColorField("Sky Ground Color", m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxGroundColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_tod.SetSkySettings(m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxExposure, m_tod.TimeOfDayProfile.TimeOfDayData.m_skyboxGroundColor);
                    }
                    EditorGUILayout.HelpBox("All lighting will be automatically calculated, so no more settings need to be setup here.", MessageType.Info);
                    break;
                }
            }
        }
        private void SunSettings()
        {
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunIntensity = EditorGUILayout.CurveField("Sun Intensity", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunIntensity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunIntensityMultiplier = EditorGUILayout.CurveField("Sun/Moon Intensity Multiplier", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunIntensityMultiplier);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunTemperature = EditorGUILayout.CurveField("Sun Temperature", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunTemperature);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunColorFilter = EditorGUILayout.GradientField("Sun Color Filter", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunColorFilter);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_moonIntensity = EditorGUILayout.CurveField("Moon Intensity", m_tod.TimeOfDayProfile.TimeOfDayData.m_moonIntensity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_moonTemperature = EditorGUILayout.CurveField("Moon Temperature", m_tod.TimeOfDayProfile.TimeOfDayData.m_moonTemperature);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_moonColorFilter = EditorGUILayout.GradientField("Moon Color Filter", m_tod.TimeOfDayProfile.TimeOfDayData.m_moonColorFilter);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunVolumetrics = EditorGUILayout.CurveField("Sun/Moon Volumetric", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunVolumetrics);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_sunVolumetricShadowDimmer = EditorGUILayout.CurveField("Sun/Moon Volumetric Dimmer", m_tod.TimeOfDayProfile.TimeOfDayData.m_sunVolumetricShadowDimmer);
        }
        private void FogSettings()
        {
            m_tod.TimeOfDayProfile.TimeOfDayData.m_fogQuality = (GeneralQuality)EditorGUILayout.EnumPopup("Fog Quality", m_tod.TimeOfDayProfile.TimeOfDayData.m_fogQuality);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useDenoising = EditorGUILayout.Toggle("Use Denoising", m_tod.TimeOfDayProfile.TimeOfDayData.m_useDenoising);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_useDenoising)
            {
                EditorGUI.indentLevel++;
                m_tod.TimeOfDayProfile.TimeOfDayData.m_denoisingQuality = (GeneralQuality)EditorGUILayout.EnumPopup("Denoising Quality", m_tod.TimeOfDayProfile.TimeOfDayData.m_denoisingQuality);
                EditorGUI.indentLevel--;
            }
            m_tod.TimeOfDayProfile.TimeOfDayData.m_fogColor = EditorGUILayout.GradientField("Fog Color", m_tod.TimeOfDayProfile.TimeOfDayData.m_fogColor);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_fogDistance = EditorGUILayout.CurveField("Fog Distance", m_tod.TimeOfDayProfile.TimeOfDayData.m_fogDistance);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_fogHeight = EditorGUILayout.CurveField("Fog Height", m_tod.TimeOfDayProfile.TimeOfDayData.m_fogHeight);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_fogDensity = EditorGUILayout.CurveField("Local Fog Density", m_tod.TimeOfDayProfile.TimeOfDayData.m_fogDensity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_localFogMultiplier = EditorGUILayout.CurveField("Local Fog Multiplier", m_tod.TimeOfDayProfile.TimeOfDayData.m_localFogMultiplier);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogDistance = EditorGUILayout.CurveField("Volumetric Distance", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogDistance);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogAnisotropy = EditorGUILayout.CurveField("Volumetric Anisotropy", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogAnisotropy);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogSliceDistributionUniformity = EditorGUILayout.CurveField("Slice Distribution Uniformity", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricFogSliceDistributionUniformity);
        }
        private void LensFlareSettings()
        {
            //Sun
            EditorGUI.BeginChangeCheck();
            TimeOfDayLensFlareProfile sunLensFlare = m_tod.TimeOfDayProfile.TimeOfDayData.m_sunLensFlareProfile;
            TimeOfDayLensFlareProfile moonLensFlare = m_tod.TimeOfDayProfile.TimeOfDayData.m_moonLensFlareProfile;
            EditorGUILayout.LabelField("Sun Lens Flare Settings", EditorStyles.boldLabel);
            if (sunLensFlare.m_useLensFlare || moonLensFlare.m_useLensFlare)
            {
                SunFlareInfoHelp();
            }
            EditorGUI.indentLevel++;
            sunLensFlare.m_useLensFlare = EditorGUILayout.Toggle("Use Sun Lens Flare", sunLensFlare.m_useLensFlare);
            if (sunLensFlare.m_useLensFlare)
            {
                sunLensFlare.m_lensFlareData = (LensFlareDataSRP)EditorGUILayout.ObjectField("Lens Flare Data", sunLensFlare.m_lensFlareData, typeof(LensFlareDataSRP), false);
                sunLensFlare.m_intensity = EditorGUILayout.CurveField("Intensity", sunLensFlare.m_intensity);
                sunLensFlare.m_scale = EditorGUILayout.CurveField("Scale", sunLensFlare.m_scale);
                sunLensFlare.m_enableOcclusion = EditorGUILayout.Toggle("Enable Occlusion", sunLensFlare.m_enableOcclusion);
                if (sunLensFlare.m_enableOcclusion)
                {
                    EditorGUI.indentLevel++;
                    sunLensFlare.m_occlusionRadius = EditorGUILayout.FloatField("Radius", sunLensFlare.m_occlusionRadius);
                    sunLensFlare.m_sampleCount = EditorGUILayout.IntSlider("Sample Count", sunLensFlare.m_sampleCount, 1, 64);
                    sunLensFlare.m_occlusionOffset = EditorGUILayout.FloatField("Offset", sunLensFlare.m_occlusionOffset);
                    sunLensFlare.m_allowOffScreen = EditorGUILayout.Toggle("Allow Off Screen", sunLensFlare.m_allowOffScreen);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_sunLensFlareProfile = sunLensFlare;
            }

            //Moon
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Moon Lens Flare Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            moonLensFlare.m_useLensFlare = EditorGUILayout.Toggle("Use Moon Lens Flare", moonLensFlare.m_useLensFlare);
            if (moonLensFlare.m_useLensFlare)
            {
                moonLensFlare.m_lensFlareData = (LensFlareDataSRP)EditorGUILayout.ObjectField("Lens Flare Data", moonLensFlare.m_lensFlareData, typeof(LensFlareDataSRP), false);
                moonLensFlare.m_intensity = EditorGUILayout.CurveField("Intensity", moonLensFlare.m_intensity);
                moonLensFlare.m_scale = EditorGUILayout.CurveField("Scale", moonLensFlare.m_scale);
                moonLensFlare.m_enableOcclusion = EditorGUILayout.Toggle("Enable Occlusion", moonLensFlare.m_enableOcclusion);
                if (moonLensFlare.m_enableOcclusion)
                {
                    EditorGUI.indentLevel++;
                    moonLensFlare.m_occlusionRadius = EditorGUILayout.FloatField("Radius", moonLensFlare.m_occlusionRadius);
                    moonLensFlare.m_sampleCount = EditorGUILayout.IntSlider("Sample Count", moonLensFlare.m_sampleCount, 1, 64);
                    moonLensFlare.m_occlusionOffset = EditorGUILayout.FloatField("Offset", moonLensFlare.m_occlusionOffset);
                    moonLensFlare.m_allowOffScreen = EditorGUILayout.Toggle("Allow Off Screen", moonLensFlare.m_allowOffScreen);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_moonLensFlareProfile = moonLensFlare;
            }
        }
        private void VolumetricCloudSettings()
        {
            EditorGUILayout.LabelField("Volumetric Cloud Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useLocalClouds = EditorGUILayout.Toggle("Use Local Clouds", m_tod.TimeOfDayProfile.TimeOfDayData.m_useLocalClouds);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_useLocalClouds)
            {
                EditorGUILayout.HelpBox("Local Clouds is enabled, you need to have a high far clip plane value to use this feature. Recommend a min value of 15000 for the far clip plane on the camera.", MessageType.Info);
            }
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudPresets = (VolumetricClouds.CloudPresets)EditorGUILayout.EnumPopup("Cloud Preset", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudPresets);
            switch (m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudPresets)
            {
                case VolumetricClouds.CloudPresets.Custom:
                {
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_erosionNoiseType = (VolumetricClouds.CloudErosionNoise)EditorGUILayout.EnumPopup("Erosion Noise Type", m_tod.TimeOfDayProfile.TimeOfDayData.m_erosionNoiseType);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricDensityMultiplier = EditorGUILayout.CurveField("Density Multiplier", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricDensityMultiplier);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricDensityCurve = EditorGUILayout.CurveField("Custom Density Curve", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricDensityCurve);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricShapeFactor = EditorGUILayout.CurveField("Shape Factor", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricShapeFactor);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricShapeScale = EditorGUILayout.CurveField("Shape Scale", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricShapeScale);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionFactor = EditorGUILayout.CurveField("Erosion Factor", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionFactor);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionScale = EditorGUILayout.CurveField("Erosion Scale", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionScale);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionCurve = EditorGUILayout.CurveField("Custom Erosion Curve", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionCurve);
                    m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricAmbientOcclusionCurve = EditorGUILayout.CurveField("Custom Ambient Occlusion Curve", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricAmbientOcclusionCurve);
                    break;
                }
            }

            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricLowestCloudAltitude = EditorGUILayout.CurveField("Lowest Cloud Altitude", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricLowestCloudAltitude);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudThickness = EditorGUILayout.CurveField("Cloud Thickness", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudThickness);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindDirection = EditorGUILayout.CurveField("Cloud Wind Direction", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindDirection);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindSpeed = EditorGUILayout.CurveField("Cloud Wind Speed", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindSpeed);
            //Lighting
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricAmbientLightProbeDimmer = EditorGUILayout.CurveField("Ambient Light Probe Dimmer", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricAmbientLightProbeDimmer);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricSunLightDimmer = EditorGUILayout.CurveField("Sun Light Dimmer", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricSunLightDimmer);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionOcclusion = EditorGUILayout.CurveField("Erosion Occlusion", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricErosionOcclusion);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricScatteringTint = EditorGUILayout.GradientField("Scattering Tint", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricScatteringTint);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricPowderEffectIntensity = EditorGUILayout.CurveField("Powder Effect Intensity", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricPowderEffectIntensity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricMultiScattering = EditorGUILayout.CurveField("Multi Scattering", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricMultiScattering);
            //Shadows
            m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadows = EditorGUILayout.Toggle("Enable Shadows", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadows);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadows)
            {
                EditorGUI.indentLevel++;
                m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadowResolution = (VolumetricClouds.CloudShadowResolution)EditorGUILayout.EnumPopup("Shadow Resolution", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadowResolution); 
                m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadowOpacity = EditorGUILayout.CurveField("Shadow Opacity", m_tod.TimeOfDayProfile.TimeOfDayData.m_volumetricCloudShadowOpacity);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
        private void ProceduralCloudSettings()
        {
            EditorGUILayout.LabelField("Procedual Cloud Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Using procedural clouds might causes graphical issues due to a rendering queue issue within Unity HDRP core code that has the clouds rendering over the top of opaque object.", MessageType.Warning);
            EditorGUI.indentLevel++;
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayers = (CloudLayerType) EditorGUILayout.EnumPopup("Cloud Layers Type", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayers);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudResolution = (CloudResolution) EditorGUILayout.EnumPopup("Cloud Resolution", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudResolution);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudOpacity = EditorGUILayout.CurveField("Cloud Opacity", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudOpacity);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayerAOpacityR = EditorGUILayout.CurveField("Cloud Layer A Opacity", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayerAOpacityR);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayers == CloudLayerType.Double)
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayerBOpacityR = EditorGUILayout.CurveField("Cloud Layer B Opacity", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLayerBOpacityR);
            }

            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudTintColor = EditorGUILayout.GradientField("Tint Color", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudTintColor);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudExposure = EditorGUILayout.CurveField("Cloud Exposure", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudExposure);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_globalCloudType == GlobalCloudType.Procedural)
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindDirection = EditorGUILayout.CurveField("Cloud Wind Direction", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindDirection);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindSpeed = EditorGUILayout.CurveField("Cloud Wind Speed", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudWindSpeed);
            }

            m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLighting = EditorGUILayout.Toggle("Use Cloud Lighting", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudLighting);
            m_tod.TimeOfDayProfile.TimeOfDayData.m_useCloudShadows = EditorGUILayout.Toggle("Use Cloud Shadows", m_tod.TimeOfDayProfile.TimeOfDayData.m_useCloudShadows);
            if (m_tod.TimeOfDayProfile.TimeOfDayData.m_useCloudShadows)
            {
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudShadowOpacity = EditorGUILayout.CurveField("Cloud Shadow Opacity", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudShadowOpacity);
                m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudShadowColor = EditorGUILayout.GradientField("Cloud Shadow Color", m_tod.TimeOfDayProfile.TimeOfDayData.m_cloudShadowColor);
            }
            EditorGUI.indentLevel--;
        }
        private void WeatherSettings()
        {
            EditorGUILayout.BeginVertical(m_boxStyle);
            EditorGUILayout.HelpBox("Coming Soon!", MessageType.Info);
            /*m_tod.m_useWeatherFX = EditorGUILayout.Toggle("Use Weather FX", m_tod.m_useWeatherFX);
            if (m_tod.m_useWeatherFX)
            {
                EditorGUI.indentLevel++;
                m_tod.m_randomWeatherTimer = EditorGUILayout.Vector2Field("Weather Min/Max Wait Time", m_tod.m_randomWeatherTimer);

                if (m_tod.WeatherProfiles.Count > 0)
                {
                    for (int i = 0; i < m_tod.WeatherProfiles.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(m_boxStyle);
                        EditorGUILayout.BeginHorizontal();
                        if (m_tod.WeatherProfiles[i] == null)
                        {
                            m_tod.WeatherProfiles[i] = (HDRPTimeOfDayWeatherProfile)EditorGUILayout.ObjectField("No Profile Specified", m_tod.WeatherProfiles[i], typeof(HDRPTimeOfDayWeatherProfile), false);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(m_tod.WeatherProfiles[i].WeatherData.m_weatherName))
                            {
                                m_tod.WeatherProfiles[i] = (HDRPTimeOfDayWeatherProfile)EditorGUILayout.ObjectField("No Name Specified", m_tod.WeatherProfiles[i], typeof(HDRPTimeOfDayWeatherProfile), false);
                            }
                            else
                            {
                                m_tod.WeatherProfiles[i] = (HDRPTimeOfDayWeatherProfile)EditorGUILayout.ObjectField("Profile: " + m_tod.WeatherProfiles[i].WeatherData.m_weatherName, m_tod.WeatherProfiles[i], typeof(HDRPTimeOfDayWeatherProfile), false);
                            }
                        }
                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(80f)))
                        {
                            m_tod.WeatherProfiles.RemoveAt(i);
                            EditorUtility.SetDirty(m_tod);
                            EditorGUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                }

                if (GUILayout.Button("Add New Weather Profile"))
                {
                    m_tod.WeatherProfiles.Add(null);
                }
                EditorGUI.indentLevel--;
            }*/
            EditorGUILayout.EndVertical();
        }
        private void SunFlareInfoHelp()
        {
            EditorGUILayout.HelpBox("Please note that Unity sun flare in HDRP does not yet get culled by volumetric clouds and they will render through the clouds. We hope Unity will add support for this in the future, if this notice is removed then unity has added support.", MessageType.Info);
        }
        /// <summary>
        /// Post Processing
        /// </summary>
        private void ColorGradingSettings()
        {
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_contrast = EditorGUILayout.CurveField("Contrast", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_contrast);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_colorFilter = EditorGUILayout.GradientField(new GUIContent("Color Filter"), m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_colorFilter, true);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_saturation = EditorGUILayout.CurveField("Saturation", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_saturation);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_temperature = EditorGUILayout.CurveField("Temperature", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_temperature);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_tint = EditorGUILayout.CurveField("Tint", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_tint);
        }
        private void BloomSettings()
        {
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomThreshold = EditorGUILayout.CurveField("Threshold", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomThreshold);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomIntensity = EditorGUILayout.CurveField("Intensity", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomIntensity);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomScatter = EditorGUILayout.CurveField("Scatter", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomScatter);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomTint = EditorGUILayout.GradientField("Tint", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_bloomTint);
        }
        private void ShadowToningSettings()
        {
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadows = EditorGUILayout.GradientField("Shadows", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadows);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_highlights = EditorGUILayout.GradientField("Highlights", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_highlights);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadowBalance = EditorGUILayout.CurveField("Balance", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_shadowBalance);
        }
        private void VignetteSettings()
        {
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteColor = EditorGUILayout.GradientField("Color", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteColor);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteIntensity = EditorGUILayout.CurveField("Intensity", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteIntensity);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteSmoothness = EditorGUILayout.CurveField("Smoothness", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_vignetteSmoothness);
        }
        private void AmbientOcclusionSettings()
        {
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientIntensity = EditorGUILayout.CurveField("Intensity", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientIntensity);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientDirectStrength = EditorGUILayout.CurveField("Direct Light Intensity", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientDirectStrength);
            m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientRadius = EditorGUILayout.CurveField("Radius", m_tod.TimeOfDayPostFxProfile.TimeOfDayPostFXData.m_ambientRadius);
        }
        private void RayTracingSettings()
        {
            EditorGUILayout.HelpBox("Ray Tracing (Alpha, Preview). Ray tracing is still in development by Unity and some features may not work or may have rendering issues in Unity or in exe builds. To use ray tracing please enable it in the HDRP Asset file and also install DX12, see unity documentation for help on installing ray tracing in your project.", MessageType.Warning);

            EditorGUI.BeginChangeCheck();
            bool useRayTracing = m_tod.UseRayTracing;
            useRayTracing = EditorGUILayout.Toggle("Use Ray Tracing", useRayTracing);
            if (EditorGUI.EndChangeCheck())
            {
                if (useRayTracing != m_tod.UseRayTracing)
                {
                    m_tod.UseRayTracing = useRayTracing;
                    if (useRayTracing)
                    {
                        SetScriptDefine("RAY_TRACING_ENABLED", true);
                    }
                    else
                    {
                        SetScriptDefine("RAY_TRACING_ENABLED", false);
                    }
                }
                EditorUtility.SetDirty(m_tod);
            }
            if (useRayTracing)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                //SSGI
                m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSGI = EditorGUILayout.Toggle("Ray Trace SSGI", m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSGI);
                if (m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSGI)
                {
                    EditorGUI.indentLevel++;
                    m_tod.TimeOfDayProfile.RayTracingSettings.m_ssgiRenderMode = (GeneralRenderMode)EditorGUILayout.EnumPopup("Render Mode", m_tod.TimeOfDayProfile.RayTracingSettings.m_ssgiRenderMode);
                    if (m_tod.TimeOfDayProfile.RayTracingSettings.m_ssgiRenderMode == GeneralRenderMode.Performance)
                    {
                        m_tod.TimeOfDayProfile.RayTracingSettings.m_ssgiQuality = (GeneralQuality)EditorGUILayout.EnumPopup("Quality", m_tod.TimeOfDayProfile.RayTracingSettings.m_ssgiQuality);
                    }
                    EditorGUI.indentLevel--;
                }
                //SSR
                m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSR = EditorGUILayout.Toggle("Ray Trace SSR", m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSR);
                if (m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSSR)
                {
                    EditorGUI.indentLevel++;
                    m_tod.TimeOfDayProfile.RayTracingSettings.m_ssrRenderMode = (GeneralRenderMode)EditorGUILayout.EnumPopup("Render Mode", m_tod.TimeOfDayProfile.RayTracingSettings.m_ssrRenderMode);
                    if (m_tod.TimeOfDayProfile.RayTracingSettings.m_ssrRenderMode == GeneralRenderMode.Performance)
                    {
                        m_tod.TimeOfDayProfile.RayTracingSettings.m_ssrQuality = (GeneralQuality)EditorGUILayout.EnumPopup("Quality", m_tod.TimeOfDayProfile.RayTracingSettings.m_ssrQuality);
                    }
                    EditorGUI.indentLevel--;
                }
                //AO
                m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceAmbientOcclusion = EditorGUILayout.Toggle("Ray Trace Ambient Occlusion", m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceAmbientOcclusion);
                if (m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceAmbientOcclusion)
                {
                    EditorGUI.indentLevel++;
                    m_tod.TimeOfDayProfile.RayTracingSettings.m_aoQuality = (GeneralQuality)EditorGUILayout.EnumPopup("Quality", m_tod.TimeOfDayProfile.RayTracingSettings.m_aoQuality);
                    EditorGUI.indentLevel--;
                }
                //Recursive Rendering
                m_tod.TimeOfDayProfile.RayTracingSettings.m_recursiveRendering = EditorGUILayout.Toggle("Recursive Rendering", m_tod.TimeOfDayProfile.RayTracingSettings.m_recursiveRendering);
                //SSS
                m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSubSurfaceScattering = EditorGUILayout.Toggle("Ray Trace Sub Surface Scattering", m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSubSurfaceScattering);
                if (m_tod.TimeOfDayProfile.RayTracingSettings.m_rayTraceSubSurfaceScattering)
                {
                    EditorGUI.indentLevel++;
                    m_tod.TimeOfDayProfile.RayTracingSettings.m_subSurfaceScatteringSampleCount = EditorGUILayout.IntSlider("Sample Count", m_tod.TimeOfDayProfile.RayTracingSettings.m_subSurfaceScatteringSampleCount, 1, 32);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_tod.TimeOfDayProfile);
                    m_tod.ApplyRayTracingSettings();
                }
            }
        }
        private void SetScriptDefine(string define, bool add)
        {
            bool updateScripting = false;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            if (add)
            {
                if (!symbols.Contains(define))
                {
                    updateScripting = true;
                    if (symbols.Length < 1)
                    {
                        symbols += define;
                    }
                    else
                    {
                        symbols += ";" + define;
                    }
                }
            }
            else
            {
                if (symbols.Contains(define))
                {
                    updateScripting = true;
                    symbols = symbols.Replace(define, "");
                }
            }

            if (updateScripting)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }

        [MenuItem("Assets/Create/Procedural Worlds/HDRP Time Of Day/Create Empty Post FX Profile")]
        public static void CreateHDRPTimeOfDayPostFXProfile()
        {
            HDRPTimeOfDayPostFXProfile asset = ScriptableObject.CreateInstance<HDRPTimeOfDayPostFXProfile>();

            AssetDatabase.CreateAsset(asset, "Assets/HDRP Time Of Day Post FX Profile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
        [MenuItem("Assets/Create/Procedural Worlds/HDRP Time Of Day/Create Empty Profile")]
        public static void CreateHDRPTimeOfDayProfile()
        {
            HDRPTimeOfDayProfile asset = ScriptableObject.CreateInstance<HDRPTimeOfDayProfile>();

            AssetDatabase.CreateAsset(asset, "Assets/HDRP Time Of Day Profile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
        [MenuItem("Assets/Create/Procedural Worlds/HDRP Time Of Day/Create Empty Weather Profile")]
        public static void CreateHDRPTimeOfDayWeatherProfile()
        {
            HDRPTimeOfDayWeatherProfile asset = ScriptableObject.CreateInstance<HDRPTimeOfDayWeatherProfile>();

            AssetDatabase.CreateAsset(asset, "Assets/HDRP Time Of Day Weather Profile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
#endif