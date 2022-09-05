#if HDPipeline && UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    [System.Serializable]
    public class HDRPTimeOfDayComponents
    {
        //Lighting
        public GameObject m_sunRotationObject;
        public Light m_sunLight;
        public HDAdditionalLightData m_sunLightData;
        public Light m_moonLight;
        public HDAdditionalLightData m_moonLightData;
        public Volume m_timeOfDayVolume;
        public VolumeProfile m_timeOfDayVolumeProfile;
        public VisualEnvironment m_visualEnvironment;
        public PhysicallyBasedSky m_physicallyBasedSky;
        public CloudLayer m_cloudLayer;
        public VolumetricClouds m_volumetricClouds;
        public GlobalIllumination m_globalIllumination;
        public Fog m_fog;
        public LocalVolumetricFog m_localVolumetricFog;
        public Exposure m_exposure;
        public GradientSky m_gradientSky;
        public ScreenSpaceReflection m_screenSpaceReflection;
        public ScreenSpaceRefraction m_screenSpaceRefraction;
        public ContactShadows m_contactShadows;
        public MicroShadowing m_microShadowing;
        public IndirectLightingController m_indirectLightingController;
        public HDShadowSettings m_shadows;
        //Post FX
        public Volume m_timeOfDayPostFXVolume;
        public VolumeProfile m_timeOfDayPostFXVolumeProfile;
        public ColorAdjustments m_colorAdjustments;
        public WhiteBalance m_whiteBalance;
        public Bloom m_bloom;
        public SplitToning m_splitToning;
        public Vignette m_vignette;
        public AmbientOcclusion m_ambientOcclusion;
        //Advanced
        public LensFlareComponentSRP m_sunLensFlare;
        public LensFlareComponentSRP m_moonLensFlare;
        //Ray tracing
#if RAY_TRACING_ENABLED
        public Volume m_rayTracingVolume;
        public GlobalIllumination m_rayTracedGlobalIllumination;
        public ScreenSpaceReflection m_rayTracedScreenSpaceReflection;
        public AmbientOcclusion m_rayTracedAmbientOcclusion;
        public RayTracingSettings m_rayTracedSettings;
        public RecursiveRendering m_rayTracedRecursiveRendering;
        public SubSurfaceScattering m_rayTracedSubSurfaceScattering;
#endif

        public bool Validated(out UnityEngine.Object failedObject)
        {
            if (m_sunRotationObject == null)
            {
                failedObject = m_sunRotationObject;
                return false;
            }
            if (m_sunLight == null)
            {
                failedObject = m_sunLight;
                return false;
            }
            if (m_sunLightData == null)
            {
                failedObject = m_sunLightData;
                return false;
            }
            if (m_moonLight == null)
            {
                failedObject = m_moonLight;
                return false;
            }
            if (m_moonLightData == null)
            {
                failedObject = m_moonLightData;
                return false;
            }
            if (m_timeOfDayVolume == null)
            {
                failedObject = m_timeOfDayVolume;
                return false;
            }
            if (m_timeOfDayVolumeProfile == null)
            {
                failedObject = m_timeOfDayVolumeProfile;
                return false;
            }
            if (m_visualEnvironment == null)
            {
                failedObject = m_visualEnvironment;
                return false;
            }
            if (m_physicallyBasedSky == null)
            {
                failedObject = m_physicallyBasedSky;
                return false;
            }
            if (m_cloudLayer == null)
            {
                failedObject = m_cloudLayer;
                return false;
            }
            if (m_volumetricClouds == null)
            {
                failedObject = m_volumetricClouds;
                return false;
            }
            if (m_globalIllumination == null)
            {
                failedObject = m_globalIllumination;
                return false;
            }
            if (m_fog == null)
            {
                failedObject = m_fog;
                return false;
            }
            if (m_localVolumetricFog == null)
            {
                failedObject = m_localVolumetricFog;
                return false;
            }
            if (m_exposure == null)
            {
                failedObject = m_exposure;
                return false;
            }
            if (m_gradientSky == null)
            {
                failedObject = m_gradientSky;
                return false;
            }
            if (m_screenSpaceReflection == null)
            {
                failedObject = m_screenSpaceReflection;
                return false;
            }
            if (m_screenSpaceRefraction == null)
            {
                failedObject = m_screenSpaceRefraction;
                return false;
            }
            if (m_contactShadows == null)
            {
                failedObject = m_contactShadows;
                return false;
            }
            if (m_microShadowing == null)
            {
                failedObject = m_microShadowing;
                return false;
            }
            if (m_shadows == null)
            {
                failedObject = m_shadows;
                return false;
            }
            if (m_indirectLightingController == null)
            {
                failedObject = m_indirectLightingController;
                return false;
            }
            if (m_timeOfDayPostFXVolume == null)
            {
                failedObject = m_timeOfDayPostFXVolume;
                return false;
            }
            if (m_timeOfDayPostFXVolumeProfile == null)
            {
                failedObject = m_timeOfDayPostFXVolumeProfile;
                return false;
            }
            if (m_colorAdjustments == null)
            {
                failedObject = m_colorAdjustments;
                return false;
            }
            if (m_whiteBalance == null)
            {
                failedObject = m_whiteBalance;
                return false;
            }
            if (m_bloom == null)
            {
                failedObject = m_bloom;
                return false;
            }
            if (m_splitToning == null)
            {
                failedObject = m_splitToning;
                return false;
            }
            if (m_vignette == null)
            {
                failedObject = m_vignette;
                return false;
            }
            if (m_ambientOcclusion == null)
            {
                failedObject = m_ambientOcclusion;
                return false;
            }
            if (m_sunLensFlare == null)
            {
                failedObject = m_sunLensFlare;
                return false;
            }
            if (m_moonLensFlare == null)
            {
                failedObject = m_moonLensFlare;
                return false;
            }
#if RAY_TRACING_ENABLED
            if (m_rayTracingVolume == null)
            {
                failedObject = m_rayTracingVolume;
                return false;
            }
            if (m_rayTracedGlobalIllumination == null)
            {
                failedObject = m_rayTracedGlobalIllumination;
                return false;
            }
            if (m_rayTracedScreenSpaceReflection == null)
            {
                failedObject = m_rayTracedScreenSpaceReflection;
                return false;
            }
            if (m_rayTracedAmbientOcclusion == null)
            {
                failedObject = m_rayTracedAmbientOcclusion;
                return false;
            }
            if (m_rayTracedSettings == null)
            {
                failedObject = m_rayTracedSettings;
                return false;
            }
            if (m_rayTracedRecursiveRendering == null)
            {
                failedObject = m_rayTracedRecursiveRendering;
                return false;
            }
            if (m_rayTracedSubSurfaceScattering == null)
            {
                failedObject = m_rayTracedSubSurfaceScattering;
                return false;
            }
#endif

            failedObject = null;
            return true;
        }
    }

    [ExecuteAlways]
    public class HDRPTimeOfDay : MonoBehaviour
    {
        #region Properties

        public static HDRPTimeOfDay Instance
        {
            get { return m_instance; }
        }
        [SerializeField] private static HDRPTimeOfDay m_instance;

        public Transform Player
        {
            get { return m_player; }
            set
            {
                m_player = value;
                UpdatePlayerTransform();
            }
        }
        [SerializeField] private Transform m_player;

        public HDRPTimeOfDayProfile TimeOfDayProfile
        {
            get { return m_timeOfDayProfile; }
            set
            {
                if (m_timeOfDayProfile != value)
                {
                    m_timeOfDayProfile = value;
                    m_hasBeenSetupCorrectly = SetupHDRPTimeOfDay();
                    ProcessTimeOfDay();
                    if (m_enableReflectionProbeSync)
                    {
                        RefreshReflectionProbes();
                    }
                }
            }
        }
        [SerializeField] private HDRPTimeOfDayProfile m_timeOfDayProfile;

        public bool UsePostFX
        {
            get { return m_usePostFX; }
            set
            {
                if (m_usePostFX != value)
                {
                    m_usePostFX = value;
                    ProcessTimeOfDay();
                    SetPostProcessingActive(value);
                }
            }
        }
        [SerializeField]
        private bool m_usePostFX = true;

        public HDRPTimeOfDayPostFXProfile TimeOfDayPostFxProfile
        {
            get { return m_timeOfDayPostFxProfile; }
            set
            {
                if (m_timeOfDayPostFxProfile != value)
                {
                    m_timeOfDayPostFxProfile = value;
                    ProcessTimeOfDay();
                }
            }
        }
        [SerializeField] private HDRPTimeOfDayPostFXProfile m_timeOfDayPostFxProfile;

        public TimeOfDayDebugSettings DebugSettings
        {
            get { return m_debugSettings; }
            set
            {
                if (m_debugSettings != value)
                {
                    m_debugSettings = value;
                }
            }
        }
        [SerializeField] private TimeOfDayDebugSettings m_debugSettings = new TimeOfDayDebugSettings();

        public float TimeOfDay
        {
            get { return m_timeOfDay; }
            set
            {
                if (m_timeOfDay != value)
                {
                    m_timeOfDay = Mathf.Clamp(value, 0f, 24f);
                    ProcessTimeOfDay();
                    if (m_enableReflectionProbeSync)
                    {
                        RefreshReflectionProbes();
                    }
                }
            }
        }
        [SerializeField] private float m_timeOfDay = 11f;

        public float DirectionY
        {
            get { return m_directionY; }
            set
            {
                if (m_directionY != value)
                {
                    m_directionY = Mathf.Clamp(value, 0f, 360f);
                    ProcessTimeOfDay();
                    if (m_enableReflectionProbeSync)
                    {
                        RefreshReflectionProbes();
                    }
                }
            }
        }
        [SerializeField] private float m_directionY = 0f;

        public float ReflectionProbeSyncTime
        {
            get { return m_reflectionProbeSyncTime; }
            set
            {
                if (m_reflectionProbeSyncTime != value)
                {
                    m_reflectionProbeSyncTime = value;
                    UpdateProbeSyncTime(value);
                }
            }
        }
        [SerializeField] private float m_reflectionProbeSyncTime = 5f;

        public bool UseRayTracing
        {
            get { return m_useRayTracing; }
            set
            {
                if (m_useRayTracing != value)
                {
                    m_useRayTracing = value;
                    m_rayTracingSetup = SetupRayTracing(value);
                }
            }
        }
        [SerializeField] private bool m_useRayTracing = false;

        public bool RefreshOverrideVolume
        {
            get { return m_refreshOverrideVolume; }
            set
            {
                if (value)
                {
                    m_overrideVolumeData.m_isInVolue = CheckOverrideVolumes();
                    m_overrideVolumeData.m_transitionTime = 0f;
                    RefreshTimeOfDay(false);
                }
            }
        }
        [SerializeField] private bool m_refreshOverrideVolume = false;

        public bool UseOverrideVolumes
        {
            get { return m_useOverrideVolumes; }
            set
            {
                if (m_useOverrideVolumes != value)
                {
                    m_useOverrideVolumes = value;
                    if (value)
                    {
                        SetupAllOverrideVolumes();
                        RefreshTimeOfDay(false);

                        HDRPTimeOfDayOverrideVolumeController controller = GetComponent<HDRPTimeOfDayOverrideVolumeController>();
                        if (controller == null)
                        {
                            controller = gameObject.AddComponent<HDRPTimeOfDayOverrideVolumeController>();
                            controller.CheckState(true);
                        }
                    }
                    else
                    {
                        m_overrideVolumeData.m_transitionTime = 0f;
                        m_overrideVolumeData.m_isInVolue = false;
                    }
                }
            }
        }
        [SerializeField] private bool m_useOverrideVolumes = false;

        #endregion
        #region Variables

        public bool m_enableTimeOfDaySystem = false;
        public float m_timeOfDayMultiplier = 1f;
        public bool m_enableReflectionProbeSync = false;
        public bool m_useWeatherFX = false;
        public bool m_showWeatherSettings = false;
        public Vector2 m_randomWeatherTimer = new Vector2(120f, 400f);
        public Dictionary<int, HDRPTimeOfDayOverrideVolume> m_overrideVolumes = new Dictionary<int, HDRPTimeOfDayOverrideVolume>();
        public List<HDRPTimeOfDayWeatherProfile> WeatherProfiles
        {
            get { return m_weatherProfiles; }
            set
            {
                if (m_weatherProfiles != value)
                {
                    m_weatherProfiles = value;
                }
            }
        }

        [SerializeField] private List<HDRPTimeOfDayWeatherProfile> m_weatherProfiles = new List<HDRPTimeOfDayWeatherProfile>();
        [SerializeField] private List<HDRPTimeOfDayReflectionProbeSync> m_reflectionProbeSyncs = new List<HDRPTimeOfDayReflectionProbeSync>();
        [SerializeField] private List<GameObject> m_disableItems = new List<GameObject>();
        [SerializeField] private bool m_rayTracingSetup = false;
        [SerializeField] private HDRPTimeOfDayComponents Components = new HDRPTimeOfDayComponents();

        private bool m_hasBeenSetupCorrectly = false;
        private HDRPTimeOfDayOverrideVolume m_lastOverrideVolume;
        private OverrideDataInfo m_overrideVolumeData = new OverrideDataInfo();
        //Weather VFX
        private bool m_weatherIsActive = false;
        private float m_currentRandomWeatherTimer;
        private float m_weatherDurationTimer;
        private int m_selectedActiveWeatherProfile = -1;
        private bool m_resetDuration = false;
        private float m_duration;
        private GameObject m_weatherVFXObject;
        private IHDRPWeatherVFX m_weatherVFX;
        private bool m_thunderInProgress = false;
        private AudioSource m_thunderSoundFX;
        private float m_lastCloudLayerOpacity;

        private const string ComponentsPrefabName = "Time Of Day Components.prefab";
        private const string TimeOfDayProfileName = "HDRP Time Of Day Profile.asset";
        private const string TimeOfDayPostFXProfileName = "HDRP Time Of Day Post FX Profile.asset";

        #endregion
        #region Unity Functions

        private void OnEnable()
        {
            m_instance = this;
            //Remove when weather is added back in
            m_useWeatherFX = false;
            m_hasBeenSetupCorrectly = SetupHDRPTimeOfDay();
            if (UseRayTracing)
            {
                m_rayTracingSetup = SetupRayTracing(UseRayTracing);
                ApplyRayTracingSettings();
            }
            if (UseOverrideVolumes)
            {
                SetupAllOverrideVolumes();
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
                EditorApplication.update += EditorUpdate;
            }
            else
            {
                EditorApplication.update -= EditorUpdate;
            }
#endif
        }
        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (m_enableTimeOfDaySystem)
            {
                ProcessTimeOfDay();
            }
        }

        #endregion
        #region Public Functions

        /// <summary>
        /// Resets the override volume blend time
        /// </summary>
        public void ResetOverrideVolumeBlendTime(bool resetIsInVolume)
        {
            m_overrideVolumeData.m_transitionTime = 0f;
            if (resetIsInVolume)
            {
                m_overrideVolumeData.m_isInVolue = false;
                m_overrideVolumeData.m_settings = null;
                m_lastOverrideVolume = null;
            }
        }
        /// <summary>
        /// Sets up time of day
        /// </summary>
        public bool SetupHDRPTimeOfDay()
        {
            m_currentRandomWeatherTimer = UnityEngine.Random.Range(m_randomWeatherTimer.x, m_randomWeatherTimer.y);

            if (TimeOfDayProfile == null)
            {
                return false;
            }

            bool successful = BuildVolumesAndCollectComponents();
            UpdatePlayerTransform();
            SetupVisualEnvironment();
            ProcessTimeOfDay();
            return successful;
        }
        /// <summary>
        /// The function that processes the time of day
        /// </summary>
        /// <param name="hasBeenSetup"></param>
        public void ProcessTimeOfDay(bool checkOverrideVolume = true)
        {
            if (!m_hasBeenSetupCorrectly)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                if (Components == null)
                {
                    Debug.LogError("HDRP Time Of Day components is null.");
                    return;
                }
                else
                {
                    if (!Components.Validated(out UnityEngine.Object debugObject))
                    {
                        if (debugObject != null)
                        {
                            Debug.LogError("HDRP Time Of Day components validate failed because " + debugObject.name + " was null");
                        }
                        else
                        {
                            Debug.LogError("HDRP Time Of Day components validate failed.");
                        }

                        return;
                    }
                }
            }

            RefreshSkies();

            //This is used to evaluate systems that can range from 0-1
            float currentTime = CovertTimeOfDay();
            UpdateSunRotation(currentTime);
            //Checks if it's day or night
            //bool isDay = IsDayTime(currentTime, TimeOfDayProfile.TimeOfDayData);
            bool isDay = IsDayTime();
            HDAdditionalLightData lightData = Components.m_sunLightData;
            if (!isDay)
            {
                lightData = Components.m_moonLightData;
            }

            UpdateTime(isDay, TimeOfDayProfile.TimeOfDayData);
            
            if (UseOverrideVolumes)
            {
                if (checkOverrideVolume)
                {
                    m_overrideVolumeData.m_isInVolue = CheckOverrideVolumes();
                    if (m_overrideVolumeData.m_transitionTime < 1f)
                    {
                        if (m_lastOverrideVolume != null)
                        {
                            m_overrideVolumeData.m_transitionTime +=
                                Time.deltaTime / m_lastOverrideVolume.m_volumeSettings.m_blendTime;
                        }
                        else
                        {
                            m_overrideVolumeData.m_transitionTime += Time.deltaTime / 3f;
                        }
                    }
                }
            }
            else
            {
                if (m_overrideVolumeData.m_transitionTime < 1f)
                {
                    m_overrideVolumeData.m_transitionTime += Time.deltaTime / 3f;
                }
            }

            if (!m_weatherIsActive)
            {
                //Process TOD
                UpdateSun(currentTime, isDay, TimeOfDayProfile.TimeOfDayData);
                UpdateSky(currentTime, TimeOfDayProfile.TimeOfDayData);
                UpdateAdvancedLighting(currentTime, TimeOfDayProfile.TimeOfDayData);
                UpdateFog(currentTime, TimeOfDayProfile.TimeOfDayData);
                UpdateShadows(currentTime, TimeOfDayProfile.TimeOfDayData);
                UpdateClouds(currentTime, TimeOfDayProfile.TimeOfDayData);
                UpdateLensFlare(currentTime, TimeOfDayProfile.TimeOfDayData, isDay);

                //Process Post FX
                if (UsePostFX)
                {
                    if (TimeOfDayPostFxProfile != null)
                    {
                        UpdateAmbientOcclusion(currentTime, TimeOfDayPostFxProfile.TimeOfDayPostFXData);
                        UpdateColorGrading(currentTime, TimeOfDayPostFxProfile.TimeOfDayPostFXData);
                        UpdateBloom(currentTime, TimeOfDayPostFxProfile.TimeOfDayPostFXData);
                        UpdateShadowToning(currentTime, TimeOfDayPostFxProfile.TimeOfDayPostFXData);
                        UpdateVignette(currentTime, TimeOfDayPostFxProfile.TimeOfDayPostFXData);
                    }
                }

                //Check when next weather profile will be active
                CheckAutoWeather();
            }
            else
            {
                HDRPTimeOfDayWeatherProfile weatherProfile = WeatherProfiles[m_selectedActiveWeatherProfile];
                if (weatherProfile == null)
                {
                    return;
                }

                if (m_weatherDurationTimer > 0f)
                {
                    if (m_resetDuration)
                    {
                        m_duration = 0f;
                        m_resetDuration = false;
                    }

                    m_duration += Time.deltaTime / WeatherProfiles[m_selectedActiveWeatherProfile].WeatherData.m_transitionDuration;
                    if (weatherProfile.WeatherData.ApplyWeather(lightData, Components.m_physicallyBasedSky, Components.m_gradientSky, Components.m_exposure, Components.m_fog, Components.m_cloudLayer, currentTime, m_duration))
                    {
                        TimeOfDayProfileData WeatherData = weatherProfile.WeatherData.m_weatherData;
                        UpdateSun(currentTime, isDay, WeatherData);
                        UpdateSky(currentTime, WeatherData);
                        UpdateAdvancedLighting(currentTime, WeatherData);
                        UpdateFog(currentTime, WeatherData);
                        UpdateShadows(currentTime, WeatherData);
                        UpdateClouds(currentTime, WeatherData);
                        UpdateLensFlare(currentTime, WeatherData, isDay);
                    }
                }

                m_weatherDurationTimer -= Time.deltaTime;
                if (m_weatherDurationTimer <= 0f)
                {
                    if (m_resetDuration)
                    {
                        m_duration = 0f;
                        m_resetDuration = false;
                        if (m_weatherVFX != null)
                        {
                            m_weatherVFX.StopWeatherFX();
                        }
                    }

                    m_duration += Time.deltaTime / weatherProfile.WeatherData.m_transitionDuration;
                    if (TimeOfDayProfile.TimeOfDayData.ReturnFromWeather(lightData, Components.m_physicallyBasedSky, Components.m_gradientSky, Components.m_exposure, Components.m_fog, Components.m_cloudLayer, currentTime, m_duration))
                    {
                        m_weatherIsActive = false;
                        DestroyImmediate(m_weatherVFXObject);
                    }
                }
            }
        }
        /// <summary>
        /// Logs what the time value is on animation curves and gradient fields
        /// This is to help you fine tune the times of day
        /// </summary>
        public void GetDebugInformation()
        {
            float currentTime = CovertTimeOfDay();
            if (DebugSettings.m_roundUp)
            {
                Debug.Log("Animation Curve time is ranged from 0-1 and the current value at " + TimeOfDay +
                          " Time Of Day is " + currentTime.ToString("n2"));
                Debug.Log("Gradients time is ranged from 0-100% and the current value at " + TimeOfDay +
                          " Time Of Day is " + Mathf.FloorToInt(currentTime * 100f) + "%");
            }
            else
            {
                Debug.Log("Animation Curve time is ranged from 0-1 and the current value at " + TimeOfDay +
                          " Time Of Day is " + currentTime);
                Debug.Log("Gradients time is ranged from 0-100% and the current value at " + TimeOfDay +
                          " Time Of Day is " + currentTime * 100f + "%");
            }
        }
        /// <summary>
        /// Checks to see if the components and systems have been setup correctly
        /// </summary>
        /// <returns></returns>
        public bool HasBeenSetup()
        {
            return m_hasBeenSetupCorrectly;
        }
        /// <summary>
        /// Manually set the systems has been setup with the bool value
        /// </summary>
        /// <param name="value"></param>
        public void SetHasBeenSetup(bool value)
        {
            m_hasBeenSetupCorrectly = value;
        }
        /// <summary>
        /// Refreshes time of day settings and if you set check setup to true
        /// It will process all the components to make sure everythign is setup correctly
        /// </summary>
        /// <param name="checkSetup"></param>
        public void RefreshTimeOfDay(bool checkSetup)
        {
            if (checkSetup)
            {
                m_hasBeenSetupCorrectly = SetupHDRPTimeOfDay();
            }

            if (!m_hasBeenSetupCorrectly)
            {
                return;
            }

            ProcessTimeOfDay();
        }
        /// <summary>
        /// Checks to see if it's day time if retuns false then it's night time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsDayTime()
        {
            if (Components.m_sunRotationObject.transform.eulerAngles.x > 0f && Components.m_sunRotationObject.transform.eulerAngles.x < 180f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Applies visual environment settings
        /// </summary>
        public void SetupVisualEnvironment()
        {
            if (Components.m_visualEnvironment != null)
            {
                Components.m_visualEnvironment.skyAmbientMode.value = SkyAmbientMode.Dynamic;
                if (Components.m_volumetricClouds != null && Components.m_cloudLayer != null)
                {
                    switch (TimeOfDayProfile.TimeOfDayData.m_globalCloudType)
                    {
                        case GlobalCloudType.Volumetric:
                        {
                            Components.m_visualEnvironment.cloudType.value = 0;
                            Components.m_volumetricClouds.enable.value = true;
                            Components.m_cloudLayer.opacity.value = 0f;
                            break;
                        }
                        case GlobalCloudType.Procedural:
                        {
                            Components.m_visualEnvironment.cloudType.value = (int) CloudType.CloudLayer;
                            Components.m_volumetricClouds.enable.value = false;
                            Components.m_cloudLayer.opacity.value = m_lastCloudLayerOpacity;
                            break;
                        }
                        case GlobalCloudType.Both:
                        {
                            Components.m_visualEnvironment.cloudType.value = (int) CloudType.CloudLayer;
                            Components.m_volumetricClouds.enable.value = true;
                            Components.m_cloudLayer.opacity.value = m_lastCloudLayerOpacity;
                            break;
                        }
                        case GlobalCloudType.None:
                        {
                            Components.m_visualEnvironment.cloudType.value = 0;
                            Components.m_volumetricClouds.enable.value = false;
                            Components.m_cloudLayer.opacity.value = 0f;
                            break;
                        }
                    }
                }

                if (Components.m_physicallyBasedSky != null && Components.m_gradientSky != null)
                {
                    switch (TimeOfDayProfile.TimeOfDayData.m_skyMode)
                    {
                        case TimeOfDaySkyMode.Gradient:
                        {
                            Components.m_visualEnvironment.skyType.value = (int)SkyType.Gradient;
                            Components.m_physicallyBasedSky.active = false;
                            Components.m_gradientSky.active = true;
                            break;
                        }
                        case TimeOfDaySkyMode.PhysicallyBased:
                        {
                            Components.m_visualEnvironment.skyType.value = (int)SkyType.PhysicallyBased;
                            Components.m_physicallyBasedSky.active = true;
                            Components.m_gradientSky.active = false;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sets the refresh timer of all the probe syncs that are registered in this system
        /// </summary>
        /// <param name="value"></param>
        public void UpdateProbeSyncTime(float value)
        {
            if (m_reflectionProbeSyncs.Count > 0)
            {
                foreach (HDRPTimeOfDayReflectionProbeSync reflectionProbeSync in m_reflectionProbeSyncs)
                {
                    reflectionProbeSync.m_waitBeforeRefreshTime = value;
                }
            }
        }
        /// <summary>
        /// Sets the skybox exposure value
        /// </summary>
        /// <param name="value"></param>
        public void SetSkySettings(float value, Color color)
        {
            if (Components.m_physicallyBasedSky != null)
            {
                Components.m_physicallyBasedSky.exposure.value = value;
                Components.m_physicallyBasedSky.groundTint.value = color;
            }
        }
        /// <summary>
        /// Refreshes the physically based or gradient sky to update the to the current lighting
        /// </summary>
        public void RefreshSkies()
        {
            Components.m_physicallyBasedSky.updateMode.value = EnvironmentUpdateMode.OnChanged;
            Components.m_gradientSky.updateMode.value = EnvironmentUpdateMode.OnChanged;
        }
        /// <summary>
        /// Starts the thunder vfx coroutine
        /// Will not run again until it has finished previous
        /// </summary>
        /// <param name="data"></param>
        public void StartThunderVFX(ThunderData data)
        {
            if (!m_thunderInProgress)
            {
                StartCoroutine(PlayThunderVFX(data));
            }
        }
        /// <summary>
        /// Sets the static singleton instance
        /// </summary>
        /// <param name="timeOfDay"></param>
        public void SetStaticInstance(HDRPTimeOfDay timeOfDay)
        {
            m_instance = timeOfDay;
        }
        /// <summary>
        /// Function used to find and refresh all the reflection probes
        /// </summary>
        public void RefreshReflectionProbes()
        {
            if (m_reflectionProbeSyncs.Count > 0)
            {
                foreach (HDRPTimeOfDayReflectionProbeSync reflectionProbeSync in m_reflectionProbeSyncs)
                {
                    reflectionProbeSync.StartSync();
                }
            }
        }
        /// <summary>
        /// Registers a reflection probe sync
        /// </summary>
        /// <param name="probe"></param>
        public void RegisterReflectionProbe(HDRPTimeOfDayReflectionProbeSync probe)
        {
            if (!m_reflectionProbeSyncs.Contains(probe))
            {
                m_reflectionProbeSyncs.Add(probe);
            }
        }
        /// <summary>
        /// Un-Registers a reflection probe sync
        /// </summary>
        /// <param name="probe"></param>
        public void UnRegisterReflectionProbe(HDRPTimeOfDayReflectionProbeSync probe)
        {
            if (m_reflectionProbeSyncs.Contains(probe))
            {
                m_reflectionProbeSyncs.Remove(probe);
            }
        }
        /// <summary>
        /// Adds a gameobject item that has been disabled by this system
        /// </summary>
        /// <param name="disableObject"></param>
        public void AddDisabledItem(GameObject disableObject)
        {
            if (!m_disableItems.Contains(disableObject))
            {
                m_disableItems.Add(disableObject);
            }
        }
        /// <summary>
        /// Re-enables all the disable objects that have been added to the system
        /// </summary>
        public void EnableAllDisabledItems()
        {
            if (m_disableItems.Count > 0)
            {
                foreach (GameObject disableItem in m_disableItems)
                {
                    if (disableItem != null)
                    {
                        disableItem.SetActive(true);
                    }
                }
            }
        }
        /// <summary>
        /// Applies ray tracing settings
        /// </summary>
        public void ApplyRayTracingSettings()
        {
            if (m_rayTracingSetup)
            {
#if RAY_TRACING_ENABLED
                //SSGI
                Components.m_rayTracedGlobalIllumination.active = TimeOfDayProfile.RayTracingSettings.m_rayTraceSSGI;
                switch (TimeOfDayProfile.RayTracingSettings.m_ssgiRenderMode)
                {
                    case GeneralRenderMode.Performance:
                    {
                        Components.m_rayTracedGlobalIllumination.mode.value = RayTracingMode.Performance;
                        break;
                    }
                    case GeneralRenderMode.Quality:
                    {
                        Components.m_rayTracedGlobalIllumination.mode.value = RayTracingMode.Quality;
                        break;
                    }
                }
                Components.m_rayTracedGlobalIllumination.quality.value = (int)TimeOfDayProfile.RayTracingSettings.m_ssgiQuality;
                //SSR
                Components.m_rayTracedScreenSpaceReflection.active = TimeOfDayProfile.RayTracingSettings.m_rayTraceSSR;
                switch (TimeOfDayProfile.RayTracingSettings.m_ssrRenderMode)
                {
                    case GeneralRenderMode.Performance:
                    {
                        Components.m_rayTracedScreenSpaceReflection.mode.value = RayTracingMode.Performance;
                        break;
                    }
                    case GeneralRenderMode.Quality:
                    {
                        Components.m_rayTracedScreenSpaceReflection.mode.value = RayTracingMode.Quality;
                        break;
                    }
                }
                Components.m_rayTracedScreenSpaceReflection.quality.value = (int)TimeOfDayProfile.RayTracingSettings.m_ssrQuality;
                //AO
                Components.m_rayTracedAmbientOcclusion.active = TimeOfDayProfile.RayTracingSettings.m_rayTraceAmbientOcclusion;
                Components.m_rayTracedAmbientOcclusion.quality.value = (int)TimeOfDayProfile.RayTracingSettings.m_aoQuality;
                //Recursive Rendering
                Components.m_rayTracedRecursiveRendering.active = TimeOfDayProfile.RayTracingSettings.m_recursiveRendering;
                //SSS
                Components.m_rayTracedSubSurfaceScattering.active = TimeOfDayProfile.RayTracingSettings.m_rayTraceSubSurfaceScattering;
                Components.m_rayTracedSubSurfaceScattering.sampleCount.value = (int)TimeOfDayProfile.RayTracingSettings.m_subSurfaceScatteringSampleCount;
#endif
            }
        }
        /// <summary>
        /// Adds an override volume
        /// </summary>
        /// <param name="id"></param>
        /// <param name="volume"></param>
        public bool AddOverrideVolume(int id, HDRPTimeOfDayOverrideVolume volume)
        {
            if (!m_overrideVolumes.ContainsKey(id))
            {
                m_overrideVolumes.Add(id, volume);
            }

            return true;
        }
        /// <summary>
        /// Removes an override volume
        /// </summary>
        /// <param name="id"></param>
        public bool RemoveOverrideVolume(int id)
        {
            if (m_overrideVolumes.ContainsKey(id))
            {
                m_overrideVolumes.Remove(id);
            }

            return false;
        }
        /// <summary>
        /// Sets up the override volumes in the scene
        /// </summary>
        public void SetupAllOverrideVolumes()
        {
            if (m_overrideVolumes.Count > 0)
            {
                foreach (KeyValuePair<int, HDRPTimeOfDayOverrideVolume> volume in m_overrideVolumes)
                {
                    if (volume.Value == null)
                    {
                        m_overrideVolumes.Remove(volume.Key);
                    }
                }
                m_overrideVolumeData.m_isInVolue = false;
                m_overrideVolumeData.m_settings = null;
                for (int i = 0; i < m_overrideVolumes.Count; i++)
                {
                    m_overrideVolumes[i].Setup(i);
                    m_overrideVolumes[i].SetupVolumeTypeToController();
                }
            }
        }

        #endregion
        #region Private Functions

        /// <summary>
        /// Editor update for scene view previewing
        /// </summary>
        private void EditorUpdate()
        {
            if (!m_hasBeenSetupCorrectly)
            {
                return;
            }

            if (UseOverrideVolumes)
            {
                m_overrideVolumeData.m_isInVolue = CheckOverrideVolumes();
                if (m_overrideVolumeData.m_transitionTime < 1f)
                {
                    if (m_lastOverrideVolume != null)
                    {
                        m_overrideVolumeData.m_transitionTime += Time.deltaTime / m_lastOverrideVolume.m_volumeSettings.m_blendTime;
                    }
                    else
                    {
                        m_overrideVolumeData.m_transitionTime += Time.deltaTime / 3f;
                    }
                    ProcessTimeOfDay(false);
                }
            }
            else
            {
                if (m_overrideVolumeData.m_transitionTime < 1f)
                {
                    m_overrideVolumeData.m_transitionTime += Time.deltaTime / 3f;
                    ProcessTimeOfDay(false);
                }
            }
        }
        /// <summary>
        /// Checks to see if you are in an override volume
        /// </summary>
        /// <returns></returns>
        private bool CheckOverrideVolumes()
        {
            if (m_overrideVolumes.Count < 1)
            {
                if (m_lastOverrideVolume != null)
                {
                    m_overrideVolumeData.m_transitionTime = 0f;
                    m_lastOverrideVolume = null;
                }
                return false;
            }
            else
            {
                HDRPTimeOfDayOverrideVolume volume = m_overrideVolumes.Last().Value;
                if (volume != null)
                {
                    if (!volume.enabled || !volume.gameObject.activeInHierarchy)
                    {
                        return false;
                    }

                    if (m_lastOverrideVolume == null || volume != m_lastOverrideVolume)
                    {
                        m_lastOverrideVolume = volume;
                        m_overrideVolumeData = new OverrideDataInfo
                        {
                            m_isInVolue = volume.m_volumeSettings.IsAnyOverrideEnabled(),
                            m_transitionTime = 0f,
                            m_settings = volume.m_volumeSettings
                        };

                        return true;
                    }

                    return volume.m_volumeSettings.IsAnyOverrideEnabled();
                }

                if (m_lastOverrideVolume != null)
                {
                    m_overrideVolumeData.m_transitionTime = 0f;
                    m_lastOverrideVolume = null;
                }
                return false;
            }
        }
        /// <summary>
        /// Sets up the ray tracing components
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetupRayTracing(bool value)
        {
#if RAY_TRACING_ENABLED
            HDRPTimeOfDayComponentType[] componentTypes = gameObject.GetComponentsInChildren<HDRPTimeOfDayComponentType>();
            if (componentTypes.Length > 0)
            {
                foreach (HDRPTimeOfDayComponentType type in componentTypes)
                {
                    if (type.m_componentType == TimeOfDayComponentType.RayTracedVolume)
                    {
                        Volume volume = type.GetComponent<Volume>();
                        if (volume != null)
                        {
                            Components.m_rayTracingVolume = volume;
                            if (volume.sharedProfile != null)
                            {
                                volume.sharedProfile.TryGet(out Components.m_rayTracedGlobalIllumination);
                                volume.sharedProfile.TryGet(out Components.m_rayTracedScreenSpaceReflection);
                                volume.sharedProfile.TryGet(out Components.m_rayTracedAmbientOcclusion);
                                volume.sharedProfile.TryGet(out Components.m_rayTracedSettings);
                                volume.sharedProfile.TryGet(out Components.m_rayTracedRecursiveRendering);
                                volume.sharedProfile.TryGet(out Components.m_rayTracedSubSurfaceScattering);
                            }

                            if (Components.m_rayTracingVolume == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedGlobalIllumination == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedScreenSpaceReflection == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedAmbientOcclusion == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedSettings == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedRecursiveRendering == null)
                            {
                                return false;
                            }
                            if (Components.m_rayTracedSubSurfaceScattering == null)
                            {
                                return false;
                            }

                            if (value)
                            {
                                Components.m_rayTracingVolume.weight = 1f;
                                return true;
                            }
                            else
                            {
                                Components.m_rayTracingVolume.weight = 0f;
                                return false;
                            }
                        }
                    }
                }
            }
#endif
            return false;
        }
        /// <summary>
        /// Plays thunder vfx
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator PlayThunderVFX(ThunderData data)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                m_thunderInProgress = true;
                //bool isDay = IsDayTime(CovertTimeOfDay(), TimeOfDayProfile.TimeOfDayData);
                bool isDay = IsDayTime();
                if (data.m_thunderLightSource == null)
                {
                    Debug.LogError("Thunder light source was null exiting thunder vfx");
                    m_thunderInProgress = false;
                    StopAllCoroutines();
                }

                if (data.m_thunderStrikeSounds.Count < 1)
                {
                    Debug.LogError("Thunder strike sounds has no audio clips exiting thunder vfx");
                    m_thunderInProgress = false;
                    StopAllCoroutines();
                }

                data.m_thunderLightSource.color = data.m_thunderLight;
                data.m_thunderLightSource.intensity = data.m_intesity;

                if (m_thunderSoundFX == null)
                {
                    m_thunderSoundFX = gameObject.AddComponent<AudioSource>();
                }

                m_thunderSoundFX.loop = false;

                int thunderCount = UnityEngine.Random.Range(data.m_thunderStrikeCountMinMax.x, data.m_thunderStrikeCountMinMax.y);
                for (int i = 0; i < thunderCount; i++)
                {
                    Light todLight = Components.m_sunLight;
                    if (!isDay)
                    {
                        todLight = Components.m_moonLight;
                    }
                    todLight.enabled = false;
                    data.m_thunderLightSource.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                    m_thunderSoundFX.PlayOneShot(data.m_thunderStrikeSounds[UnityEngine.Random.Range(0, data.m_thunderStrikeSounds.Count - 1)], data.m_volume);
                    data.m_thunderLightSource.enabled = false;
                    todLight.enabled = true;
                    yield return new WaitForSeconds(data.m_pauseBetweenStrike);
                }

                yield return new WaitForEndOfFrame();
                m_thunderInProgress = false;
                StopAllCoroutines();
            }
        }
        /// <summary>
        /// Updates the time of day by adding time to it if it's enabled
        /// </summary>
        /// <param name="isDay"></param>
        /// <param name="data"></param>
        private void UpdateTime(bool isDay, TimeOfDayProfileData data)
        {
            if (!Application.isPlaying || !m_enableTimeOfDaySystem)
            {
                return;
            }

            if (isDay)
            {
                m_timeOfDay += (Time.deltaTime * m_timeOfDayMultiplier) / data.m_dayDuration;
            }
            else
            {
                m_timeOfDay += (Time.deltaTime * m_timeOfDayMultiplier) / data.m_nightDuration;
            }
            if (m_timeOfDay >= 24f)
            {
                m_timeOfDay = 0f;
            }
        }
        /// <summary>
        /// Updates the sun/moon position and rotation
        /// </summary>
        /// <param name="time"></param>
        /// <param name="isDay"></param>
        /// <param name="data"></param>
        private void UpdateSun(float time, bool isDay, TimeOfDayProfileData data)
        {
            if (data.ValidateSun())
            {
                if (isDay)
                {
                    Components.m_sunLight.enabled = true;
                    Components.m_moonLight.enabled = false;
                }
                else
                {
                    Components.m_sunLight.enabled = false;
                    Components.m_moonLight.enabled = true;
                }

                //Apply Settings
                if (isDay)
                {
                    data.ApplySunSettings(Components.m_sunLightData, time, isDay, m_overrideVolumeData);
                }
                else
                {
                    data.ApplySunSettings(Components.m_moonLightData, time, isDay, m_overrideVolumeData);
                }
            }
        }
        /// <summary>
        /// Sets the sun rotation
        /// </summary>
        private void UpdateSunRotation(float time)
        {
            //Set rotation
            Components.m_sunRotationObject.transform.eulerAngles = new Vector3(270f + Mathf.Lerp(0f, 360f, time), DirectionY, 0f);
        }
        /// <summary>
        /// Updates the sky settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateSky(float time, TimeOfDayProfileData data)
        {
            if (data.ValidateSky())
            {
                data.ApplySkySettings(Components.m_physicallyBasedSky, Components.m_gradientSky, time);
            }
        }
        /// <summary>
        /// Updates the fog settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateFog(float time, TimeOfDayProfileData data)
        {
            if (data.ValidateFog())
            {
                data.ApplyFogSettings(Components.m_fog, Components.m_localVolumetricFog, time);
            }
        }
        /// <summary>
        /// Updates the shadow settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateShadows(float time, TimeOfDayProfileData data)
        {
            if (data.ValidateShadows())
            {
                data.ApplyShadowSettings(Components.m_shadows, time);
            }
        }
        /// <summary>
        /// Updates advanced lighting settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateAdvancedLighting(float time, TimeOfDayProfileData data)
        {
            if (data.ValidateAdvancedLighting())
            {
                data.ApplyAdvancedLighting(Components.m_globalIllumination, Components.m_exposure, Components.m_screenSpaceReflection, Components.m_screenSpaceRefraction,
                    Components.m_contactShadows, Components.m_microShadowing, Components.m_indirectLightingController, time, m_overrideVolumeData);
            }
        }
        /// <summary>
        /// Updates cloud settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateClouds(float time, TimeOfDayProfileData data)
        {
            if (data.ValidateClouds())
            {
                m_lastCloudLayerOpacity = data.ApplyCloudSettings(Components.m_volumetricClouds, Components.m_cloudLayer, Components.m_visualEnvironment, time);
            }
        }
        /// <summary>
        /// Updates ambient occlusion Settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateAmbientOcclusion(float time, TimeOfDayPostFXProfileData data)
        {
            if (data.ValidateAmbientOcclusion())
            {
                data.ApplyAmbientOcclusion(Components.m_ambientOcclusion, time);
            }
        }
        /// <summary>
        /// Updates color grading settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateColorGrading(float time, TimeOfDayPostFXProfileData data)
        {
            if (data.ValidateColorGrading())
            {
                data.ApplyColorGradingSettings(Components.m_colorAdjustments, Components.m_whiteBalance, time);
            }
        }
        /// <summary>
        /// Updates bloom settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateBloom(float time, TimeOfDayPostFXProfileData data)
        {
            if (data.ValidateBloom())
            {
                data.ApplyBloomSettings(Components.m_bloom, time);
            }
        }
        /// <summary>
        /// Updates shadow toning settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateShadowToning(float time, TimeOfDayPostFXProfileData data)
        {
            if (data.ValidateShadowToning())
            {
                data.ApplyShadowToningSettings(Components.m_splitToning, time);
            }
        }
        /// <summary>
        /// Updates vignette settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        private void UpdateVignette(float time, TimeOfDayPostFXProfileData data)
        {
            if (data.ValidateVignette())
            {
                data.ApplyVignetteSettings(Components.m_vignette, time);
            }
        }
        /// <summary>
        /// Updates lens flare settings
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        /// <param name="isDay"></param>
        private void UpdateLensFlare(float time, TimeOfDayProfileData data, bool isDay)
        {
            if (data.ValidateSunLensFlare())
            {
                data.ApplySunLensFlare(Components.m_sunLensFlare, time, isDay);
            }
            if (data.ValidateMoonLensFlare())
            {
                data.ApplyMoonLensFlare(Components.m_moonLensFlare, time, isDay);
            }
        }
        /// <summary>
        /// Converts the time of day float from 24 = 0-1
        /// </summary>
        /// <returns></returns>
        private float CovertTimeOfDay()
        {
            return TimeOfDay / 24f;
        }
        /// <summary>
        /// Sets post processing state
        /// </summary>
        /// <param name="isActive"></param>
        private void SetPostProcessingActive(bool isActive)
        {
            if (Components.m_timeOfDayPostFXVolume != null)
            {
                Components.m_timeOfDayPostFXVolume.gameObject.SetActive(isActive);
            }
        }
        /// <summary>
        /// Checks if a weather profile is going to be active
        /// </summary>
        private void CheckAutoWeather()
        {
            if (!m_useWeatherFX || !Application.isPlaying)
            {
                return;
            }

            m_currentRandomWeatherTimer -= Time.deltaTime;
            if (m_currentRandomWeatherTimer <= 0f)
            {
                m_selectedActiveWeatherProfile = UnityEngine.Random.Range(0, WeatherProfiles.Count - 1);
                if (m_selectedActiveWeatherProfile <= WeatherProfiles.Count - 1)
                {
                    m_weatherIsActive = true;
                    m_weatherDurationTimer = WeatherProfiles[m_selectedActiveWeatherProfile].WeatherData.m_weatherDuration;
                    m_weatherVFXObject = Instantiate(WeatherProfiles[m_selectedActiveWeatherProfile].WeatherFXData.m_weatherEffect);
                    m_weatherVFX = m_weatherVFXObject.GetComponent<IHDRPWeatherVFX>();
                    if (m_weatherVFX == null)
                    {
                        m_weatherVFX = m_weatherVFXObject.GetComponentInChildren<IHDRPWeatherVFX>();
                    }
                    if (m_weatherVFX != null)
                    {
                        m_weatherVFX.SetWeatherProfile(WeatherProfiles[m_selectedActiveWeatherProfile]);
                        m_weatherVFX.StartWeatherFX();
                    }
                    m_resetDuration = true;
                }
            }
        }
        /// <summary>
        /// Sets up the time of day prefab and volume
        /// </summary>
        private bool BuildVolumesAndCollectComponents()
        {
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }

            if (Components.m_timeOfDayVolume == null)
            {
                GameObject timeOfDayVolume = GameObject.Find("Time Of Day Components");
                if (timeOfDayVolume == null)
                {
#if UNITY_EDITOR
                    GameObject componentsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(ComponentsPrefabName));
                    if (componentsPrefab == null)
                    {
                        Debug.LogError("Time Of Day Components Prefab is missing and could not be found in the project. It's normally found in 'HDRP Time Of Day/Resources' folder");
                        return false;
                    }

                    timeOfDayVolume = Instantiate(componentsPrefab);
                    timeOfDayVolume.name = "Time Of Day Components";
#endif
                }

                timeOfDayVolume.transform.SetParent(transform);

                Components.m_timeOfDayVolume = timeOfDayVolume.GetComponent<Volume>();
            }

            HDRPTimeOfDayComponentType[] componentType = FindObjectsOfType<HDRPTimeOfDayComponentType>();
            if (componentType.Length > 0)
            {
                foreach (HDRPTimeOfDayComponentType type in componentType)
                {
                    if (type.m_componentType == TimeOfDayComponentType.PostProcessing)
                    {
                        if (Components.m_timeOfDayPostFXVolumeProfile == null || Components.m_timeOfDayPostFXVolume == null)
                        {
                            Components.m_timeOfDayPostFXVolume = type.GetComponent<Volume>();
                            if (Components.m_timeOfDayPostFXVolume != null)
                            {
                                Components.m_timeOfDayPostFXVolumeProfile = Components.m_timeOfDayPostFXVolume.sharedProfile;
                            }
                        }
                    }
                    else if (type.m_componentType == TimeOfDayComponentType.SunRotationObject)
                    {
                        Components.m_sunRotationObject = type.gameObject;
                    }
                }
            }

            if (m_player == null)
            {
                m_player = GetCamera();
            }

            if (m_usePostFX && Components.m_timeOfDayPostFXVolume != null)
            {
                if (TimeOfDayPostFxProfile == null)
                {
                    Components.m_timeOfDayPostFXVolume.weight = 0f;
                }
                else
                {
                    Components.m_timeOfDayPostFXVolume.weight = 1f;
                }
            }

            if (UsePostFX)
            {
                if (Components.m_timeOfDayPostFXVolumeProfile == null)
                {
                    return false;
                }

                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_colorAdjustments))
                {
                    return false;
                }
                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_whiteBalance))
                {
                    return false;
                }
                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_bloom))
                {
                    return false;
                }
                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_splitToning))
                {
                    return false;
                }
                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_vignette))
                {
                    return false;
                }
                if (!Components.m_timeOfDayPostFXVolumeProfile.TryGet(out Components.m_ambientOcclusion))
                {
                    return false;
                }
            }

            if (Components.m_timeOfDayVolume != null)
            {
                Components.m_timeOfDayVolume.isGlobal = true;
                Components.m_timeOfDayVolume.priority = 50;
                Components.m_timeOfDayVolumeProfile = Components.m_timeOfDayVolume.sharedProfile;
            }

            if (Components.m_timeOfDayVolumeProfile == null)
            {
                return false;
            }

            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_visualEnvironment))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_physicallyBasedSky))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_cloudLayer))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_volumetricClouds))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_globalIllumination))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_fog))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_exposure))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_gradientSky))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_screenSpaceReflection))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_screenSpaceRefraction))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_contactShadows))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_microShadowing))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_indirectLightingController))
            {
                return false;
            }
            if (!Components.m_timeOfDayVolumeProfile.TryGet(out Components.m_shadows))
            {
                return false;
            }

            Components.m_localVolumetricFog = Components.m_timeOfDayVolume.gameObject.GetComponentInChildren<LocalVolumetricFog>();
            if (Components.m_localVolumetricFog == null)
            {
                return false;
            }

            Light[] lights = Components.m_timeOfDayVolume.gameObject.GetComponentsInChildren<Light>();
            if (lights.Length < 1)
            {
                Debug.LogError("Sun and moon light could not be found");
                return false;
            }
            else
            {
                foreach (Light light in lights)
                {
                    HDRPTimeOfDayComponentType lightType = light.GetComponent<HDRPTimeOfDayComponentType>();
                    if (lightType != null)
                    {
                        if (lightType.m_componentType == TimeOfDayComponentType.Sun)
                        {
                            Components.m_sunLight = light;
                        }
                        else if (lightType.m_componentType == TimeOfDayComponentType.Moon)
                        {
                            Components.m_moonLight = light;
                        }
                    }
                }
            }

            if (Components.m_sunLight == null)
            {
                return false;
            }
            else
            {
                Components.m_sunLightData = Components.m_sunLight.GetComponent<HDAdditionalLightData>();
                if (Components.m_sunLightData == null)
                {
                    Components.m_sunLightData = Components.m_sunLight.gameObject.AddComponent<HDAdditionalLightData>();
                }
            }
            if (Components.m_moonLight == null)
            {
                return false;
            }
            else
            {
                Components.m_moonLightData = Components.m_moonLight.GetComponent<HDAdditionalLightData>();
                if (Components.m_moonLightData == null)
                {
                    Components.m_moonLightData = Components.m_moonLight.gameObject.AddComponent<HDAdditionalLightData>();
                }
            }

            Light[] directionalLights = FindObjectsOfType<Light>();
            if (directionalLights.Length > 0)
            {
                foreach (Light directionalLight in directionalLights)
                {
                    if (directionalLight.type == LightType.Directional)
                    {
                        if (directionalLight != Components.m_sunLight && directionalLight != Components.m_moonLight)
                        {
                            if (directionalLight.enabled)
                            {
                                directionalLight.gameObject.SetActive(false);
                                AddDisabledItem(directionalLight.gameObject);
                                Debug.Log(directionalLight.name + " was disabled as it conflicts with HDRP Time Of Day. If you ever remove time of day system you can just re-enable this light source.");
                            }
                        }
                    }
                }
            }

            //Sun lens flare
            Components.m_sunLensFlare = Components.m_sunLight.GetComponent<LensFlareComponentSRP>();
            if (Components.m_sunLensFlare == null)
            {
                Components.m_sunLensFlare = Components.m_sunLight.gameObject.AddComponent<LensFlareComponentSRP>();
            }
            if (Components.m_sunLensFlare == null)
            {
                return false;
            }

            //Moon lens flare
            Components.m_moonLensFlare = Components.m_moonLight.GetComponent<LensFlareComponentSRP>();
            if (Components.m_moonLensFlare == null)
            {
                Components.m_moonLensFlare = Components.m_moonLight.gameObject.AddComponent<LensFlareComponentSRP>();
            }
            if (Components.m_moonLensFlare == null)
            {
                return false;
            }

            if (Components.m_sunRotationObject == null)
            {
                return false;
            }

            SetSkySettings(TimeOfDayProfile.TimeOfDayData.m_skyboxExposure, TimeOfDayProfile.TimeOfDayData.m_skyboxGroundColor);

            return true;
        }
        /// <summary>
        /// Sets the new player transform
        /// </summary>
        private void UpdatePlayerTransform()
        {
            if (Player == null)
            {
                return;
            }

            HDRPTimeOfDayComponentType[] componentTypes = FindObjectsOfType<HDRPTimeOfDayComponentType>();
            if (componentTypes.Length > 0)
            {
                foreach (HDRPTimeOfDayComponentType type in componentTypes)
                {
                    type.SetNewPlayer(Player);
                }
            }
        }
        /// <summary>
        /// Gets the camera transform that is used for the player
        /// </summary>
        /// <returns></returns>
        private Transform GetCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                return camera.transform;
            }

            Camera[] cameras = FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
            {
                foreach (Camera cam in cameras)
                {
                    if (cam.isActiveAndEnabled)
                    {
                        return cam.transform;
                    }
                }
            }

            return null;
        }

        #endregion
        #region Public Static Functions

        /// <summary>
        /// Adds time of day to the scene
        /// </summary>
        public static void CreateTimeOfDay(GameObject parent, bool selection = true)
        {
            HDRPTimeOfDay timeOfDay = Instance;
            if (timeOfDay == null)
            {
                GameObject timeOfDayGameObject = new GameObject("HDRP Time Of Day");
                timeOfDay = timeOfDayGameObject.AddComponent<HDRPTimeOfDay>();
#if UNITY_EDITOR
                timeOfDay.TimeOfDayProfile = AssetDatabase.LoadAssetAtPath<HDRPTimeOfDayProfile>(GetAssetPath(TimeOfDayProfileName));
                timeOfDay.TimeOfDayPostFxProfile = AssetDatabase.LoadAssetAtPath<HDRPTimeOfDayPostFXProfile>(GetAssetPath(TimeOfDayPostFXProfileName));
#endif
                timeOfDay.SetHasBeenSetup(timeOfDay.BuildVolumesAndCollectComponents());
                timeOfDay.SetStaticInstance(timeOfDay);

                Volume[] localVolumes = FindObjectsOfType<Volume>();
                if (localVolumes.Length > 0)
                {
                    bool localFound = false;
                    List<Volume> allLocalVolumes = new List<Volume>();
                    foreach (Volume localVolume in localVolumes)
                    {
                        if (!localVolume.isGlobal)
                        {
                            localFound = true;
                            allLocalVolumes.Add(localVolume);
                        }
                    }

                    if (localFound)
                    {
#if UNITY_EDITOR
                        if (EditorUtility.DisplayDialog("Local Volumes Found",
                            "We have detected lcoal volumes in your scene, this could affect the lighting quality of the time of day system as it might override some important settings. Would you like us to disable these volumes?",
                            "Yes", "No"))
                        {
                            foreach (Volume volume in allLocalVolumes)
                            {
                                volume.gameObject.SetActive(false);
                                timeOfDay.AddDisabledItem(volume.gameObject);
                                Debug.Log(volume.name + " has been disabled");
                            }
                        }
#endif
                    }

                    if (parent != null)
                    {
                        timeOfDay.transform.SetParent(parent.transform);
                    }
#if UNITY_EDITOR
                    if (selection)
                    {
                        Selection.activeObject = timeOfDay;
                    }
#endif
                    timeOfDay.TimeOfDay = 11f;
                }
            }

        }
        /// <summary>
        /// Removes time of day from the scene
        /// </summary>
        public static void RemoveTimeOfDay()
        {
            HDRPTimeOfDay timeOfDay = Instance;
            if (timeOfDay != null)
            {
#if UNITY_EDITOR
                if (EditorUtility.DisplayDialog("Re-enable disabled gameobjects", "HDRP Time Of Day has been removed would you like to activate all the disable objects that this system disabled?", "Yes", "No"))
                {
                    timeOfDay.EnableAllDisabledItems();
                }
#endif
                DestroyImmediate(timeOfDay.gameObject);
            }
        }
        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string fileName)
        {
#if UNITY_EDITOR
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = UnityEditor.AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    return path;
                }
            }
#endif
            return "";
        }

        #endregion
    }
}
#endif