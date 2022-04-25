#if HDPipeline && UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.HDRPTOD
{
    public enum OverrideTODType { Day, Night }

    [System.Serializable]
    public class OverrideVolumeSettings
    {
        public bool m_drawGimzo = true;
        public bool m_addOverrideVolumeType = true;
        public bool m_removeFromController = true;
        public OverrideTODType m_volumeType = OverrideTODType.Day;
        public Color m_gizmoColor = new Color(1f, 0f, 0f, 0.4f);
        public float m_blendTime = 3f;
        public ClampedFloatParameter m_sunVolumetric = new ClampedFloatParameter(1f, 0f, 16f, false);
        public ClampedFloatParameter m_sunVolumetricDimmer = new ClampedFloatParameter(1f, 0f, 1f, false);
        public ClampedFloatParameter m_exposure = new ClampedFloatParameter(14f, -5f, 20f);
        public ClampedFloatParameter m_ambientIntensity = new ClampedFloatParameter(1f, 0f, 4f, false);
        public ClampedFloatParameter m_ambientReflectionIntensity = new ClampedFloatParameter(1f, 0f, 4f, false);

        public bool IsAnyOverrideEnabled()
        {
            if (m_sunVolumetric.overrideState)
            {
                return true;
            }
            if (m_sunVolumetricDimmer.overrideState)
            {
                return true;
            }
            if (m_exposure.overrideState)
            {
                return true;
            }
            if (m_ambientIntensity.overrideState)
            {
                return true;
            }
            if (m_ambientReflectionIntensity.overrideState)
            {
                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public class OverrideDataInfo
    {
        public bool m_isInVolue = false;
        public float m_transitionTime = 0f;
        public OverrideVolumeSettings m_settings = new OverrideVolumeSettings();
    }
    public enum OverrideVolumeType { Box, Sphere }

    [ExecuteAlways]
    public class HDRPTimeOfDayOverrideVolume : MonoBehaviour
    {
        public OverrideVolumeSettings m_volumeSettings = new OverrideVolumeSettings();

        [SerializeField] private int m_id;
        [SerializeField] private Bounds m_bounds;
        [SerializeField] private bool m_isInBounds = false;

        private void OnEnable()
        {
            Setup();
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
        private void OnDestroy()
        {
            m_isInBounds = HDRPTimeOfDay.Instance.RemoveOverrideVolume(m_id);
            HDRPTimeOfDayOverrideVolumeController controller = HDRPTimeOfDayOverrideVolumeController.Instance;
            if (controller != null)
            {
                controller.RemoveOverrideVolume(this, m_volumeSettings.m_volumeType);
            }
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnDisable()
        {
            m_isInBounds = HDRPTimeOfDay.Instance.RemoveOverrideVolume(m_id);
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnDrawGizmos()
        {
            if (!m_volumeSettings.m_drawGimzo)
            {
                return;
            }

            Gizmos.color = m_volumeSettings.m_gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, m_bounds.size);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }

        private void OnDrawGizmosSelected()
        {
            if (!m_volumeSettings.m_drawGimzo)
            {
                return;
            }

            m_bounds = BuildAndGetBounds();
        }
        private void Update()
        {
            if (!HDRPTimeOfDay.Instance.UseOverrideVolumes || !Application.isPlaying)
            {
                return;
            }

            IsInBounds(HDRPTimeOfDay.Instance.Player);
        }

        /// <summary>
        /// Sets up this override volume and sets the volume ID
        /// </summary>
        /// <param name="ID"></param>
        public void Setup(int ID = -1)
        {
            if (ID != -1)
            {
                m_id = ID;
            }
            LoadVolume();
        }
        /// <summary>
        /// Loads the volume data and sets it up
        /// </summary>
        public void LoadVolume()
        {
            m_isInBounds = false;
            m_bounds = BuildAndGetBounds();
        }
        /// <summary>
        /// Checks to see if the transform player is in the bounding volume
        /// </summary>
        /// <param name="player"></param>
        public void IsInBounds(Transform player)
        {
            if (!enabled)
            {
                return;
            }

            if (player != null)
            {
                bool inBounds = m_bounds.Contains(player.position);
                if (inBounds)
                {
                    m_isInBounds = HDRPTimeOfDay.Instance.AddOverrideVolume(m_id, this);
                }
                else
                {
                    if (m_isInBounds)
                    {
                        m_isInBounds = HDRPTimeOfDay.Instance.RemoveOverrideVolume(m_id);
                    }
                }
            }
        }
        /// <summary>
        /// Adds the volume to the controller that with be managed is active state if it's day or night
        /// </summary>
        public void SetupVolumeTypeToController()
        {
            HDRPTimeOfDayOverrideVolumeController controller = HDRPTimeOfDayOverrideVolumeController.Instance;
            if (!m_volumeSettings.m_addOverrideVolumeType)
            {
                if (controller != null && m_volumeSettings.m_removeFromController)
                {
                    controller.RemoveOverrideVolume(this, m_volumeSettings.m_volumeType);
                }
                return;
            }

            if (controller != null)
            {
                controller.AddOverrideVolume(this, m_volumeSettings.m_volumeType);
            }
        }

        /// <summary>
        /// Creates and returns the bounds
        /// </summary>
        /// <returns></returns>
        private Bounds BuildAndGetBounds()
        {
            return new Bounds(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z));
        }
        /// <summary>
        /// Editor update function
        /// </summary>
        private void EditorUpdate()
        {
#if UNITY_EDITOR
            if (!HDRPTimeOfDay.Instance.UseOverrideVolumes)
            {
                return;
            }

            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                Camera camera = sceneView.camera;
                if (camera != null)
                {
                    IsInBounds(camera.transform);
                }
            }
#endif
        }
    }
}
#endif