#if HDPipeline && UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.HDRPTOD
{
    public class WeatherFXRainWithThunder : MonoBehaviour, IHDRPWeatherVFX
    {
        public HDRPTimeOfDayWeatherProfile WeatherProfile;
        public AudioSource m_audioSource;
        public Vector2 m_thunderStrikeTime = new Vector2(5f, 10f);
        public ThunderData m_thunderData;
        [HideInInspector] public WeatherVFXState m_state = WeatherVFXState.InActive;
        [HideInInspector] public float m_duration;

        private float m_thunderTimer;

        public void SetWeatherProfile(HDRPTimeOfDayWeatherProfile profile)
        {
            WeatherProfile = profile;
        }
        public void StartWeatherFX()
        {
            m_thunderTimer = UnityEngine.Random.Range(m_thunderStrikeTime.x, m_thunderStrikeTime.y);
            if (m_audioSource == null)
            {
                m_audioSource = GetComponent<AudioSource>();
                if (m_audioSource == null)
                {
                    m_audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            m_audioSource.loop = true;
            m_audioSource.playOnAwake = false;
            m_audioSource.clip = WeatherProfile.WeatherFXData.m_weatherAudio;
            m_audioSource.volume = 0f;

            m_duration = 0f;
            m_state = WeatherVFXState.FadeIn;
        }
        public void StopWeatherFX()
        {
            m_duration = 0f;
            m_state = WeatherVFXState.FadeOut;
        }

        private void Update()
        {
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }

            if (m_state == WeatherVFXState.FadeIn)
            {
                m_duration += Time.deltaTime / WeatherProfile.WeatherData.m_transitionDuration;
                m_audioSource.volume = Mathf.Lerp(0f, WeatherProfile.WeatherFXData.m_maxVolume, m_duration);
            }
            else if (m_state == WeatherVFXState.FadeOut)
            {
                m_duration += Time.deltaTime / WeatherProfile.WeatherData.m_transitionDuration;
                m_audioSource.volume = Mathf.Lerp(m_audioSource.volume, 0f, m_duration);
            }

            if (m_duration >= 1f)
            {
                if (m_state == WeatherVFXState.FadeIn)
                {
                    m_state = WeatherVFXState.Active;
                }

                m_thunderTimer -= Time.deltaTime;
                if (m_thunderTimer <= 0f)
                {
                    m_thunderTimer = UnityEngine.Random.Range(m_thunderStrikeTime.x, m_thunderStrikeTime.y);
                    HDRPTimeOfDay timeOfDay = HDRPTimeOfDay.Instance;
                    if (timeOfDay != null)
                    {
                        timeOfDay.StartThunderVFX(m_thunderData);
                    }
                }
            }
        }
    }
}
#endif