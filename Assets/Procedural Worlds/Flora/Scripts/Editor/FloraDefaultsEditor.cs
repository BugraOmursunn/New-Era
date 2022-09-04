using System.Collections;
using System.Collections.Generic;
using PWCommon5;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Mesh = UnityEngine.Mesh;

namespace ProceduralWorlds.Flora
{
    [CustomEditor(typeof(FloraDefaults))]
    public class FloraDefaultsEditor : PWEditor
    {
        private GUIStyle m_boxStyle;
        private FloraDefaults m_defaults;
        private EditorUtils m_editorUtils;
        private bool m_helpEnabled;

        private bool m_skippableLODFoldout;
        private bool m_terrainCellCountFoldout;
        private bool m_treeSubDivisionFoldout;
        private bool m_detailSubDivisionFoldout;
        private bool m_baseColorShaderFoldout;
        private bool m_albedoShaderFoldout;
        private bool m_normalShaderFoldout;
        private bool m_maskMapShaderFoldout;
        private bool m_cutoutShaderFoldout;
        private bool m_windMaterialFoldout;
        private bool m_treeTypesFoldout;
        private bool m_grassTypesFoldout;
        private bool m_plantTypesFoldout;
        private bool m_logTypesFoldout;
        private bool m_hdrpKeywordFoldout;
        private bool m_urpKeywordFoldout;
        private bool m_builtInCompatibleShadersFoldout;
        private bool m_urpCompatibleShadersFoldout;
        private bool m_hdrpCompatibleShadersFoldout;

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
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = FloraApp.GetEditorUtils(this);
            }
        }
        /// <summary>
        /// Inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize();
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

            if (m_defaults == null)
            {
                m_defaults = (FloraDefaults) target;
                EditorGUIUtility.ExitGUI();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
            m_editorUtils.HelpToggle(ref m_helpEnabled);
            EditorGUILayout.EndHorizontal();
            DefaultsEditor(m_defaults, m_editorUtils, m_boxStyle, m_helpEnabled);
        }

        public void DefaultsEditor(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(boxStyle);
            editorUtils.Heading("TerrainSettings");
            EditorGUI.indentLevel++;
            defaults.m_defaultCellDensity = (CoreCommonFloraData.CellDensity)editorUtils.EnumPopup("TerrainCellDensity", defaults.m_defaultCellDensity, helpEnabled);
            EditorGUI.indentLevel--;
            DrawTerrainCellCountData(defaults, editorUtils, boxStyle, helpEnabled);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(boxStyle);
            editorUtils.Heading("DetailSettings");
            EditorGUI.indentLevel++;
            defaults.m_defaultDetailShadows = (ShadowCastingMode)editorUtils.EnumPopup("DetailShadowCastingMode", defaults.m_defaultDetailShadows, helpEnabled);
            defaults.m_defaultTerrainDetailLODBias = editorUtils.FloatField("DetailLODBias", defaults.m_defaultTerrainDetailLODBias, helpEnabled);
            if (defaults.m_defaultTerrainDetailLODBias < 0.01f)
            {
                defaults.m_defaultTerrainDetailLODBias = 0.01f;
                EditorUtility.SetDirty(defaults);
            }
            defaults.m_detailDensityMultiplier = editorUtils.FloatField("DetailDensityMultiplier", defaults.m_detailDensityMultiplier, helpEnabled);
            if (defaults.m_detailDensityMultiplier < 0.01f)
            {
                defaults.m_detailDensityMultiplier = 0.01f;
                EditorUtility.SetDirty(defaults);
            }
            defaults.m_defaultDetailsFadeOutDistance = editorUtils.FloatField("DetailFadeOutDistance", defaults.m_defaultDetailsFadeOutDistance, helpEnabled);
            defaults.m_minMaxDetailVertexCount = editorUtils.Vector2Field("MinMaxDetailVertexCount", defaults.m_minMaxDetailVertexCount, helpEnabled);
            defaults.m_defaultsLODDetails = editorUtils.Toggle("LODDetails", defaults.m_defaultsLODDetails, helpEnabled);
            if (defaults.m_defaultsLODDetails)
            {
                EditorGUI.indentLevel++;
                defaults.m_defaultTerrainDetailMeshLOD0 = (Mesh)editorUtils.ObjectField("DetailLOD0Mesh", defaults.m_defaultTerrainDetailMeshLOD0, typeof(Mesh), false, helpEnabled);
                defaults.m_defaultTerrainDetailMeshLOD1 = (Mesh)editorUtils.ObjectField("DetailLOD1Mesh", defaults.m_defaultTerrainDetailMeshLOD1, typeof(Mesh), false, helpEnabled);
                defaults.m_terrainDetailLOD1Shadows = editorUtils.Toggle("DetailLOD1CastShadows", defaults.m_terrainDetailLOD1Shadows, helpEnabled);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;
                defaults.m_defaultTerrainDetailMesh = (Mesh)editorUtils.ObjectField("DetailMesh", defaults.m_defaultTerrainDetailMesh, typeof(Mesh), false, helpEnabled);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            DrawDetailSubDivisionData(defaults, editorUtils, boxStyle, helpEnabled);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

#pragma warning disable 162
            if (FloraGlobalData.TreesEnabled)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                editorUtils.Heading("TreeSettings");
                EditorGUI.indentLevel++;
                defaults.m_minimumRecommendedTreeCount = editorUtils.IntField("MinRecommendedTreeCount", defaults.m_minimumRecommendedTreeCount, helpEnabled);
                defaults.m_defaultTreeLODBias = editorUtils.FloatField("TreeLODBias", defaults.m_defaultTreeLODBias, helpEnabled);
                if (defaults.m_defaultTreeLODBias < 0.01f)
                {
                    defaults.m_defaultTreeLODBias = 0.01f;
                    EditorUtility.SetDirty(defaults);
                }
                defaults.m_defaultTreeFadeOutDistance = editorUtils.FloatField("TreeFadeOutDistance", defaults.m_defaultTreeFadeOutDistance, helpEnabled);
                defaults.m_minMaxTreeVertexCount = editorUtils.Vector2Field("MinMaxTreeVertexCount", defaults.m_minMaxTreeVertexCount, helpEnabled);
                defaults.m_enableLODSkipping = editorUtils.Toggle("EnableLODSkipping", defaults.m_enableLODSkipping, helpEnabled);
                EditorGUI.indentLevel--;
                if (defaults.m_enableLODSkipping)
                {
                    DrawSkippableLODData(defaults, editorUtils, boxStyle, helpEnabled);
                }
                DrawTreeSubDivisionData(defaults, editorUtils, boxStyle, helpEnabled);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
#pragma warning restore 162

            EditorGUILayout.BeginVertical(boxStyle);
            editorUtils.Heading("MaterialSettings");
            EditorGUI.indentLevel++;
            defaults.m_debugColorGradient = EditorGUILayout.GradientField(new GUIContent(editorUtils.GetTextValue("DebugColorGradient"), editorUtils.GetTooltip("DebugColorGradient")), defaults.m_debugColorGradient);
            editorUtils.InlineHelp("DebugColorGradient", helpEnabled);
            defaults.m_builtInMaterialTemplate = (Material)editorUtils.ObjectField("BuiltInMaterialTemplate", defaults.m_builtInMaterialTemplate, typeof(Material), false, helpEnabled);
            DrawStringList(ref m_builtInCompatibleShadersFoldout, "BuiltInCompatibleShaders", defaults.m_builtInCompatibleShaderNames, defaults, editorUtils, boxStyle, helpEnabled);
            defaults.m_urpMaterialTemplate = (Material)editorUtils.ObjectField("URPMaterialTemplate", defaults.m_urpMaterialTemplate, typeof(Material), false, helpEnabled);
            DrawStringList(ref m_urpCompatibleShadersFoldout, "URPCompatibleShaders", defaults.m_urpCompatibleShaderNames, defaults, editorUtils, boxStyle, helpEnabled);
            defaults.m_hdrpMaterialTemplate = (Material)editorUtils.ObjectField("HDRPMaterialTemplate", defaults.m_hdrpMaterialTemplate, typeof(Material), false, helpEnabled);
            DrawStringList(ref m_hdrpCompatibleShadersFoldout, "HDRPCompatibleShaders", defaults.m_hdrpCompatibleShaderNames, defaults, editorUtils, boxStyle, helpEnabled);
            defaults.m_defaultCutoutValue = editorUtils.Slider("DefaultCutoutValue", defaults.m_defaultCutoutValue, 0f, 1f, helpEnabled);
            defaults.m_enableGPUInstancing = editorUtils.Toggle("EnableGPUInstacing", defaults.m_enableGPUInstancing, helpEnabled);
            defaults.m_useSpeedTreeHueColors = editorUtils.Toggle("UseSpeedTreeHueColors", defaults.m_useSpeedTreeHueColors, helpEnabled);
            if (defaults.m_useSpeedTreeHueColors)
            {
                EditorGUI.indentLevel++;
                defaults.m_hueColor1 = editorUtils.ColorField("HueColor1", defaults.m_hueColor1, helpEnabled);
                defaults.m_hueColor2 = editorUtils.ColorField("HueColor2", defaults.m_hueColor2, helpEnabled);
                EditorGUI.indentLevel--;
            }

            defaults.m_defaultNormalMap = (Texture2D)editorUtils.ObjectField("DefaultNormalMap", defaults.m_defaultNormalMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
            defaults.m_defaultMaskMap = (Texture2D)editorUtils.ObjectField("DefaultMaskMap", defaults.m_defaultMaskMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(boxStyle);
            editorUtils.Heading("ShaderSettings");
            DrawStringList(ref m_cutoutShaderFoldout, "CutoutShaderProperties", defaults.m_cutoutShaderProperties, defaults, editorUtils, boxStyle, helpEnabled);
            DrawStringList(ref m_baseColorShaderFoldout, "BaseColorShaderProperties", defaults.m_baseColorShaderProperties, defaults, editorUtils, boxStyle, helpEnabled);
            DrawStringList(ref m_albedoShaderFoldout, "AlbedoShaderProperties", defaults.m_albedoShaderProperties, defaults, editorUtils, boxStyle, helpEnabled);
            DrawStringList(ref m_normalShaderFoldout, "NormalShaderProperties", defaults.m_normalShaderProperties, defaults, editorUtils, boxStyle, helpEnabled);
            DrawStringList(ref m_maskMapShaderFoldout, "MaskMapShaderProperties", defaults.m_maskMapShaderProperties, defaults, editorUtils, boxStyle, helpEnabled);
            DrawStringList(ref m_windMaterialFoldout, "WindMaterialNames", defaults.m_windMaterialNames, defaults, editorUtils, boxStyle, helpEnabled);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(boxStyle);
            editorUtils.Heading("SRPSettings");
            DrawHDRPKeywords(defaults, editorUtils, boxStyle, helpEnabled);
            DrawURPKeywords(defaults, editorUtils, boxStyle, helpEnabled);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

#pragma warning disable 162
            if (FloraGlobalData.TreesEnabled)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                editorUtils.Heading("TreeTypeSettings");
                DrawStringList(ref m_treeTypesFoldout, "TreeTypes", defaults.m_treeTypes, defaults, editorUtils, boxStyle, helpEnabled);
                DrawStringList(ref m_grassTypesFoldout, "GrassTypes", defaults.m_grassTypes, defaults, editorUtils, boxStyle, helpEnabled);
                DrawStringList(ref m_plantTypesFoldout, "PlantTypes", defaults.m_plantTypes, defaults, editorUtils, boxStyle, helpEnabled);
                DrawStringList(ref m_logTypesFoldout, "LogTypes", defaults.m_logTypes, defaults, editorUtils, boxStyle, helpEnabled);
                EditorGUILayout.EndVertical();
            }
#pragma warning restore 162


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(defaults);
            }
        }

        
        private void DrawSkippableLODData(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_skippableLODFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_skippableLODFoldout, editorUtils.GetTextValue("SkippableLODData"));
            if (m_skippableLODFoldout)
            {
                if (defaults.m_treeSkippableLODData.Count > 0)
                {
                    for (int i = 0; i < defaults.m_treeSkippableLODData.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_treeSkippableLODData[i].m_foldoutGUI = EditorGUILayout.Foldout(
                            defaults.m_treeSkippableLODData[i].m_foldoutGUI, defaults.m_treeSkippableLODData[i].m_name);
                        if (defaults.m_treeSkippableLODData[i].m_foldoutGUI)
                        {
                            defaults.m_treeSkippableLODData[i].m_name =
                                EditorGUILayout.TextField("Name", defaults.m_treeSkippableLODData[i].m_name);
                            defaults.m_treeSkippableLODData[i].m_enabled =
                                EditorGUILayout.Toggle("Enabled", defaults.m_treeSkippableLODData[i].m_enabled);
                            defaults.m_treeSkippableLODData[i].m_skipLOD =
                                EditorGUILayout.IntField("Skip LOD", defaults.m_treeSkippableLODData[i].m_skipLOD);
                            defaults.m_treeSkippableLODData[i].m_ifHasMoreOrEqualLODS = EditorGUILayout.IntField(
                                "If Has More Or Equal LODS", defaults.m_treeSkippableLODData[i].m_ifHasMoreOrEqualLODS);
                            if (editorUtils.Button("RemoveElement"))
                            {
                                defaults.m_treeSkippableLODData.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_treeSkippableLODData.Add(new SkippableLODData
                    {
                        m_foldoutGUI = true
                    });
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("SkippableLODData", helpEnabled);
        }
        private void DrawDetailSubDivisionData(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_detailSubDivisionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_detailSubDivisionFoldout, editorUtils.GetTextValue("DetailSubDivision"));
            if (m_detailSubDivisionFoldout)
            {
                if (defaults.m_detailSubDivisionData.Count > 0)
                {
                    for (int i = 0; i < defaults.m_detailSubDivisionData.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_detailSubDivisionData[i].m_foldoutGUI = EditorGUILayout.Foldout(
                            defaults.m_detailSubDivisionData[i].m_foldoutGUI,
                            defaults.m_detailSubDivisionData[i].m_lodName);
                        if (defaults.m_detailSubDivisionData[i].m_foldoutGUI)
                        {
                            defaults.m_detailSubDivisionData[i].m_lodName =
                                EditorGUILayout.TextField("LOD Name", defaults.m_detailSubDivisionData[i].m_lodName);
                            defaults.m_detailSubDivisionData[i].m_lodID =
                                EditorGUILayout.IntField("LOD ID", defaults.m_detailSubDivisionData[i].m_lodID);
                            defaults.m_detailSubDivisionData[i].m_subDivisionValue =
                                EditorGUILayout.IntField("Sub Division Value",
                                    defaults.m_detailSubDivisionData[i].m_subDivisionValue);
                            if (editorUtils.Button("RemoveElement"))
                            {
                                defaults.m_detailSubDivisionData.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_detailSubDivisionData.Add(new LODSubDivisionData
                    {
                        m_foldoutGUI = true
                    });
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("SubDivisionData", helpEnabled);
        }
        private void DrawTreeSubDivisionData(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_treeSubDivisionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_treeSubDivisionFoldout, editorUtils.GetTextValue("TreeSubDivision"));
            if (m_treeSubDivisionFoldout)
            {
                if (defaults.m_treeSubDivisionData.Count > 0)
                {
                    for (int i = 0; i < defaults.m_treeSubDivisionData.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_treeSubDivisionData[i].m_foldoutGUI = EditorGUILayout.Foldout(
                            defaults.m_treeSubDivisionData[i].m_foldoutGUI,
                            defaults.m_treeSubDivisionData[i].m_lodName);
                        if (defaults.m_treeSubDivisionData[i].m_foldoutGUI)
                        {
                            defaults.m_treeSubDivisionData[i].m_lodName =
                                EditorGUILayout.TextField("LOD Name", defaults.m_treeSubDivisionData[i].m_lodName);
                            defaults.m_treeSubDivisionData[i].m_lodID =
                                EditorGUILayout.IntField("LOD ID", defaults.m_treeSubDivisionData[i].m_lodID);
                            defaults.m_treeSubDivisionData[i].m_subDivisionValue =
                                EditorGUILayout.IntField("Sub Division Value",
                                    defaults.m_treeSubDivisionData[i].m_subDivisionValue);
                            if (editorUtils.Button("RemoveElement"))
                            {
                                defaults.m_treeSubDivisionData.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_treeSubDivisionData.Add(new LODSubDivisionData
                    {
                        m_foldoutGUI = true
                    });
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("SubDivisionData", helpEnabled);
        }
        private void DrawTerrainCellCountData(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_terrainCellCountFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_terrainCellCountFoldout, editorUtils.GetTextValue("TerrainCellCountData"));

            if (m_terrainCellCountFoldout)
            {
                if (defaults.m_terrainCellCountData.Count > 0)
                {
                    for (int i = 0; i < defaults.m_terrainCellCountData.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_terrainCellCountData[i].m_foldoutGUI = EditorGUILayout.Foldout(
                            defaults.m_terrainCellCountData[i].m_foldoutGUI, defaults.m_terrainCellCountData[i].m_name);
                        if (defaults.m_terrainCellCountData[i].m_foldoutGUI)
                        {
                            defaults.m_terrainCellCountData[i].m_name =
                                EditorGUILayout.TextField("Name", defaults.m_terrainCellCountData[i].m_name);
                            defaults.m_terrainCellCountData[i].m_terrainSize = EditorGUILayout.FloatField(
                                "Terrain Size",
                                defaults.m_terrainCellCountData[i].m_terrainSize);
                            defaults.m_terrainCellCountData[i].m_cellDensity =
                                (CoreCommonFloraData.CellDensity) EditorGUILayout.EnumPopup("Cell Density",
                                    defaults.m_terrainCellCountData[i].m_cellDensity);
                            if (editorUtils.Button("RemoveElement"))
                            {
                                defaults.m_terrainCellCountData.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_terrainCellCountData.Add(new TerrainCellCountData
                    {
                        m_foldoutGUI = true
                    });
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("TerrainCellCountData", helpEnabled);
        }

        private void DrawStringList(ref bool foldOut, string listName, List<string> list, FloraDefaults floraDefaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(foldOut, editorUtils.GetTextValue(listName));
            if (foldOut)
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        EditorGUILayout.BeginHorizontal();
                        list[i] = EditorGUILayout.TextField("Name", list[i]);
                        if (editorUtils.Button("RemoveElementString", GUILayout.MaxWidth(30f)))
                        {
                            list.RemoveAt(i);
                            EditorUtility.SetDirty(floraDefaults);
                            EditorGUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    list.Add(null);
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp(listName, helpEnabled);
        }

        private void DrawHDRPKeywords(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_hdrpKeywordFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_hdrpKeywordFoldout, editorUtils.GetTextValue("HDRPKeywords"));
            if (m_hdrpKeywordFoldout)
            {
                if (defaults.m_hdrpMaterialKeywordSetup.Count > 0)
                {
                    for (int i = 0; i < defaults.m_hdrpMaterialKeywordSetup.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_hdrpMaterialKeywordSetup[i].m_foldoutGUI = EditorGUILayout.Foldout(defaults.m_hdrpMaterialKeywordSetup[i].m_foldoutGUI, defaults.m_hdrpMaterialKeywordSetup[i].m_name);
                        if (defaults.m_hdrpMaterialKeywordSetup[i].m_foldoutGUI)
                        {
                            EditorGUILayout.BeginHorizontal();
                            defaults.m_hdrpMaterialKeywordSetup[i].m_name =
                                EditorGUILayout.TextField("Name", defaults.m_hdrpMaterialKeywordSetup[i].m_name);
                            if (editorUtils.Button("RemoveElementString", GUILayout.MaxWidth(30f)))
                            {
                                defaults.m_hdrpMaterialKeywordSetup.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }

                            EditorGUILayout.EndHorizontal();

                            if (defaults.m_hdrpMaterialKeywordSetup[i].m_enabled == FloraKeywordAddRemove.None)
                            {
                                defaults.m_hdrpMaterialKeywordSetup[i].m_enabled = (FloraKeywordAddRemove)EditorGUILayout.EnumPopup("Keyword Mode", defaults.m_hdrpMaterialKeywordSetup[i].m_enabled);
                            }
                            else
                            {
                                EditorGUILayout.BeginHorizontal();
                                defaults.m_hdrpMaterialKeywordSetup[i].m_enabled = (FloraKeywordAddRemove)EditorGUILayout.EnumPopup(defaults.m_hdrpMaterialKeywordSetup[i].m_enabled, GUILayout.MaxWidth(70f));
                                defaults.m_hdrpMaterialKeywordSetup[i].m_keywordValue = EditorGUILayout.TextField("Keyword Value", defaults.m_hdrpMaterialKeywordSetup[i].m_keywordValue);
                                EditorGUILayout.EndHorizontal();
                            }

                            defaults.m_hdrpMaterialKeywordSetup[i].m_setShaderProperty = EditorGUILayout.Toggle("Set Shader Property", defaults.m_hdrpMaterialKeywordSetup[i].m_setShaderProperty);
                            if (defaults.m_hdrpMaterialKeywordSetup[i].m_setShaderProperty)
                            {
                                defaults.m_hdrpMaterialKeywordSetup[i].m_shaderPropertyName =
                                    EditorGUILayout.TextField("Shader Property Name",
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_shaderPropertyName);
                                defaults.m_hdrpMaterialKeywordSetup[i].m_valueType =
                                    (FloraKeywordValueType) EditorGUILayout.EnumPopup("Shader Property Type",
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_valueType);
                                switch (defaults.m_hdrpMaterialKeywordSetup[i].m_valueType)
                                {
                                    case FloraKeywordValueType.Float:
                                    {
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_floatValue =
                                            EditorGUILayout.FloatField("Float Value",
                                                defaults.m_hdrpMaterialKeywordSetup[i].m_floatValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Int:
                                    {
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_intValue =
                                            EditorGUILayout.IntField("Int Value",
                                                defaults.m_hdrpMaterialKeywordSetup[i].m_intValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Color:
                                    {
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_colorValue =
                                            EditorGUILayout.ColorField("Color Value",
                                                defaults.m_hdrpMaterialKeywordSetup[i].m_colorValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Texture:
                                    {
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_textureValue =
                                            (Texture) EditorGUILayout.ObjectField("Float Value",
                                                defaults.m_hdrpMaterialKeywordSetup[i].m_textureValue, typeof(Texture),
                                                false, GUILayout.MaxHeight(16f));
                                        break;
                                    }
                                    case FloraKeywordValueType.Bool:
                                    {
                                        defaults.m_hdrpMaterialKeywordSetup[i].m_boolValue =
                                            EditorGUILayout.Toggle("Bool Value",
                                                defaults.m_hdrpMaterialKeywordSetup[i].m_boolValue);
                                        break;
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_hdrpMaterialKeywordSetup.Add(new MaterialKeywordData());
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("HDRPKeywords", helpEnabled);
        }
        private void DrawURPKeywords(FloraDefaults defaults, EditorUtils editorUtils, GUIStyle boxStyle, bool helpEnabled)
        {
            m_urpKeywordFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_urpKeywordFoldout, editorUtils.GetTextValue("URPKeywords"));
            if (m_urpKeywordFoldout)
            {
                if (defaults.m_urpMaterialKeywordSetup.Count > 0)
                {
                    for (int i = 0; i < defaults.m_urpMaterialKeywordSetup.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(boxStyle);
                        defaults.m_urpMaterialKeywordSetup[i].m_foldoutGUI = EditorGUILayout.Foldout(defaults.m_urpMaterialKeywordSetup[i].m_foldoutGUI, defaults.m_urpMaterialKeywordSetup[i].m_name);
                        if (defaults.m_urpMaterialKeywordSetup[i].m_foldoutGUI)
                        {
                            EditorGUILayout.BeginHorizontal();
                            defaults.m_urpMaterialKeywordSetup[i].m_name =
                                EditorGUILayout.TextField("Name", defaults.m_urpMaterialKeywordSetup[i].m_name);
                            if (editorUtils.Button("RemoveElementString", GUILayout.MaxWidth(30f)))
                            {
                                defaults.m_urpMaterialKeywordSetup.RemoveAt(i);
                                EditorUtility.SetDirty(defaults);
                                EditorGUIUtility.ExitGUI();
                            }

                            EditorGUILayout.EndHorizontal();

                            if (defaults.m_urpMaterialKeywordSetup[i].m_enabled == FloraKeywordAddRemove.None)
                            {
                                defaults.m_urpMaterialKeywordSetup[i].m_enabled = (FloraKeywordAddRemove)EditorGUILayout.EnumPopup("Keyword Mode", defaults.m_urpMaterialKeywordSetup[i].m_enabled);
                            }
                            else
                            {
                                EditorGUILayout.BeginHorizontal();
                                defaults.m_urpMaterialKeywordSetup[i].m_enabled = (FloraKeywordAddRemove)EditorGUILayout.EnumPopup(defaults.m_urpMaterialKeywordSetup[i].m_enabled, GUILayout.MaxWidth(70f));
                                defaults.m_urpMaterialKeywordSetup[i].m_keywordValue = EditorGUILayout.TextField("Keyword Value", defaults.m_urpMaterialKeywordSetup[i].m_keywordValue);
                                EditorGUILayout.EndHorizontal();
                            }

                            defaults.m_urpMaterialKeywordSetup[i].m_setShaderProperty = EditorGUILayout.Toggle("Set Shader Property", defaults.m_urpMaterialKeywordSetup[i].m_setShaderProperty);
                            if (defaults.m_urpMaterialKeywordSetup[i].m_setShaderProperty)
                            {
                                defaults.m_urpMaterialKeywordSetup[i].m_shaderPropertyName =
                                    EditorGUILayout.TextField("Shader Property Name",
                                        defaults.m_urpMaterialKeywordSetup[i].m_shaderPropertyName);
                                defaults.m_urpMaterialKeywordSetup[i].m_valueType =
                                    (FloraKeywordValueType) EditorGUILayout.EnumPopup("Shader Property Type",
                                        defaults.m_urpMaterialKeywordSetup[i].m_valueType);
                                switch (defaults.m_urpMaterialKeywordSetup[i].m_valueType)
                                {
                                    case FloraKeywordValueType.Float:
                                    {
                                        defaults.m_urpMaterialKeywordSetup[i].m_floatValue =
                                            EditorGUILayout.FloatField("Float Value",
                                                defaults.m_urpMaterialKeywordSetup[i].m_floatValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Int:
                                    {
                                        defaults.m_urpMaterialKeywordSetup[i].m_intValue =
                                            EditorGUILayout.IntField("Int Value",
                                                defaults.m_urpMaterialKeywordSetup[i].m_intValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Color:
                                    {
                                        defaults.m_urpMaterialKeywordSetup[i].m_colorValue =
                                            EditorGUILayout.ColorField("Color Value",
                                                defaults.m_urpMaterialKeywordSetup[i].m_colorValue);
                                        break;
                                    }
                                    case FloraKeywordValueType.Texture:
                                    {
                                        defaults.m_urpMaterialKeywordSetup[i].m_textureValue =
                                            (Texture) EditorGUILayout.ObjectField("Float Value",
                                                defaults.m_urpMaterialKeywordSetup[i].m_textureValue, typeof(Texture),
                                                false, GUILayout.MaxHeight(16f));
                                        break;
                                    }
                                    case FloraKeywordValueType.Bool:
                                    {
                                        defaults.m_urpMaterialKeywordSetup[i].m_boolValue =
                                            EditorGUILayout.Toggle("Bool Value",
                                                defaults.m_urpMaterialKeywordSetup[i].m_boolValue);
                                        break;
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                if (editorUtils.Button("AddNewElement"))
                {
                    defaults.m_urpMaterialKeywordSetup.Add(new MaterialKeywordData());
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            editorUtils.InlineHelp("URPKeywords", helpEnabled);
        }
    }
}