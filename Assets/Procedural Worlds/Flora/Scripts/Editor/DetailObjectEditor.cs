using PWCommon5;
using UnityEditor;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    [CustomEditor(typeof(DetailObject))]
    public class DetailObjectEditor : PWEditor
    {
        private DetailObject m_detailObject;
        // We need to use and to call an instance of the default MaterialEditor
        private MaterialEditor[] m_materialEditor;
        private void OnEnable()
        {
            m_detailObject = (DetailObject) target;
            if (m_detailObject.DetailScriptableObject != null)
            {
                if (m_detailObject.DetailScriptableObject.Mat != null)
                {
                    // Create an instance of the default MaterialEditor
                    m_materialEditor = new MaterialEditor[m_detailObject.DetailScriptableObject.Mat.Length];
                    for (int i = 0; i < m_materialEditor.Length; i++)
                    {
                        if (m_detailObject.DetailScriptableObject.Mat[i] != null)
                        {
                            m_materialEditor[i] = (MaterialEditor) CreateEditor(m_detailObject.DetailScriptableObject.Mat[i]);
                        }
                    }
                }
            }

            Undo.undoRedoPerformed -= UndoBuffer;
            Undo.undoRedoPerformed += UndoBuffer;
        }
        private void OnDisable()
        {
            if (m_materialEditor != null)
            {
                // Free the memory used by default MaterialEditor
                for (int i = 0; i < m_materialEditor.Length; i++)
                {
                    if (m_materialEditor[i] != null)
                    {
                        DestroyImmediate(m_materialEditor[i]);
                    }
                }

                m_materialEditor = null;
            }
            Undo.undoRedoPerformed -= UndoBuffer;
        }
        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoBuffer;
        }

        public override void OnInspectorGUI()
        {
            FloraEditorUtility.EditorUtils.Initialize();

            CoreDetailScriptableObject DetailScriptableObject = m_detailObject.DetailScriptableObject;
            EditorGUI.BeginChangeCheck();
            DetailScriptableObject = (CoreDetailScriptableObject)FloraEditorUtility.EditorUtils.ObjectField("DetailScriptableObject", DetailScriptableObject, typeof(CoreDetailScriptableObject), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_detailObject, "Flora Renderer Object Changed");
                m_detailObject.DetailScriptableObject = DetailScriptableObject;
                m_detailObject.RefreshAll();
                EditorUtility.SetDirty(m_detailObject);
            }

            EditorGUI.BeginChangeCheck();
            FloraEditorUtility.EditorUtils.Panel("DetailerEditor", DetailerEditor, true);
            // Draw the material field of GrassTerrainObject
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_detailObject.DetailScriptableObject);
                // Free the memory used by the previous MaterialEditor
                if (m_materialEditor.Length != m_detailObject.DetailScriptableObject.Mat.Length)
                {
                    m_materialEditor = new MaterialEditor[m_detailObject.DetailScriptableObject.Mat.Length];
                }
                for (int i = 0; i < m_materialEditor.Length; i++)
                {
                    if (m_materialEditor[i] != null)
                    {
                        DestroyImmediate(m_materialEditor[i]);
                    }
                    if (m_detailObject.DetailScriptableObject.Mat[i] != null)
                    {
                        // Create a new instance of the default MaterialEditor
                        m_materialEditor[i] = (MaterialEditor) CreateEditor(m_detailObject.DetailScriptableObject.Mat[i]);
                    }
                }
            
                m_detailObject.RefreshAll();
            }
            if (m_materialEditor != null)
            {
                for (int i = 0; i < m_materialEditor.Length; i++)
                {
                    // Draw the material's foldout and the material shader field
                    // Required to call _materialEditor.OnInspectorGUI ();
                    if (m_materialEditor[i] != null)
                    {
                        m_materialEditor[i].DrawHeader();
                    
                        //  We need to prevent the user to edit Unity default materials
                        bool isDefaultMaterial = !AssetDatabase.GetAssetPath(m_detailObject.DetailScriptableObject.Mat[i]).StartsWith("Assets");
                        using (new EditorGUI.DisabledGroupScope(isDefaultMaterial))
                        {
                            // Draw the material properties
                            // Works only if the foldout of _materialEditor.DrawHeader () is open
                            EditorGUI.BeginChangeCheck();
                            m_materialEditor[i].OnInspectorGUI();
                            if (EditorGUI.EndChangeCheck())
                            {
                                m_detailObject.RefreshMaterials();
                            }
                        }
                    }
                }
            }
        }
        private void DetailerEditor(bool helpEnabled)
        {
            FloraEditorUtility.HelpEnabled = helpEnabled;
            FloraEditorUtility.DetailerEditor(m_detailObject.DetailScriptableObject);
        }
        public void UndoBuffer()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (m_detailObject != null)
            {
                m_detailObject.RefreshAll();
            }
        }
    }
}