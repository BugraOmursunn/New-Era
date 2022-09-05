#if HDPipeline && UNITY_2021_2_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    public enum GlobalCloudType { None, Volumetric, Procedural, Both }
    public enum CloudLayerType { Single, Double }
    public enum CloudResolution { Resolution256, Resolution512, Resolution1024, Resolution2048, Resolution4096, Resolution8192 }
    public enum TimeOfDaySkyMode { PhysicallyBased, Gradient }
    public enum GeneralQuality { Low, Medium, High }
    public enum GeneralRenderMode { Performance, Quality }

    [System.Serializable]
    public class TimeOfDayLensFlareProfile
    {
        public bool m_useLensFlare = true;
        public LensFlareDataSRP m_lensFlareData;
        public AnimationCurve m_intensity;
        public AnimationCurve m_scale;
        public bool m_enableOcclusion = true;
        public float m_occlusionRadius = 0.1f;
        public int m_sampleCount = 32;
        public float m_occlusionOffset = 0.05f;
        public bool m_allowOffScreen = false;
    }
    [System.Serializable]
    public class TimeOfDayDebugSettings
    {
        public bool m_showSettings = false;
        public bool m_roundUp = true;
    }
    [System.Serializable]
    public class TimeOfDayProfileData
    {
        [Header("Duration")]
        public bool m_durationSettings = false;
        public float m_dayDuration = 300f;
        public float m_nightDuration = 120f;
        [Header("Sun")]
        public bool m_sunSettings = false;
        public AnimationCurve m_sunIntensity;
        public AnimationCurve m_moonIntensity;
        public AnimationCurve m_sunIntensityMultiplier;
        public AnimationCurve m_sunTemperature;
        public Gradient m_sunColorFilter;
        public AnimationCurve m_moonTemperature;
        public Gradient m_moonColorFilter;
        public AnimationCurve m_sunVolumetrics;
        public AnimationCurve m_sunVolumetricShadowDimmer;
        [Header("Sky")]
        public bool m_skySettings = false;
        public TimeOfDaySkyMode m_skyMode = TimeOfDaySkyMode.Gradient;
        public float m_skyboxExposure = 2.2f;
        public Color m_skyboxGroundColor = new Color(0.9609969f, 0.9024923f, 0.8708023f, 1f);
        //Gradient
        public Gradient m_skyTopColor;
        public Gradient m_skyMiddleColor;
        public Gradient m_skyBottomColor;
        public AnimationCurve m_skyGradientDiffusion;
        public AnimationCurve m_skyExposureGradient;
        [Header("Shadows")]
        public AnimationCurve m_shadowDistance;
        public AnimationCurve m_shadowTransmissionMultiplier;
        public int m_shadowCascadeCount = 4;
        public float m_shadowDistanceMultiplier = 1f;
        [Header("Fog")]
        public bool m_fogSettings = false;
        public GeneralQuality m_fogQuality = GeneralQuality.Medium;
        public bool m_useDenoising = true;
        public GeneralQuality m_denoisingQuality = GeneralQuality.Medium;
        public Gradient m_fogColor;
        public AnimationCurve m_fogDistance;
        public AnimationCurve m_fogDensity;
        public AnimationCurve m_fogHeight;
        public AnimationCurve m_volumetricFogDistance;
        public AnimationCurve m_volumetricFogAnisotropy;
        public AnimationCurve m_volumetricFogSliceDistributionUniformity;
        public AnimationCurve m_localFogMultiplier;
        [Header("Clouds")]
        public bool m_cloudSettings = false;
        public GlobalCloudType m_globalCloudType = GlobalCloudType.Volumetric;
        //Volumetric
        public bool m_useLocalClouds = false;
        public VolumetricClouds.CloudPresets m_cloudPresets = VolumetricClouds.CloudPresets.Custom;
        public AnimationCurve m_volumetricDensityMultiplier;
        public AnimationCurve m_volumetricDensityCurve;
        public AnimationCurve m_volumetricShapeFactor;
        public AnimationCurve m_volumetricShapeScale;
        public AnimationCurve m_volumetricErosionFactor;
        public AnimationCurve m_volumetricErosionScale;
        public VolumetricClouds.CloudErosionNoise m_erosionNoiseType;
        public AnimationCurve m_volumetricErosionCurve;
        public AnimationCurve m_volumetricAmbientOcclusionCurve;
        public AnimationCurve m_volumetricLowestCloudAltitude;
        public AnimationCurve m_volumetricCloudThickness;
        public AnimationCurve m_volumetricAmbientLightProbeDimmer;
        public AnimationCurve m_volumetricSunLightDimmer;
        public AnimationCurve m_volumetricErosionOcclusion;
        public Gradient m_volumetricScatteringTint;
        public AnimationCurve m_volumetricPowderEffectIntensity;
        public AnimationCurve m_volumetricMultiScattering;
        public bool m_volumetricCloudShadows = true;
        public VolumetricClouds.CloudShadowResolution m_volumetricCloudShadowResolution = VolumetricClouds.CloudShadowResolution.Medium256;
        public AnimationCurve m_volumetricCloudShadowOpacity;
        //Procedural
        public CloudLayerType m_cloudLayers = CloudLayerType.Single;
        public CloudResolution m_cloudResolution = CloudResolution.Resolution1024;
        public bool m_useCloudShadows = false;
        public AnimationCurve m_cloudOpacity;
        public Gradient m_cloudTintColor;
        public AnimationCurve m_cloudExposure;
        public AnimationCurve m_cloudWindDirection;
        public AnimationCurve m_cloudWindSpeed;
        public AnimationCurve m_cloudShadowOpacity;
        public bool m_cloudLighting = true;
        public Gradient m_cloudShadowColor;
        public AnimationCurve m_cloudLayerAOpacityR;
        public AnimationCurve m_cloudLayerAOpacityG;
        public AnimationCurve m_cloudLayerAOpacityB;
        public AnimationCurve m_cloudLayerAOpacityA;
        public AnimationCurve m_cloudLayerBOpacityR;
        public AnimationCurve m_cloudLayerBOpacityG;
        public AnimationCurve m_cloudLayerBOpacityB;
        public AnimationCurve m_cloudLayerBOpacityA;
        [Header("Advanced Lighting")]
        public bool m_advancedLightingSettings = false;
        public bool m_useSSGI = false;
        public GeneralQuality m_ssgiQuality = GeneralQuality.Low;
        public bool m_useSSR = false;
        public GeneralQuality m_ssrQuality = GeneralQuality.Medium;
        public bool m_useContactShadows = true;
        public bool m_useMicroShadows = false;
        public AnimationCurve m_generalExposure;
        public AnimationCurve m_ambientIntensity;
        public AnimationCurve m_ambientReflectionIntensity;
        public TimeOfDayLensFlareProfile m_sunLensFlareProfile;
        public TimeOfDayLensFlareProfile m_moonLensFlareProfile;

        /// <summary>
        /// Applies the weather and lerps the values from current to this profile settings
        /// </summary>
        /// <param name="lightData"></param>
        /// <param name="physicallyBasedSky"></param>
        /// <param name="gradientSky"></param>
        /// <param name="exposure"></param>
        /// <param name="fog"></param>
        /// <param name="clouds"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool ReturnFromWeather(HDAdditionalLightData lightData, PhysicallyBasedSky physicallyBasedSky, GradientSky gradientSky, Exposure exposure, Fog fog, CloudLayer clouds, float time, float duration)
        {
            if (duration >= 1f)
            {
                return true;
            }

            //Sun
            if (ValidateSun())
            {
                lightData.color = Color.Lerp(lightData.color, Mathf.CorrelatedColorTemperatureToRGB(m_sunTemperature.Evaluate(time)), duration);
                lightData.intensity = Mathf.Lerp(lightData.intensity, m_sunIntensity.Evaluate(time), duration);
                lightData.volumetricDimmer = Mathf.Lerp(lightData.volumetricDimmer, m_sunVolumetrics.Evaluate(time), duration);
                lightData.volumetricShadowDimmer = Mathf.Lerp(lightData.volumetricShadowDimmer, m_sunVolumetricShadowDimmer.Evaluate(time), duration);
                lightData.lightDimmer = Mathf.Lerp(lightData.lightDimmer, m_sunIntensityMultiplier.Evaluate(time), duration);
            }
            //Sky
            if (ValidateSky())
            {
                //Physically based
                physicallyBasedSky.updateMode.value = EnvironmentUpdateMode.OnChanged;
                //Gradient
                gradientSky.top.value = Color.Lerp(gradientSky.top.value, m_skyTopColor.Evaluate(time), duration);
                gradientSky.middle.value = Color.Lerp(gradientSky.middle.value, m_skyMiddleColor.Evaluate(time), duration);
                gradientSky.bottom.value = Color.Lerp(gradientSky.bottom.value, m_skyBottomColor.Evaluate(time), duration);
                gradientSky.gradientDiffusion.value = Mathf.Lerp(gradientSky.gradientDiffusion.value, m_skyGradientDiffusion.Evaluate(time), duration);
                gradientSky.exposure.value = Mathf.Lerp(gradientSky.exposure.value, m_skyExposureGradient.Evaluate(time), duration);
            }
            //Advanced Lighting
            if (ValidateAdvancedLighting())
            {
                //Exposure
                exposure.fixedExposure.value = Mathf.Lerp(exposure.fixedExposure.value, m_generalExposure.Evaluate(time), duration);
            }
            //Fog
            if (ValidateFog())
            {
                fog.albedo.value = Color.Lerp(fog.albedo.value, m_fogColor.Evaluate(time), duration);
                fog.meanFreePath.value = Mathf.Lerp(fog.meanFreePath.value,m_fogDistance.Evaluate(time), duration);
                fog.baseHeight.value = Mathf.Lerp(fog.baseHeight.value, m_fogHeight.Evaluate(time), duration);
                fog.depthExtent.value = Mathf.Lerp(fog.depthExtent.value, m_volumetricFogDistance.Evaluate(time), duration);
                fog.anisotropy.value = Mathf.Lerp(fog.anisotropy.value, m_volumetricFogAnisotropy.Evaluate(time), duration);
            }
            //Clouds
            if (ValidateClouds())
            {
                clouds.opacity.value = Mathf.Lerp(clouds.opacity.value, m_cloudOpacity.Evaluate(time), duration);
                clouds.layerA.tint.value = Color.Lerp(clouds.layerA.tint.value, m_cloudTintColor.Evaluate(time), duration);
                clouds.layerB.tint.value = Color.Lerp(clouds.layerB.tint.value, m_cloudTintColor.Evaluate(time), duration);
                clouds.layerA.exposure.value = Mathf.Lerp(clouds.layerA.exposure.value, m_cloudExposure.Evaluate(time), duration);
                clouds.layerB.exposure.value = Mathf.Lerp(clouds.layerB.exposure.value, m_cloudExposure.Evaluate(time), duration);
                clouds.shadowMultiplier.value = Mathf.Lerp(clouds.shadowMultiplier.value, m_cloudShadowOpacity.Evaluate(time), duration);
                clouds.shadowTint.value = Color.Lerp(clouds.shadowTint.value, m_cloudShadowColor.Evaluate(time), duration);
                clouds.layerA.opacityR.value = Mathf.Lerp(clouds.layerA.opacityR.value, m_cloudLayerAOpacityR.Evaluate(time), duration);
                clouds.layerA.opacityG.value = Mathf.Lerp(clouds.layerA.opacityG.value, m_cloudLayerAOpacityG.Evaluate(time), duration);
                clouds.layerA.opacityB.value = Mathf.Lerp(clouds.layerA.opacityB.value, m_cloudLayerAOpacityB.Evaluate(time), duration);
                clouds.layerA.opacityA.value = Mathf.Lerp(clouds.layerA.opacityA.value, m_cloudLayerAOpacityA.Evaluate(time), duration);
                clouds.layerB.opacityR.value = Mathf.Lerp(clouds.layerB.opacityR.value, m_cloudLayerBOpacityR.Evaluate(time), duration);
                clouds.layerB.opacityG.value = Mathf.Lerp(clouds.layerB.opacityG.value, m_cloudLayerBOpacityG.Evaluate(time), duration);
                clouds.layerB.opacityB.value = Mathf.Lerp(clouds.layerB.opacityB.value, m_cloudLayerBOpacityB.Evaluate(time), duration);
                clouds.layerB.opacityA.value = Mathf.Lerp(clouds.layerB.opacityA.value, m_cloudLayerBOpacityA.Evaluate(time), duration);
            }

            return false;
        }

        public bool ValidateSun()
        {
            if (m_sunIntensity == null)
            {
                return false;
            }
            if (m_moonIntensity == null)
            {
                return false;
            }
            if (m_sunIntensityMultiplier == null)
            {
                return false;
            }
            if (m_sunTemperature == null)
            {
                return false;
            }
            if (m_sunColorFilter == null)
            {
                return false;
            }
            if (m_moonTemperature == null)
            {
                return false;
            }
            if (m_moonColorFilter == null)
            {
                return false;
            }
            if (m_sunVolumetrics == null)
            {
                return false;
            }
            if (m_sunVolumetricShadowDimmer == null)
            {
                return false;
            }

            return true;
        }
        public void ApplySunSettings(HDAdditionalLightData lightData, float time, bool isDay, OverrideDataInfo overrideData)
        {
            lightData.EnableColorTemperature(true);
            if (isDay)
            {
                lightData.SetIntensity(m_sunIntensity.Evaluate(time));
                lightData.SetColor(m_sunColorFilter.Evaluate(time), m_sunTemperature.Evaluate(time));
            }
            else
            {
                lightData.SetIntensity(m_moonIntensity.Evaluate(time));
                lightData.SetColor(m_moonColorFilter.Evaluate(time), m_moonTemperature.Evaluate(time));
            }
            lightData.affectsVolumetric = true;
            lightData.lightDimmer = m_sunIntensityMultiplier.Evaluate(time);
            bool transitionComplete = overrideData.m_transitionTime < 1f;
            if (!overrideData.m_isInVolue)
            {
                if (transitionComplete)
                {
                    lightData.volumetricDimmer = Mathf.Lerp(lightData.volumetricDimmer, m_sunVolumetrics.Evaluate(time), overrideData.m_transitionTime);
                    lightData.volumetricShadowDimmer = Mathf.Lerp(lightData.volumetricShadowDimmer, m_sunVolumetricShadowDimmer.Evaluate(time), overrideData.m_transitionTime);
                }
                else
                {
                    lightData.volumetricDimmer = m_sunVolumetrics.Evaluate(time);
                    lightData.volumetricShadowDimmer = m_sunVolumetricShadowDimmer.Evaluate(time);
                }
            }
            else
            {
                if (overrideData.m_settings != null)
                {
                    if (overrideData.m_settings.m_sunVolumetric.overrideState)
                    {
                        if (transitionComplete)
                        {
                            lightData.volumetricDimmer = Mathf.Lerp(lightData.volumetricDimmer, overrideData.m_settings.m_sunVolumetric.value, overrideData.m_transitionTime);
                        }
                        else
                        {
                            lightData.volumetricDimmer = overrideData.m_settings.m_sunVolumetric.value;
                        }
                    }
                    else
                    {
                        if (transitionComplete)
                        {
                            lightData.volumetricDimmer = Mathf.Lerp(lightData.volumetricDimmer, m_sunVolumetrics.Evaluate(time), overrideData.m_transitionTime);
                        }
                        else
                        {
                            lightData.volumetricDimmer = m_sunVolumetrics.Evaluate(time);
                        }
                    }

                    if (overrideData.m_settings.m_sunVolumetricDimmer.overrideState)
                    {
                        if (transitionComplete)
                        {
                            lightData.volumetricShadowDimmer = Mathf.Lerp(lightData.volumetricShadowDimmer, overrideData.m_settings.m_sunVolumetricDimmer.value, overrideData.m_transitionTime);
                        }
                        else
                        {
                            lightData.volumetricShadowDimmer = overrideData.m_settings.m_sunVolumetricDimmer.value;
                        }
                    }
                    else
                    {
                        if (transitionComplete)
                        {
                            lightData.volumetricShadowDimmer = Mathf.Lerp(lightData.volumetricShadowDimmer, m_sunVolumetricShadowDimmer.Evaluate(time), overrideData.m_transitionTime);
                        }
                        else
                        {
                            lightData.volumetricShadowDimmer = m_sunVolumetricShadowDimmer.Evaluate(time);
                        }
                    }
                }
            }
        }

        public bool ValidateSky()
        {
            if (m_skyTopColor == null)
            {
                return false;
            }
            if (m_skyMiddleColor == null)
            {
                return false;
            }
            if (m_skyBottomColor == null)
            {
                return false;
            }
            if (m_skyGradientDiffusion == null)
            {
                return false;
            }
            return true;
        }
        public void ApplySkySettings(PhysicallyBasedSky physicallyBasedSky, GradientSky gradientSky, float time)
        {
            //Physically based
            physicallyBasedSky.updateMode.value = EnvironmentUpdateMode.OnChanged;
            //Gradient
            gradientSky.top.value = m_skyTopColor.Evaluate(time);
            gradientSky.middle.value = m_skyMiddleColor.Evaluate(time);
            gradientSky.bottom.value = m_skyBottomColor.Evaluate(time);
            gradientSky.gradientDiffusion.value = m_skyGradientDiffusion.Evaluate(time);
            gradientSky.exposure.value = m_skyExposureGradient.Evaluate(time);
        }

        public bool ValidateAdvancedLighting()
        {
            if (m_generalExposure == null)
            {
                return false;
            }
            if (m_ambientIntensity == null)
            {
                m_ambientIntensity = AnimationCurve.Constant(0f, 1f, 1f);
                return false;
            }
            if (m_ambientReflectionIntensity == null)
            {
                m_ambientReflectionIntensity = AnimationCurve.Constant(0f, 1f, 1f);
                return false;
            }

            return true;
        }
        public void ApplyAdvancedLighting(GlobalIllumination gi, Exposure exposure, ScreenSpaceReflection screenSpaceReflection, ScreenSpaceRefraction screenSpaceRefraction, ContactShadows contactShadows, MicroShadowing microShadowing, IndirectLightingController indirectLightingController, float time, OverrideDataInfo overrideData)
        {
            bool transitionComplete = overrideData.m_transitionTime < 1f;
            //SSGI
            gi.active = true;
            gi.enable.value = m_useSSGI;
            gi.quality.value = (int)m_ssgiQuality;
            //SSR
            screenSpaceReflection.active = true;
            screenSpaceReflection.enabled.value = m_useSSR;
            screenSpaceRefraction.active = m_useSSR;
            screenSpaceReflection.quality.value = (int)m_ssrQuality;
            //Exposure
            if (!overrideData.m_isInVolue)
            {
                if (transitionComplete)
                {
                    exposure.fixedExposure.value = Mathf.Lerp(exposure.fixedExposure.value, m_generalExposure.Evaluate(time), overrideData.m_transitionTime);
                }
                else
                {
                    exposure.fixedExposure.value = m_generalExposure.Evaluate(time);
                }
            }
            else
            {
                if (overrideData.m_settings.m_exposure.overrideState)
                {
                    if (transitionComplete)
                    {
                        exposure.fixedExposure.value = Mathf.Lerp(exposure.fixedExposure.value, overrideData.m_settings.m_exposure.value, overrideData.m_transitionTime);
                    }
                    else
                    {
                        exposure.fixedExposure.value = overrideData.m_settings.m_exposure.value;
                    }
                }
                else
                {
                    if (transitionComplete)
                    {
                        exposure.fixedExposure.value = Mathf.Lerp(exposure.fixedExposure.value, m_generalExposure.Evaluate(time), overrideData.m_transitionTime);
                    }
                    else
                    {
                        exposure.fixedExposure.value = m_generalExposure.Evaluate(time);
                    }
                }
            }
            //Shadows
            contactShadows.active = true;
            contactShadows.enable.value = m_useContactShadows;
            microShadowing.active = true;
            microShadowing.enable.value = m_useMicroShadows;
            //Ambient
            if (!overrideData.m_isInVolue)
            {
                if (transitionComplete)
                {
                    indirectLightingController.indirectDiffuseLightingMultiplier.value = Mathf.Lerp(indirectLightingController.indirectDiffuseLightingMultiplier.value, m_ambientIntensity.Evaluate(time), overrideData.m_transitionTime);
                    indirectLightingController.reflectionLightingMultiplier.value = Mathf.Lerp(indirectLightingController.reflectionLightingMultiplier.value, m_ambientReflectionIntensity.Evaluate(time), overrideData.m_transitionTime);
                }
                else
                {
                    indirectLightingController.indirectDiffuseLightingMultiplier.value = m_ambientIntensity.Evaluate(time);
                    indirectLightingController.reflectionLightingMultiplier.value = m_ambientReflectionIntensity.Evaluate(time);
                }
            }
            else
            {
                if (overrideData.m_settings.m_ambientIntensity.overrideState)
                {
                    if (transitionComplete)
                    {
                        indirectLightingController.indirectDiffuseLightingMultiplier.value = Mathf.Lerp(indirectLightingController.indirectDiffuseLightingMultiplier.value, overrideData.m_settings.m_ambientIntensity.value, overrideData.m_transitionTime);
                    }
                    else
                    {
                        indirectLightingController.indirectDiffuseLightingMultiplier.value = overrideData.m_settings.m_ambientIntensity.value;
                    }
                }
                else
                {
                    if (transitionComplete)
                    {
                        indirectLightingController.indirectDiffuseLightingMultiplier.value = Mathf.Lerp(indirectLightingController.indirectDiffuseLightingMultiplier.value, m_ambientIntensity.Evaluate(time), overrideData.m_transitionTime);
                    }
                    else
                    {
                        indirectLightingController.indirectDiffuseLightingMultiplier.value = m_ambientIntensity.Evaluate(time);
                    }
                }
                
                if (overrideData.m_settings.m_ambientReflectionIntensity.overrideState)
                {
                    if (transitionComplete)
                    {
                        indirectLightingController.reflectionLightingMultiplier.value = Mathf.Lerp(indirectLightingController.reflectionLightingMultiplier.value, overrideData.m_settings.m_ambientReflectionIntensity.value, overrideData.m_transitionTime);
                    }
                    else
                    {
                        indirectLightingController.reflectionLightingMultiplier.value = overrideData.m_settings.m_ambientReflectionIntensity.value;
                    }
                }
                else
                {
                    if (transitionComplete)
                    {
                        indirectLightingController.reflectionLightingMultiplier.value = Mathf.Lerp(indirectLightingController.reflectionLightingMultiplier.value, m_ambientReflectionIntensity.Evaluate(time), overrideData.m_transitionTime);
                    }
                    else
                    {
                        indirectLightingController.reflectionLightingMultiplier.value = m_ambientReflectionIntensity.Evaluate(time);
                    }
                }
            }
        }

        public bool ValidateFog()
        {
            if (m_fogColor == null)
            {
                return false;
            }
            if (m_fogDistance == null)
            {
                return false;
            }
            if (m_fogHeight == null)
            {
                return false;
            }
            if (m_volumetricFogDistance == null)
            {
                return false;
            }
            if (m_volumetricFogAnisotropy == null)
            {
                return false;
            }
            if (m_volumetricFogSliceDistributionUniformity == null)
            {
                return false;
            }
            if (m_localFogMultiplier == null)
            {
                return false;
            }
            if (m_fogDensity == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyFogSettings(Fog fog, LocalVolumetricFog localVolumetricFog, float time)
        {
            localVolumetricFog.parameters.meanFreePath = m_fogDensity.Evaluate(time);
            localVolumetricFog.parameters.albedo = m_fogColor.Evaluate(time) * m_localFogMultiplier.Evaluate(time);
            fog.albedo.value = m_fogColor.Evaluate(time);
            fog.meanFreePath.value = m_fogDistance.Evaluate(time);
            float fogHeight = m_fogHeight.Evaluate(time);
            fog.baseHeight.value = fogHeight;
            fog.maximumHeight.value = fogHeight * 2f;
            fog.depthExtent.value = m_volumetricFogDistance.Evaluate(time);
            fog.anisotropy.value = m_volumetricFogAnisotropy.Evaluate(time);
            fog.sliceDistributionUniformity.value = m_volumetricFogSliceDistributionUniformity.Evaluate(time);
            fog.quality.value = (int)m_fogQuality;
            if (m_useDenoising)
            {
                switch (m_denoisingQuality)
                {
                    case GeneralQuality.Low:
                    {
                        fog.denoisingMode.value = FogDenoisingMode.Reprojection;
                        break;
                    }
                    case GeneralQuality.Medium:
                    {
                        fog.denoisingMode.value = FogDenoisingMode.Gaussian;
                        break;
                    }
                    case GeneralQuality.High:
                    {
                        fog.denoisingMode.value = FogDenoisingMode.Both;
                        break;
                    }
                }
            }
            else
            {
                fog.denoisingMode.value = FogDenoisingMode.None;
            }
        }

        public bool ValidateShadows()
        {
            if (m_shadowDistance == null)
            {
                return false;
            }
            if (m_shadowTransmissionMultiplier == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyShadowSettings(HDShadowSettings shadows, float time)
        {
            shadows.maxShadowDistance.value = (m_shadowDistance.Evaluate(time) * m_shadowDistanceMultiplier);
            shadows.directionalTransmissionMultiplier.value = m_shadowTransmissionMultiplier.Evaluate(time);
            shadows.cascadeShadowSplitCount.value = Mathf.Clamp(m_shadowCascadeCount, 1, 4);
        }

        public bool ValidateClouds()
        {
            //Procedural
            if (m_cloudOpacity == null)
            {
                return false;
            }
            if (m_cloudTintColor == null)
            {
                return false;
            }
            if (m_cloudExposure == null)
            {
                return false;
            }
            if (m_cloudWindDirection == null)
            {
                return false;
            }
            if (m_cloudWindSpeed == null)
            {
                return false;
            }
            if (m_cloudShadowOpacity == null)
            {
                return false;
            }
            if (m_cloudShadowColor == null)
            {
                return false;
            }
            if (m_cloudLayerAOpacityR == null)
            {
                return false;
            }
            if (m_cloudLayerAOpacityG == null)
            {
                return false;
            }
            if (m_cloudLayerAOpacityB == null)
            {
                return false;
            }
            if (m_cloudLayerAOpacityA == null)
            {
                return false;
            }
            if (m_cloudLayerBOpacityR == null)
            {
                return false;
            }
            if (m_cloudLayerBOpacityG == null)
            {
                return false;
            }
            if (m_cloudLayerBOpacityB == null)
            {
                return false;
            }
            if (m_cloudLayerBOpacityA == null)
            {
                return false;
            }
            //Volumetic
            if (m_volumetricDensityMultiplier == null)
            {
                return false;
            }
            if (m_volumetricDensityCurve == null)
            {
                return false;
            }
            if (m_volumetricShapeFactor == null)
            {
                return false;
            }
            if (m_volumetricShapeScale == null)
            {
                return false;
            }
            if (m_volumetricErosionFactor == null)
            {
                return false;
            }
            if (m_volumetricErosionScale == null)
            {
                return false;
            }
            if (m_volumetricErosionCurve == null)
            {
                return false;
            }
            if (m_volumetricAmbientOcclusionCurve == null)
            {
                return false;
            }
            if (m_volumetricAmbientLightProbeDimmer == null)
            {
                return false;
            }
            if (m_volumetricSunLightDimmer == null)
            {
                return false;
            }
            if (m_volumetricErosionOcclusion == null)
            {
                return false;
            }
            if (m_volumetricScatteringTint == null)
            {
                return false;
            }
            if (m_volumetricPowderEffectIntensity == null)
            {
                return false;
            }
            if (m_volumetricMultiScattering == null)
            {
                return false;
            }
            if (m_volumetricCloudShadowOpacity == null)
            {
                return false;
            }

            return true;
        }
        public float ApplyCloudSettings(VolumetricClouds volumetricClouds, CloudLayer clouds, VisualEnvironment visualEnvironment, float time)
        {
            #region Volumetric

            volumetricClouds.cloudPreset.value = m_cloudPresets;
            volumetricClouds.localClouds.value = m_useLocalClouds;
            volumetricClouds.densityMultiplier.value = m_volumetricDensityMultiplier.Evaluate(time);
            volumetricClouds.customDensityCurve.value = m_volumetricDensityCurve;
            volumetricClouds.densityMultiplier.value = m_volumetricDensityMultiplier.Evaluate(time);
            volumetricClouds.shapeFactor.value = m_volumetricShapeFactor.Evaluate(time);
            volumetricClouds.shapeScale.value = m_volumetricShapeScale.Evaluate(time);
            volumetricClouds.erosionFactor.value = m_volumetricErosionFactor.Evaluate(time);
            volumetricClouds.erosionScale.value = m_volumetricErosionScale.Evaluate(time);
            volumetricClouds.erosionNoiseType.value = m_erosionNoiseType;
            volumetricClouds.customErosionCurve.value = m_volumetricErosionCurve;
            volumetricClouds.customAmbientOcclusionCurve.value = m_volumetricAmbientOcclusionCurve;
            volumetricClouds.lowestCloudAltitude.value = m_volumetricLowestCloudAltitude.Evaluate(time);
            volumetricClouds.cloudThickness.value = m_volumetricCloudThickness.Evaluate(time);
            //Lighting
            volumetricClouds.ambientLightProbeDimmer.value = m_volumetricAmbientLightProbeDimmer.Evaluate(time);
            volumetricClouds.sunLightDimmer.value = m_volumetricSunLightDimmer.Evaluate(time);
            volumetricClouds.erosionOcclusion.value = m_volumetricErosionOcclusion.Evaluate(time);
            volumetricClouds.scatteringTint.value = m_volumetricScatteringTint.Evaluate(time);
            volumetricClouds.powderEffectIntensity.value = m_volumetricPowderEffectIntensity.Evaluate(time);
            volumetricClouds.multiScattering.value = m_volumetricMultiScattering.Evaluate(time);
            //Shadow
            volumetricClouds.shadows.value = m_volumetricCloudShadows;
            if (m_volumetricCloudShadows)
            {
                volumetricClouds.shadowResolution.value = m_volumetricCloudShadowResolution;
                volumetricClouds.shadowOpacity.value = m_volumetricCloudShadowOpacity.Evaluate(time);
            }

            #endregion
            #region Procedural

            clouds.layers.value = (CloudMapMode)m_cloudLayers;
            switch (m_cloudResolution)
            {
                case CloudResolution.Resolution256:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution256;
                    break;
                }
                case CloudResolution.Resolution512:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution512;
                    break;
                }
                case CloudResolution.Resolution1024:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution1024;
                    break;
                }
                case CloudResolution.Resolution2048:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution2048;
                    break;
                }
                case CloudResolution.Resolution4096:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution4096;
                    break;
                }
                case CloudResolution.Resolution8192:
                {
                    clouds.resolution.value = UnityEngine.Rendering.HighDefinition.CloudResolution.CloudResolution8192;
                    break;
                }
            }
            clouds.opacity.value = m_cloudOpacity.Evaluate(time);
            clouds.layerA.castShadows.value = m_useCloudShadows;
            clouds.layerA.lighting.value = m_cloudLighting;
            clouds.layerB.castShadows.value = m_useCloudShadows;
            clouds.layerB.lighting.value = m_cloudLighting;
            clouds.layerA.tint.value = m_cloudTintColor.Evaluate(time);
            clouds.layerB.tint.value = m_cloudTintColor.Evaluate(time);
            clouds.layerA.exposure.value = m_cloudExposure.Evaluate(time);
            clouds.layerB.exposure.value = m_cloudExposure.Evaluate(time);
            clouds.shadowMultiplier.value = m_cloudShadowOpacity.Evaluate(time);
            clouds.shadowTint.value = m_cloudShadowColor.Evaluate(time);
            clouds.layerA.opacityR.value = m_cloudLayerAOpacityR.Evaluate(time);
            clouds.layerA.opacityG.value = m_cloudLayerAOpacityG.Evaluate(time);
            clouds.layerA.opacityB.value = m_cloudLayerAOpacityB.Evaluate(time);
            clouds.layerA.opacityA.value = m_cloudLayerAOpacityA.Evaluate(time);
            clouds.layerB.opacityR.value = m_cloudLayerBOpacityR.Evaluate(time);
            clouds.layerB.opacityG.value = m_cloudLayerBOpacityG.Evaluate(time);
            clouds.layerB.opacityB.value = m_cloudLayerBOpacityB.Evaluate(time);
            clouds.layerB.opacityA.value = m_cloudLayerBOpacityA.Evaluate(time);
            visualEnvironment.windOrientation.value = Mathf.Clamp01(m_cloudWindDirection.Evaluate(time)) * 360f;
            visualEnvironment.windSpeed.value = m_cloudWindSpeed.Evaluate(time);
            return clouds.opacity.value;

            #endregion
        }

        public bool ValidateSunLensFlare()
        {
            if (m_sunLensFlareProfile.m_lensFlareData == null)
            {
                return false;
            }
            if (m_sunLensFlareProfile.m_intensity == null)
            {
                return false;
            }
            if (m_sunLensFlareProfile.m_scale == null)
            {
                return false;
            }

            return true;
        }
        public void ApplySunLensFlare(LensFlareComponentSRP lensFlare, float time, bool isDay)
        {
            if (isDay)
            {
                lensFlare.enabled = m_sunLensFlareProfile.m_useLensFlare;
                if (m_sunLensFlareProfile.m_useLensFlare)
                {
                    lensFlare.lensFlareData = m_sunLensFlareProfile.m_lensFlareData;
                    lensFlare.intensity = m_sunLensFlareProfile.m_intensity.Evaluate(time);
                    lensFlare.scale = m_sunLensFlareProfile.m_scale.Evaluate(time);
                    lensFlare.useOcclusion = m_sunLensFlareProfile.m_enableOcclusion;
                    lensFlare.occlusionRadius = m_sunLensFlareProfile.m_occlusionRadius;
                    lensFlare.sampleCount = (uint) m_sunLensFlareProfile.m_sampleCount;
                    lensFlare.occlusionOffset = m_sunLensFlareProfile.m_occlusionOffset;
                    lensFlare.allowOffScreen = m_sunLensFlareProfile.m_allowOffScreen;
                }
            }
            else
            {
                lensFlare.enabled = false;
            }
        }

        public bool ValidateMoonLensFlare()
        {
            if (m_moonLensFlareProfile.m_lensFlareData == null)
            {
                return false;
            }
            if (m_moonLensFlareProfile.m_intensity == null)
            {
                return false;
            }
            if (m_moonLensFlareProfile.m_scale == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyMoonLensFlare(LensFlareComponentSRP lensFlare, float time, bool isDay)
        {
            if (!isDay)
            {
                lensFlare.enabled = m_moonLensFlareProfile.m_useLensFlare;
                if (m_moonLensFlareProfile.m_useLensFlare)
                {
                    lensFlare.lensFlareData = m_moonLensFlareProfile.m_lensFlareData;
                    lensFlare.intensity = m_moonLensFlareProfile.m_intensity.Evaluate(time);
                    lensFlare.scale = m_moonLensFlareProfile.m_scale.Evaluate(time);
                    lensFlare.useOcclusion = m_moonLensFlareProfile.m_enableOcclusion;
                    lensFlare.occlusionRadius = m_moonLensFlareProfile.m_occlusionRadius;
                    lensFlare.sampleCount = (uint)m_moonLensFlareProfile.m_sampleCount;
                    lensFlare.occlusionOffset = m_moonLensFlareProfile.m_occlusionOffset;
                    lensFlare.allowOffScreen = m_moonLensFlareProfile.m_allowOffScreen;
                }
            }
            else
            {
                lensFlare.enabled = false;
            }
        }
    }
    [System.Serializable]
    public class RayTraceSettings
    {
        public bool m_rayTraceSettings = false;
        public bool m_rayTraceSSGI = false;
        public GeneralRenderMode m_ssgiRenderMode = GeneralRenderMode.Performance;
        public GeneralQuality m_ssgiQuality = GeneralQuality.High;
        public bool m_rayTraceSSR = true;
        public GeneralRenderMode m_ssrRenderMode = GeneralRenderMode.Performance;
        public GeneralQuality m_ssrQuality = GeneralQuality.High;
        public bool m_rayTraceAmbientOcclusion = false;
        public GeneralQuality m_aoQuality = GeneralQuality.High;
        public bool m_recursiveRendering = true;
        public bool m_rayTraceSubSurfaceScattering = false;
        public int m_subSurfaceScatteringSampleCount = 2;
    }

    public class HDRPTimeOfDayProfile : ScriptableObject
    {
        public TimeOfDayProfileData TimeOfDayData
        {
            get { return m_timeOfDayData; }
            set
            {
                if (m_timeOfDayData != value)
                {
                    m_timeOfDayData = value;
                }
            }
        }
        [SerializeField] private TimeOfDayProfileData m_timeOfDayData = new TimeOfDayProfileData();

        public RayTraceSettings RayTracingSettings
        {
            get { return m_rayTracingSettings; }
            set
            {
                if (m_rayTracingSettings != value)
                {
                    m_rayTracingSettings = value;
                }
            }
        }
        [SerializeField] private RayTraceSettings m_rayTracingSettings = new RayTraceSettings();
    }
}
#endif