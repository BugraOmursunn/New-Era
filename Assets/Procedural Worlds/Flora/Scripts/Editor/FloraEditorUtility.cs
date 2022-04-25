using PWCommon5;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace ProceduralWorlds.Flora
{
    [CanEditMultipleObjects]
    public class FloraEditorUtility : PWEditor
    {
        public static EditorUtils EditorUtils
        {
            get
            {
                if (m_editorUtils == null)
                {
                    m_editorUtils = FloraApp.GetEditorUtils(Editor.CreateInstance<FloraEditorUtility>());
                }

                return m_editorUtils;
            }
            set
            {
                m_editorUtils = value;
            }
        }

        [SerializeField] private static EditorUtils m_editorUtils;

        public static bool HelpEnabled;

        public static void DetailerEditor(CoreDetailScriptableObject data)
        {
            EditorUtils.Initialize();

            //Variables
            string Name = data.Name;
            Mesh Mesh = data.Mesh;
            Material[] Mat = data.Mat;
            Shader ZprimeShader = data.ZprimeShader;
            CoreCommonFloraData.SourceDataType SourceDataType = data.SourceDataType;
            int SubCellDivision = data.SubCellDivision;
            float MaxDistSQR = data.MaxDistSQR;
            Color ColorA = data.ColorA;
            Color ColorB = data.ColorB;
            AnimationCurve ColorTransition = data.ColorTransition;
            float ColorRandomization = data.ColorRandomization;
            bool DisableDraw = data.DisableDraw;
            int DrawBias = data.DrawBias;
            float InDistance = data.InDistance;
            float InFadeDistance = data.InFadeDistance;
            float OutDistance = data.OutDistance;
            float OutFadeDistance = data.OutFadeDistance;
            ShadowCastingMode ShadowMode = data.ShadowMode;
            bool Zprime = data.Zprime;
            float ZprimeMaxDistance = data.ZprimeMaxDistance;
            int ZprimeDrawBias = data.ZprimeDrawBias;
            int RandomSeed = data.RandomSeed;
            int SourceDataIndex = data.SourceDataIndex;
            int Density = data.Density;
            float MaxJitter = data.MaxJitter;
            float ScaleRangeMin = data.ScaleRangeMin;
            float ScaleRangeMax = data.ScaleRangeMax;
            AnimationCurve ScaleByAlpha = data.ScaleByAlpha;
            float ForwardAngleBias = data.ForwardAngleBias;
            float RandomRotationRange = data.RandomRotationRange;
            bool UseAdvancedRotations = data.UseAdvancedRotations;
            float AlignToUp = data.AlignToUp;
            CoreDetailObjectData.SlopeType SlopeType = data.SlopeType;
            float AlignForwardToSlope = data.AlignForwardToSlope;
            float AlphaThreshold = data.AlphaThreshold;
            bool UseNoise = data.UseNoise;
            float NoiseScale = data.NoiseScale;
            float NoiseMaskContrast = data.NoiseMaskContrast;
            float NoiseMaskOffset = data.NoiseMaskOffset;
            bool InvertNoise = data.InvertNoise;
            bool UseHeight = data.UseHeight;
            AnimationCurve ScaleByHeight = data.ScaleByHeight;
            bool UseSlope = data.UseSlope;
            AnimationCurve ScaleBySlope = data.ScaleBySlope;
            bool DrawCPUSpawnLocations = data.DrawCPUSpawnLocations;
            Color DebugColor = data.DebugColor;

            EditorGUI.BeginChangeCheck();

            EditorUtils.LabelField("ObjectName", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Name = EditorUtils.TextField("Name", Name, HelpEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("SourceData", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Mesh = (Mesh)EditorUtils.ObjectField("Mesh", Mesh, typeof(Mesh), false, HelpEnabled);

            bool meshReady = false;
            bool matReady = false;
            //Check if mesh is ready
            if (Mesh != null)
            {
                if (Mesh.subMeshCount > 1)
                {
                    EditorUtils.LabelField("Multiple Submeshes Detected", EditorStyles.label);
                    meshReady = true;
                }
                else
                {
                    meshReady = true;
                }
            }
            // init material array
            if (data.Mat == null)
            {
                if (meshReady)
                {
                    if (Mesh.subMeshCount > 1)
                    {
                        data.Mat = new Material[data.Mesh.subMeshCount];
                    }
                    else
                    {
                        data.Mat = new Material[1];
                    }
                }
                else
                {
                    data.Mat = new Material[1];
                }
            }
            // copy old data for material if it exists
            if (data.Mat != null && Mesh != null && data.Mesh !=null)
            {
                if (Mesh.subMeshCount != data.Mat.Length && meshReady)
                {
                    var tempMat = new Material[data.Mesh.subMeshCount];
                    for (int i = 0; i < (int)Mathf.Min(tempMat.Length, data.Mat.Length); i++)
                    {
                        tempMat[i] = data.Mat[i];
                    }
                    data.Mat = tempMat;
                    matReady = true;
                }
                if (data.Mesh.subMeshCount == data.Mat.Length && meshReady)
                {
                    matReady = true;
                }

                if (data.Mesh.subMeshCount == 1)
                {
                    if (data.Mat.Length > 1)
                    {
                        var tempMat = new Material[1];
                        tempMat[0] = data.Mat[0];
                        data.Mat = tempMat;
                        matReady = true;
                    }
                    else
                    {
                        matReady = true;
                    }
                }
            }

            if (matReady)
            {
                if (Mesh.subMeshCount > 1)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < Mat.Length; i++)
                    {
                        Mat[i] = (Material)EditorUtils.ObjectField("Material", Mat[i], typeof(Material), false, HelpEnabled);
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    Mat[0] = (Material)EditorUtils.ObjectField("Material", Mat[0], typeof(Material), false, HelpEnabled);
                }
            }

            ZprimeShader = (Shader)EditorUtils.ObjectField("Z-prime Shader", ZprimeShader, typeof(Shader), false, HelpEnabled);

            if (!FloraGlobalData.TreesEnabled)
            {
                FakeSourceData sourceData = FakeSourceData.Detail;
                switch (SourceDataType)
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
                        SourceDataType = CoreCommonFloraData.SourceDataType.Detail;
                        break;
                    case FakeSourceData.Splat:
                        SourceDataType = CoreCommonFloraData.SourceDataType.Splat;
                        break;
                    case FakeSourceData.TransformList:
                        SourceDataType = CoreCommonFloraData.SourceDataType.TransformList;
                        break;

                }
            }
            else
            {
#pragma warning disable 162
                SourceDataType = (CoreCommonFloraData.SourceDataType)EditorUtils.EnumPopup("Source Data Type", SourceDataType, HelpEnabled);
#pragma warning restore 162
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Cell Data", EditorStyles.boldLabel); 
            EditorGUI.indentLevel++;
            SubCellDivision = EditorUtils.IntSlider("Subdivision Cell Value", SubCellDivision, 1, 16, HelpEnabled);
            //MaxDistSQR = EditorUtils.FloatField("Max Distance Sqr", 1, HelpEnabled);
            MaxDistSQR = EditorUtils.FloatField("Max Distance Sqr", MaxDistSQR, HelpEnabled);
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Color", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            ColorA = EditorUtils.ColorField("Color A", ColorA, HelpEnabled);
            ColorB = EditorUtils.ColorField("Color B", ColorB, HelpEnabled);
            ColorTransition = EditorUtils.CurveField("ColorTransition", ColorTransition, HelpEnabled);
            ColorRandomization = EditorUtils.Slider("ColorRandomization", ColorRandomization, 0f, 1f, HelpEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Draw", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            DisableDraw = EditorUtils.Toggle("Disable Draw", DisableDraw, HelpEnabled);
            if (!DisableDraw)
            {
                DrawBias = EditorUtils.IntField("Draw Bias", DrawBias, HelpEnabled);
                InDistance = EditorUtils.FloatField("In Distance", InDistance, HelpEnabled);
                InFadeDistance = EditorUtils.FloatField("In Fade Distance", InFadeDistance, HelpEnabled);
                OutDistance = EditorUtils.FloatField("Out Distance", OutDistance, HelpEnabled);
                OutFadeDistance = EditorUtils.FloatField("Out Fade Distance", OutFadeDistance, HelpEnabled);
                ShadowMode = (ShadowCastingMode) EditorUtils.EnumPopup("Shadow Mode", ShadowMode, HelpEnabled);
                Zprime = EditorUtils.Toggle("Z-Prime", Zprime, HelpEnabled);
                if (Zprime)
                {
                    EditorGUI.indentLevel++;
                    if (ZprimeShader == null)
                    {
                        EditorGUILayout.LabelField("Z-Prime Shader Missing",EditorStyles.label);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Z-Prime shader: " + ZprimeShader.name,EditorStyles.label);
                    }
                    ZprimeMaxDistance = EditorUtils.Slider("Z-Prime Max Draw",ZprimeMaxDistance, InDistance + InFadeDistance, OutDistance - OutFadeDistance, HelpEnabled);
                    if (ZprimeMaxDistance > OutDistance)
                    {
                        ZprimeMaxDistance = OutDistance;
                    }
                    if (ZprimeMaxDistance < InDistance)
                    {
                        ZprimeMaxDistance = InDistance;
                    }
                    
                    ZprimeDrawBias = (int)EditorUtils.Slider("Z-Prime Draw Bias",ZprimeDrawBias, DrawBias - 500, DrawBias - 1, HelpEnabled);
                    EditorGUI.indentLevel--;
                }
                
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Position", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RandomSeed = EditorUtils.IntField("Random Seed", RandomSeed, HelpEnabled);
            SourceDataIndex = EditorUtils.IntSlider("Source Data Index", SourceDataIndex, 0, 32, HelpEnabled);
            Density = EditorUtils.IntSlider("Density", Density, 2, 1023, HelpEnabled);
            MaxJitter = EditorUtils.Slider("Max Jitter", MaxJitter, 0f, 1f, HelpEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Scale", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            ScaleRangeMin = EditorUtils.FloatField("Scale Range Min", ScaleRangeMin, HelpEnabled);
            ScaleRangeMax = EditorUtils.FloatField("Scale Range Max", ScaleRangeMax, HelpEnabled);
            ScaleByAlpha = EditorUtils.CurveField("Scale By Alpha", ScaleByAlpha, HelpEnabled);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("RotationsBasic", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            ForwardAngleBias = EditorUtils.Slider("Forward Angle Bias", ForwardAngleBias, 0, 360, HelpEnabled);
            RandomRotationRange = EditorUtils.Slider("Random Rotation Range", RandomRotationRange, 0, 1, HelpEnabled);
            UseAdvancedRotations = EditorUtils.Toggle("Use Advanced Rotations", UseAdvancedRotations, HelpEnabled);
            if (UseAdvancedRotations)
            {
                EditorGUI.indentLevel++;
                AlignToUp = EditorUtils.Slider("Align To Up", AlignToUp, 0, 1, HelpEnabled);
                SlopeType = (CoreDetailObjectData.SlopeType)EditorUtils.EnumPopup("Slope Type", SlopeType, HelpEnabled);
                AlignForwardToSlope = EditorUtils.Slider("Align Forward To Slope", AlignForwardToSlope, 0, 1, HelpEnabled);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("MaskSettings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            AlphaThreshold = EditorUtils.Slider("Alpha Threshold", AlphaThreshold, 0, 1, HelpEnabled);

            UseNoise = EditorUtils.Toggle("Use Noise", UseNoise, HelpEnabled);
            if (UseNoise)
            {
                EditorGUI.indentLevel++;
                NoiseScale = EditorUtils.Slider("Noise Scale", NoiseScale, 0.001f, 1, HelpEnabled);
                NoiseMaskContrast = EditorUtils.Slider("Noise Mask Contrast", NoiseMaskContrast, 0.001f, 8, HelpEnabled);
                NoiseMaskOffset = EditorUtils.Slider("Noise Mask Offset", NoiseMaskOffset, -4, 4, HelpEnabled);
                InvertNoise = EditorUtils.Toggle("Invert Noise", InvertNoise, HelpEnabled);
                EditorGUI.indentLevel--;
            }

            UseHeight = EditorUtils.Toggle("Use Height", UseHeight, HelpEnabled);
            if (UseHeight)
            {
                EditorGUI.indentLevel++;
                ScaleByHeight = EditorUtils.CurveField("Scale By Height", ScaleByHeight, HelpEnabled);
                EditorGUI.indentLevel--;
            }

            UseSlope = EditorUtils.Toggle("Use Slope", UseSlope, HelpEnabled);
            if (UseSlope)
            {
                EditorGUI.indentLevel++;
                ScaleBySlope = EditorUtils.CurveField("Scale By Slope", ScaleBySlope, HelpEnabled);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorUtils.LabelField("Debug", EditorStyles.boldLabel); 
            EditorGUI.indentLevel++;
            DrawCPUSpawnLocations = EditorUtils.Toggle("Draw CPU Spawn Locations", DrawCPUSpawnLocations, HelpEnabled);
            if (DrawCPUSpawnLocations)
            {
                EditorGUI.indentLevel++;
                DebugColor = EditorUtils.ColorField("Debug Color", DebugColor, HelpEnabled);
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Flora Renderer Changes Made");
                data.Name = Name;
                data.Mesh = Mesh;
                data.Mat = Mat;
                data.ZprimeShader = ZprimeShader;
                data.SourceDataType = SourceDataType;
                data.SubCellDivision = SubCellDivision;
                data.MaxDistSQR = MaxDistSQR;
                data.ColorA = ColorA;
                data.ColorB = ColorB;
                data.ColorTransition = ColorTransition;
                data.ColorRandomization = ColorRandomization;
                data.DisableDraw = DisableDraw;
                data.DrawBias = DrawBias;
                data.InDistance = InDistance;
                data.InFadeDistance = InFadeDistance;
                data.OutDistance = OutDistance;
                data.OutFadeDistance = OutFadeDistance;
                data.ShadowMode = ShadowMode;
                data.Zprime = Zprime;
                data.ZprimeMaxDistance = ZprimeMaxDistance;
                data.ZprimeDrawBias = ZprimeDrawBias;
                data.RandomSeed = RandomSeed;
                data.SourceDataIndex = SourceDataIndex;
                data.Density = Density;
                data.MaxJitter = MaxJitter;
                data.ScaleRangeMin = ScaleRangeMin;
                data.ScaleRangeMax = ScaleRangeMax;
                data.ScaleByAlpha = ScaleByAlpha;
                data.ForwardAngleBias = ForwardAngleBias;
                data.RandomRotationRange = RandomRotationRange;
                data.UseAdvancedRotations = UseAdvancedRotations;
                data.AlignToUp = AlignToUp;
                data.SlopeType = SlopeType;
                data.AlignForwardToSlope = AlignForwardToSlope;
                data.AlphaThreshold = AlphaThreshold;
                data.UseNoise = UseNoise;
                data.NoiseScale = NoiseScale;
                data.NoiseMaskContrast = NoiseMaskContrast;
                data.NoiseMaskOffset = NoiseMaskOffset;
                data.InvertNoise = InvertNoise;
                data.UseHeight = UseHeight;
                data.ScaleByHeight = ScaleByHeight;
                data.UseSlope = UseSlope;
                data.ScaleBySlope = ScaleBySlope;
                data.DrawCPUSpawnLocations = DrawCPUSpawnLocations;
                data.DebugColor = DebugColor;
                EditorUtility.SetDirty(data);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}