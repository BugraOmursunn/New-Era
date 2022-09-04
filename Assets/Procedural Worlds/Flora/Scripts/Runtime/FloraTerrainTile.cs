using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProceduralWorlds.Flora
{
    
    /// <summary>
    /// Information about details
    /// </summary>
    [Serializable]
    public class DetailOverrideData
    {
        public DetailScriptableObject DetailScriptableObject;
        public CoreCommonFloraData.SourceDataType SourceDataType;
        public int SourceDataIndex;
        public int SubCellDivision;
        public Color DebugColor;
    }
    
    /// <summary>
    /// External Data from texture maps
    /// </summary>
    [Serializable]
    public class ExternalMapData
    {
        public Vector3 Position;
        public Vector3 Size;
        public Texture2D Height;
        public Texture2D[] Splat;
        public Texture2D[] Detail;
    }
    
    /// <summary>
    /// External Data from texture maps
    /// </summary>
    [Serializable]
    public class ExternalObjectPositionalData
    {
        public Matrix4x4[] TransMatrix;
    }

    public class FloraTerrainTile : CoreFloraTerrainTileObjectData
    {
        public List<DetailOverrideData> m_detailObjectList = new List<DetailOverrideData>();
        private CameraCells m_detailCameraCells;
        public CoreCameraCellsObjectData DetailCameraCellsData => m_detailCameraCells;
        public ExternalMapData m_externalMapData;

        public ExternalObjectPositionalData[] m_extrnalObjectPositionalData;

        private void DrawDebug()
        {
            var cameraCellData = CameraCellData;
            var pwTerrainData = CoreFloraTerrainData;
            DetailCameraCellsData.DrawDebug(ref cameraCellData, ref pwTerrainData);
            CameraCellData = cameraCellData;
            CoreFloraTerrainData = pwTerrainData;
        }

        private void OnEnable()
        {
            if (!FloraGlobals.DetailData.Contains(this))
            {
                FloraGlobals.DetailData.Add(this);
            }
            Refresh();
            FloraGlobals.onRefreshDetailTerrainTile += Refresh;
        }

        private void OnDisable()
        {
            if (FloraGlobals.DetailData.Contains(this))
            {
                FloraGlobals.DetailData.Remove(this);
            }

            FloraGlobals.onRefreshDetailTerrainTile -= Refresh;
            CleanUpCameraCells();
        }
        
        public enum FLORARP
        {
            Builtin,
            URP,
            HDRP
        }

        private FLORARP m_floaraRP;
        private FloraSettings m_floraSettings;
        private Vector3 m_lastCellDataPosition;

        public static FLORARP GetRenderPipline()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                return FLORARP.Builtin;
            }
            if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HighDefinition"))
            {
                return FLORARP.HDRP;
            }
            else
            {
                return FLORARP.URP;
            }
        }

        private FloraSettings GetFloraSettings()
        {
#if UNITY_EDITOR
            FloraSettings fs;
            var assetPath = AssetDatabase.FindAssets("t:FloraSettings", new string[] {"Assets/Procedural Worlds/Flora/Content Resources/Settings"});
            if (assetPath.Length > 0)
            {
                fs = (FloraSettings) AssetDatabase.LoadAssetAtPath<FloraSettings>(AssetDatabase.GUIDToAssetPath(assetPath[0]));
                return fs;
            }
            else
            {
                return null;
            }
#else
            return null;
#endif
        }

        private Shader setShader(Shader currentshader, FloraSettings.ShaderProfiles shaderProfiles )
        {
            switch (m_floaraRP)
            {
                case FLORARP.Builtin:
                    return shaderProfiles.Builtin as Shader;
                case FLORARP.URP:
                    return shaderProfiles.URP as Shader;
                case FLORARP.HDRP:
                    return shaderProfiles.HDRP as Shader;
            }
            return currentshader;
        }

        private string GetGuid(Object obj)
        {
            string guid = "";
#if UNITY_EDITOR
            long localId;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localId);
