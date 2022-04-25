// Copyright © 2021 Procedural Worlds Pty Limited. All Rights Reserved.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEditor;
using PWCommon5;
using UnityEditor.Compilation;

#if FLORA_NAV_MESH
using UnityEngine.AI;
#endif

namespace ProceduralWorlds.Flora
{
    /// <summary>
    /// Main Workflow Editor Window
    /// </summary>
    public class FloraMainWindow : EditorWindow, IPWEditor
    {
        private TabSet m_mainTabs;
        private Vector2 m_scrollPosition = Vector2.zero;
        private EditorUtils m_editorUtils;
        private GUIStyle m_boxStyle;
        private bool m_inScene;
        private bool m_applyToAllTerrains = true;
        private bool m_windInScene;
        private float m_detailDensity;
        private float m_detailDistance;
        private int m_cameraCellDensity;
        private float m_cameraDistance;
        //Tree collider
        private TreeBakerProfile m_profile;
        private Color m_backgroundColor;
        private TreePrototype[] m_lastTreePrototypes = new TreePrototype[0];
#if FLORA_NAV_MESH
        private NavMeshSurface m_navMesh;
#endif
        //Wind
        private float m_windDistance;
        private bool m_useWindAudio;
        private AudioClip m_windAudioClip;
        private float m_windSpeed;
        private float m_windTurbulence;

        //Settings Tab
        private FloraDefaultsEditor m_defaultsEditor;
        private FloraDefaults m_floraDefaults;
        private EditorUtils m_defaultsEditorUtils;

        //Gaia
#if GAIA_2_PRESENT
        private bool m_gaiaWindPresent;
        private GameObject m_gaiaWind;
#endif

        public bool PositionChecked { get; set; }
        
        #region Custom Menu Items

        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Flora/Flora... %q", false, 40)]
        public static void MenuDetailerMainWindow()
        {
            var window = EditorWindow.GetWindow<FloraMainWindow>(false, "Flora");
            Vector2 initialSize = new Vector2(500f, 450f);
            window.position = new Rect(new Vector2(Screen.currentResolution.width / 2f - initialSize.x / 2f, Screen.currentResolution.height / 2f - initialSize.y / 2f), initialSize);
            window.Show();
        }

        #endregion
        #region Constructors destructors and related delegates

        /// <summary>
        /// On destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }
        /// <summary>
        /// On enable
        /// </summary>
        private void OnEnable()
        {
            FloraAutomationAPI.AddMissingTag();
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = FloraApp.GetEditorUtils(this);
            }

            m_scrollPosition = Vector2.zero;
            m_defaultsEditor = FloraDefaultsEditor.CreateInstance<FloraDefaultsEditor>();
            m_floraDefaults = FloraAutomationAPI.GetFloraDefaults();
            m_defaultsEditorUtils = FloraApp.GetEditorUtils(this, true, "FloraDefaultsEditor");

            //Tabs
            Tab[] mainTabs = new Tab[] 
            {
                new Tab ("Setup", SetupTab),
                new Tab ("Utils", UtilsTab),
                //new Tab ("Colision Baking", CollisionBakerTab),
                new Tab("Settings", SettingsTab)
            };

            m_mainTabs = new TabSet(m_editorUtils, mainTabs);

#if !FLORA_NAV_MESH
            if (CheckNavMesh())
            {
                FloraScriptDefine.AddNavMeshDesign();
            }
#endif

#if GAIA_2_PRESENT
            m_gaiaWindPresent = CheckIfGaiaWindPresent();
#endif
            m_windInScene = FloraAutomationAPI.GetWindSettings(out m_windDistance, out m_useWindAudio, out m_windAudioClip, out m_windSpeed, out m_windTurbulence);
            m_inScene = FindObjectOfType<FloraTerrainTile>() != null;
            if (m_inScene)
            {
                FloraAutomationAPI.GetGlobalSettings(out m_detailDensity, out m_detailDistance, out m_cameraCellDensity, out m_cameraDistance);
            }

            if (m_profile == null)
            {
                m_profile = AssetDatabase.LoadAssetAtPath<TreeBakerProfile>(FloraAutomationAPI.GetAssetPath("Tree Collider Baker Profile.asset"));
            }

