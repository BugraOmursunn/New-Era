using System.Collections.Generic;
using System.IO;
using PWCommon5;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.Flora
{
    public enum FakeSourceData {
        Splat,
        Detail,
        TransformList,
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(FloraTerrainTile))]
    public class FloraTerrainTileEditor : PWEditor
    {
        private FloraTerrainTile m_detailTerrainTile;
        private CoreFloraTerrainTileObjectData m_data;
        private List<bool> showPositions = new List<bool>();
        private EditorUtils m_editorUtils;

        private void OnEnable()
        {
            m_detailTerrainTile = target as FloraTerrainTile;
            if (m_editorUtils == null)
            {
                m_editorUtils = FloraApp.GetEditorUtils(this);
            }

            m_data = m_detailTerrainTile;

            showPositions = new List<bool>();
            var list = m_detailTerrainTile.m_detailObjectList;
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                showPositions.Add(false);
            }
        }
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }
        private DetailOverrideData DrawDetailObjectData(DetailOverrideData data)
        {
            if (!FloraGlobalData.TreesEnabled)
            {
                FakeSourceData sourceData = FakeSourceData.Detail;
                switch (data.SourceDataType)
                {
                    case CoreCommonFloraData.SourceDataType.Detail:
                        sourceData = FakeSourceData.Detail;
                        break;
                    case CoreCommonFloraData.SourceDataType.Splat:
                        sourceData = FakeSourceData.Splat;
                        break;
                    case CoreCommonFloraData.SourceDataType.TransformList:
                        sourceData = FakeSourceData.TransformList;
                        break;

                }

                sourceData = (FakeSourceData)EditorGUILayout.EnumPopup("Source Data Type", sourceData);

                switch (sourceData)
                {
                    case FakeSourceData.Detail:
                        data.SourceDataType = CoreCommonFloraData.SourceDataType.Detail;
                        break;
                    case FakeSourceData.Splat:
                        data.SourceDataType = CoreCommonFloraData.SourceDataType.Splat;
                        break;
                    case FakeSourceData.TransformList:
                        data.SourceDataType = CoreCommonFloraData.SourceDataType.TransformList;
                        break;

                }
            }
            else
            {
#pragma warning disable 162
                data.SourceDataType = (CoreCommonFloraData.SourceDataType)EditorGUILayout.EnumPopup("Source Data Type", data.SourceDataType);
#pragma warning restore 162

            }
            data.SourceDataIndex = EditorGUILayout.IntField("Source Data Index", data.SourceDataIndex);
            data.DebugColor = EditorGUILayout.ColorField("Debug Color", data.DebugColor);
            return data;
        }
        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize();
            m_editorUtils.Panel("GlobalSettings", GlobalPanel, true);
        }

        private void GlobalPanel(bool helpEnabled)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            {
                m_data.DetailCamera = (Camera)m_editorUtils.ObjectField("Camera", m_data.DetailCamera, typeof(Camera), true, helpEnabled);
                m_data.TerrainType = (FloraTerrainType)m_editorUtils.EnumPopup("TerrainType", m_data.TerrainType, helpEnabled);
                
                EditorGUI.indentLevel++;
                switch (  m_data.TerrainType)
                {
                    case FloraTerrainType.UnityTerrain :
                        m_data.UnityTerrain = (Terrain)m_editorUtils.ObjectField("Terrain", m_data.UnityTerrain, typeof(Terrain), true, helpEnabled);
                        break;
                    
                    case FloraTerrainType.ExternalMap :
                        
                        var externalMapData = serializedObject.FindProperty("m_externalMapData");
                        var externalMapDataPosition = externalMapData.FindPropertyRelative("Position");
                        var externalMapDataSize = externalMapData.FindPropertyRelative("Size");
                        var externalMapDataHeight = externalMapData.FindPropertyRelative("Height");
                        var externalMapDataSplat = externalMapData.FindPropertyRelative("Splat");
                        var externalMapDataDetail = externalMapData.FindPropertyRelative("Detail");
                        
                        m_editorUtils.Label("externalPositionalData",helpEnabled);
                        m_editorUtils.PropertyField("externalMapDataPosition", externalMapDataPosition, helpEnabled);
                        m_editorUtils.PropertyField("externalMapDataSize", externalMapDataSize, helpEnabled);
                        m_editorUtils.Label("externalTextureData",helpEnabled);
                        m_editorUtils.PropertyField("externalMapDataHeight", externalMapDataHeight, helpEnabled);
                        m_editorUtils.PropertyField("externalMapDataSplat", externalMapDataSplat, helpEnabled);
                        m_editorUtils.PropertyField("externalMapDataDetail", externalMapDataDetail, helpEnabled);
                        
                        break;
                }
                EditorGUI.indentLevel--;
                
                m_editorUtils.Label("Draw Settings",helpEnabled);
                EditorGUI.indentLevel++;
                    m_data.BaselineCellDensity = (CoreCommonFloraData.CellDensity)m_editorUtils.EnumPopup("BaselineCellDensity", m_data.BaselineCellDensity, helpEnabled);
                    m_data.MaximumDrawDistance = m_editorUtils.FloatField("MaximumDrawDistance", m_data.MaximumDrawDistance, helpEnabled);
                    m_data.DrawDebugInfo = m_editorUtils.Toggle("DrawDebugInfo", m_data.DrawDebugInfo, helpEnabled);
                EditorGUI.indentLevel--;
                
                var detailObjectsListProperty = serializedObject.FindProperty("m_detailObjectList");
                EditorGUI.indentLevel++;
                    m_editorUtils.PropertyField("DetailObjectList", detailObjectsListProperty, helpEnabled);
                EditorGUI.indentLevel--;

            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_detailTerrainTile);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}