#if HDPipeline && UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProceduralWorlds.HDRPTOD
{
    [ExecuteAlways]
    public class HDRPTimeOfDayOverrideVolumeController : MonoBehaviour
    {
        public static HDRPTimeOfDayOverrideVolumeController Instance
        {
            get { return m_instance; }
        }
        [SerializeField] private static HDRPTimeOfDayOverrideVolumeController m_instance;

        public List<HDRPTimeOfDayOverrideVolume> m_dayTimeOverrideVolumes = new List<HDRPTimeOfDayOverrideVolume>();
        public List<HDRPTimeOfDayOverrideVolume> m_nightTimeOverrideVolumes = new List<HDRPTimeOfDayOverrideVolume>();

        private bool m_lastIsDayValue = false;

        private void OnEnable()
        {
            m_instance = this;
            CheckState(true);
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
            }
            else
            {
                EditorApplication.update -= EditorUpdate;
                EditorApplication.update += EditorUpdate;
            }
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                CheckState();
            }
        }

        public void AddOverrideVolume(HDRPTimeOfDayOverrideVolume volume, OverrideTODType type)
        {
            if (volume != null)
            {
                switch (type)
                {
                    case OverrideTODType.Day:
                    {
                        if (!m_dayTimeOverrideVolumes.Contains(volume))
                        {
                            m_dayTimeOverrideVolumes.Add(volume);
                        }
                        if (m_nightTimeOverrideVolumes.Contains(volume))
                        {
                            m_nightTimeOverrideVolumes.Remove(volume);
                        }
                        break;
                    }
                    case OverrideTODType.Night:
                    {
                        if (!m_nightTimeOverrideVolumes.Contains(volume))
                        {
                            m_nightTimeOverrideVolumes.Add(volume);
                        }
                        if (m_dayTimeOverrideVolumes.Contains(volume))
                        {
                            m_dayTimeOverrideVolumes.Remove(volume);
                        }
                        break;
                    }
                }
                CheckState(true);
            }
        }
        public void RemoveOverrideVolume(HDRPTimeOfDayOverrideVolume volume, OverrideTODType type)
        {
            if (volume != null)
            {
                switch (type)
                {
                    case OverrideTODType.Day:
                    {
                        if (m_dayTimeOverrideVolumes.Contains(volume))
                        {
                            m_dayTimeOverrideVolumes.Remove(volume);
                        }
                        break;
                    }
                    case OverrideTODType.Night:
                    {
                        if (m_nightTimeOverrideVolumes.Contains(volume))
                        {
                            m_nightTimeOverrideVolumes.Remove(volume);
                        }
                        break;
                    }
                }
                CheckState(true);
            }
        }
        public void CheckState(bool overrideApply = false)
        {
            HDRPTimeOfDay timeOfDay = HDRPTimeOfDay.Instance;
            if (timeOfDay != null)
            {
                if (!timeOfDay.UseOverrideVolumes)
                {
                    return;
                }

                bool isDay = timeOfDay.IsDayTime();
                if (overrideApply)
                {
                    m_lastIsDayValue = isDay;
                    ProcessVolumes(isDay);
                    timeOfDay.SetupAllOverrideVolumes();
                    timeOfDay.ResetOverrideVolumeBlendTime(true);
                }
                else
                {
                    if (isDay != m_lastIsDayValue)
                    {
                        m_lastIsDayValue = isDay;
                        ProcessVolumes(isDay);
                        timeOfDay.SetupAllOverrideVolumes();
                        timeOfDay.ResetOverrideVolumeBlendTime(true);
                    }
                }
            }
        }

        private void SetupOverrideVolumes(HDRPTimeOfDayOverrideVolume volumeObject)
        {
            if (volumeObject != null)
            {
                volumeObject.LoadVolume();
            }
        }
        private void ProcessVolumes(bool isDay)
        {
            if (isDay)
            {
                if (m_dayTimeOverrideVolumes.Count > 0)
                {
                    foreach (HDRPTimeOfDayOverrideVolume overrideVolumeDay in m_dayTimeOverrideVolumes)
                    {
                        overrideVolumeDay.enabled = true;
                        SetupOverrideVolumes(overrideVolumeDay);
                    }
                }
                if (m_nightTimeOverrideVolumes.Count > 0)
                {
                    foreach (HDRPTimeOfDayOverrideVolume overrideVolumeNight in m_nightTimeOverrideVolumes)
                    {
                        overrideVolumeNight.enabled = false;
                    }
                }
            }
            else
            {
                if (m_dayTimeOverrideVolumes.Count > 0)
                {
                    foreach (HDRPTimeOfDayOverrideVolume overrideVolumeDay in m_dayTimeOverrideVolumes)
                    {
                        overrideVolumeDay.enabled = false;
                    }
                }
                if (m_nightTimeOverrideVolumes.Count > 0)
                {
                    foreach (HDRPTimeOfDayOverrideVolume overrideVolumeNight in m_nightTimeOverrideVolumes)
                    {
                        overrideVolumeNight.enabled = true;
                        SetupOverrideVolumes(overrideVolumeNight);
                    }
                }
            }
        }
        private void EditorUpdate()
        {
            CheckState();
        }
    }
}
#endif