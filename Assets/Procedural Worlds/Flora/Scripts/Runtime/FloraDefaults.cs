using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.Flora
{
    public enum FloraKeywordValueType { Bool, Float, Int, Color, Texture }
    public enum FloraKeywordAddRemove { Add, Remove, None }

    [System.Serializable]
    public class LODSubDivisionData
    {
        public string m_lodName;
        public int m_lodID = 0;
        public int m_subDivisionValue;
        public bool m_foldoutGUI = false;
    }
    [System.Serializable]
    public class TerrainCellCountData
    {
        public string m_name;
        public float m_terrainSize;
        public CoreCommonFloraData.CellDensity m_cellDensity = CoreCommonFloraData.CellDensity.Sixteen;
        public bool m_foldoutGUI = false;
    }
    [System.Serializable]
    public class SkippableLODData
    {
        public string m_name;
        public bool m_enabled = true;
        public int m_skipLOD;
        public int m_ifHasMoreOrEqualLODS;
        public bool m_foldoutGUI = false;
    }
    /// <summary>
    /// Custom LOD data that you can use to create your own LOD information.
    /// Note that the list count must match the LOD Group length for it to be used.
    /// </summary>
    [System.Serializable]
    public class CustomLODData
    {
        public List<LODData> m_lodData = new List<LODData>();
    }
    /// <summary>
    /// Contains the in/out float values for flroa lodding
    /// </summary>
    [System.Serializable]
    public class LODData
    {
        public float m_inDistance = 0f;
        public float m_inFadeDistance = 0.001f;
        public float m_outDistance = 50f;
        public float m_outFadeDistance = 5f;
    }
    [System.Serializable]
    public class MaterialKeywordData
    {
        public string m_name;
        public FloraKeywordAddRemove m_enabled = FloraKeywordAddRemove.Add;
        public string m_keywordValue;
        public bool m_setShaderProperty = false;
        public string m_shaderPropertyName;
        public FloraKeywordValueType m_valueType = FloraKeywordValueType.Bool;
        public bool m_boolValue = true;
        public float m_floatValue = 1f;
        public int m_intValue = 0;
        public Color m_colorValue = Color.white;
        public Texture m_textureValue;
        public bool m_foldoutGUI = false;
    }

    public class FloraDefaults : ScriptableObject
    {
        //Detail Settings
        [Header("Detail Settings")]
        public List<LODSubDivisionData> m_detailSubDivisionData = new List<LODSubDivisionData>();
        public bool m_defaultsLODDetails = true;
        public Mesh m_defaultTerrainDetailMeshLOD0;
        public Mesh m_defaultTerrainDetailMeshLOD1;
        public Mesh m_defaultTerrainDetailMesh;
        public float m_defaultTerrainDetailLODBias = 3f;
        public bool m_terrainDetailLOD1Shadows = false;
        public ShadowCastingMode m_defaultDetailShadows = ShadowCastingMode.Off;
        public float m_defaultDetailsFadeOutDistance = 1f;
        public float m_detailDensityMultiplier = 1f;
        public Vector2 m_minMaxDetailVertexCount = new Vector2(4, 200);
        
        //Tree Settings
        [Header("Tree Settings")]
        public List<LODSubDivisionData> m_treeSubDivisionData = new List<LODSubDivisionData>();
        public bool m_enableLODSkipping = false;
        public List<SkippableLODData> m_treeSkippableLODData = new List<SkippableLODData>();
        public float m_defaultTreeLODBias = 3f;
        public float m_defaultTreeFadeOutDistance = 1f;
        public int m_minimumRecommendedTreeCount = 1000;
        public Vector2 m_minMaxTreeVertexCount = new Vector2(5000, 20000);

        //Terrain
        [Header("Terrain Settings")]
        public CoreCommonFloraData.CellDensity m_defaultCellDensity = CoreCommonFloraData.CellDensity.Sixteen;
        public List<TerrainCellCountData> m_terrainCellCountData = new List<TerrainCellCountData>();

        //Material Setup
        [Header("Material Setup")]
        public Material m_builtInMaterialTemplate;
        public List<string> m_builtInCompatibleShaderNames = new List<string>();
        public Material m_urpMaterialTemplate;
        public List<string> m_urpCompatibleShaderNames = new List<string>();
        public Material m_hdrpMaterialTemplate;
        public List<string> m_hdrpCompatibleShaderNames = new List<string>();

        public bool m_enableGPUInstancing = true;
        [Range(0f, 1f)]
        public float m_defaultCutoutValue = 0.35f;
        public Texture2D m_defaultNormalMap;
        public Texture2D m_defaultMaskMap;
        public bool m_useSpeedTreeHueColors = true;
        public Color m_hueColor1 = new Color(1f, 1f, 1f, 1f);
        public Color m_hueColor2 = new Color(1f, 0.374343f, 0f, 1f);
        public List<string> m_baseColorShaderProperties = new List<string>();
        public List<string> m_albedoShaderProperties = new List<string>();
        public List<string> m_normalShaderProperties = new List<string>();
        public List<string> m_maskMapShaderProperties = new List<string>();
        public List<string> m_cutoutShaderProperties = new List<string>();
        public List<string> m_windMaterialNames = new List<string>();
        public Gradient m_debugColorGradient = new Gradient();

        //Types
        [Header("Tree Type Settings")]
        public List<string> m_treeTypes = new List<string>();
        public List<string> m_grassTypes = new List<string>();
        public List<string> m_plantTypes = new List<string>();
        public List<string> m_logTypes = new List<string>();

        //SRP Setup
        [Header("SRP Settings")] 
        public bool m_checkURPKeywordList = false;
        public List<MaterialKeywordData> m_urpMaterialKeywordSetup = new List<MaterialKeywordData>();
        public bool m_checkHDRPKeywordList = false;
        public List<MaterialKeywordData> m_hdrpMaterialKeywordSetup = new List<MaterialKeywordData>();

        public bool ValidateDefaults()
        {
            bool success = true;
            //Templates
            if (m_builtInMaterialTemplate == null)
            {
                Debug.LogError("Built In material template is null please assign one that uses the flora shader.");
                success = false;
            }
            if (m_urpMaterialTemplate == null)
            {
                Debug.LogError("URP material template is null please assign one that uses the flora shader.");
                success = false;
            }
            if (m_hdrpMaterialTemplate == null)
            {
                Debug.LogError("HDRP material template is null please assign one that uses the flora shader.");
                success = false;
            }
            //Mesh
            if (m_detailSubDivisionData.Count < 1)
            {
                Debug.LogError("Detail sub division list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_defaultTerrainDetailMeshLOD0 == null)
            {
                Debug.LogError("Detail LOD0 Mesh is null");
                success = false;
            }
            if (m_defaultTerrainDetailMeshLOD1 == null)
            {
                Debug.LogError("Detail LOD1 Mesh is null");
                success = false;
            }
            if (m_defaultTerrainDetailMesh == null)
            {
                Debug.LogError("Detail Mesh is null");
                success = false;
            }
            //Tree
            if (m_treeSubDivisionData.Count < 1)
            {
                Debug.LogError("Tree sub division list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            //Terrain
            if (m_terrainCellCountData.Count < 1)
            {
                Debug.LogError("Terrain cell count list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            //Material
            if (m_defaultNormalMap == null)
            {
                Debug.LogError("Default normal map is missing");
                success = false;
            }
            if (m_defaultMaskMap == null)
            {
                Debug.LogError("Default mask map is missing");
                success = false;
            }
            if (m_baseColorShaderProperties.Count < 1)
            {
                Debug.LogError("Base Color shader properties list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_albedoShaderProperties.Count < 1)
            {
                Debug.LogError("Albedo shader properties list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_normalShaderProperties.Count < 1)
            {
                Debug.LogError("Normal shader properties list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_maskMapShaderProperties.Count < 1)
            {
                Debug.LogError("Mask Map shader properties list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_cutoutShaderProperties.Count < 1)
            {
                Debug.LogError("Cutout shader properties list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_windMaterialNames.Count < 1)
            {
                Debug.LogError("Wind material names list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            //Tree Types
            if (m_treeTypes.Count < 1)
            {
                Debug.LogError("Tree types list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_grassTypes.Count < 1)
            {
                Debug.LogError("Grass types list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_plantTypes.Count < 1)
            {
                Debug.LogError("Plant types list is empty there needs to be at least 1 value in there.");
                success = false;
            }
            if (m_logTypes.Count < 1)
            {
                Debug.LogError("Log types list is empty there needs to be at least 1 value in there.");
                success = false;
            }

            if (m_checkURPKeywordList)
            {
                if (m_urpMaterialKeywordSetup.Count < 1)
                {
                    Debug.LogError(
                        "URP Material Keywords setup list is empty there needs to be at least 1 value in there.");
                    success = false;
                }
            }

            if (m_checkHDRPKeywordList)
            {
                if (m_hdrpMaterialKeywordSetup.Count < 1)
                {
                    Debug.LogError(
                        "HDRP Material Keywords setup list is empty there needs to be at least 1 value in there.");
                    success = false;
                }
            }

            return success;
        }
        public int GetDetailDensityMultiplier(Terrain terrain, Mesh mesh, int densityValue = 32)
        {
            if (terrain == null)
            {
                return 1;
            }

            densityValue *= Mathf.RoundToInt((terrain.detailObjectDensity * 5f) * m_detailDensityMultiplier);
            return densityValue;
            //return (int)Mathf.Lerp(densityValue / 1.5f, densityValue * 2, Mathf.InverseLerp(m_minMaxDetailVertexCount.x, m_minMaxDetailVertexCount.y, mesh.vertexCount));
        }
    }
}