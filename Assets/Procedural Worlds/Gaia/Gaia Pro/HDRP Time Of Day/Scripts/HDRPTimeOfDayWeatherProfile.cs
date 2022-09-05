#if HDPipeline && UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace ProceduralWorlds.HDRPTOD
{
    public enum WeatherVFXState { InActive, Active, FadeIn, FadeOut }

    public interface IHDRPWeatherVFX
    {
        void SetWeatherProfile(HDRPTimeOfDayWeatherProfile profile);
        void StartWeatherFX();
        void StopWeatherFX();
    }

    [System.Serializable]
    public class HDRPTimeOfDayWeatherProfileData
    {
        public string m_weatherName;
        public float m_transitionDuration = 10f;
        public float m_weatherDuration = 120f;
        public TimeOfDayProfileData m_weatherData;

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
        public bool ApplyWeather(HDAdditionalLightData lightData, PhysicallyBasedSky physicallyBasedSky, GradientSky gradientSky, Exposure exposure, Fog fog, CloudLayer clouds, float time, float duration)
        {
            if (duration >= 1f)
            {
                return true;
            }

            //Sun
            if (ValidateSun())
            {
                lightData.color = Color.Lerp(lightData.color, Mathf.CorrelatedColorTemperatureToRGB(m_weatherData.m_sunTemperature.Evaluate(time)), duration);
                lightData.intensity = Mathf.Lerp(lightData.intensity, m_weatherData.m_sunIntensity.Evaluate(time), duration);
                lightData.volumetricDimmer = Mathf.Lerp(lightData.volumetricDimmer, m_weatherData.m_sunVolumetrics.Evaluate(time), duration);
                lightData.volumetricShadowDimmer = Mathf.Lerp(lightData.volumetricShadowDimmer, m_weatherData.m_sunVolumetricShadowDimmer.Evaluate(time), duration);
                lightData.lightDimmer = Mathf.Lerp(lightData.lightDimmer, m_weatherData.m_sunIntensityMultiplier.Evaluate(time), duration);
            }
            //Sky
            if (ValidateSky())
            {
                //Gradient
                gradientSky.top.value = Color.Lerp(gradientSky.top.value, m_weatherData.m_skyTopColor.Evaluate(time), duration);
                gradientSky.middle.value = Color.Lerp(gradientSky.middle.value, m_weatherData.m_skyMiddleColor.Evaluate(time), duration);
                gradientSky.bottom.value = Color.Lerp(gradientSky.bottom.value, m_weatherData.m_skyBottomColor.Evaluate(time), duration);
                gradientSky.gradientDiffusion.value = Mathf.Lerp(gradientSky.gradientDiffusion.value, m_weatherData.m_skyGradientDiffusion.Evaluate(time), duration);
                gradientSky.exposure.value = Mathf.Lerp(gradientSky.exposure.value, m_weatherData.m_skyExposureGradient.Evaluate(time), duration);
            }
            //Advanced Lighting
            if (ValidateAdvancedLighting())
            {
                //Exposure
                exposure.fixedExposure.value = Mathf.Lerp(exposure.fixedExposure.value, m_weatherData.m_generalExposure.Evaluate(time), duration);
            }
            //Fog
            if (ValidateFog())
            {
                fog.albedo.value = Color.Lerp(fog.albedo.value, m_weatherData.m_fogColor.Evaluate(time), duration);
                fog.meanFreePath.value = Mathf.Lerp(fog.meanFreePath.value,m_weatherData.m_fogDistance.Evaluate(time), duration);
                fog.baseHeight.value = Mathf.Lerp(fog.baseHeight.value, m_weatherData.m_fogHeight.Evaluate(time), duration);
                fog.depthExtent.value = Mathf.Lerp(fog.depthExtent.value, m_weatherData.m_volumetricFogDistance.Evaluate(time), duration);
                fog.anisotropy.value = Mathf.Lerp(fog.anisotropy.value, m_weatherData.m_volumetricFogAnisotropy.Evaluate(time), duration);
            }
            //Clouds
            if (ValidateClouds())
            {
                clouds.opacity.value = Mathf.Lerp(clouds.opacity.value, m_weatherData.m_cloudOpacity.Evaluate(time), duration);
                clouds.layerA.tint.value = Color.Lerp(clouds.layerA.tint.value, m_weatherData.m_cloudTintColor.Evaluate(time), duration);
                clouds.layerB.tint.value = Color.Lerp(clouds.layerB.tint.value, m_weatherData.m_cloudTintColor.Evaluate(time), duration);
                clouds.layerA.exposure.value = Mathf.Lerp(clouds.layerA.exposure.value, m_weatherData.m_cloudExposure.Evaluate(time), duration);
                clouds.layerB.exposure.value = Mathf.Lerp(clouds.layerB.exposure.value, m_weatherData.m_cloudExposure.Evaluate(time), duration);
                //clouds.layerA.scrollDirection.value = Mathf.Lerp(clouds.layerA.scrollDirection.value, m_weatherData.m_cloudWindDirection.Evaluate(time), duration);
                //clouds.layerB.scrollDirection.value = Mathf.Lerp(clouds.layerB.scrollDirection.value, m_weatherData.m_cloudWindDirection.Evaluate(time) / 2f, duration);
                //clouds.layerA.scrollSpeed.value = Mathf.Lerp(clouds.layerA.scrollSpeed.value, m_weatherData.m_cloudWindSpeed.Evaluate(time), duration);
                //clouds.layerB.scrollSpeed.value = Mathf.Lerp(clouds.layerB.scrollSpeed.value, m_weatherData.m_cloudWindSpeed.Evaluate(time) / 2f, duration);
                clouds.shadowMultiplier.value = Mathf.Lerp(clouds.shadowMultiplier.value, m_weatherData.m_cloudShadowOpacity.Evaluate(time), duration);
                clouds.shadowTint.value = Color.Lerp(clouds.shadowTint.value, m_weatherData.m_cloudShadowColor.Evaluate(time), duration);
                clouds.layerA.opacityR.value = Mathf.Lerp(clouds.layerA.opacityR.value, m_weatherData.m_cloudLayerAOpacityR.Evaluate(time), duration);
                clouds.layerA.opacityG.value = Mathf.Lerp(clouds.layerA.opacityG.value, m_weatherData.m_cloudLayerAOpacityG.Evaluate(time), duration);
                clouds.layerA.opacityB.value = Mathf.Lerp(clouds.layerA.opacityB.value, m_weatherData.m_cloudLayerAOpacityB.Evaluate(time), duration);
                clouds.layerA.opacityA.value = Mathf.Lerp(clouds.layerA.opacityA.value, m_weatherData.m_cloudLayerAOpacityA.Evaluate(time), duration);
                clouds.layerB.opacityR.value = Mathf.Lerp(clouds.layerB.opacityR.value, m_weatherData.m_cloudLayerBOpacityR.Evaluate(time), duration);
                clouds.layerB.opacityG.value = Mathf.Lerp(clouds.layerB.opacityG.value, m_weatherData.m_cloudLayerBOpacityG.Evaluate(time), duration);
                clouds.layerB.opacityB.value = Mathf.Lerp(clouds.layerB.opacityB.value, m_weatherData.m_cloudLayerBOpacityB.Evaluate(time), duration);
                clouds.layerB.opacityA.value = Mathf.Lerp(clouds.layerB.opacityA.value, m_weatherData.m_cloudLayerBOpacityA.Evaluate(time), duration);
            }

            return false;
        }

        public bool ValidateSun()
        {
            if (m_weatherData.m_sunIntensity == null)
            {
                return false;
            }
            if (m_weatherData.m_sunIntensityMultiplier == null)
            {
                return false;
            }
            if (m_weatherData.m_sunTemperature == null)
            {
                return false;
            }
            if (m_weatherData.m_sunVolumetrics == null)
            {
                return false;
            }
            if (m_weatherData.m_sunVolumetricShadowDimmer == null)
            {
                return false;
            }

            return true;
        }
        public void ApplySunSettings(HDAdditionalLightData lightData, float time)
        {
            lightData.SetColor(Color.white, m_weatherData.m_sunTemperature.Evaluate(time));
            lightData.SetIntensity(m_weatherData.m_sunIntensity.Evaluate(time));
            lightData.affectsVolumetric = true;
            lightData.volumetricDimmer = m_weatherData.m_sunVolumetrics.Evaluate(time);
            lightData.volumetricShadowDimmer = m_weatherData.m_sunVolumetricShadowDimmer.Evaluate(time);
            lightData.lightDimmer = m_weatherData.m_sunIntensityMultiplier.Evaluate(time);
        }

        public bool ValidateSky()
        {
            if (m_weatherData.m_skyTopColor == null)
            {
                return false;
            }
            if (m_weatherData.m_skyMiddleColor == null)
            {
                return false;
            }
            if (m_weatherData.m_skyBottomColor == null)
            {
                return false;
            }
            if (m_weatherData.m_skyGradientDiffusion == null)
            {
                return false;
            }
            return true;
        }
        public void ApplySkySettings(PhysicallyBasedSky physicallyBasedSky, GradientSky gradientSky, float time)
        {
            //Gradient
            gradientSky.top.value = m_weatherData.m_skyTopColor.Evaluate(time);
            gradientSky.middle.value = m_weatherData.m_skyMiddleColor.Evaluate(time);
            gradientSky.bottom.value = m_weatherData.m_skyBottomColor.Evaluate(time);
            gradientSky.gradientDiffusion.value = m_weatherData.m_skyGradientDiffusion.Evaluate(time);
            gradientSky.exposure.value = m_weatherData.m_skyExposureGradient.Evaluate(time);
        }

        public bool ValidateAdvancedLighting()
        {
            if (m_weatherData.m_generalExposure == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyAdvancedLighting(GlobalIllumination gi, Exposure exposure, ScreenSpaceReflection screenSpaceReflection, ScreenSpaceRefraction screenSpaceRefraction, float time)
        {
            //SSGI
            gi.active = m_weatherData.m_useSSGI;
            gi.enable.value = m_weatherData.m_useSSGI;
            //SSR
            screenSpaceReflection.active = m_weatherData.m_useSSR;
            screenSpaceReflection.enabled.value = m_weatherData.m_useSSR;
            screenSpaceRefraction.active = m_weatherData.m_useSSR;
            //Exposure
            exposure.fixedExposure.value = m_weatherData.m_generalExposure.Evaluate(time);
        }

        public bool ValidateFog()
        {
            if (m_weatherData.m_fogColor == null)
            {
                return false;
            }
            if (m_weatherData.m_fogDistance == null)
            {
                return false;
            }
            if (m_weatherData.m_fogHeight == null)
            {
                return false;
            }
            if (m_weatherData.m_volumetricFogDistance == null)
            {
                return false;
            }
            if (m_weatherData.m_volumetricFogAnisotropy == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyFogSettings(Fog fog, float time)
        {
            fog.albedo.value = m_weatherData.m_fogColor.Evaluate(time);
            fog.meanFreePath.value = m_weatherData.m_fogDistance.Evaluate(time);
            fog.baseHeight.value = m_weatherData.m_fogHeight.Evaluate(time);
            fog.depthExtent.value = m_weatherData.m_volumetricFogDistance.Evaluate(time);
            fog.anisotropy.value = m_weatherData.m_volumetricFogAnisotropy.Evaluate(time);
        }

        public bool ValidateClouds()
        {
            if (m_weatherData.m_cloudOpacity == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudTintColor == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudExposure == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudWindDirection == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudWindSpeed == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudShadowOpacity == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudShadowColor == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerAOpacityR == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerAOpacityG == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerAOpacityB == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerAOpacityA == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerBOpacityR == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerBOpacityG == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerBOpacityB == null)
            {
                return false;
            }
            if (m_weatherData.m_cloudLayerBOpacityA == null)
            {
                return false;
            }

            return true;
        }
        public void ApplyCloudSettings(CloudLayer clouds, float time)
        {
            clouds.layers.value = (CloudMapMode)m_weatherData.m_cloudLayers;
            switch (m_weatherData.m_cloudResolution)
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
            clouds.opacity.value = m_weatherData.m_cloudOpacity.Evaluate(time);
            clouds.layerA.castShadows.value = m_weatherData.m_useCloudShadows;
            clouds.layerB.castShadows.value = m_weatherData.m_useCloudShadows;
            clouds.layerA.tint.value = m_weatherData.m_cloudTintColor.Evaluate(time);
            clouds.layerB.tint.value = m_weatherData.m_cloudTintColor.Evaluate(time);
            clouds.layerA.exposure.value = m_weatherData.m_cloudExposure.Evaluate(time);
            clouds.layerB.exposure.value = m_weatherData.m_cloudExposure.Evaluate(time);
            clouds.shadowMultiplier.value = m_weatherData.m_cloudShadowOpacity.Evaluate(time);
            clouds.shadowTint.value = m_weatherData.m_cloudShadowColor.Evaluate(time);
            clouds.layerA.opacityR.value = m_weatherData.m_cloudLayerAOpacityR.Evaluate(time);
            clouds.layerA.opacityG.value = m_weatherData.m_cloudLayerAOpacityG.Evaluate(time);
            clouds.layerA.opacityB.value = m_weatherData.m_cloudLayerAOpacityB.Evaluate(time);
            clouds.layerA.opacityA.value = m_weatherData.m_cloudLayerAOpacityA.Evaluate(time);
            clouds.layerB.opacityR.value = m_weatherData.m_cloudLayerBOpacityR.Evaluate(time);
            clouds.layerB.opacityG.value = m_weatherData.m_cloudLayerBOpacityG.Evaluate(time);
            clouds.layerB.opacityB.value = m_weatherData.m_cloudLayerBOpacityB.Evaluate(time);
            clouds.layerB.opacityA.value = m_weatherData.m_cloudLayerBOpacityA.Evaluate(time);
        }
    }

    [System.Serializable]
    public class WeatherEffectsData
    {
        public GameObject m_weatherEffect;
        public AudioClip m_weatherAudio;
        public float m_maxVolume = 1f;
    }

    [System.Serializable]
    public class ThunderData
    {
        public Light m_thunderLightSource;
        public Color m_thunderLight = Color.cyan;
        public float m_intesity = 40000f;
        public Vector2Int m_thunderStrikeCountMinMax = new Vector2Int(1, 5);
        public float m_pauseBetweenStrike = 0.15f;
        public List<AudioClip> m_thunderStrikeSounds = new List<AudioClip>();
        public float m_volume = 0.5f;
    }

    public class HDRPTimeOfDayWeatherProfile : ScriptableObject
    {
        public HDRPTimeOfDayWeatherProfileData WeatherData
        {
            get { return m_weatherData; }
            set
            {
                if (m_weatherData != value)
                {
                    m_weatherData = value;
                }
            }
        }
        [SerializeField] private HDRPTimeOfDayWeatherProfileData m_weatherData = new HDRPTimeOfDayWeatherProfileData();

        public WeatherEffectsData WeatherFXData
        {
            get { return m_weatherFXData; }
            set
            {
                if (m_weatherFXData != value)
                {
                    m_weatherFXData = value;
                }
            }
        }
        [SerializeField] private WeatherEffectsData m_weatherFXData = new WeatherEffectsData();
    }
}
#endif