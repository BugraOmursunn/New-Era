using UnityEditor;
using UnityEngine;
#if HDPipeline
using UnityEngine.Rendering.HighDefinition;
#endif

namespace ProceduralWorlds.HDRPTOD
{
    [System.Serializable]
    public class TimeOfDayPostFXProfileData
    {
        public bool m_colorGradingSettings = false;
        public bool m_bloomSettings = false;
        public bool m_shadowToningSettings = false;
        public bool m_vignetteSettings = false;
        public bool m_ambientOcclusionSettings = false;
        public AnimationCurve m_contrast;
        public Gradient m_colorFilter;
        public AnimationCurve m_saturation;
        public AnimationCurve m_temperature;
        public AnimationCurve m_tint;
        public AnimationCurve m_bloomThreshold;
        public AnimationCurve m_bloomIntensity;
        public AnimationCurve m_bloomScatter;
        public Gradient m_bloomTint;
        public Gradient m_shadows;
        public Gradient m_highlights;
        public AnimationCurve m_shadowBalance;
        public Gradient m_vignetteColor;
        public AnimationCurve m_vignetteIntensity;
        public AnimationCurve m_vignetteSmoothness;
        public AnimationCurve m_ambientIntensity;
        public AnimationCurve m_ambientDirectStrength;
        public AnimationCurve m_ambientRadius;

        public bool ValidateColorGrading()
        {
            if (m_contrast == null)
            {
                return false;
            }
            if (m_colorFilter == null)
            {
                return false;
            }
            if (m_saturation == null)
            {
                return false;
            }
            if (m_temperature == null)
            {
                return false;
            }
            if (m_tint == null)
            {
                return false;
            }


            return true;
        }
#if HDPipeline
        public void ApplyColorGradingSettings(ColorAdjustments colorAdjustments, WhiteBalance whiteBalance, float time)
        {
            colorAdjustments.contrast.value = m_contrast.Evaluate(time);
            colorAdjustments.colorFilter.value = m_colorFilter.Evaluate(time);
            colorAdjustments.saturation.value = m_saturation.Evaluate(time);
            whiteBalance.temperature.value = m_temperature.Evaluate(time);
            whiteBalance.tint.value = m_tint.Evaluate(time);
        }
#endif

        public bool ValidateBloom()
        {
            if (m_bloomThreshold == null)
            {
                return false;
            }
            if (m_bloomIntensity == null)
            {
                return false;
            }
            if (m_bloomScatter == null)
            {
                return false;
            }
            if (m_bloomTint == null)
            {
                return false;
            }

            return true;
        }
#if HDPipeline
        public void ApplyBloomSettings(Bloom bloom, float time)
        {
            bloom.threshold.value = m_bloomThreshold.Evaluate(time);
            bloom.intensity.value = m_bloomIntensity.Evaluate(time);
            bloom.scatter.value = m_bloomScatter.Evaluate(time);
            bloom.tint.value = m_bloomTint.Evaluate(time);
        }
#endif

        public bool ValidateShadowToning()
        {
            if (m_shadows == null)
            {
                return false;
            }
            if (m_highlights == null)
            {
                return false;
            }
            if (m_shadowBalance == null)
            {
                return false;
            }

            return true;
        }
#if HDPipeline
        public void ApplyShadowToningSettings(SplitToning splitToning, float time)
        {
            splitToning.shadows.value = m_shadows.Evaluate(time);
            splitToning.highlights.value = m_highlights.Evaluate(time);
            splitToning.balance.value = m_shadowBalance.Evaluate(time);
        }
#endif

        public bool ValidateVignette()
        {
            if (m_vignetteColor == null)
            {
                return false;
            }
            if (m_vignetteIntensity == null)
            {
                return false;
            }
            if (m_vignetteSmoothness == null)
            {
                return false;
            }

            return true;
        }
#if HDPipeline
        public void ApplyVignetteSettings(Vignette vignette, float time)
        {
            vignette.color.value = m_vignetteColor.Evaluate(time);
            vignette.intensity.value = m_vignetteIntensity.Evaluate(time);
            vignette.smoothness.value = m_vignetteSmoothness.Evaluate(time);
        }
#endif

        public bool ValidateAmbientOcclusion()
        {
            if (m_ambientIntensity == null)
            {
                return false;
            }
            if (m_ambientDirectStrength == null)
            {
                return false;
            }
            if (m_ambientRadius == null)
            {
                return false;
            }

            return true;
        }
#if HDPipeline
        public void ApplyAmbientOcclusion(AmbientOcclusion ambientOcclusion, float time)
        {
            ambientOcclusion.intensity.value = m_ambientIntensity.Evaluate(time);
            ambientOcclusion.directLightingStrength.value = m_ambientDirectStrength.Evaluate(time);
            ambientOcclusion.radius.value = m_ambientRadius.Evaluate(time);
        }
#endif
    }

    public class HDRPTimeOfDayPostFXProfile : ScriptableObject
    {
        public TimeOfDayPostFXProfileData TimeOfDayPostFXData
        {
            get { return m_timeOfDayPostFXData; }
            set
            {
                if (m_timeOfDayPostFXData != value)
                {
                    m_timeOfDayPostFXData = value;
                }
            }
        }
        [SerializeField] private TimeOfDayPostFXProfileData m_timeOfDayPostFXData = new TimeOfDayPostFXProfileData();
    }
}