            if (m_profile != null)
            {
                m_lastTreePrototypes = BuildTerrainTreeData();
                EditorUtility.SetDirty(m_profile);
            }
        }

        #endregion
        #region GUI main

        /// <summary>
        /// On GUI
        /// </summary>
        private void OnGUI()
        {
            m_backgroundColor = GUI.backgroundColor;

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

            m_editorUtils.Initialize(); // Do not remove this!
            m_editorUtils.GUIHeader();

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            //Load tabs
            m_editorUtils.Tabs(m_mainTabs);

            //End scroll
            GUILayout.EndScrollView();
            m_editorUtils.GUINewsFooter();
        }
        /// <summary>
        /// Setup tab
        /// </summary>
        private void SetupTab()
        {
            m_editorUtils.Panel("SetupPanel", SetupPanel, true);
        }
        /// <summary>
        /// Utils tab
        /// </summary>
        private void UtilsTab()
        {
            m_editorUtils.Panel("UtilsPanel", UtilsPanel, true);
        }
        /// <summary>
        /// Collision baking tab
        /// </summary>
        private void CollisionBakerTab()
        {
            m_editorUtils.Panel("TreeColliderBakerPanel", TreeColliderBakerPanel, true);
        }
        /// <summary>
        /// Settings tab
        /// All the defaults that can be found in the defaults profile
        /// </summary>
        private void SettingsTab()
        {
            if (m_defaultsEditor == null)
            {
                m_defaultsEditor = FloraDefaultsEditor.CreateInstance<FloraDefaultsEditor>();
                EditorGUIUtility.ExitGUI();
            }

            if (m_defaultsEditorUtils != null && m_floraDefaults != null)
            {
                m_editorUtils.Panel("SettingsPanel", SettingsPanel, true);
            }
        }
        /// <summary>
        /// Settings panel
        /// </summary>
        /// <param name="showHelp"></param>
        private void SettingsPanel(bool showHelp)
        {
            m_defaultsEditorUtils.Initialize();
            m_defaultsEditor.DefaultsEditor(m_floraDefaults, m_defaultsEditorUtils, m_boxStyle, showHelp);
        }
        /// <summary>
        /// Global panel
        /// </summary>
        /// <param name="showHelp"></param>
        private void SetupPanel(bool showHelp)
        {
            if (Application.isPlaying)
            {
                DrawGlobalsPanel(showHelp);
                return;
            }

            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("Setup");
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("Introduction"), MessageType.None);

            if (m_editorUtils.Button("AddToScene"))
            {
                AddToScene();
            }

            if (!m_inScene)
            {
                GUI.enabled = false;
            }
            if (m_editorUtils.Button("RemoveFromScene"))
            {
                RemoveFromScene();
            }

            DrawGlobalsPanel(showHelp);

            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// Global panel
        /// </summary>
        /// <param name="showHelp"></param>
        private void UtilsPanel(bool showHelp)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("InPlayMode"), MessageType.Info);
                return;
            }

            if (!m_inScene)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("FloraNotInScene"), MessageType.Info);
                GUI.enabled = false;
            }

            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("QuickSetup");
