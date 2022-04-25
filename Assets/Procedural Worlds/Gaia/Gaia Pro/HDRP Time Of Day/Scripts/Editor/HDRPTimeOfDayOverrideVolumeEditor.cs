#if HDPipeline && UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.HDRPTOD
{
    [CustomEditor(typeof(HDRPTimeOfDayOverrideVolume))]
    public class HDRPTimeOfDayOverrideVolumeEditor : Editor
    {
        private HDRPTimeOfDayOverrideVolume m_overrideVolume;
        private GUIStyle m_boxStyle;

        private void OnEnable()
        {
            if (HDRPTimeOfDay.Instance != null)
            {
                HDRPTimeOfDay.Instance.SetupAllOverrideVolumes();
            }
        }
        public override void OnInspectorGUI()
        {
            m_overrideVolume = (HDRPTimeOfDayOverrideVolume) target;
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

            if (HDRPTimeOfDay.Instance != null)
            {
                if (HDRPTimeOfDay.Instance.UseOverrideVolumes)
                {
                    Color color = m_overrideVolume.m_volumeSettings.m_gizmoColor;
                    float blendTime = m_overrideVolume.m_volumeSettings.m_blendTime;
                    bool addOverrideVolumeType = m_overrideVolume.m_volumeSettings.m_addOverrideVolumeType;
                    bool removeFromController = m_overrideVolume.m_volumeSettings.m_removeFromController;
                    OverrideTODType volumeType = m_overrideVolume.m_volumeSettings.m_volumeType;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    color = EditorGUILayout.ColorField("Gizmo Color", color);
                    blendTime = EditorGUILayout.FloatField("Blend Time", blendTime);
                    addOverrideVolumeType = EditorGUILayout.Toggle("Add Volume To Controller", addOverrideVolumeType);
                    EditorGUI.indentLevel++;
                    if (addOverrideVolumeType)
                    {
                        volumeType = (OverrideTODType)EditorGUILayout.EnumPopup("Volume Time Of Day Type", volumeType);
                    }
                    else
                    {
                        removeFromController = EditorGUILayout.Toggle("Remove Volume From Controller", removeFromController);
                    }
                    EditorGUI.indentLevel--;
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_overrideVolume, "Override Volume Changes Made");
                        m_overrideVolume.m_volumeSettings.m_gizmoColor = color;
                        m_overrideVolume.m_volumeSettings.m_blendTime = blendTime;
                        m_overrideVolume.m_volumeSettings.m_addOverrideVolumeType = addOverrideVolumeType;
                        m_overrideVolume.m_volumeSettings.m_removeFromController = removeFromController;
                        m_overrideVolume.m_volumeSettings.m_volumeType = volumeType;
                        m_overrideVolume.SetupVolumeTypeToController();
                        EditorUtility.SetDirty(m_overrideVolume);
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginVertical(m_boxStyle);
                    EditorGUILayout.LabelField("Override Settings", EditorStyles.boldLabel);
                    DrawClampedSliderParamaterGUI(ref m_overrideVolume.m_volumeSettings.m_sunVolumetric, m_overrideVolume, "Sun Volumetric");
                    DrawClampedSliderParamaterGUI(ref m_overrideVolume.m_volumeSettings.m_sunVolumetricDimmer, m_overrideVolume, "Sun Volumetric Dimmer");
                    DrawClampedSliderParamaterGUI(ref m_overrideVolume.m_volumeSettings.m_exposure, m_overrideVolume, "Exposure");
                    DrawClampedSliderParamaterGUI(ref m_overrideVolume.m_volumeSettings.m_ambientIntensity, m_overrideVolume, "Ambient Intensity");
                    DrawClampedSliderParamaterGUI(ref m_overrideVolume.m_volumeSettings.m_ambientReflectionIntensity, m_overrideVolume, "Ambient Reflection Intensity");
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (HDRPTimeOfDay.Instance != null)
                        {
                            HDRPTimeOfDay.Instance.RefreshOverrideVolume = true;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Use Override Volumes is disabled, to use override volumes in the Time Of Day system please enable this feature", MessageType.Info);
                    if (GUILayout.Button("Enable Override Volume Support"))
                    {
                        HDRPTimeOfDay.Instance.UseOverrideVolumes = true;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("HDRP Time Of Day was not found this system requires it to work. You can add it 'Window/Procedural Worlds/HDRP/Time Of Day/Add Time Of Day'", MessageType.Warning);
            }
        }

        /// <summary>
        /// Draws bool paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawBoolParamaterGUI(ref BoolParameter boolParameter, Object recordedObject, string localization)
        {
            bool overrideState = boolParameter.overrideState;
            bool value = boolParameter.value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            overrideState = EditorGUILayout.Toggle(new GUIContent(""), overrideState, GUILayout.MaxWidth(25f));
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.LabelField(localization, GUILayout.MaxWidth(145f));
            value = EditorGUILayout.Toggle(new GUIContent(""), value);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                boolParameter.overrideState = overrideState;
                boolParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
        /// <summary>
        /// Draws clamped float paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawClampedFloatParamaterGUI(ref ClampedFloatParameter floatParameter, Object recordedObject, string localization)
        {
            bool overrideState = floatParameter.overrideState;
            float value = floatParameter.value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            overrideState = EditorGUILayout.Toggle(new GUIContent(""), overrideState, GUILayout.MaxWidth(25f));
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.LabelField(localization, GUILayout.MaxWidth(145f));
            value = EditorGUILayout.FloatField(new GUIContent(""), value);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                floatParameter.overrideState = overrideState;
                floatParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
        /// <summary>
        /// Draws clamped float paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawFloatParamaterGUI(ref FloatParameter floatParameter, Object recordedObject, string localization)
        {
            bool overrideState = floatParameter.overrideState;
            float value = floatParameter.value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            overrideState = EditorGUILayout.Toggle(new GUIContent(""), overrideState, GUILayout.MaxWidth(25f));
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.LabelField(localization, GUILayout.MaxWidth(145f));
            value = EditorGUILayout.FloatField(new GUIContent(""), value);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                floatParameter.overrideState = overrideState;
                floatParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
        /// <summary>
        /// Draws clamped float paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawClampedSliderParamaterGUI(ref ClampedFloatParameter floatParameter, Object recordedObject, string localization)
        {
            bool overrideState = floatParameter.overrideState;
            float value = floatParameter.value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            overrideState = EditorGUILayout.Toggle(new GUIContent(""), overrideState, GUILayout.MaxWidth(25f));
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.LabelField(localization, GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 25f));
            value = EditorGUILayout.Slider(new GUIContent(""), value, floatParameter.min, floatParameter.max);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                floatParameter.overrideState = overrideState;
                floatParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
        /// <summary>
        /// Draws clamped float paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawClampedSliderParamaterGUI(Rect rect, ref ClampedFloatParameter floatParameter, Object recordedObject, string localization)
        {
            bool overrideState = floatParameter.overrideState;
            float value = floatParameter.value;

            EditorGUI.BeginChangeCheck();
            rect.x = EditorGUIUtility.currentViewWidth / 4f;
            overrideState = EditorGUI.Toggle(rect, overrideState);
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            rect.x = EditorGUIUtility.currentViewWidth / 3f;
            EditorGUI.LabelField(rect, localization);
            rect.x = EditorGUIUtility.currentViewWidth / 2f;
            value = EditorGUI.Slider(rect, value, floatParameter.min, floatParameter.max);
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                floatParameter.overrideState = overrideState;
                floatParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
        /// <summary>
        /// Draws clamped float paramater GUI
        /// </summary>
        /// <param name="boolParameter"></param>
        /// <param name="localization"></param>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static void DrawEnumPopupParamaterGUI(ref ClampedIntParameter intParameter, Object recordedObject, string localization, string[] options)
        {
            bool overrideState = intParameter.overrideState;
            int value = intParameter.value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            overrideState = EditorGUILayout.Toggle(new GUIContent(""), overrideState, GUILayout.MaxWidth(25f));
            if (!overrideState)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.LabelField(localization, GUILayout.MaxWidth(145f));
            value = EditorGUILayout.Popup(new GUIContent(""), value, options);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(recordedObject, "Changes made");
                intParameter.overrideState = overrideState;
                intParameter.value = value;
                EditorUtility.SetDirty(recordedObject);
            }
        }
    }
}
#endif