#endif
            return guid;
        }
        

        private Shader GetShaderForPipeline( Material mat , FloraSettings fs)
        {
            var currentShader = mat.shader;
            if (fs != null)
            {
                var currentShaderGuid = GetGuid((Object)currentShader);
                foreach (var shadergroup in fs.shaderProfiles)
                {
                    if (shadergroup.BuiltinGUID == currentShaderGuid || shadergroup.UrpGUID == currentShaderGuid || shadergroup.HdrpGUID == currentShaderGuid)
                    {
                        return setShader(currentShader, shadergroup);
                    }
                }
            }
            return currentShader;
        }
        

        private void Start()    
        {
            if (UnityTerrain == null)
            {
                UnityTerrain = GetComponent<Terrain>();
            }

            var pwTerrainData = CoreFloraTerrainData;
            m_floaraRP = GetRenderPipline();
            m_floraSettings = GetFloraSettings();
            
            if (CoreFloraTerrainData.IsReady && CameraCellData.IsReady)
            {
                for (int i = 0; i < m_detailObjectList.Count; i++)
                {
                    var detailObjectData = m_detailObjectList[i];
                    
                    // get render pipeline shader
                    for (int j = 0; j < detailObjectData.DetailScriptableObject.Mat.Length; j++)
                    {
                        if (detailObjectData.DetailScriptableObject.Mat[j] != null)
                        {
                            detailObjectData.DetailScriptableObject.Mat[j].shader = GetShaderForPipeline(detailObjectData.DetailScriptableObject.Mat[j], m_floraSettings);
                        }
                    }
                   
                    float[,,] alphaMap = GetAlpha(ref pwTerrainData, detailObjectData.SourceDataType);

                    int numTrees = UnityTerrain.terrainData.treePrototypes.Length;

                    bool canProceed = false;
                    if (detailObjectData.SourceDataType == SourceDataType.Tree)
                    {
                        if ((numTrees - 1) >= detailObjectData.SourceDataIndex)
                        {
                            canProceed = true;
                        }
                    }
                    else
                    {
                        if (alphaMap.GetLength(2) >= detailObjectData.SourceDataIndex)
                        {
                            canProceed = true;
                        }
                    }

                    if (canProceed)
                    {
                        var obj = new GameObject(detailObjectData.DetailScriptableObject.Name);
                        obj.transform.parent = this.transform;
                        
                        var detailObj = obj.AddComponent<DetailObject>();
                        detailObj.DetailScriptableObject = detailObjectData.DetailScriptableObject;
                        var detailScriptableObject = detailObj.DetailScriptableObject;
                        
                        detailScriptableObject.SubCellDivision = detailObjectData.SubCellDivision;
                        
                        var count = CameraCellData.Count * detailObjectData.SubCellDivision;
                        var size = pwTerrainData.Size.x / count;
                        var radius = (new Vector3(size, size, size) * 0.5f).magnitude;
                        
                        float maxDistPlusCellMag = detailScriptableObject.ScaleRangeMax + radius + radius;
                        detailScriptableObject.MaxDistSQR = new Vector2 (detailScriptableObject.ScaleRangeMax + maxDistPlusCellMag,0f).sqrMagnitude;
                        
                        detailScriptableObject.DebugColor = detailObjectData.DebugColor;
                        detailScriptableObject.DebugCellSize = 0.6f / (float) m_detailObjectList.Count * (float) i + 0.2f;
                        detailScriptableObject.SourceDataType = detailObjectData.SourceDataType;
                        detailScriptableObject.SourceDataIndex = detailObjectData.SourceDataIndex;

                        var detailScriptableDebugColor = detailScriptableObject.DebugColor;
                        if (detailScriptableDebugColor.r == 0 && detailScriptableDebugColor.g == 0 && detailScriptableDebugColor.b == 0)
                        {
                            var col = new Vector3(Random.value, Random.value, Random.value).normalized;
                            detailScriptableObject.DebugColor = new Color(col.x, col.y, col.z, 1f);
                            var detailObjectDataDebugColor = detailObjectData.DebugColor;
                            detailObjectDataDebugColor.r = col.x;
                            detailObjectData.DebugColor = detailObjectDataDebugColor;
                        }

                        detailScriptableObject.DebugColor = detailScriptableDebugColor;
                    }
                    else
                    {
                        Debug.LogWarning("Requested index of " + detailObjectData.SourceDataIndex.ToString() +
                                         " of the type " + detailObjectData.ToString() +
                                         " is not in range.  Detail Instance not created on Terrain tile " + this.name);
                    }
                }
            }

            CoreFloraTerrainData = pwTerrainData;
        }

   

        private void Update()
        {
            if (DetailCamera == null)
            {
                DetailCamera = Camera.main;
                return;
            }

            var pwTerrainData = CoreFloraTerrainData;
            var cameraCellData = CameraCellData;
            var detailCamera = DetailCamera;
            if (pwTerrainData.IsReady && cameraCellData.IsReady)
            {
                // Test terrain for visibility
                //CalculateFrustumPlanes(cFPlanes, detailCamera);
                GeometryUtility.CalculateFrustumPlanes(detailCamera, cFPlanes);
                bool boundsTest = BoundsTest(detailCamera, cFPlanes);
                bool distanceTest = DistanceTest(pwTerrainData.MAXDrawDistMinusExtentsSqr,
                    detailCamera.transform.position, pwTerrainData.Bounds.center);
                if (boundsTest && distanceTest)
                    pwTerrainData.IsVisible = true;
                else
                    pwTerrainData.IsVisible = false;
                
                // If terrain is visable test camera cells for visabillity
                if (pwTerrainData.IsVisible)
                    DetailCameraCellsData.CameraCellUpdate(detailCamera, cFPlanes, ref cameraCellData,
                        ref pwTerrainData);
            }

            CoreFloraTerrainData = pwTerrainData;
            CameraCellData = cameraCellData;

            if (m_lastCellDataPosition != transform.position)
            {
                UpdateCellData();
            }    

        }

        internal void Refresh()
        {
            //Disable terrain details
            DisableTerrainVegetation();
            // Get Init Terrain
            InitTerrain();
            //Init CameraCells
            InitCameraCells();
            FloraGlobals.SetShaderGlobals();
        }

        private void OnDrawGizmos()
        {
            if (DetailCamera == null)
            {
                DetailCamera = Camera.main;
                return;
            }

            var drawDebugInfo = DrawDebugInfo;
            var detailCamera = DetailCamera;
            if (Application.isPlaying && Application.isEditor && CoreFloraTerrainData.IsReady &&
                CameraCellData.IsReady && drawDebugInfo)
            {
                if (CoreFloraTerrainData.IsVisible)
                {
                    DrawDebug();
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(CoreFloraTerrainData.Bounds.center, CoreFloraTerrainData.Bounds.size);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(CoreFloraTerrainData.Bounds.center, CoreFloraTerrainData.Bounds.size * 0.25f);
                    Gizmos.DrawLine(CoreFloraTerrainData.Bounds.center, detailCamera.transform.position);
                }
                else
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(CoreFloraTerrainData.Bounds.center, CoreFloraTerrainData.Bounds.size);
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(detailCamera.transform.position, Vector3.one * 30f);
            }
        }

        internal bool InitCameraCells()
        {
            CleanUpCameraCells();
            var cameraCellData = CameraCellData;
            cameraCellData.IsReady = false;
            if (FindCamera())
            {
                var pwTerrainData = CoreFloraTerrainData;
                var baselineCellDensity = BaselineCellDensity;
                m_detailCameraCells = gameObject.AddComponent<CameraCells>();

                int cellDensity = (int) baselineCellDensity;
                if (FloraGlobals.Settings.CameraCellGlobalSubdivisionModifier > 0)
                {
                    cellDensity =
                        Math.Min(
                            (int) baselineCellDensity << FloraGlobals.Settings.CameraCellGlobalSubdivisionModifier,
                            64);
                }
                else if (FloraGlobals.Settings.CameraCellGlobalSubdivisionModifier < 0)
                {
                    cellDensity =
                        Math.Max(
                            (int) baselineCellDensity >>
                            Math.Abs(FloraGlobals.Settings.CameraCellGlobalSubdivisionModifier), 2);
                }

                DetailCameraCellsData.InitCells(ref cameraCellData, ref pwTerrainData, cellDensity);
                DetailCameraCellsData.BuildCameraCells(ref cameraCellData, ref pwTerrainData);
                m_lastCellDataPosition = transform.position;
                cameraCellData.IsReady = true;
                CoreFloraTerrainData = pwTerrainData;
            }
            else
            {
                Debug.LogWarning(" Procedural Worlds Details Camera init failed to find camera");
            }

            CameraCellData = cameraCellData;
            return CameraCellData.IsReady;
        }

        public void UpdateCellData()
        {
            if (DetailCameraCellsData == null)
            {
                return;
            }
            var pwTerrainData = CoreFloraTerrainData;
            pwTerrainData.Position = transform.position;
            Terrain terrain = GetComponent<Terrain>();
            pwTerrainData.Bounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);
            CoreFloraTerrainData = pwTerrainData;
            InitCameraCells();

            foreach (DetailObject detailObject  in GetComponentsInChildren<DetailObject>())
            {
                detailObject.RefreshAll();
            }
        }

        internal bool InitTerrain()
        {
            var pwTerrainData = CoreFloraTerrainData;
            pwTerrainData.IsReady = false;
            switch (TerrainType)
            {
                case FloraTerrainType.UnityTerrain:
                    var terrain = UnityTerrain;
                    
                    FindUnityTerrain(ref terrain);
                    CaptureUnityTerrain(ref terrain, ref pwTerrainData);

                    if (terrain != null)
                        pwTerrainData.IsReady = true;
                    break;
                
                case FloraTerrainType.ExternalMap:

                    pwTerrainData.IsVisible = false;
                    pwTerrainData.Position = m_externalMapData.Position;
                    pwTerrainData.Size = m_externalMapData.Size;
                    pwTerrainData.Bounds = new Bounds(m_externalMapData.Position + m_externalMapData.Size * 0.5f,m_externalMapData.Size);
                    
                    
                    // basic checks
                    if (m_externalMapData.Height == null)
                    {
                        return false;
                    }
                    
                    if (m_externalMapData.Splat == null && m_externalMapData.Detail == null)
                    {
                        return false;
                    }

                    // get heights
                    if (m_externalMapData.Height.width == m_externalMapData.Height.height)
                    {
                        pwTerrainData.Height = new float[m_externalMapData.Height.width,m_externalMapData.Height.height];
                        Color[] heightSamples = m_externalMapData.Height.GetPixels(0, 0, m_externalMapData.Height.width, m_externalMapData.Height.height);
                        
                        for (int y = 0; y < m_externalMapData.Height.height; y++)
                        {
                            for (int x = 0; x < m_externalMapData.Height.width; x++)
                            {
                                pwTerrainData.Height[x, y] = heightSamples[x + y * m_externalMapData.Height.width].r;
                            }
                        }
                        pwTerrainData.HeightmapResolution = m_externalMapData.Height.width;
                        pwTerrainData.MAXDrawDistSqr = new Vector3(0, 0, Mathf.Abs(m_maximumDrawDistance * FloraGlobals.Settings.TerrainTileGlobalDistanceModifier)).sqrMagnitude;
                        pwTerrainData.MAXDrawDistMinusExtentsSqr = new Vector3(0, 0, Mathf.Abs(m_maximumDrawDistance * FloraGlobals.Settings.TerrainTileGlobalDistanceModifier) + Mathf.Abs(pwTerrainData.Bounds.extents.magnitude)).sqrMagnitude;
                    }
                    
                    // get splats

                    pwTerrainData.SplatPrototypesCount = m_externalMapData.Splat.Length;

                    if (m_externalMapData.Splat.Length > 0 && m_externalMapData.Splat[0] != null)
                    {
                        if (m_externalMapData.Splat[0].width == m_externalMapData.Splat[0].height)
                        {
                            pwTerrainData.Splat = new float[m_externalMapData.Splat[0].width,m_externalMapData.Splat[0].height,m_externalMapData.Splat.Length * 4];
                            for (int splatMapIndex = 0; splatMapIndex < m_externalMapData.Splat.Length; splatMapIndex++)
                            {
                                if (m_externalMapData.Splat[splatMapIndex] != null)
                                {
                                    if (m_externalMapData.Splat[0].width == m_externalMapData.Splat[splatMapIndex].width && m_externalMapData.Splat[0].height == m_externalMapData.Splat[splatMapIndex].height)
                                    {
                                        Color[] splatSample = m_externalMapData.Splat[splatMapIndex].GetPixels(0, 0, m_externalMapData.Splat[splatMapIndex].width, m_externalMapData.Splat[splatMapIndex].height);
                                        for (int y = 0; y < m_externalMapData.Splat[0].height; y++)
                                        {
                                            for (int x = 0; x < m_externalMapData.Splat[0].width; x++)
                                            {
                                                for (int splatChannel = 0; splatChannel < 4; splatChannel++)
                                                {
                                                    pwTerrainData.Splat[x, y, splatMapIndex * 4 + splatChannel] = splatSample[x + y * m_externalMapData.Splat[splatMapIndex].width][splatChannel];
                                                }
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                    }
                    
                    // get details
                    if (m_externalMapData.Detail.Length > 0 && m_externalMapData.Detail[0] != null)
                    {
                        if (m_externalMapData.Detail[0].width == m_externalMapData.Detail[0].height)
                        {
                            pwTerrainData.Detail = new float[m_externalMapData.Detail[0].width,m_externalMapData.Detail[0].height,m_externalMapData.Detail.Length ];
                            for (int detailMapIndex = 0; detailMapIndex < m_externalMapData.Detail.Length; detailMapIndex++)
                            {
                                if (m_externalMapData.Detail[detailMapIndex] != null)
                                {
                                    if (m_externalMapData.Detail[0].width == m_externalMapData.Detail[detailMapIndex].width && m_externalMapData.Detail[0].height == m_externalMapData.Detail[detailMapIndex].height)
                                    {
                                        Color[] detailSample = m_externalMapData.Detail[detailMapIndex].GetPixels(0, 0, m_externalMapData.Detail[detailMapIndex].width, m_externalMapData.Detail[detailMapIndex].height);
                                        for (int y = 0; y < m_externalMapData.Detail[detailMapIndex].height; y++)
                                        {
                                            for (int x = 0; x < m_externalMapData.Detail[detailMapIndex].width; x++)
                                            {
                                                pwTerrainData.Detail[x, y, detailMapIndex] = detailSample[x + y * m_externalMapData.Detail[detailMapIndex].width].r;
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                    }
                    pwTerrainData.IsReady = true;
                    break;
            }

            CoreFloraTerrainData = pwTerrainData;
            return CoreFloraTerrainData.IsReady;
        }
        /// <summary>
        /// Disables the vegetation if a type tree or detail is found in the detail object list
        /// </summary>
        internal void DisableTerrainVegetation()
        {
            bool treesFound = false;
            bool detailFound = false;
            //Disable Trees
            foreach (DetailOverrideData data in m_detailObjectList)
            {
                if (data.DetailScriptableObject != null)
                {
                    switch (data.DetailScriptableObject.SourceDataType)
                    {
                        case SourceDataType.Detail:
                        {
                            detailFound = true;
                            break;
                        }
                        case SourceDataType.Tree:
                            {
                                treesFound = true;
                                break;
                            }
                    }
                }
            }

            if (m_unityTerrain != null)
            {
                if (treesFound && FloraGlobalData.TreesEnabled)
                {
                    m_unityTerrain.treeDistance = 0f;
                }

                if (detailFound)
                {
                    m_unityTerrain.detailObjectDistance = 0f;
                    m_unityTerrain.detailObjectDensity = 0f;
                }
            }
        }

        private void FindUnityTerrain(ref Terrain terrain)
        {
            if (terrain == null)
                terrain = GetComponentInParent<Terrain>();
            if (terrain == null)
                terrain = GetComponentInChildren<Terrain>();
            if (terrain == null)
                terrain = FindObjectOfType<Terrain>();
            if (terrain == null)
                Debug.LogWarning(" Procedural Worlds Detail Manager failed to find suitable terrain");
        }

        public void CleanUpCameraCells()
        {
            if (m_detailCameraCells != null)
                Destroy(m_detailCameraCells);
        }

        /// <summary>
        /// Gets the terrain that the system uses + assigns the terrain it if it's null
        /// </summary>
        /// <returns></returns>
        public Terrain GetTerrain()
        {
            if (UnityTerrain == null)
            {
                UnityTerrain = GetComponent<Terrain>();
            }

            return UnityTerrain;
        }
    }
}