#pragma warning disable 162
            if (FloraGlobalData.TreesEnabled)
            {
                if (m_editorUtils.Button("GetDetailsAndTrees"))
                {
                    ApplyToTerrainPopup();
                    FloraAutomationAPI.GetDetails(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                    FloraAutomationAPI.GetTrees(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (m_editorUtils.Button("GetDetailsOnly"))
            {
                ApplyToTerrainPopup();
                FloraAutomationAPI.GetDetails(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
            
            }
            if (FloraGlobalData.TreesEnabled)
            {
                if (m_editorUtils.Button("GetTreesOnly"))
                {
                    ApplyToTerrainPopup();
                    FloraAutomationAPI.GetTrees(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                }
            }
#pragma warning restore 162
            EditorGUILayout.EndHorizontal();

            if (m_editorUtils.Button("CleanUpMissingDetailRenderers"))
            {
                FloraAutomationAPI.CleanUpMissingDetails();
            }

            GUI.enabled = true;
            if (m_editorUtils.Button("DeleteAllGeneratedUserData"))
            {
                int response = EditorUtility.DisplayDialogComplex(
                    m_editorUtils.GetTextValue("DeletingUserGeneratedDataTitle"),
                    m_editorUtils.GetTextValue("DeletingUserGeneratedDataMessage"),
                    m_editorUtils.GetTextValue("DeletingUserGeneratedDataOkOption"),
                    m_editorUtils.GetTextValue("DeletingUserGeneratedDataAltOption"),
                    m_editorUtils.GetTextValue("DeletingUserGeneratedDataCancelOption"));

                if (response != 2)
                {
                    switch (response)
                    {
                        //Delete Scene Only Data
                        case 0:
                        {
                            string pathFolder = FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath);
                            if (!string.IsNullOrEmpty(pathFolder))
                            {
                                pathFolder = pathFolder.Substring(0, pathFolder.Length - 1);
                                if (Directory.Exists(pathFolder))
                                {
                                    Directory.Delete(pathFolder, true);
                                    AssetDatabase.DeleteAsset(pathFolder + ".meta");
                                    AssetDatabase.Refresh();
                                }
                            }

                            break;
                        }
                        //Delete All Data
                        case 1:
                        {
                            string pathFolder = FloraAutomationAPI.DefaultSavePath;
                            if (!string.IsNullOrEmpty(pathFolder))
                            {
                                pathFolder = pathFolder.Substring(0, pathFolder.Length - 1);
                                if (Directory.Exists(pathFolder))
                                {
                                    Directory.Delete(pathFolder, true);
                                    AssetDatabase.DeleteAsset(pathFolder + ".meta");
                                    AssetDatabase.Refresh();
                                }
                            }

                            break;
                        }
                    }
                    //Cleans up details on components that have been deleted
                    FloraAutomationAPI.CleanUpMissingDetails();
                }
            }

            if (!m_inScene)
            {
                GUI.enabled = false;
            }
            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("Drag and Drop Setup");
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("DragandDropInfo"), MessageType.None);
            DragAndDropWindow();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            GUI.enabled = true;
        }
        /// <summary>
        /// Tree Collider panel
        /// </summary>
        /// <param name="showHelp"></param>
        private void TreeColliderBakerPanel(bool helpEnabled)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("InPlayMode"), MessageType.Info);
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("Setup");
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("TreeBakerInfo"), MessageType.None);
            EditorGUI.indentLevel++;
            m_profile = (TreeBakerProfile)m_editorUtils.ObjectField("Profile", m_profile, typeof(TreeBakerProfile), false, helpEnabled);
            if (m_profile != null)
            {
                if (m_profile.CheckLastTreePrototypes(m_lastTreePrototypes))
                {
                    m_lastTreePrototypes = BuildTerrainTreeData();
                    EditorUtility.SetDirty(m_profile);
                }

                Terrain terrain = m_profile.m_terrain;
                EditorGUILayout.BeginHorizontal();
                terrain = (Terrain) m_editorUtils.ObjectField("Terrain", terrain, typeof(Terrain), true);
                if (m_editorUtils.Button("Find", GUILayout.MaxWidth(45f)))
                {
                    if (terrain == null)
                    {
                        terrain = Terrain.activeTerrain;
                    }
                }
                EditorGUILayout.EndHorizontal();
                m_editorUtils.InlineHelp("Terrain", helpEnabled);
                if (terrain != m_profile.m_terrain)
                {
                    m_profile.m_terrain = terrain;
                    m_lastTreePrototypes = BuildTerrainTreeData();
                    EditorUtility.SetDirty(m_profile);
                }

                EditorGUI.indentLevel--;

                if (m_profile.m_terrain != null)
                {
                    if (m_profile.Data.Count > 0)
                    {
                        EditorGUILayout.BeginVertical(m_boxStyle);
                        m_editorUtils.Heading("CollisionBakingandSetup");

                        EditorGUILayout.BeginVertical(m_boxStyle);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Terrain: " + m_profile.m_terrain.name, EditorStyles.boldLabel);
                        if (m_editorUtils.Button("DisableAll", GUILayout.MaxWidth(100f)))
                        {
                            m_profile.SetBuildState(false);
                        }

                        if (m_editorUtils.Button("EnableAll", GUILayout.MaxWidth(100f)))
                        {
                            m_profile.SetBuildState(true);
                        }
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < m_profile.Data.Count; i++)
                        {
                            TreeColliderData data = m_profile.Data[i];

                            if (m_profile.Data[i].m_buildCollider)
                            {
                                GUI.backgroundColor = Color.green;
                            }
                            else
                            {
                                GUI.backgroundColor = Color.red;
                            }

                            EditorGUILayout.BeginVertical(m_boxStyle);
                            EditorGUILayout.BeginHorizontal();
                            m_profile.Data[i].m_buildCollider = EditorGUILayout.ToggleLeft("", m_profile.Data[i].m_buildCollider, GUILayout.MaxWidth(25f));
                            EditorGUILayout.LabelField(data.m_treeName, EditorStyles.boldLabel);
                            EditorGUILayout.LabelField("Type: " + data.m_vegetationType, EditorStyles.boldLabel, GUILayout.MaxWidth(100f));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            GUI.backgroundColor = m_backgroundColor;
                        }
                        EditorGUILayout.EndVertical();

                        m_profile.m_colliderData.m_drawMeshPreview = m_editorUtils.Toggle("Bake Mesh Preview", m_profile.m_colliderData.m_drawMeshPreview, helpEnabled);
                        if (m_profile.m_colliderData.m_drawMeshPreview)
                        {
                            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("MeshPreviewHelpInfo"), MessageType.Info);
                            EditorGUI.indentLevel++;
                            m_profile.m_colliderData.m_defaultMaterial = (Material) m_editorUtils.ObjectField("Default Material", m_profile.m_colliderData.m_defaultMaterial, typeof(Material), false, helpEnabled);
                            m_profile.m_colliderData.m_box = (Mesh) m_editorUtils.ObjectField("Default Box Mesh", m_profile.m_colliderData.m_box, typeof(Mesh), false, helpEnabled);
                            m_profile.m_colliderData.m_capsule = (Mesh) m_editorUtils.ObjectField("Default Capsule Mesh", m_profile.m_colliderData.m_capsule, typeof(Mesh), false, helpEnabled);
                            m_profile.m_colliderData.m_sphere = (Mesh) m_editorUtils.ObjectField("Default Sphere Mesh", m_profile.m_colliderData.m_sphere, typeof(Mesh), false, helpEnabled);
                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.EndVertical();
                    }

                    GUI.backgroundColor = m_backgroundColor;

                    EditorGUILayout.BeginVertical(m_boxStyle);
                    EditorGUILayout.BeginHorizontal();

                    if (m_profile.Data.Count < 1)
                    {
                        GUI.enabled = false;
                    }
                    else
                    {
                        GUI.enabled = true;
                    }

                    float buttonWidth = (EditorGUIUtility.currentViewWidth - 35f) / 2f;

                    if (m_editorUtils.Button("Bake Colliders", GUILayout.MaxWidth(buttonWidth)))
                    {
                        m_profile.BakeTerrainTreeColliders(m_profile.m_terrain);
                    }
                    if (m_editorUtils.Button("Clear Baked Colliders", GUILayout.MaxWidth(buttonWidth)))
                    {
                        m_profile.ClearBakedColliders();
                    }

                    EditorGUILayout.EndHorizontal();

                    if (m_profile.m_parentObject == null)
                    {
                        GUI.enabled = false;
                    }

#if FLORA_NAV_MESH
                    EditorGUILayout.BeginHorizontal();
                    if (m_editorUtils.Button("Bake Nav Mesh", GUILayout.MaxWidth(buttonWidth)))
                    {
                        m_navMesh = FindObjectOfType<NavMeshSurface>();
                        if (m_navMesh == null)
                        {
                            GameObject navMeshObject = new GameObject("Tree Nav Mesh Baker");
                            m_navMesh = navMeshObject.AddComponent<NavMeshSurface>();
                            m_navMesh.collectObjects = CollectObjects.All;
                            m_navMesh.layerMask = -1;
                            m_navMesh.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                        }

                        m_navMesh.BuildNavMesh();
                        Selection.activeGameObject = m_navMesh.gameObject;
                    }

                    GUI.enabled = true;

                    if (m_navMesh == null)
                    {
                        GUI.enabled = false;
                    }
                    if (m_editorUtils.Button("Clear Nav Mesh", GUILayout.MaxWidth(buttonWidth)))
                    {
                        NavMeshSurface navMesh = FindObjectOfType<NavMeshSurface>();
                        if (navMesh != null)
                        {
                            navMesh.RemoveData();
                            DestroyImmediate(navMesh.gameObject);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;
#endif
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    m_profile.m_terrain = Terrain.activeTerrain;
                    EditorGUILayout.HelpBox("You need to assign a terrain to get started.", MessageType.Error);
                }
            }

            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                if (m_profile != null)
                {
                    EditorUtility.SetDirty(m_profile);
                }
            }
        }
        /// <summary>
        /// Global Panel
        /// </summary>
        /// <param name="showHelp"></param>
        private void DrawGlobalsPanel(bool showHelp)
        {
            if (!m_inScene && Application.isPlaying)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("InPlayMode"), MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("GlobalSettings");
            EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("GlobalSettingsInfo"), MessageType.None);
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            m_detailDensity = m_editorUtils.Slider("DetailDensity", m_detailDensity, 0.01f, 4f, showHelp);
            m_detailDistance = m_editorUtils.Slider("DetailDistance", m_detailDistance, 0.01f, 4f, showHelp);
            m_cameraCellDensity = m_editorUtils.IntSlider("CameraCellDensity", m_cameraCellDensity, -8, 8, showHelp);
            m_cameraDistance = m_editorUtils.Slider("CameraDistance", m_cameraDistance, 0.01f, 4f, showHelp);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                FloraAutomationAPI.SetGlobalSettings(m_detailDensity, m_detailDistance, m_cameraCellDensity, m_cameraDistance);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(m_boxStyle);
            m_editorUtils.Heading("WindSettings");
