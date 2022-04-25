#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ProceduralWorlds.HDRPTOD
{
    public enum TimeOfDayComponentType { Lighting, PostProcessing, Sun, Moon, SunRotationObject, LocalFog, RayTracedVolume }

    [ExecuteAlways]
    public class HDRPTimeOfDayComponentType : MonoBehaviour
    {
        public TimeOfDayComponentType m_componentType = TimeOfDayComponentType.Lighting;

        [SerializeField, HideInInspector] private Transform Player;

        private void OnEnable()
        {
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
        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (m_componentType == TimeOfDayComponentType.LocalFog)
            {
                UpdateTransform(Player);
            }
        }

        private void EditorUpdate()
        {
#if UNITY_EDITOR
            if (m_componentType == TimeOfDayComponentType.LocalFog)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    Camera sceneCamera = sceneView.camera;
                    if (sceneCamera != null)
                    {
                        UpdateTransform(sceneCamera.transform);
                    }
                }
            }
#endif
        }
        private void UpdateTransform(Transform player)
        {
            if (player != null)
            {
                gameObject.transform.SetPositionAndRotation(player.position, Quaternion.identity);
            }
        }
        /// <summary>
        /// Sets the new player transform
        /// </summary>
        /// <param name="player"></param>
        public void SetNewPlayer(Transform player)
        {
            Player = player;
        }
    }
}