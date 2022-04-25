using PWCommon5;
using UnityEditor;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    [CustomEditor(typeof(FloraGlobalManager))]
    [CanEditMultipleObjects] public class FloraGobalManagerEditor : PWEditor
    {
        //
        private FloraGlobalManager m_detailGlobalManager;
        private EditorUtils m_editorUtils;

        public void OnEnable()
        {
            m_detailGlobalManager = target as FloraGlobalManager;
            if (m_editorUtils == null)
            {
                m_editorUtils = FloraApp.GetEditorUtils(this);
            }
        }
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize();

            m_editorUtils.Panel("GlobalSettings", GlobalPanel, true);
        }

        private void GlobalPanel(bool helpEnabled)
        {
            FloraGlobalData settings = m_detailGlobalManager.Settings;
            EditorGUI.BeginChangeCheck();
            {
                m_editorUtils.InlineHelp("SettingsInfo", true);

                m_editorUtils.Heading("ActiveDetailGlobalValues");
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Detail Density: " + FloraGlobals.Settings.ObjectGlobalDensityModifier);
                EditorGUILayout.LabelField("Detail Distance: " + FloraGlobals.Settings.ObjectGlobalDistanceModifier);
                EditorGUILayout.LabelField("Camera Density: " + FloraGlobals.Settings.CameraCellGlobalSubdivisionModifier);
                EditorGUILayout.LabelField("Camera Distance: " + FloraGlobals.Settings.TerrainTileGlobalDistanceModifier);
                EditorGUILayout.LabelField("Details Tracked: " + FloraGlobals.DetailData.Count);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                
                m_editorUtils.Heading("DetailObjectGlobals");
                EditorGUI.indentLevel++;
                settings.ObjectGlobalDensityModifier = m_editorUtils.Slider("DensityModifier", settings.ObjectGlobalDensityModifier, 0.01f, 4, helpEnabled);
                settings.ObjectGlobalDistanceModifier = m_editorUtils.Slider("DetailDistanceModifier", settings.ObjectGlobalDistanceModifier, 0.01f, 4, helpEnabled);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                
                m_editorUtils.Heading("CameraCellGlobals");
                EditorGUI.indentLevel++;
                settings.CameraCellGlobalSubdivisionModifier = m_editorUtils.IntSlider("CameraCellDivision", settings.CameraCellGlobalSubdivisionModifier,-8, 8, helpEnabled);
                settings.TerrainTileGlobalDistanceModifier = m_editorUtils.Slider("CameraDistanceModifier", settings.TerrainTileGlobalDistanceModifier, 0.01f, 4, helpEnabled);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (m_editorUtils.Button("GetGlobals"))
                {
                    m_detailGlobalManager.GetGlobals();
                }
                if (m_editorUtils.Button("SetGlobals"))
                {
                    m_detailGlobalManager.SetGlobals();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (m_editorUtils.Button("StoreGlobals"))
                {
                    m_detailGlobalManager.StoreGlobals();
                }
                if (m_editorUtils.Button("ResetGlobals"))
                {
                    m_detailGlobalManager.ResetGlobals();
                }
                EditorGUILayout.EndHorizontal();

            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_detailGlobalManager);
            }
        }
    }
}