#if GAIA_2_PRESENT
            if (m_gaiaWindPresent)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("GaiaWindPresent"), MessageType.Info);
                if (m_editorUtils.Button("SelectGaiaWind"))
                {
                    if (m_gaiaWind != null)
                    {
                        Selection.activeGameObject = m_gaiaWind;
                        EditorGUIUtility.PingObject(m_gaiaWind);
                    }
                }
            }
            else
            {
                FloraWindGUI(showHelp);
            }
#else
            FloraWindGUI(showHelp);
#endif
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// The flora wind UI
        /// </summary>
        /// <param name="showHelp"></param>
        private void FloraWindGUI(bool showHelp)
        {
            if (!m_windInScene)
            {
                if (m_editorUtils.Button("AddFloraWindSystem"))
                {
                    AddFloraWind(true);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                m_windDistance = m_editorUtils.Slider("MaxWindDistance", m_windDistance, 0f, 10000f, showHelp);
                m_windSpeed = m_editorUtils.Slider("WindSpeed", m_windSpeed, 0f, 5f, showHelp);
                m_windTurbulence = m_editorUtils.Slider("WindTurbulence", m_windTurbulence, 0f, 5f, showHelp);
                m_useWindAudio = m_editorUtils.Toggle("UseWindAudio", m_useWindAudio, showHelp);
                if (m_useWindAudio)
                {
                    EditorGUI.indentLevel++;
                    m_windAudioClip = (AudioClip)m_editorUtils.ObjectField("WindAudio", m_windAudioClip, typeof(AudioClip), false, showHelp);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    FloraAutomationAPI.SetWindSettings(m_windDistance, m_useWindAudio, m_windAudioClip, m_windSpeed, m_windTurbulence);
                }

                if (m_editorUtils.Button("RemoveFloraWindSystem"))
                {
                    RemoveFloraWind();
                }
            }
        }

        /// <summary>
        /// Check if unity nav mesh from github is installed
        /// </summary>
        /// <returns></returns>
        private bool CheckNavMesh()
        {
            //Look for assembly
            Assembly[] assemblies = CompilationPipeline.GetAssemblies();
            foreach (UnityEditor.Compilation.Assembly assembly in assemblies)
            {
                if (assembly.name.Contains("NavMeshComponents"))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Adds flora to the terrains in all open active scenes
        /// </summary>
        private void AddToScene()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains.Length > 0)
            {
                m_inScene = true;

                FloraGlobalManager floraGlobalManager = GameObject.FindObjectOfType<FloraGlobalManager>();
                if (floraGlobalManager == null)
                {
                    GameObject floraObject = new GameObject("Flora Global Manager");
                    floraGlobalManager = floraObject.AddComponent<FloraGlobalManager>();
                }

#if GAIA_2_PRESENT
                bool addWind = !m_gaiaWindPresent;
                if (addWind)
                {
                    if (EditorUtility.DisplayDialog("Add Wind Zone",
                        "Would you like to add Flora wind system to the scene?", "Yes", "No"))
                    {
                        AddFloraWind(false);
                    }
                }
#else
                if (EditorUtility.DisplayDialog("Add Wind Zone",
                    "Would you like to add Flora wind system to the scene?", "Yes", "No"))
                {
                    AddFloraWind(false);
                }
#endif

                Camera mainCamera = FloraAutomationAPI.GetCamera();

                foreach (Terrain terrain in terrains)
                {
                    if (terrain.GetComponent<FloraTerrainTile>() == null)
                    {
                        FloraTerrainTile tile = terrain.gameObject.AddComponent<FloraTerrainTile>();
                        tile.UnityTerrain = terrain;
                        if (mainCamera != null)
                        {
                            tile.MaximumDrawDistance = mainCamera.farClipPlane;
                        }
                    }
                }

                FloraAutomationAPI.GetGlobalSettings(out m_detailDensity, out m_detailDistance, out m_cameraCellDensity, out m_cameraDistance);

                if (EditorUtility.DisplayDialog("Build Flora Renderers", "Would you like to build all the terrain details and terrain trees into flora renders and apply them to the terrain?", "Yes", "No"))
                {
                    m_applyToAllTerrains = true;
                    FloraAutomationAPI.GetDetails(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                    FloraAutomationAPI.GetTrees(Terrain.activeTerrain, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                    m_mainTabs.ActiveTabIndex = 2;
                }
                else
                {
                    m_mainTabs.ActiveTabIndex = 1;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("No terrains found","Flora did not find any terrains in the scene. You would need to have at least one active terrain in the scene to use Flora for rendering.", "OK");
                m_inScene = false;
                return;
            }

            m_inScene = true;
        }
        /// <summary>
        /// Removes flora from the open active scenes
        /// </summary>
        private void RemoveFromScene()
        {
            if (EditorUtility.DisplayDialog("Removing Flora",
                "This will remove the flroa terrain tile components of any terrains in your scene and remove the flora global manager. Are you sure you want to proceed?",
                "Yes", "No"))
            {
                Terrain[] terrains = Terrain.activeTerrains;
                if (terrains.Length > 0)
                {
                    foreach (Terrain terrain in terrains)
                    {
                        FloraTerrainTile tile = terrain.GetComponent<FloraTerrainTile>();
                        if (tile != null)
                        {
                            GameObject.DestroyImmediate(tile);
                            EditorUtility.SetDirty(terrain);
                        }
                    }
                }

                FloraGlobalManager floraGlobalManager = GameObject.FindObjectOfType<FloraGlobalManager>();
                if (floraGlobalManager != null)
                {
                    GameObject.DestroyImmediate(floraGlobalManager.gameObject);
                }

                RemoveFloraWind();

                m_windInScene = false;
                m_inScene = false;
            }
        }
        /// <summary>
        /// Displays a popup that will ask you if you want to add the generated results to the terrains
        /// </summary>
        private void ApplyToTerrainPopup()
        {
            m_applyToAllTerrains = false;
            if (EditorUtility.DisplayDialog("Apply To Terrains",
                "Would you like to apply all the generated flora data to all flora terrain tiles in your scene?",
                "Yes", "No"))
            {
                m_applyToAllTerrains = true;
            }
        }
        /// <summary>
        /// Handle drop area for new objects
        /// </summary>
        public bool DragAndDropWindow()
        {
            // Ok - set up for drag and drop
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            string dropMsg = m_editorUtils.GetTextValue("Drag And Drop Prefab, Detail ScriptableObject");
            GUI.Box(dropArea, dropMsg, EditorStyles.helpBox);
            if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
            {
                if (!dropArea.Contains(evt.mousePosition))
                {
                    return false;
                }

                bool castShadows = false;
                bool shadowsBeenSet = false;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    // Handle prefabs / detail scriptable object
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject go && FloraGlobalData.TreesEnabled)
                        {
                            TreePrototype treePrototype = new TreePrototype
                            {
                                bendFactor = 0.5f,
                                navMeshLod = 0,
                                prefab = go
                            }; 

                            if (EditorUtility.DisplayDialog("Add Prototype To Terrains",
                                "Would you like to add " + go.name + " as a detail prototype to the terrains",
                                "Yes", "No"))
                            {
                                FloraAutomationAPI.AddTreePrototypeToTerrains(treePrototype);
                                m_applyToAllTerrains = true;
                            }

                            if (!m_applyToAllTerrains)
                            {
                                ApplyToTerrainPopup();
                            }

                            List<DetailScriptableObject> details = FloraAutomationAPI.CreateFloraRenderFromPrefab(Terrain.activeTerrain, go, true, CoreCommonFloraData.SourceDataType.Tree, FloraAutomationAPI.GetUniquePathWithSceneName(FloraAutomationAPI.DefaultSavePath));
                            FloraAutomationAPI.ApplyToTerrains(details, false, m_applyToAllTerrains);
                        }
                        else if (draggedObject is DetailScriptableObject detail)
                        {
                            m_applyToAllTerrains = true;
                            FloraAutomationAPI.ApplyToTerrains(detail, false, m_applyToAllTerrains);
                        }
                        else if (draggedObject is Texture2D texture)
                        {
                            DetailPrototype detailPrototype = new DetailPrototype
                            {
                                prototypeTexture = texture,
                                renderMode = DetailRenderMode.Grass,
                                healthyColor = Color.white,
                                dryColor = Color.white / 0.25f
                            };

                            if (EditorUtility.DisplayDialog("Add Prototype To Terrains",
                                "Would you like to add " + texture.name + " as a detail prototype to the terrains",
                                "Yes", "No"))
                            {
                                FloraAutomationAPI.AddDetailPrototypeToTerrains(detailPrototype);
                                m_applyToAllTerrains = true;
                            }

                            if (!m_applyToAllTerrains)
                            {
                                ApplyToTerrainPopup();
                            }

                            if (!shadowsBeenSet)
                            {
                                if (EditorUtility.DisplayDialog("Detail Shadows",
                                    "Would you like to enable shadows for details", "Yes", "No"))
                                {
                                    castShadows = true;
                                }

                                shadowsBeenSet = true;
                            }

                            List<DetailScriptableObject> details = FloraAutomationAPI.CreateFloraRenderFromTexture(detailPrototype, CoreCommonFloraData.SourceDataType.Detail, FloraAutomationAPI.DefaultSavePath, -1, castShadows);
                            FloraAutomationAPI.ApplyToTerrains(details, true, m_applyToAllTerrains);
                        }
                    }

                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Adds flora wind system
        /// </summary>
        private void AddFloraWind(bool exitGUI)
        {
#if GAIA_2_PRESENT
            if (m_gaiaWindPresent)
            {
                return;
            }
#endif
            FloraWindManager windManager = GameObject.FindObjectOfType<FloraWindManager>();
            if (windManager == null)
            {
                GameObject windObject = new GameObject("Flora Wind Settings");
                windManager = windObject.AddComponent<FloraWindManager>();
            }
            windManager.m_windAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(FloraAutomationAPI.GetAssetPath("Flora Ambient Wind.mp3"));
            windManager.InstantWindApply();

            FloraGlobalManager manager = GameObject.FindObjectOfType<FloraGlobalManager>();
            if (manager != null)
            {
                windManager.transform.SetParent(manager.transform);
            }

            m_windInScene = FloraAutomationAPI.GetWindSettings(out m_windDistance, out m_useWindAudio, out m_windAudioClip, out m_windSpeed, out m_windTurbulence);
            if (exitGUI)
            {
                EditorGUIUtility.ExitGUI();
            }
        }
        /// <summary>
        /// Checks to see if gaia wind is found
        /// </summary>
        /// <returns></returns>
        private bool CheckIfGaiaWindPresent()
        {
#if GAIA_2_PRESENT
            MonoBehaviour[] allScripts = GameObject.FindObjectsOfType<MonoBehaviour>();
            if (allScripts.Length > 0)
            {
                foreach (MonoBehaviour monoBehaviour in allScripts)
                {
                    if (monoBehaviour.GetType().Name.Equals("WindManager"))
                    {
                        m_gaiaWind = monoBehaviour.gameObject;
                        return true;
                    }
                }
            }
#endif
            return false;
        }
        /// <summary>
        /// Removes flora wind system
        /// </summary>
        private void RemoveFloraWind()
        {
            FloraWindManager windManager = GameObject.FindObjectOfType<FloraWindManager>();
            if (windManager != null)
            {
                m_windInScene = false;
                GameObject.DestroyImmediate(windManager.gameObject);
            }
        }
        private TreePrototype[] BuildTerrainTreeData()
        {
            if (m_profile == null)
            {
                return new TreePrototype[0];
            }

            FloraDefaults floraDefaults = FloraAutomationAPI.GetFloraDefaults();
            if (floraDefaults == null)
            {
                Debug.Log("Flora Defaults validate failed, see console for more information");
                return new TreePrototype[0];
            }

            if (m_profile.m_terrain != null)
            {
                m_profile.Data.Clear();

                TreePrototype[] treePrototypes = m_profile.m_terrain.terrainData.treePrototypes;
                if (treePrototypes.Length > 0)
                {
                    for (int i = 0; i < treePrototypes.Length; i++)
                    {
                        TreePrototype prototype = treePrototypes[i];
                        if (prototype.prefab != null)
                        {
                            m_profile.Data.Add(new TreeColliderData
                            {
                                m_treeName = prototype.prefab.name,
                                m_id = i,
                                m_prefab = prototype.prefab,
                                m_buildCollider = true
                            });

                            bool breakCurrent = false;
                            foreach (string treeType in floraDefaults.m_treeTypes)
                            {
                                if (prototype.prefab.name.Contains(treeType))
                                {
                                    m_profile.Data[m_profile.Data.Count - 1].m_vegetationType = "Tree";
                                    breakCurrent = true;
                                }
                            }
                            if (breakCurrent)
                            {
                                continue;
                            }

                            foreach (string treeType in floraDefaults.m_grassTypes)
                            {
                                if (prototype.prefab.name.Contains(treeType))
                                {
                                    m_profile.Data[m_profile.Data.Count - 1].m_vegetationType = "Grass";
                                    breakCurrent = true;
                                }
                            }
                            if (breakCurrent)
                            {
                                continue;
                            }

                            foreach (string treeType in floraDefaults.m_plantTypes)
                            {
                                if (prototype.prefab.name.Contains(treeType))
                                {
                                    m_profile.Data[m_profile.Data.Count - 1].m_vegetationType = "Plant";
                                    breakCurrent = true;
                                }
                            }
                            if (breakCurrent)
                            {
                                continue;
                            }

                            foreach (string treeType in floraDefaults.m_logTypes)
                            {
                                if (prototype.prefab.name.Contains(treeType))
                                {
                                    m_profile.Data[m_profile.Data.Count - 1].m_vegetationType = "Log";
                                    breakCurrent = true;
                                }
                            }
                            if (breakCurrent)
                            {
                                continue;
                            }
                            else
                            {
                                m_profile.Data[m_profile.Data.Count - 1].m_vegetationType = "Unknown";
                            }
                        }
                    }

                    return treePrototypes;
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Creates tree baker profile
        /// </summary>
        [MenuItem("Assets/Create/Procedural Worlds/Flora/Tree Collider Baker Profile")]
        public static void CreateTreeColliderBakerProfile()
        {
            TreeBakerProfile asset = ScriptableObject.CreateInstance<TreeBakerProfile>();
            AssetDatabase.CreateAsset(asset, "Assets/Tree Collider Baker Profile.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        /// <summary>
        /// Creates tree baker profile
        /// </summary>
        [MenuItem("Assets/Create/Procedural Worlds/Flora/Flora Defaults Profile")]
        public static void CreateFLoraDefaultsProfile()
        {
            FloraDefaults asset = ScriptableObject.CreateInstance<FloraDefaults>();
            AssetDatabase.CreateAsset(asset, "Assets/Flora Defaults.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}