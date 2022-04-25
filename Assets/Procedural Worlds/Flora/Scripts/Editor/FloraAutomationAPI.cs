using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ProceduralWorlds.Flora
{
    public class FloraAutomationAPI
    {
        public static bool ShowDebugging = false;
        public const bool TreesEnabled = false;
        public const string DefaultSavePath = "Assets/Flora User Data/";
        public const string DefaultSavePathMeta = "Assets/Flora User Data.meta";
        private const string DefaultURPShader = "PWS/Details/PW_Details_Foliage_URP";
        private const string DefaultHDRPShader = "PWS/Details/PW_Details_Foliage_HDRP";
        private const string DefaultBuiltInShader = "PWS/Details/PW_Details_Foliage_BuiltIn";
        private static List<GameObject> CurrentPrefabs = new List<GameObject>();
        private static float LastLODDistanceValue = -1f;
        private static float DebugConstFloat = 0f;

        #region Public Functions

        /// <summary>
        /// Sets a new camera that is used to render in all Flora Terrain Tiles
        /// </summary>
        /// <param name="camera"></param>
        public static void SetRenderCamera(Camera camera)
        {
            if (camera != null)
            {
                FloraTerrainTile[] tiles = GameObject.FindObjectsOfType<FloraTerrainTile>();
                if (tiles.Length > 0)
                {
                    foreach (FloraTerrainTile floraTerrainTile in tiles)
                    {
                        floraTerrainTile.DetailCamera = camera;
                        floraTerrainTile.UpdateCellData();
                    }
                }
            }
        }
        /// <summary>
        /// Creates a flora detail scritable object from a prefab
        /// This is mostly used from a unity tree
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="dataType"></param>
        /// <param name="sourceID"></param>
        /// <param name="savePath"></param>
        /// <param name="lodData"></param>
        /// <returns></returns>
        public static List<DetailScriptableObject> CreateFloraRenderFromPrefab(Terrain terrain, GameObject prefab, bool useDensityCheck, CoreCommonFloraData.SourceDataType dataType = CoreCommonFloraData.SourceDataType.Splat, string savePath = DefaultSavePath, int sourceID = -1, CustomLODData lodData = null)
        {
            FloraDefaults floraDefaults = GetFloraDefaults();
            if (floraDefaults == null)
            {
                Debug.LogError("Flora defaults was not found, please make sure the Flora Defaults profile exists within your project.");
                return new List<DetailScriptableObject>();
            }

            if (!floraDefaults.ValidateDefaults())
            {
                Debug.Log("Flora Defaults validate failed, see console for more information");
                return new List<DetailScriptableObject>();
            }

            AddMissingTag();

            LastLODDistanceValue = -1f;
            CurrentPrefabs.Clear();
            List<DetailScriptableObject> floraRenderers = new List<DetailScriptableObject>();
            Camera camera = Camera.main;
            if (camera == null)
            {
                camera = GameObject.FindObjectOfType<Camera>();
            }

            bool useMaterialName = false;
            if (prefab != null)
            {
                CurrentPrefabs.Add(prefab);
                CreateDirectory(savePath);

                LODGroup lodGroup = prefab.GetComponent<LODGroup>();
                if (lodGroup == null)
                {
                    lodGroup = prefab.GetComponentInChildren<LODGroup>();
                }

                float farClipPlane = 1000f;
                if (camera != null)
                {
                    farClipPlane = camera.farClipPlane;
                }

                if (lodGroup != null)
                {
                    LOD[] lods = lodGroup.GetLODs();
                    for (int i = 0; i < lods.Length; i++)
                    {
                        if (floraDefaults.m_enableLODSkipping)
                        {
                            bool skipLOD = false;
                            foreach (SkippableLODData skippableLodData in floraDefaults.m_treeSkippableLODData)
                            {
                                if (lods.Length >= skippableLodData.m_ifHasMoreOrEqualLODS)
                                {
                                    if (i == skippableLodData.m_skipLOD)
                                    {
                                        if (skippableLodData.m_enabled)
                                        {
                                            skipLOD = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (skipLOD)
                            {
                                continue;
                            }
                        }

                        LOD currentLOD = lods[i];
                        if (currentLOD.renderers.Length > 0)
                        {
                            foreach (Renderer renderer in currentLOD.renderers)
                            {
                                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                                if (meshFilter != null)
                                {
                                    DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                                    //Apply Base Settings
                                    detailObject.Name =  prefab.name + " " + renderer.name;
                                    detailObject.SourceDataType = dataType;
                                    //Shadows
                                    detailObject.ShadowMode = renderer.shadowCastingMode;
                                    if (dataType == CoreCommonFloraData.SourceDataType.Tree || sourceID == -1)
                                    {
                                        detailObject.SourceDataIndex = GetTreeSourceIDFromTerrain(prefab, terrain);
                                    }
                                    else
                                    {
                                        detailObject.SourceDataIndex = sourceID;
                                    }

                                    detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);

                                    if (useDensityCheck)
                                    {
                                        detailObject.Density = GetFloraRenderDensityByTreeCount(floraDefaults, prefab, terrain, meshFilter.sharedMesh);
                                    }

                                    detailObject.Mesh = meshFilter.sharedMesh;
                                    if (lodData != null)
                                    {
                                        if (lodData.m_lodData.Count == lods.Length)
                                        {
                                            if (i == 0)
                                            {
                                                detailObject.InDistance = lodData.m_lodData[lods.Length - 1].m_inDistance;
                                                detailObject.InFadeDistance = lodData.m_lodData[lods.Length - 1].m_inFadeDistance;
                                                detailObject.OutDistance = lodData.m_lodData[lods.Length - 1].m_outDistance;
                                                detailObject.OutFadeDistance = lodData.m_lodData[lods.Length - 1].m_outFadeDistance;
                                            }
                                            else
                                            {
                                                detailObject.InDistance = lodData.m_lodData[lods.Length - 1 - i].m_inDistance;
                                                detailObject.InFadeDistance = lodData.m_lodData[lods.Length - 1 - i].m_inFadeDistance;
                                                detailObject.OutDistance = lodData.m_lodData[lods.Length - 1 - i].m_outDistance;
                                                detailObject.OutFadeDistance = lodData.m_lodData[lods.Length - 1 - i].m_outFadeDistance;
                                            }
                                        }
                                        else
                                        {
                                            if (i == 0)
                                            {
                                                detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                                if (LastLODDistanceValue != -1f)
                                                {
                                                    detailObject.InDistance = LastLODDistanceValue;
                                                }
                                                else
                                                {
                                                    detailObject.InDistance = 0f;
                                                }

                                                LastLODDistanceValue = detailObject.OutDistance;
                                            }
                                            else
                                            {
                                                if (i == lods.Length - 1)
                                                {
                                                    detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, 1f);
                                                }
                                                else if (i == lods.Length - 2)
                                                {
                                                    detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias + 0.5f);
                                                }
                                                else
                                                {
                                                    detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                                }

                                                if (LastLODDistanceValue != -1f)
                                                {
                                                    detailObject.InDistance = LastLODDistanceValue;
                                                }
                                                else
                                                {
                                                    detailObject.InDistance = 0f;
                                                }

                                                LastLODDistanceValue = detailObject.OutDistance;
                                            }

                                            detailObject.OutDistance += floraDefaults.m_defaultTreeFadeOutDistance;
                                            detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance;
                                        }
                                    }
                                    else
                                    {
                                        if (i == 0)
                                        {
                                            detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                            if (LastLODDistanceValue != -1f)
                                            {
                                                detailObject.InDistance = LastLODDistanceValue;
                                            }
                                            else
                                            {
                                                detailObject.InDistance = 0f;
                                            }

                                            LastLODDistanceValue = detailObject.OutDistance + floraDefaults.m_defaultTreeFadeOutDistance / 2f;
                                        }
                                        else
                                        {
                                            if (i == lods.Length - 1)
                                            {
                                                detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, 1f);
                                            }
                                            else
                                            {
                                                detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                            }
                                            if (LastLODDistanceValue != -1f)
                                            {
                                                detailObject.InDistance = LastLODDistanceValue;
                                            }
                                            else
                                            {
                                                detailObject.InDistance = 0f;
                                            }

                                            LastLODDistanceValue = detailObject.OutDistance + floraDefaults.m_defaultTreeFadeOutDistance / 2f;
                                        }

                                        detailObject.OutDistance += floraDefaults.m_defaultTreeFadeOutDistance;
                                        detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance;
                                    }

                                    if (floraDefaults.m_useSpeedTreeHueColors)
                                    {
                                        detailObject.ColorA = floraDefaults.m_hueColor1;
                                        detailObject.ColorB = floraDefaults.m_hueColor2;
                                    }

                                    //only take the sub division data into account for LOD0, all other lods should inherit that subdivision value, else flora items can jump or disappear in rendering
                                    if (i == 0)
                                    {
                                        //LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[floraDefaults.m_treeSubDivisionData.Count - 1];
                                        //if (i <= floraDefaults.m_treeSubDivisionData.Count - 1)
                                        //{
                                        //    LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[i];
                                        //    detailObject.SubCellDivision = 1;
                                        //    if (subDivisionData != null)
                                        //    {
                                        //        detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        //    }
                                        //}
                                        //else if (i == floraDefaults.m_treeSubDivisionData.Count - 1)
                                        //{
                                        //    LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[floraDefaults.m_treeSubDivisionData.Count - 1];
                                        //    detailObject.SubCellDivision = 1;
                                        //    if (subDivisionData != null)
                                        //    {
                                        //        detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[0];
                                        detailObject.SubCellDivision = 1;
                                        if (subDivisionData != null)
                                        {
                                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        detailObject.SubCellDivision = floraRenderers[0].SubCellDivision;
                                    }
                                    detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance / detailObject.SubCellDivision;

                                    //Apply Material
                                    List<Material> materials = new List<Material>();
                                    for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                                    {
                                        if (i == lods.Length - 1)
                                        {
                                            materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora", false, -1, false, false));
                                        }
                                        else
                                        {
                                            materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora"));
                                        }
                                    }

                                    detailObject.Mat = materials.ToArray();
                                    floraRenderers.Add(detailObject);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
                    Renderer renderer = prefab.GetComponent<Renderer>();
                    if (meshFilter != null && renderer != null)
                    {
                        useMaterialName = true;
                        DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                        //Apply Base Settings
                        detailObject.Name = renderer.name;
                        detailObject.SourceDataType = dataType;
                        if (dataType == CoreCommonFloraData.SourceDataType.Tree || sourceID == -1)
                        {
                            detailObject.SourceDataIndex = GetTreeSourceIDFromTerrain(prefab, terrain);
                        }
                        else
                        {
                            detailObject.SourceDataIndex = sourceID;
                        }
                        detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);
                        if (useDensityCheck)
                        {
                            detailObject.Density = GetFloraRenderDensityByTreeCount(floraDefaults, prefab, terrain, meshFilter.sharedMesh);
                        }

                        detailObject.Mesh = meshFilter.sharedMesh;
                        if (lodData != null)
                        {
                            if (lodData.m_lodData.Count >= 1)
                            {
                                detailObject.InDistance = lodData.m_lodData[0].m_inDistance;
                                detailObject.InFadeDistance = lodData.m_lodData[0].m_inFadeDistance;
                                detailObject.OutDistance = lodData.m_lodData[0].m_outDistance;
                                detailObject.OutFadeDistance = lodData.m_lodData[0].m_outFadeDistance;
                            }
                            else
                            {
                                detailObject.OutDistance = farClipPlane * QualitySettings.lodBias;
                            }
                        }
                        else
                        {
                            detailObject.OutDistance = farClipPlane * QualitySettings.lodBias;
                        }

                        if (floraDefaults.m_useSpeedTreeHueColors)
                        {
                            detailObject.ColorA = floraDefaults.m_hueColor1;
                            detailObject.ColorB = floraDefaults.m_hueColor2;
                        }

                        LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[0];
                        detailObject.SubCellDivision = 1;
                        if (subDivisionData != null)
                        {
                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                        }
                        detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance / detailObject.SubCellDivision;

                        //Apply Material
                        List<Material> materials = new List<Material>();
                        for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                        {
                            if (j == renderer.sharedMaterials.Length - 1)
                            {
                                materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora"));
                            }
                            else
                            {
                                materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora"));
                            }
                        }
                        detailObject.Mat = materials.ToArray();
                        floraRenderers.Add(detailObject);
                    }
                }
            }
            else
            {
                Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
            }

            List<string> detailPaths = CreateDetails(floraRenderers, savePath, useMaterialName);
            floraRenderers.Clear();
            foreach (string detailPath in detailPaths)
            {
                floraRenderers.Add(AssetDatabase.LoadAssetAtPath<DetailScriptableObject>(detailPath));
            }

            //RepairDetails();

            if (floraRenderers.Count > 0)
            {
                if (ShowDebugging)
                {
                    Debug.Log(prefab.name + " was successfully created and converted into flora renders. You can find these at: " + savePath);
                }
            }

            return floraRenderers;
        }
        /// <summary>
        /// Creates a flora detail scritable object from a prefab
        /// This is mostly used from a unity tree
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="data"></param>
        /// <param name="savePath"></param>
        /// <param name="fetchSourceID"></param>
        /// <returns></returns>
        public static List<DetailScriptableObject> CreateFloraRenderFromPrefab(GameObject prefab, DetailScriptableObject data, string savePath = DefaultSavePath, bool fetchSourceID = true)
        {
            FloraDefaults floraDefaults = GetFloraDefaults();
            if (floraDefaults == null)
            {
                Debug.LogError("Flora defaults was not found, please make sure the Flora Defaults profile exists within your project.");
                return new List<DetailScriptableObject>();
            }

            if (!floraDefaults.ValidateDefaults())
            {
                Debug.Log("Flora Defaults validate failed, see console for more information");
                return new List<DetailScriptableObject>();
            }

            if (data == null)
            {
                Debug.LogError("The data provided was null please make sure you are passing in a valid DetailScriptableObject. Exiting function");
                return new List<DetailScriptableObject>();
            }

            AddMissingTag();

            LastLODDistanceValue = -1f;
            CurrentPrefabs.Clear();
            List<DetailScriptableObject> floraRenderers = new List<DetailScriptableObject>();
            Camera camera = Camera.main;
            if (camera == null)
            {
                camera = GameObject.FindObjectOfType<Camera>();
            }

            bool useMaterialName = false;
            if (prefab != null)
            {
                CurrentPrefabs.Add(prefab);
                CreateDirectory(savePath);

                LODGroup lodGroup = prefab.GetComponent<LODGroup>();
                if (lodGroup == null)
                {
                    lodGroup = prefab.GetComponentInChildren<LODGroup>();
                }

                float farClipPlane = 1000f;
                if (camera != null)
                {
                    farClipPlane = camera.farClipPlane;
                }

                if (lodGroup != null)
                {
                    LOD[] lods = lodGroup.GetLODs();
                    for (int i = 0; i < lods.Length; i++)
                    {
                        if (floraDefaults.m_enableLODSkipping)
                        {
                            bool skipLOD = false;
                            foreach (SkippableLODData skippableLodData in floraDefaults.m_treeSkippableLODData)
                            {
                                if (lods.Length >= skippableLodData.m_ifHasMoreOrEqualLODS)
                                {
                                    if (i == skippableLodData.m_skipLOD)
                                    {
                                        if (skippableLodData.m_enabled)
                                        {
                                            skipLOD = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (skipLOD)
                            {
                                continue;
                            }
                        }

                        LOD currentLOD = lods[i];
                        if (currentLOD.renderers.Length > 0)
                        {
                            foreach (Renderer renderer in currentLOD.renderers)
                            {
                                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                                if (meshFilter != null)
                                {
                                    DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                                    //Apply Base Settings
                                    detailObject.Name =  prefab.name + " " + renderer.name;
                                    detailObject.SourceDataType = data.SourceDataType;
                                    //Shadows
                                    detailObject.ShadowMode = renderer.shadowCastingMode;
                                    //ID
                                    if (fetchSourceID)
                                    {
                                        detailObject.SourceDataIndex = GetTreeSourceIDFromTerrain(prefab);
                                    }
                                    else
                                    {
                                        detailObject.SourceDataIndex = data.SourceDataIndex;
                                    }
                                    detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);
                                    //Mesh
                                    detailObject.Mesh = meshFilter.sharedMesh;
                                    //LOD Info
                                    if (i == 0)
                                    {
                                        detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                        if (LastLODDistanceValue != -1f)
                                        {
                                            detailObject.InDistance = LastLODDistanceValue;
                                        }
                                        else
                                        {
                                            detailObject.InDistance = 0f;
                                        }

                                        LastLODDistanceValue = detailObject.OutDistance + 1f;
                                    }
                                    else
                                    {
                                        if (i == lods.Length - 1)
                                        {
                                            detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, 1f);
                                        }
                                        else
                                        {
                                            detailObject.OutDistance = ConvertLODToDistance(meshFilter, lods[lods.Length - 1 - i].screenRelativeTransitionHeight, floraDefaults.m_defaultTreeLODBias);
                                        }
                                        if (LastLODDistanceValue != -1f)
                                        {
                                            detailObject.InDistance = LastLODDistanceValue;
                                        }
                                        else
                                        {
                                            detailObject.InDistance = 0f;
                                        }

                                        LastLODDistanceValue = detailObject.OutDistance + 1f;
                                    }
                                    detailObject.OutDistance += floraDefaults.m_defaultTreeFadeOutDistance;
                                    detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance;
                                    //Colors
                                    detailObject.ColorA = data.ColorA;
                                    detailObject.ColorB = data.ColorB;
                                    //Sub Division
                                    //only take the sub division data into account for LOD0, all other lods should inherit that subdivision value, else flora items can jump or disappear in rendering
                                    if (i == 0)
                                    {
                                        //if (i <= floraDefaults.m_treeSubDivisionData.Count - 1)
                                        //{
                                        //    LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[i];
                                        //    detailObject.SubCellDivision = 1;
                                        //    if (subDivisionData != null)
                                        //    {
                                        //        detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        //    }
                                        //}
                                        //else if (i == floraDefaults.m_treeSubDivisionData.Count - 1)
                                        //{
                                        //    LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[floraDefaults.m_treeSubDivisionData.Count - 1];
                                        //    detailObject.SubCellDivision = 1;
                                        //    if (subDivisionData != null)
                                        //    {
                                        //        detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[0];
                                        detailObject.SubCellDivision = 1;
                                        if (subDivisionData != null)
                                        {
                                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        detailObject.SubCellDivision = floraRenderers[0].SubCellDivision;
                                    }

                                    //Other settings
                                    detailObject.ColorTransition = data.ColorTransition;
                                    detailObject.ColorRandomization = data.ColorRandomization;
                                    detailObject.DisableDraw = data.DisableDraw;
                                    detailObject.DrawBias = data.DrawBias;
                                    detailObject.Zprime = data.Zprime;
                                    detailObject.ZprimeMaxDistance = data.ZprimeMaxDistance;
                                    detailObject.ZprimeDrawBias = data.ZprimeDrawBias;
                                    detailObject.ZprimeShader = data.ZprimeShader;
                                    detailObject.RandomSeed = data.RandomSeed;
                                    detailObject.Density = data.Density;
                                    detailObject.MaxJitter = data.MaxJitter;
                                    detailObject.ScaleRangeMin = data.ScaleRangeMin;
                                    detailObject.ScaleRangeMax = data.ScaleRangeMax;
                                    detailObject.ScaleByAlpha = data.ScaleByAlpha;
                                    detailObject.ForwardAngleBias = data.ForwardAngleBias;
                                    detailObject.RandomRotationRange = data.RandomRotationRange;
                                    detailObject.UseAdvancedRotations = data.UseAdvancedRotations;
                                    detailObject.AlignToUp = data.AlignToUp;
                                    detailObject.SlopeType = data.SlopeType;
                                    detailObject.ScaleBySlope = data.ScaleBySlope;
                                    detailObject.AlignForwardToSlope = data.AlignForwardToSlope;
                                    detailObject.AlphaThreshold = data.AlphaThreshold;
                                    detailObject.UseNoise = data.UseNoise;
                                    detailObject.NoiseScale = data.NoiseScale;
                                    detailObject.NoiseMaskContrast = data.NoiseMaskContrast;
                                    detailObject.NoiseMaskOffset = data.NoiseMaskOffset;
                                    detailObject.InvertNoise = data.InvertNoise;
                                    detailObject.UseHeight = data.UseHeight;
                                    detailObject.ScaleByHeight = data.ScaleByHeight;
                                    detailObject.UseSlope = data.UseSlope;
                                    detailObject.DrawCPUSpawnLocations = data.DrawCPUSpawnLocations;
                                    detailObject.DebugColor = data.DebugColor;
                                    detailObject.DebugCellSize = data.DebugCellSize;

                                    //Apply Material
                                    List<Material> materials = new List<Material>();
                                    for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                                    {
                                        if (j == renderer.sharedMaterials.Length - 1)
                                        {
                                            materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora", false, -1, false, false));
                                        }
                                        else
                                        {
                                            materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name + " Flora"));
                                        }
                                    }

                                    detailObject.Mat = materials.ToArray();
                                    floraRenderers.Add(detailObject);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
                    Renderer renderer = prefab.GetComponent<Renderer>();
                    if (meshFilter != null && renderer != null)
                    {
                        useMaterialName = true;
                        DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                        //Apply Base Settings
                        detailObject.Name = renderer.name;
                        detailObject.SourceDataType = data.SourceDataType;
                        //Shadows
                        detailObject.ShadowMode = renderer.shadowCastingMode;
                        //ID
                        if (fetchSourceID)
                        {
                            detailObject.SourceDataIndex = GetTreeSourceIDFromTerrain(prefab);
                        }
                        else
                        {
                            detailObject.SourceDataIndex = data.SourceDataIndex;
                        }
                        detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);
                        detailObject.Mesh = meshFilter.sharedMesh;
                        //LOD Info
                        detailObject.OutDistance = farClipPlane;
                        //Colors
                        detailObject.ColorA = data.ColorA;
                        detailObject.ColorB = data.ColorB;
                        //Sub Division
                        LODSubDivisionData subDivisionData = floraDefaults.m_treeSubDivisionData[0];
                        detailObject.SubCellDivision = 1;
                        if (subDivisionData != null)
                        {
                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                        }

                        //Other settings
                        detailObject.ColorTransition = data.ColorTransition;
                        detailObject.ColorRandomization = data.ColorRandomization;
                        detailObject.DisableDraw = data.DisableDraw;
                        detailObject.DrawBias = data.DrawBias;
                        detailObject.Zprime = data.Zprime;
                        detailObject.ZprimeMaxDistance = data.ZprimeMaxDistance;
                        detailObject.ZprimeDrawBias = data.ZprimeDrawBias;
                        detailObject.ZprimeShader = data.ZprimeShader;
                        detailObject.RandomSeed = data.RandomSeed;
                        detailObject.Density = data.Density;
                        detailObject.MaxJitter = data.MaxJitter;
                        detailObject.ScaleRangeMin = data.ScaleRangeMin;
                        detailObject.ScaleRangeMax = data.ScaleRangeMax;
                        detailObject.ScaleByAlpha = data.ScaleByAlpha;
                        detailObject.ForwardAngleBias = data.ForwardAngleBias;
                        detailObject.RandomRotationRange = data.RandomRotationRange;
                        detailObject.UseAdvancedRotations = data.UseAdvancedRotations;
                        detailObject.AlignToUp = data.AlignToUp;
                        detailObject.SlopeType = data.SlopeType;
                        detailObject.ScaleBySlope = data.ScaleBySlope;
                        detailObject.AlignForwardToSlope = data.AlignForwardToSlope;
                        detailObject.AlphaThreshold = data.AlphaThreshold;
                        detailObject.UseNoise = data.UseNoise;
                        detailObject.NoiseScale = data.NoiseScale;
                        detailObject.NoiseMaskContrast = data.NoiseMaskContrast;
                        detailObject.NoiseMaskOffset = data.NoiseMaskOffset;
                        detailObject.InvertNoise = data.InvertNoise;
                        detailObject.UseHeight = data.UseHeight;
                        detailObject.ScaleByHeight = data.ScaleByHeight;
                        detailObject.UseSlope = data.UseSlope;
                        detailObject.DrawCPUSpawnLocations = data.DrawCPUSpawnLocations;
                        detailObject.DebugColor = data.DebugColor;
                        detailObject.DebugCellSize = data.DebugCellSize;

                        //Apply Material
                        List<Material> materials = new List<Material>();

                        for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                        {
                            if (j == renderer.sharedMaterials.Length - 1)
                            {
                                materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name));
                            }
                            else
                            {
                                materials.Add(CreateMaterial(floraDefaults, prefab, renderer.sharedMaterials[j], savePath + renderer.sharedMaterials[j].name));
                            }
                        }

                        detailObject.Mat = materials.ToArray();
                        floraRenderers.Add(detailObject);
                    }
                }
            }
            else
            {
                Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
            }

            List<string> detailPaths = CreateDetails(floraRenderers, savePath, useMaterialName);
            floraRenderers.Clear();
            foreach (string detailPath in detailPaths)
            {
                floraRenderers.Add(AssetDatabase.LoadAssetAtPath<DetailScriptableObject>(detailPath));
            }

            //RepairDetails();

            if (floraRenderers.Count > 0)
            {
                if (ShowDebugging)
                {
                    Debug.Log(prefab.name + " was successfully created and converted into flora renders. You can find these at: " + savePath);
                }
            }

            return floraRenderers;
        }
        /// <summary>
        /// Creates a flora detail scritable object from a texture
        /// This is used from a unity grass terrain detail
        /// </summary>
        /// <param name="detailPrototype"></param>
        /// <param name="dataType"></param>
        /// <param name="sourceID"></param>
        /// <param name="savePath"></param>
        /// <param name="lodData"></param>
        /// <returns></returns>
        public static List<DetailScriptableObject> CreateFloraRenderFromTexture(DetailPrototype detailPrototype, CoreCommonFloraData.SourceDataType dataType = CoreCommonFloraData.SourceDataType.Splat, string savePath = DefaultSavePath, int sourceID = -1, bool castShadows = false, LODData lodData = null)
        {
            FloraDefaults floraDefaults = GetFloraDefaults();
            if (floraDefaults == null)
            {
                Debug.LogError("Flora defaults was not found, please make sure the Flora Defaults profile exists within your project.");
                return new List<DetailScriptableObject>();
            }

            if (!floraDefaults.ValidateDefaults())
            {
                Debug.Log("Flora Defaults validate failed, see console for more information");
                return new List<DetailScriptableObject>();
            }

            if (castShadows)
            {
                if (floraDefaults.m_defaultDetailShadows == ShadowCastingMode.Off)
                {
                    floraDefaults.m_defaultDetailShadows = ShadowCastingMode.TwoSided;
                }
            }
            else
            {
                if (floraDefaults.m_defaultDetailShadows != ShadowCastingMode.Off)
                {
                    floraDefaults.m_defaultDetailShadows = ShadowCastingMode.Off;
                }
            }

            List<DetailScriptableObject> floraRenders = new List<DetailScriptableObject>();
            Terrain terrain = Terrain.activeTerrain;
            if (!floraDefaults.m_defaultsLODDetails)
            {
                if (detailPrototype != null)
                {
                    CreateDirectory(savePath);
                    float detailDistance = 100f;
                    if (terrain != null)
                    {
                        detailDistance = terrain.detailObjectDistance;
                    }

                    if (detailPrototype != null)
                    {
                        DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                        //Apply Base Settings
                        Color colorA = detailPrototype.healthyColor;
                        colorA.a = 0.05f;
                        Color colorB = detailPrototype.dryColor;
                        colorB.a = 0.05f;
                        detailObject.ColorA = colorA;
                        detailObject.ColorB = colorB;
                        if (castShadows)
                        {
                            detailObject.ShadowMode = ShadowCastingMode.TwoSided;
                        }
                        else
                        {
                            detailObject.ShadowMode = ShadowCastingMode.Off;
                        }

                        LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[0];
                        detailObject.SubCellDivision = 1;
                        if (subDivisionData != null)
                        {
                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                        }
                        detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMesh);
                        detailObject.Name = detailPrototype.prototypeTexture.name + " PW Grass Data";
                        detailObject.SourceDataType = dataType;
                        detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMesh;
                        detailObject.ScaleRangeMin = detailPrototype.minHeight;
                        detailObject.ScaleRangeMax = detailPrototype.maxHeight;
                        if (lodData != null)
                        {
                            detailObject.InDistance = lodData.m_inDistance;
                            detailObject.InFadeDistance = lodData.m_inFadeDistance;
                            detailObject.OutDistance = lodData.m_outDistance;
                            detailObject.OutFadeDistance = lodData.m_outFadeDistance;
                        }
                        else
                        {
                            detailObject.OutDistance = detailDistance * floraDefaults.m_defaultTerrainDetailLODBias;
                            detailObject.OutFadeDistance = floraDefaults.m_defaultDetailsFadeOutDistance;
                        }

                        //Apply Material
                        Material generalMaterial = new Material(Shader.Find(DefaultBuiltInShader));
                        generalMaterial.SetTexture("_BaseMap", detailPrototype.prototypeTexture);
                        List<Material> materials = new List<Material>
                        {
                            CreateMaterial(floraDefaults, null, generalMaterial, savePath + detailPrototype.prototypeTexture.name, true, -1, true)
                        };

                        BuildGrassWindData(detailPrototype, materials[0]);
                        detailObject.Mat = materials.ToArray();
                        //ID
                        if (sourceID == -1)
                        {
                            detailObject.SourceDataIndex = GetDetailSourceIDFromTerrain(GetBaseMapTexture(detailObject.Mat[0]));
                        }
                        else
                        {
                            detailObject.SourceDataIndex = sourceID;
                        }
                        detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);

                        floraRenders.Add(detailObject);
                    }
                    else
                    {
                        Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
                    }
                }
            }
            else
            {
                LastLODDistanceValue = -1f;
                for (int i = 0; i < 2; i++)
                {
                    if (detailPrototype != null)
                    {
                        CreateDirectory(savePath);
                        float detailDistance = 100f;
                        if (terrain != null)
                        {
                            detailDistance = terrain.detailObjectDistance;
                        }

                        if (detailPrototype != null)
                        {
                            DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                            //Apply Base Settings
                            Color colorA = detailPrototype.healthyColor;
                            colorA.a = 0.05f;
                            Color colorB = detailPrototype.dryColor;
                            colorB.a = 0.05f;
                            detailObject.ColorA = colorA;
                            detailObject.ColorB = colorB;
                            if (i == 0)
                            {
                                detailObject.ShadowMode = floraDefaults.m_defaultDetailShadows;
                            }
                            else
                            {
                                if (floraDefaults.m_terrainDetailLOD1Shadows)
                                {
                                    detailObject.ShadowMode = floraDefaults.m_defaultDetailShadows;
                                }
                                else
                                {
                                    detailObject.ShadowMode = ShadowCastingMode.Off;
                                }
                            }

                            detailObject.Name = detailPrototype.prototypeTexture.name + " PW Grass LOD" + i;
                            detailObject.SourceDataType = dataType;
                            //ID
                            if (sourceID == -1)
                            {
                                detailObject.SourceDataIndex = GetDetailSourceIDFromTerrain(GetBaseMapTexture(detailObject.Mat[0]));
                            }
                            else
                            {
                                detailObject.SourceDataIndex = sourceID;
                            }
                            detailObject.DebugColor = GetDebugColor(detailObject.SourceDataIndex);
                            if (i == 0)
                            {
                                detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMeshLOD0;
                                detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMeshLOD0);
                            }
                            else
                            {
                                detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMeshLOD1;
                                detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMeshLOD1);
                            }
    
                            detailObject.ScaleRangeMin = detailPrototype.minHeight;
                            detailObject.ScaleRangeMax = detailPrototype.maxHeight;
                            if (lodData != null)
                            {
                                detailObject.InDistance = lodData.m_inDistance;
                                detailObject.InFadeDistance = lodData.m_inFadeDistance;
                                detailObject.OutDistance = lodData.m_outDistance;
                                detailObject.OutFadeDistance = lodData.m_outFadeDistance;
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    detailObject.OutDistance = (detailDistance / 2f) * floraDefaults.m_defaultTerrainDetailLODBias;
                                }
                                else
                                {
                                    detailObject.OutDistance = detailDistance * floraDefaults.m_defaultTerrainDetailLODBias;
                                }

                                if (LastLODDistanceValue != -1f)
                                {
                                    detailObject.InDistance = LastLODDistanceValue - floraDefaults.m_defaultDetailsFadeOutDistance;
                                }
                                else
                                {
                                    detailObject.InDistance = 0f;
                                }

                                LastLODDistanceValue = detailObject.OutDistance + 1f;
                                detailObject.OutDistance += floraDefaults.m_defaultDetailsFadeOutDistance;
                                if (i == 1)
                                {
                                    detailObject.InFadeDistance = floraDefaults.m_defaultDetailsFadeOutDistance / 2f;
                                }
                            }

                            if (i <= floraDefaults.m_detailSubDivisionData.Count - 1)
                            {
                                LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[i];
                                detailObject.SubCellDivision = 1;
                                if (subDivisionData != null)
                                {
                                    detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                }
                            }
                            else
                            {
                                LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[0];
                                detailObject.SubCellDivision = 1;
                                if (subDivisionData != null)
                                {
                                    detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                }
                            }
                            detailObject.OutFadeDistance = floraDefaults.m_defaultTreeFadeOutDistance / detailObject.SubCellDivision;

                            //Apply Material
                            Material generalMaterial = new Material(Shader.Find(DefaultBuiltInShader));
                            generalMaterial.SetTexture("_BaseMap", detailPrototype.prototypeTexture);
                            List<Material> materials = new List<Material>
                            {
                                CreateMaterial(floraDefaults,null, generalMaterial, savePath + detailPrototype.prototypeTexture.name + "LOD" + i, true, i, true)
                            };

                            BuildGrassWindData(detailPrototype, materials[0]);
                            detailObject.Mat = materials.ToArray();
                            floraRenders.Add(detailObject);
                        }
                        else
                        {
                            Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
                        }
                    }
                }
            }

            List<string> detailPaths = CreateDetails(floraRenders, savePath, true);
            floraRenders.Clear();
            foreach (string detailPath in detailPaths)
            {
                floraRenders.Add(AssetDatabase.LoadAssetAtPath<DetailScriptableObject>(detailPath));
            }

            if (floraRenders.Count > 0)
            {
                if (ShowDebugging)
                {
                    Debug.Log(detailPrototype.prototypeTexture.name + " was successfully created and converted into flora renders. You can find these at: " + savePath);
                }
            }

            return floraRenders;
        }
        /// <summary>
        /// Creates a flora detail scritable object from a texture
        /// This is used from a unity grass terrain detail
        /// </summary>
        /// <param name="detailPrototype"></param>
        /// <param name="dataType"></param>
        /// <param name="sourceID"></param>
        /// <param name="savePath"></param>
        /// <param name="lodData"></param>
        /// <returns></returns>
        public static List<DetailScriptableObject> CreateFloraRenderFromTexture(DetailPrototype detailPrototype, DetailScriptableObject data, string savePath = DefaultSavePath)
        {
            FloraDefaults floraDefaults = GetFloraDefaults();
            if (floraDefaults == null)
            {
                Debug.LogError("Flora defaults was not found, please make sure the Flora Defaults profile exists within your project.");
                return new List<DetailScriptableObject>();
            }

            if (!floraDefaults.ValidateDefaults())
            {
                Debug.Log("Flora Defaults validate failed, see console for more information");
                return new List<DetailScriptableObject>();
            }

            if (data == null)
            {
                Debug.LogError("The data provided was null please make sure you are passing in a valid DetailScriptableObject. Exiting function");
                return new List<DetailScriptableObject>();
            }

            List<DetailScriptableObject> floraRenders = new List<DetailScriptableObject>();
            Terrain terrain = Terrain.activeTerrain;
            if (!floraDefaults.m_defaultsLODDetails)
            {
                if (detailPrototype != null)
                {
                    CreateDirectory(savePath);
                    float detailDistance = 100f;
                    if (terrain != null)
                    {
                        detailDistance = terrain.detailObjectDistance;
                    }

                    if (detailPrototype != null)
                    {
                        DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                        //Apply Base Settings
                        Color colorA = detailPrototype.healthyColor;
                        colorA.a = 0.05f;
                        Color colorB = detailPrototype.dryColor;
                        colorB.a = 0.05f;
                        detailObject.ColorA = colorA;
                        detailObject.ColorB = colorB;

                        LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[0];
                        detailObject.SubCellDivision = 1;
                        if (subDivisionData != null)
                        {
                            detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                        }
                        detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMesh);
                        detailObject.Name = detailPrototype.prototypeTexture.name + " PW Grass Data";
                        detailObject.SourceDataType = data.SourceDataType;
                        detailObject.SourceDataIndex = data.SourceDataIndex;
                        detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMesh;
                        detailObject.ScaleRangeMin = detailPrototype.minHeight;
                        detailObject.ScaleRangeMax = detailPrototype.maxHeight;
                        detailObject.OutDistance = detailDistance * floraDefaults.m_defaultTerrainDetailLODBias;
                        detailObject.OutFadeDistance = floraDefaults.m_defaultDetailsFadeOutDistance;

                        //Other settings
                        detailObject.ColorTransition = data.ColorTransition;
                        detailObject.ColorRandomization = data.ColorRandomization;
                        detailObject.DisableDraw = data.DisableDraw;
                        detailObject.DrawBias = data.DrawBias;
                        detailObject.Zprime = data.Zprime;
                        detailObject.ZprimeMaxDistance = data.ZprimeMaxDistance;
                        detailObject.ZprimeDrawBias = data.ZprimeDrawBias;
                        detailObject.ZprimeShader = data.ZprimeShader;
                        detailObject.RandomSeed = data.RandomSeed;
                        detailObject.Density = data.Density;
                        detailObject.MaxJitter = data.MaxJitter;
                        detailObject.ScaleRangeMin = data.ScaleRangeMin;
                        detailObject.ScaleRangeMax = data.ScaleRangeMax;
                        detailObject.ScaleByAlpha = data.ScaleByAlpha;
                        detailObject.ForwardAngleBias = data.ForwardAngleBias;
                        detailObject.RandomRotationRange = data.RandomRotationRange;
                        detailObject.UseAdvancedRotations = data.UseAdvancedRotations;
                        detailObject.AlignToUp = data.AlignToUp;
                        detailObject.SlopeType = data.SlopeType;
                        detailObject.ScaleBySlope = data.ScaleBySlope;
                        detailObject.AlignForwardToSlope = data.AlignForwardToSlope;
                        detailObject.AlphaThreshold = data.AlphaThreshold;
                        detailObject.UseNoise = data.UseNoise;
                        detailObject.NoiseScale = data.NoiseScale;
                        detailObject.NoiseMaskContrast = data.NoiseMaskContrast;
                        detailObject.NoiseMaskOffset = data.NoiseMaskOffset;
                        detailObject.InvertNoise = data.InvertNoise;
                        detailObject.UseHeight = data.UseHeight;
                        detailObject.ScaleByHeight = data.ScaleByHeight;
                        detailObject.UseSlope = data.UseSlope;
                        detailObject.DrawCPUSpawnLocations = data.DrawCPUSpawnLocations;
                        detailObject.DebugColor = data.DebugColor;
                        detailObject.DebugCellSize = data.DebugCellSize;

                        //Apply Material
                        Material generalMaterial = new Material(Shader.Find(DefaultBuiltInShader));
                        generalMaterial.SetTexture("_BaseMap", detailPrototype.prototypeTexture);
                        List<Material> materials = new List<Material>
                        {
                            CreateMaterial(floraDefaults, null, generalMaterial, savePath + detailPrototype.prototypeTexture.name, true, -1, true)
                        };

                        BuildGrassWindData(detailPrototype, materials[0]);
                        detailObject.Mat = materials.ToArray();
                        detailObject.SourceDataIndex = GetDetailSourceIDFromTerrain(GetBaseMapTexture(detailObject.Mat[0]));

                        floraRenders.Add(detailObject);
                    }
                    else
                    {
                        Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
                    }
                }
            }
            else
            {
                LastLODDistanceValue = -1f;
                for (int i = 0; i < 2; i++)
                {
                    if (detailPrototype != null)
                    {
                        CreateDirectory(savePath);
                        float detailDistance = 100f;
                        if (terrain != null)
                        {
                            detailDistance = terrain.detailObjectDistance;
                        }

                        if (detailPrototype != null)
                        {
                            DetailScriptableObject detailObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
                            //Apply Base Settings
                            Color colorA = detailPrototype.healthyColor;
                            colorA.a = 0.05f;
                            Color colorB = detailPrototype.dryColor;
                            colorB.a = 0.05f;
                            detailObject.ColorA = colorA;
                            detailObject.ColorB = colorB;
                            if (i == 0)
                            {
                                detailObject.ShadowMode = floraDefaults.m_defaultDetailShadows;
                            }
                            else
                            {
                                if (floraDefaults.m_terrainDetailLOD1Shadows)
                                {
                                    detailObject.ShadowMode = floraDefaults.m_defaultDetailShadows;
                                }
                                else
                                {
                                    detailObject.ShadowMode = ShadowCastingMode.Off;
                                }
                            }

                            detailObject.Name = detailPrototype.prototypeTexture.name + " PW Grass LOD" + i;
                            detailObject.SourceDataType = data.SourceDataType;
                            detailObject.SourceDataIndex = data.SourceDataIndex;
                            if (i == 0)
                            {
                                detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMeshLOD0;
                                detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMeshLOD0);
                            }
                            else
                            {
                                detailObject.Mesh = floraDefaults.m_defaultTerrainDetailMeshLOD1;
                                detailObject.Density = floraDefaults.GetDetailDensityMultiplier(terrain, floraDefaults.m_defaultTerrainDetailMeshLOD1);
                            }
    
                            detailObject.ScaleRangeMin = detailPrototype.minHeight;
                            detailObject.ScaleRangeMax = detailPrototype.maxHeight;
                            //LOD Info
                            if (i == 0)
                            {
                                detailObject.OutDistance = (detailDistance / 2f) * floraDefaults.m_defaultTerrainDetailLODBias;
                            }
                            else
                            {
                                detailObject.OutDistance = detailDistance * floraDefaults.m_defaultTerrainDetailLODBias;
                            }

                            if (LastLODDistanceValue != -1f)
                            {
                                detailObject.InDistance = LastLODDistanceValue - floraDefaults.m_defaultDetailsFadeOutDistance;
                            }
                            else
                            {
                                detailObject.InDistance = 0f;
                            }

                            LastLODDistanceValue = detailObject.OutDistance + 1f;
                            detailObject.OutDistance += floraDefaults.m_defaultDetailsFadeOutDistance;
                            detailObject.OutFadeDistance = floraDefaults.m_defaultDetailsFadeOutDistance;
                            if (i == 1)
                            {
                                detailObject.InFadeDistance = floraDefaults.m_defaultDetailsFadeOutDistance / 2f;
                            }
                            //Sub Division
                            if (i <= floraDefaults.m_detailSubDivisionData.Count - 1)
                            {
                                LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[i];
                                detailObject.SubCellDivision = 1;
                                if (subDivisionData != null)
                                {
                                    detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                }
                            }
                            else
                            {
                                LODSubDivisionData subDivisionData = floraDefaults.m_detailSubDivisionData[0];
                                detailObject.SubCellDivision = 1;
                                if (subDivisionData != null)
                                {
                                    detailObject.SubCellDivision = subDivisionData.m_subDivisionValue;
                                }
                            }

                            //Other settings
                            detailObject.ColorTransition = data.ColorTransition;
                            detailObject.ColorRandomization = data.ColorRandomization;
                            detailObject.DisableDraw = data.DisableDraw;
                            detailObject.DrawBias = data.DrawBias;
                            detailObject.Zprime = data.Zprime;
                            detailObject.ZprimeMaxDistance = data.ZprimeMaxDistance;
                            detailObject.ZprimeDrawBias = data.ZprimeDrawBias;
                            detailObject.ZprimeShader = data.ZprimeShader;
                            detailObject.RandomSeed = data.RandomSeed;
                            detailObject.MaxJitter = data.MaxJitter;
                            detailObject.ScaleByAlpha = data.ScaleByAlpha;
                            detailObject.ForwardAngleBias = data.ForwardAngleBias;
                            detailObject.RandomRotationRange = data.RandomRotationRange;
                            detailObject.UseAdvancedRotations = data.UseAdvancedRotations;
                            detailObject.AlignToUp = data.AlignToUp;
                            detailObject.SlopeType = data.SlopeType;
                            detailObject.ScaleBySlope = data.ScaleBySlope;
                            detailObject.AlignForwardToSlope = data.AlignForwardToSlope;
                            detailObject.AlphaThreshold = data.AlphaThreshold;
                            detailObject.UseNoise = data.UseNoise;
                            detailObject.NoiseScale = data.NoiseScale;
                            detailObject.NoiseMaskContrast = data.NoiseMaskContrast;
                            detailObject.NoiseMaskOffset = data.NoiseMaskOffset;
                            detailObject.InvertNoise = data.InvertNoise;
                            detailObject.UseHeight = data.UseHeight;
                            detailObject.ScaleByHeight = data.ScaleByHeight;
                            detailObject.UseSlope = data.UseSlope;
                            detailObject.DrawCPUSpawnLocations = data.DrawCPUSpawnLocations;
                            detailObject.DebugColor = data.DebugColor;
                            detailObject.DebugCellSize = data.DebugCellSize;

                            //Apply Material
                            Material generalMaterial = new Material(Shader.Find(DefaultBuiltInShader));
                            generalMaterial.SetTexture("_BaseMap", detailPrototype.prototypeTexture);
                            List<Material> materials = new List<Material>
                            {
                                CreateMaterial(floraDefaults,null, generalMaterial, savePath + detailPrototype.prototypeTexture.name + "LOD" + i, true, i, true)
                            };

                            BuildGrassWindData(detailPrototype, materials[0]);
                            detailObject.Mat = materials.ToArray();
                            detailObject.SourceDataIndex = GetDetailSourceIDFromTerrain(GetBaseMapTexture(detailObject.Mat[0]));

                            floraRenders.Add(detailObject);
                        }
                        else
                        {
                            Debug.LogError("No prefab was provided for CreateFloraRenderFromPrefab()");
                        }
                    }
                }
            }

            List<string> detailPaths = CreateDetails(floraRenders, savePath, true);
            floraRenders.Clear();
            foreach (string detailPath in detailPaths)
            {
                floraRenders.Add(AssetDatabase.LoadAssetAtPath<DetailScriptableObject>(detailPath));
            }

            if (floraRenders.Count > 0)
            {
                if (ShowDebugging)
                {
                    Debug.Log(detailPrototype.prototypeTexture.name + " was successfully created and converted into flora renders. You can find these at: " + savePath);
                }
            }

            return floraRenders;
        }
        /// <summary>
        /// Adds a new prefab to the current list
        /// </summary>
        /// <param name="prefab"></param>
        public static void AddCurrentPrefab(GameObject prefab)
        {
            if (!DoesCurrentPrefabExist(prefab))
            {
                CurrentPrefabs.Add(prefab);
            }
        }
        /// <summary>
        /// Removes a prefab from the current list
        /// </summary>
        /// <param name="prefab"></param>
        public static void RemoveCurrentPrefab(GameObject prefab)
        {
            if (DoesCurrentPrefabExist(prefab))
            {
                CurrentPrefabs.Remove(prefab);
            }
        }
        /// <summary>
        /// Creates a density value for flora based on how many tree counts there are.
        /// </summary>
        /// <param name="floraDefaults"></param>
        /// <param name="treePrefab"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static int GetFloraRenderDensityByTreeCount(FloraDefaults floraDefaults, GameObject treePrefab, Terrain terrain, Mesh mesh, int defaultDensityValue = 32)
        {
            int densityValue = defaultDensityValue;
            if (terrain != null && treePrefab != null && floraDefaults != null)
            {
                int treeIndex = GetTreeIDFromPrefab(terrain, treePrefab);
                if (treeIndex != -1)
                {
                    densityValue = GetTreeDensityBasedOnTreeCount(floraDefaults, terrain, treeIndex, mesh, densityValue);
                }
            }

            return densityValue;
        }
        /// <summary>
        /// Gets the minimum recommended tree count from flora defaults
        /// Will return -1 if defaults is null
        /// </summary>
        /// <returns></returns>
        public static int GetFloraMinimunRecommendedTreeCount()
        {
            FloraDefaults defaults = GetFloraDefaults();
            if (defaults != null)
            {
                return defaults.m_minimumRecommendedTreeCount;
            }

            return -1;
        }
        /// <summary>
        /// Gets a tree prototype ID based on the tree prefab on the provided terrain
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="treePrefab"></param>
        /// <returns></returns>
        public static int GetTreeIDFromPrefab(Terrain terrain, GameObject treePrefab)
        {
            TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
            if (treePrototypes.Length > 0)
            {
                int treeIndex = -1;
                for (int i = 0; i < treePrototypes.Length; i++)
                {
                    if (treePrototypes[i].prefab == treePrefab)
                    {
                        treeIndex = i;
                        break;
                    }
                }

                if (treeIndex != -1)
                {
                    return treeIndex;
                }
            }

            return -1;
        }
        /// <summary>
        /// Gets tree count from a terrain based on the ID
        /// Will calculate a value if the tree count is above the minimum recommended tree count from flora defaults
        /// Returns current value if tree minimum count is not greater than the value
        /// </summary>
        /// <param name="floraDefaults"></param>
        /// <param name="terrain"></param>
        /// <param name="treeIndex"></param>
        /// <returns></returns>
        public static int GetTreeDensityBasedOnTreeCount(FloraDefaults floraDefaults, Terrain terrain, int treeIndex, Mesh mesh, int currentValue = 32)
        {
            List<TreeInstance> treeInstances = terrain.terrainData.treeInstances.ToList();
            int count = treeInstances.Count(x => x.prototypeIndex == treeIndex);
            if (count >= floraDefaults.m_minimumRecommendedTreeCount)
            {
                float treesPerSquareMeter = count / (terrain.terrainData.size.x * terrain.terrainData.size.z);
                int densityValue = Mathf.RoundToInt(Mathf.Lerp(2,1023,treesPerSquareMeter));
                densityValue *= (int)GetCellDensityFromTerrain(floraDefaults, terrain);
                if (mesh != null)
                {
                    densityValue =  (int)Mathf.Lerp(densityValue / 1.5f, densityValue * 2, Mathf.InverseLerp(floraDefaults.m_minMaxTreeVertexCount.x, floraDefaults.m_minMaxTreeVertexCount.y, mesh.vertexCount));
                }

                return densityValue;
            }

            return currentValue;
        }
        /// <summary>
        /// Adds the scene name to the current path
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static string GetUniquePathWithSceneName(string currentPath)
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (scene != null)
            {
                if (!string.IsNullOrEmpty(scene.name))
                {
                    return currentPath += scene.name + "/";
                }
            }
            else
            {
                EditorSceneManager.SaveOpenScenes();
                scene = EditorSceneManager.GetActiveScene();
                if (scene != null)
                {
                    if (!string.IsNullOrEmpty(scene.name))
                    {
                        return currentPath += scene.name + "/";
                    }
                }
            }

            return currentPath;
        }
        /// <summary>
        /// Get all the trees of the terrains and creates the flora renderers and adds it to the flora terrain tiles if the tree exists on the terrain
        /// </summary>
        /// <param name="terrain"></param>
        public static void GetTrees(Terrain terrain, string savePath)
        {
            if (!FloraGlobalData.TreesEnabled)
            {
                return;
            }
#pragma warning disable 162
            if (terrain == null)
            {
                Debug.LogError("No terrain in your scene.");
                return;
            }

            TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
            CleanUpMissingDetails();
            if (treePrototypes.Length > 0)
            {
                foreach (TreePrototype prototype in treePrototypes)
                {
                    if (prototype.prefab != null)
                    {
                        List<DetailScriptableObject> details = CreateFloraRenderFromPrefab(terrain, prototype.prefab, true, CoreCommonFloraData.SourceDataType.Tree, savePath);
                        ApplyToTerrains(details, false);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("No Trees Found", "No trees were found on any terrain in your scene.", "Ok");
            }
#pragma warning restore 162

        }
        /// <summary>
        /// Get all the details of the terrains and creates the flora renderers and adds it to the flora terrain tiles if the detail exists on the terrain
        /// </summary>
        /// <param name="terrain"></param>
        public static void GetDetails(Terrain terrain, string savePath)
        {
            if (terrain == null)
            {
                Debug.LogError("No terrain in your scene.");
                return;
            }

            bool castShadows = false;
            bool shadowsBeenSet = false;
            DetailPrototype[] detailPrototypes = terrain.terrainData.detailPrototypes;
            CleanUpMissingDetails();
            if (detailPrototypes.Length > 0)
            {
                for (int i = 0; i < detailPrototypes.Length; i++)
                {
                    switch (detailPrototypes[i].renderMode)
                    {
                        case DetailRenderMode.VertexLit:
                        {
                            if (detailPrototypes[i].prototype != null)
                            {
                                List<DetailScriptableObject> details = CreateFloraRenderFromPrefab(terrain, detailPrototypes[i].prototype, true, CoreCommonFloraData.SourceDataType.Detail, savePath, i);
                                ApplyToTerrains(details, true);
                            }
                            break;
                        }
                        default:
                        {
                            if (detailPrototypes[i].prototypeTexture != null)
                            {
                                if (!shadowsBeenSet)
                                {
                                    if (EditorUtility.DisplayDialog("Detail Shadows",
                                        "Would you like to enable shadows for details", "Yes", "No"))
                                    {
                                        castShadows = true;
                                    }

                                    shadowsBeenSet = true;
                                }
                                List<DetailScriptableObject> details = CreateFloraRenderFromTexture(detailPrototypes[i], CoreCommonFloraData.SourceDataType.Detail, savePath, i, castShadows);
                                ApplyToTerrains(details, true);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("No Details Found", "No details were found on any terrain in your scene.", "Ok");
            }
        }
        /// <summary>
        /// Adds the tag Flora Tree Collision if it is missing
        /// </summary>
        public static void AddMissingTag()
        {
            string[] tags = InternalEditorUtility.tags;
            if (tags.Length > 0)
            {
                bool addTag = true;
                foreach (string tag in tags)
                {
                    if (tag.Equals("Flora Tree Collision"))
                    {
                        addTag = false;
                    }
                }

                if (addTag)
                {
                    InternalEditorUtility.AddTag("Flora Tree Collision");
                }
            }
        }
        /// <summary>
        /// Applies all the detail renderers to the flora terrain tile
        /// </summary>
        /// <param name="details"></param>
        /// <param name="isTerrainDetail"></param>
        /// <param name="applyToTerrains"></param>
        public static void ApplyToTerrains(List<DetailScriptableObject> details, bool isTerrainDetail, bool applyToTerrains = true)
        {
            if (applyToTerrains)
            {
                FloraDefaults defaults = GetFloraDefaults();
                FloraTerrainTile[] terrainTiles = GameObject.FindObjectsOfType<FloraTerrainTile>();
                foreach (FloraTerrainTile tile in terrainTiles)
                {
                    FloraAutomationAPI.AddToTerrain(tile, details);
                    SetupTerrainCellCount(defaults, tile, tile.UnityTerrain);
                }
            }
        }
        /// <summary>
        /// Applies all the detail renderer to the flora terrain tile
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="isTerrainDetail"></param>
        /// <param name="applyToTerrains"></param>
        public static void ApplyToTerrains(DetailScriptableObject detail, bool isTerrainDetail, bool applyToTerrains = true)
        {
            if (applyToTerrains)
            {
                FloraDefaults defaults = GetFloraDefaults();
                FloraTerrainTile[] terrainTiles = GameObject.FindObjectsOfType<FloraTerrainTile>();
                foreach (FloraTerrainTile tile in terrainTiles)
                {
                    FloraAutomationAPI.AddToTerrain(tile, detail);
                    SetupTerrainCellCount(defaults, tile, tile.UnityTerrain);
                }
            }
        }
        /// <summary>
        /// Gets all the main global settings
        /// </summary>
        /// <param name="cellDensity"></param>
        /// <param name="drawDistance"></param>
        /// <param name="sourceCamera"></param>
        public static void GetGlobalSettings(out float detailDensity, out float detailDistance, out int cameraCellDensity, out float cameraDistance)
        {
            detailDensity = 1f;
            detailDistance = 1f;
            cameraCellDensity = 0;
            cameraDistance = 1f;

            FloraGlobalManager manager = GameObject.FindObjectOfType<FloraGlobalManager>();
            if (manager != null)
            {
                detailDensity = manager.Settings.ObjectGlobalDensityModifier;
                detailDistance = manager.Settings.ObjectGlobalDistanceModifier;
                cameraCellDensity = manager.Settings.CameraCellGlobalSubdivisionModifier;
                cameraDistance = manager.Settings.TerrainTileGlobalDistanceModifier;
            }
        }
        /// <summary>
        /// Sets all the main global settings
        /// </summary>
        /// <param name="cellDensity"></param>
        /// <param name="drawDistance"></param>
        /// <param name="sourceCamera"></param>
        public static void SetGlobalSettings(float detailDensity, float detailDistance, int cameraCellDensity, float cameraDistance)
        {
            FloraGlobalManager manager = GameObject.FindObjectOfType<FloraGlobalManager>();
            if (manager != null)
            {
                manager.Settings.ObjectGlobalDensityModifier = detailDensity;
                manager.Settings.ObjectGlobalDistanceModifier = detailDistance;
                manager.Settings.CameraCellGlobalSubdivisionModifier = cameraCellDensity;
                manager.Settings.TerrainTileGlobalDistanceModifier = cameraDistance;
                manager.SetGlobals();
            }
        }
        /// <summary>
        /// Gets the current installed SRP
        /// </summary>
        /// <returns></returns>
        public static FloraTerrainTile.FLORARP GetActivePipeline()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                return FloraTerrainTile.FLORARP.Builtin;
            }
            else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HDRenderPipelineAsset"))
            {
                return FloraTerrainTile.FLORARP.HDRP;
            }
            else
            {
                return FloraTerrainTile.FLORARP.URP;
            }
        }
        /// <summary>
        /// Gets and returns the main camera in the scene
        /// If check tag is set to ture it will check to see if the camera found tag is marked as MainCamera or Player
        /// </summary>
        /// <returns></returns>
        public static Camera GetCamera(bool checkTag = false)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                if (checkTag)
                {
                    if (camera.tag == "MainCamera" || camera.tag == "Player")
                    {
                        return camera;
                    }
                }
                return camera;
            }

            camera = GameObject.FindObjectOfType<Camera>();
            if (camera != null)
            {
                if (checkTag)
                {
                    if (camera.tag == "MainCamera" || camera.tag == "Player")
                    {
                        return camera;
                    }
                }

                return camera;
            }

            return null;
        }
        /// <summary>
        /// Gets a debug Color
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static Color GetDebugColor(int idx)
        {
            DebugConstFloat += 0.1f;
            if (DebugConstFloat > 1f)
            {
                DebugConstFloat = 0f;
            }
            FloraDefaults defaults = GetFloraDefaults();
            if (defaults != null)
            {
                if (defaults.m_debugColorGradient != null && defaults.m_debugColorGradient.colorKeys.Length >= 2)
                {
                    return defaults.m_debugColorGradient.Evaluate(DebugConstFloat / idx);
                }
            }

            Gradient gradient = new Gradient
            {
                mode = GradientMode.Blend
            };
            GradientColorKey[] keys = new GradientColorKey[5];
            keys[0].time = 0f;
            keys[0].color = Color.white;

            keys[1].time = 0.25f;
            keys[1].color = Color.blue;

            keys[2].time = 0.5f;
            keys[2].color = Color.magenta;

            keys[3].time = 0.75f;
            keys[3].color = Color.red;

            keys[4].time = 1f;
            keys[4].color = Color.yellow;

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1f;
            alphaKeys[0].time = 0f;
            gradient.SetKeys(keys, alphaKeys);
            return gradient.Evaluate(DebugConstFloat / idx);
        }
        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string fileName)
        {
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    return path;
                }
            }
            return "";
        }
        /// <summary>
        /// Removes any missing detail renderer from the terrain tiles. This is useful if a users has deleted a detail scriptableobject asset
        /// </summary>
        public static void CleanUpMissingDetails()
        {
            int amount = 0;
            FloraTerrainTile[] terrainTiles = GameObject.FindObjectsOfType<FloraTerrainTile>();
            if (terrainTiles.Length > 0)
            {
                foreach (FloraTerrainTile tile in terrainTiles)
                {
                    bool hasChanged = false;
                    if (tile.m_detailObjectList.Count > 0)
                    {
                        for (int i = tile.m_detailObjectList.Count; i --> 0;)
                        {
                            if (tile.m_detailObjectList[i].DetailScriptableObject == null)
                            {
                                tile.m_detailObjectList.RemoveAt(i);
                                hasChanged = true;
                                amount++;
                            }
                        }
                    }

                    if (hasChanged)
                    {
                        EditorUtility.SetDirty(tile);
                    }
                }

                if (ShowDebugging)
                {
                    Debug.Log(amount + " was removed from terrain tiles in your scene");
                }
            }
        }
        /// <summary>
        /// Adds detail prototype to all terrains
        /// </summary>
        /// <param name="detailPrototype"></param>
        public static void AddDetailPrototypeToTerrains(DetailPrototype detailPrototype)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length > 0)
            {
                foreach (Terrain terrain in terrains)
                {
                    DetailPrototype[] currentDetailPrototypes = terrain.terrainData.detailPrototypes;
                    List<DetailPrototype> prototypes = new List<DetailPrototype>();
                    prototypes.AddRange(currentDetailPrototypes);
                    prototypes.Add(detailPrototype);
                    terrain.terrainData.detailPrototypes = prototypes.ToArray();
                    terrain.Flush();
                }
            }
        }
        /// <summary>
        /// Adds tree prototype to all terrains
        /// </summary>
        /// <param name="detailPrototype"></param>
        public static void AddTreePrototypeToTerrains(TreePrototype treePrototype)
        {
#pragma warning disable 162
            if (FloraGlobalData.TreesEnabled)
            {
                Terrain[] terrains = Terrain.activeTerrains;
                if (terrains.Length > 0)
                {
                    foreach (Terrain terrain in terrains)
                    {
                        TreePrototype[] currentTreePrototypes = terrain.terrainData.treePrototypes;
                        List<TreePrototype> prototypes = new List<TreePrototype>();
                        prototypes.AddRange(currentTreePrototypes);
                        prototypes.Add(treePrototype);
                        terrain.terrainData.treePrototypes = prototypes.ToArray();
                        terrain.Flush();
                    }
                }
            }
#pragma warning restore 162

        }
        /// <summary>
        /// Gets all the wind settings
        /// </summary>
        /// <param name="globalWindDistance"></param>
        /// <param name="useWindAudio"></param>
        /// <param name="windClip"></param>
        /// <param name="windSpeed"></param>
        /// <param name="windTurbulence"></param>
        /// <returns></returns>
        public static bool GetWindSettings(out float globalWindDistance, out bool useWindAudio, out AudioClip windClip, out float windSpeed, out float windTurbulence)
        {
            bool present = false;
            globalWindDistance = 1000f;
            useWindAudio = false;
            windClip = null;
            windSpeed = 0.5f;
            windTurbulence = 0.25f;

            FloraWindManager windManager = GameObject.FindObjectOfType<FloraWindManager>();
            if (windManager != null)
            {
                present = true;
                globalWindDistance = windManager.windGlobalMaxDist;
                useWindAudio = windManager.m_useWindAudio;
                windClip = windManager.m_windAudioClip;

                WindZone windZone = GameObject.FindObjectOfType<WindZone>();
                if (windZone != null)
                {
                    windSpeed = windZone.windMain;
                    windTurbulence = windZone.windTurbulence;
                }
            }

            return present;
        }
        /// <summary>
        /// Sets all the wind settings
        /// </summary>
        /// <param name="globalWindDistance"></param>
        /// <param name="useWindAudio"></param>
        /// <param name="windClip"></param>
        /// <param name="windSpeed"></param>
        /// <param name="windTurbulence"></param>
        public static void SetWindSettings(float globalWindDistance, bool useWindAudio, AudioClip windClip, float windSpeed, float windTurbulence)
        {
            FloraWindManager windManager = GameObject.FindObjectOfType<FloraWindManager>();
            if (windManager != null)
            {
                windManager.windGlobalMaxDist = globalWindDistance;
                windManager.m_useWindAudio = useWindAudio;
                windManager.m_windAudioClip = windClip;

                WindZone windZone = GameObject.FindObjectOfType<WindZone>();
                if (windZone != null)
                {
                    windZone.windMain = windSpeed;
                    windZone.windTurbulence = windTurbulence;
                }
            }

        }
        /// <summary>
        /// Gets the flora defaults profile
        /// </summary>
        /// <returns></returns>
        public static FloraDefaults GetFloraDefaults()
        {
            return AssetDatabase.LoadAssetAtPath<FloraDefaults>(GetAssetPath("Flora Defaults.asset"));
        }
        /// <summary>
        /// Sets up the terrain cell count
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="terrain"></param>
        public static void SetupTerrainCellCount(FloraDefaults defaults, FloraTerrainTile tile, Terrain terrain)
        {
            if (defaults != null && tile != null && terrain != null)
            {
                bool set = false;
                foreach (TerrainCellCountData data in defaults.m_terrainCellCountData)
                {
                    if (terrain.terrainData.size.x <= data.m_terrainSize)
                    {
                        tile.BaselineCellDensity = data.m_cellDensity;
                        set = true;
                        break;
                    }
                }

                if (!set)
                {
                    tile.BaselineCellDensity = defaults.m_defaultCellDensity;
                }
            }
        }
        /// <summary>
        /// Gets the cell density based on the terrain size
        /// </summary>
        /// <param name="defaults"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static CoreCommonFloraData.CellDensity GetCellDensityFromTerrain(FloraDefaults defaults, Terrain terrain)
        {
            foreach (TerrainCellCountData data in defaults.m_terrainCellCountData)
            {
                if (terrain.terrainData.size.x <= data.m_terrainSize)
                {
                    return data.m_cellDensity;
                }
            }

            return defaults.m_defaultCellDensity;
        }

        #endregion
        #region Private Functions

        /// <summary>
        /// Checks to see if a prefab exists in the current prefab list
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static bool DoesCurrentPrefabExist(GameObject prefab)
        {
            return CurrentPrefabs.Contains(prefab);
        }
        /// <summary>
        /// Builds the flora shader wind data info based on the object bounds
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="material"></param>
        private static void BuildTreeWindData(GameObject tree, Material material)
        {
            if (tree != null && material != null)
            {
                MeshRenderer meshRenderer = tree.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    meshRenderer = tree.GetComponentInChildren<MeshRenderer>();
                }

                if (meshRenderer != null)
                {
                    Bounds bound = meshRenderer.bounds;
                    if (bound.size != Vector3.zero)
                    {
                        material.SetVector("_PW_WindTreeFrequency", new Vector4(bound.extents.x, bound.extents.y, bound.extents.z, 0f));
                        material.SetVector("_PW_WindTreeWidthHeight", new Vector4(bound.size.x, bound.size.y, bound.size.z, 0.66f));
                    }
                }
            }
        }
        /// <summary>
        /// Builds the flora shader wind data info based on the detail prototype
        /// </summary>
        /// <param name="detailPrototype"></param>
        /// <param name="material"></param>
        private static void BuildGrassWindData(DetailPrototype detailPrototype, Material material, float multiplyValue = 2f)
        {
            if (detailPrototype != null && material != null)
            {
                material.SetVector("_PW_WindTreeFrequency", new Vector4((detailPrototype.minWidth * multiplyValue), (detailPrototype.maxWidth * multiplyValue), 0.33f, 0f));
                material.SetVector("_PW_WindTreeWidthHeight", new Vector4((detailPrototype.minHeight * multiplyValue), (detailPrototype.maxHeight * multiplyValue), 0.66f, 0f));
            }
        }
        /// <summary>
        /// Converts a LOD from a lodgroup into a distance value
        /// </summary>
        /// <param name="meshFilter"></param>
        /// <param name="lodPercent"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static float ConvertLODToDistance(MeshFilter meshFilter, float lodPercent, float treeLODBias, float defaultValue = 50f)
        {
            if (meshFilter != null)
            {
                float newLodPercent = lodPercent * 10f; //Converts 0-1 into 0-100% makes better for multiplying with
                if (newLodPercent <= 0f)
                {
                    newLodPercent = 1f;
                }
                Bounds bounds = meshFilter.sharedMesh.bounds;
                float objectSize = bounds.size.x * bounds.size.y;

                return objectSize * newLodPercent * treeLODBias;
            }

            return defaultValue;
        }
        /// <summary>
        /// Creates a material from an instance into an asset
        /// </summary>
        /// <param name="material"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private static Material CreateMaterial(FloraDefaults defaults, GameObject prefab, Material material, string savePath, bool loadDefaultNormalMask = false, int detailLODID = -1, bool details = false, bool doubleSided = true)
        {
            if (material != null && defaults != null)
            {
                Material newMat = null;
                List<string> compatibleShaderNames = null;

                switch (GetActivePipeline())
                {
                    case FloraTerrainTile.FLORARP.URP:
                        {
                            compatibleShaderNames = defaults.m_urpCompatibleShaderNames;
                            break;
                        }
                    case FloraTerrainTile.FLORARP.HDRP:
                        {
                            compatibleShaderNames = defaults.m_hdrpCompatibleShaderNames;
                            break;
                        }
                    case FloraTerrainTile.FLORARP.Builtin:
                        {
                            compatibleShaderNames = defaults.m_builtInCompatibleShaderNames;
                            break;
                        }
                }

                if (compatibleShaderNames.Contains(material.shader.name))
                {
                    newMat = Material.Instantiate(material);
                }
                else
                {
                    newMat = new Material(Shader.Find(DefaultBuiltInShader));
                    switch (GetActivePipeline())
                    {
                        case FloraTerrainTile.FLORARP.URP:
                            {
                                newMat.CopyPropertiesFromMaterial(defaults.m_urpMaterialTemplate);
                                newMat.shader = Shader.Find(DefaultURPShader);
                                break;
                            }
                        case FloraTerrainTile.FLORARP.HDRP:
                            {
                                newMat.shader = Shader.Find(DefaultHDRPShader);
                                newMat.CopyPropertiesFromMaterial(defaults.m_hdrpMaterialTemplate);
                                break;
                            }
                        case FloraTerrainTile.FLORARP.Builtin:
                            {
                                newMat.CopyPropertiesFromMaterial(defaults.m_builtInMaterialTemplate);
                                break;
                            }
                    }

                    //Build tree wind data from prefab bounds
                    if (prefab != null)
                    {
                        BuildTreeWindData(prefab, newMat);
                    }

                    //Color
                    foreach (string property in defaults.m_baseColorShaderProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            newMat.SetColor("_BaseColor", material.GetColor(property));
                        }
                    }

                    //Albedo
                    Texture albedo = null;
                    foreach (string property in defaults.m_albedoShaderProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            albedo = material.GetTexture(property);
                        }

                        if (albedo != null)
                        {
                            break;
                        }
                    }
                    if (albedo != null)
                    {
                        newMat.SetTexture("_BaseMap", albedo);
                    }

                    //Normal
                    Texture normal = null;
                    foreach (string property in defaults.m_normalShaderProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            normal = material.GetTexture(property);
                        }

                        if (normal != null)
                        {
                            break;
                        }
                    }
                    if (normal != null)
                    {
                        newMat.SetTexture("_NormalMap", normal);
                    }

                    //Mask
                    Texture mask = null;
                    foreach (string property in defaults.m_maskMapShaderProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            mask = material.GetTexture(property);
                        }

                        if (mask != null)
                        {
                            break;
                        }
                    }
                    if (mask != null)
                    {
                        newMat.SetTexture("_MaskMap", mask);
                    }
                    else
                    {
                        newMat.SetTexture("_MaskMap", defaults.m_defaultMaskMap);
                    }
                    //Default normal and mask map
                    if (loadDefaultNormalMask)
                    {
                        newMat.SetTexture("_NormalMap", defaults.m_defaultNormalMap);
                        newMat.SetTexture("_MaskMap", defaults.m_defaultMaskMap);
                    }
                    //Cutoff
                    float cutoutValue = 0f;
                    bool useCutout = false;
                    foreach (string property in defaults.m_cutoutShaderProperties)
                    {
                        if (material.HasProperty(property))
                        {
                            cutoutValue = material.GetFloat(property);
                            useCutout = true;
                            break;
                        }
                    }

                    if (details)
                    {
                        if (detailLODID == 1)
                        {
                            newMat.SetFloat("_Cutoff", defaults.m_defaultCutoutValue / 2f);
                        }
                        else
                        {
                            newMat.SetFloat("_Cutoff", defaults.m_defaultCutoutValue);
                        }
                    }
                    else
                    {
                        if (useCutout)
                        {
                            newMat.SetFloat("_Cutoff", cutoutValue);
                        }
                        else
                        {
                            newMat.SetFloat("_Cutoff", 0f);
                        }
                    }

                    //Wind
                    bool useWind = cutoutValue > 0f;
                    if (!useWind)
                    {
                        foreach (string materialName in defaults.m_windMaterialNames)
                        {
                            if (material.name.Contains(materialName))
                            {
                                useWind = true;
                                break;
                            }
                        }
                    }
                    if (useWind)
                    {
                        newMat.SetFloat("_PW_SF_WIND", 1f);
                    }
                    else
                    {
                        newMat.SetFloat("_PW_SF_WIND", 0f);
                    }
                    //Thickness
                    newMat.SetFloat("_Thickness", 1f);
                    //GPU instance
                    newMat.enableInstancing = defaults.m_enableGPUInstancing;
                   
                }

                string path = savePath + ".mat";

                bool justBeenCreated = false;
                if (AssetDatabase.LoadAssetAtPath<Material>(path) == null)
                {
                    AssetDatabase.CreateAsset(newMat, path);
                    AssetDatabase.Refresh();
                    justBeenCreated = true;
                }

                Material createdMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!justBeenCreated)
                {
                    createdMaterial.CopyPropertiesFromMaterial(newMat);
                }

                if (compatibleShaderNames.Contains(material.shader.name))
                {
                    createdMaterial.EnableKeyword("_PW_SF_FLORA_INDIRECT_ON");
                    createdMaterial.SetFloat("_PW_SF_FLORA_INDIRECT", 1.0f);
                }

                //Keywords
                switch (GetActivePipeline())
                {
                    case FloraTerrainTile.FLORARP.URP:
                        {
                            foreach (MaterialKeywordData keywordSetup in defaults.m_urpMaterialKeywordSetup)
                            {
                                SetupKeywordOnMaterial(createdMaterial, keywordSetup, doubleSided);
                            }
                            break;
                        }
                    case FloraTerrainTile.FLORARP.HDRP:
                        {
                            foreach (MaterialKeywordData keywordSetup in defaults.m_hdrpMaterialKeywordSetup)
                            {
                                SetupKeywordOnMaterial(createdMaterial, keywordSetup, doubleSided);
                            }
                            break;
                        }
                }
                return createdMaterial;
            }
            return null;
        }
        /// <summary>
        /// Sets the keyword and value up based on the keyword setup data
        /// </summary>
        /// <param name="newMat"></param>
        /// <param name="keywordSetup"></param>
        private static void SetupKeywordOnMaterial(Material newMat, MaterialKeywordData keywordSetup, bool doubleSided = true)
        {
            if (keywordSetup == null || newMat == null)
            {
                return;
            }

            if (!doubleSided)
            {
                if (keywordSetup.m_name.Contains("Double Sided"))
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(keywordSetup.m_keywordValue))
            {
                if (keywordSetup.m_enabled == FloraKeywordAddRemove.Add)
                {
                    newMat.EnableKeyword(keywordSetup.m_keywordValue);
                }
                else
                {
                    newMat.DisableKeyword(keywordSetup.m_keywordValue);
                }
            }

            if (keywordSetup.m_setShaderProperty)
            {
                if (newMat.HasProperty(keywordSetup.m_shaderPropertyName))
                {
                    switch (keywordSetup.m_valueType)
                    {
                        case FloraKeywordValueType.Float:
                        {
                            newMat.SetFloat(keywordSetup.m_shaderPropertyName, keywordSetup.m_floatValue);
                            break;
                        }
                        case FloraKeywordValueType.Int:
                        {
                            newMat.SetInt(keywordSetup.m_shaderPropertyName, keywordSetup.m_intValue);
                            break;
                        }
                        case FloraKeywordValueType.Color:
                        {
                            newMat.SetColor(keywordSetup.m_shaderPropertyName, keywordSetup.m_colorValue);
                            break;
                        }
                        case FloraKeywordValueType.Texture:
                        {
                            newMat.SetTexture(keywordSetup.m_shaderPropertyName, keywordSetup.m_textureValue);
                            break;
                        }
                        case FloraKeywordValueType.Bool:
                        {
                            if (keywordSetup.m_boolValue)
                            {
                                newMat.SetFloat(keywordSetup.m_shaderPropertyName, 1f);
                            }
                            else
                            {
                                newMat.SetFloat(keywordSetup.m_shaderPropertyName, 0f);
                            }
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Creates the details scriptable object into assets.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private static List<string> CreateDetails(List<DetailScriptableObject> details, string savePath, bool useMaterialName = false)
        {
            if (details != null && details.Count > 0)
            {
                List<string> paths = new List<string>();
                foreach (DetailScriptableObject detail in details)
                {
                    string path = savePath;
                    if (useMaterialName)
                    {
                        path += detail.Mat[0].name + ".asset";
                    }
                    else
                    {
                        path += detail.Mesh.name + ".asset";
                    }

                    if (AssetDatabase.LoadAssetAtPath<DetailScriptableObject>(path) == null)
                    {
                        AssetDatabase.CreateAsset(detail, path);
                    }
                    else
                    {
                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.CreateAsset(detail, path);
                    }

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    paths.Add(path);
                }

                return paths;
            }

            return new List<string>();
        }
        /// <summary>
        /// Adds the detail object to the flora tile
        /// </summary>
        /// <param name="floraTile"></param>
        /// <param name="details"></param>
        /// <param name="skipPrototypeCheck"></param>
        public static void AddToTerrain(FloraTerrainTile floraTile, List<DetailScriptableObject> details, bool skipPrototypeCheck = false)
        {
            if (floraTile != null)
            {
                for (int i = 0; i < details.Count; i++)
                {
                    if (details[i] == null)
                    {
                        continue;
                    }

                    bool add = true;
                    foreach (DetailOverrideData data in floraTile.m_detailObjectList)
                    {
                        if (data == null || data.DetailScriptableObject == null)
                        {
                            continue;
                        }

                        if (data.DetailScriptableObject.Name == details[i].Name)
                        {
                            add = false;
                        }
                    }

                    bool isTerrainDetail = details[i].SourceDataType == CoreCommonFloraData.SourceDataType.Detail;
                    //if (details[i].SourceDataType == CoreCommonFloraData.SourceDataType.Splat |
                    //    details[i].SourceDataType == CoreCommonFloraData.SourceDataType.TransformList)
                    //{
                    //    //Do something here
                    //}

                    if (add)
                    {
                        if (DoesPrototypeExistOnTerrain(GetPrefabByMesh(details[i].Mesh), details[i].Mat[0], floraTile, isTerrainDetail) || skipPrototypeCheck)
                        {
                            floraTile.m_detailObjectList.Add(new DetailOverrideData
                            {
                                DetailScriptableObject = details[i],
                                SourceDataType = details[i].SourceDataType,
                                SourceDataIndex = details[i].SourceDataIndex,
                                SubCellDivision = details[i].SubCellDivision,
                                DebugColor = details[i].DebugColor
                            });

                            //details[i].SubCellDivision = 1;
                        }
                        else
                        {
                            if (ShowDebugging)
                            {
                                Debug.Log(details[i].Name + " Was not added to flora terrain tile as the tree prototype was not found on the terrain.");
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Add detail object to the flora tile
        /// </summary>
        /// <param name="floraTile"></param>
        /// <param name="detail"></param>
        /// <param name="skipPrototypeCheck"></param>
        private static void AddToTerrain(FloraTerrainTile floraTile, DetailScriptableObject detail, bool skipPrototypeCheck = false)
        {
            AddToTerrain(floraTile, new List<DetailScriptableObject>() { detail }, skipPrototypeCheck);
        }

        /// <summary>
        /// Gets the prefab based of the mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private static GameObject GetPrefabByMesh(Mesh mesh)
        {
            if (mesh != null)
            {
                foreach (GameObject prefab in CurrentPrefabs)
                {
                    List<MeshFilter> meshFilters = new List<MeshFilter>();
                    MeshFilter[] meshFilter = prefab.GetComponents<MeshFilter>();
                    meshFilters.AddRange(meshFilter);
                    if (meshFilter.Length < 1)
                    {
                        meshFilter = prefab.GetComponentsInChildren<MeshFilter>();
                    }

                    meshFilters.AddRange(meshFilter);

                    if (meshFilter != null)
                    {
                        foreach (MeshFilter filter in meshFilters)
                        {
                            if (filter.sharedMesh == mesh)
                            {
                                return prefab;
                            }
                        }
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Checks if the prefab exists on the terrain it's trying to adding to.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="floraTile"></param>
        /// <returns></returns>
        private static bool DoesPrototypeExistOnTerrain(GameObject prefab, Material mat, FloraTerrainTile floraTile, bool isDetail)
        {
            if (floraTile != null)
            {
                Terrain terrain = GetTerrain(floraTile);
                if (terrain != null)
                {
                    if (isDetail)
                    {
                        DetailPrototype[] prototypes = terrain.terrainData.detailPrototypes;
                        foreach (DetailPrototype prototype in prototypes)
                        {
                            if (GetBaseMapTexture(mat) == prototype.prototypeTexture)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (prefab != null)
                        {
                            TreePrototype[] prototypes = terrain.terrainData.treePrototypes;
                            foreach (TreePrototype treePrototype in prototypes)
                            {
                                if (treePrototype.prefab != null)
                                {
                                    if (prefab == treePrototype.prefab)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Gets the basemap texture from our shader
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private static Texture2D GetBaseMapTexture(Material mat)
        {
            if (mat != null)
            {
                if (mat.HasProperty("_BaseMap"))
                {
                    return (Texture2D)mat.GetTexture("_BaseMap");
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the terrain from flora tile
        /// </summary>
        /// <param name="floraTile"></param>
        /// <returns></returns>
        private static Terrain GetTerrain(FloraTerrainTile floraTile)
        {
            Terrain terrain = floraTile.UnityTerrain;
            if (terrain == null)
            {
                terrain = floraTile.GetComponent<Terrain>();
            }

            return terrain;
        }
        /// <summary>
        /// Creates the directory
        /// </summary>
        /// <param name="path"></param>
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Gets the tree prototype ID from the prefab provided
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static int GetTreeSourceIDFromTerrain(GameObject prefab)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length > 0 && prefab != null)
            {
                foreach (Terrain terrain in terrains)
                {
                    TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
                    for (int i = 0; i < treePrototypes.Length; i++)
                    {
                        if (treePrototypes[i].prefab == prefab)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }
        /// <summary>
        /// Gets the tree prototype ID from the prefab provided
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static int GetTreeSourceIDFromTerrain(GameObject prefab, Terrain terrain)
        {
            if (terrain != null && prefab != null)
            {
                TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
                for (int i = 0; i < treePrototypes.Length; i++)
                {
                    if (treePrototypes[i].prefab == prefab)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
        /// <summary>
        /// Gets the detail prototype ID from the prefab provided
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static int GetDetailSourceIDFromTerrain(Texture2D texture)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length > 0 && texture != null)
            {
                foreach (Terrain terrain in terrains)
                {
                    DetailPrototype[] detailPrototypes = terrain.terrainData.detailPrototypes;
                    for (int i = 0; i < detailPrototypes.Length; i++)
                    {
                        if (detailPrototypes[i].prototypeTexture == texture)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        #endregion
    }
}