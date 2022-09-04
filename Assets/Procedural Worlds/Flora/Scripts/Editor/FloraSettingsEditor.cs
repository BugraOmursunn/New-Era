using System;
using UnityEditor;
using Object = UnityEngine.Object;
namespace ProceduralWorlds.Flora
{
    [CustomEditor(typeof(FloraSettings))]


    public class FloraSettingsEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UpdatSettings();
            
        }
        private void UpdatSettings()
        {
            EditorGUI.BeginChangeCheck();
            {
                var t = target as FloraSettings;
                for (int i = 0; i < t.shaderProfiles.Count; i++)
                {
                    // load pre existing shader object if null and guid already exists
                    if (t.shaderProfiles[i].Builtin == null && t.shaderProfiles[i].BuiltinGUID == null)
                    {
                        t.shaderProfiles[i].Builtin = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(t.shaderProfiles[i].BuiltinGUID));
                        if (t.shaderProfiles[i].Builtin == null)
                        {
                            t.shaderProfiles[i].BuiltinGUID = null;
                        }
                    }
                    if (t.shaderProfiles[i].URP == null && t.shaderProfiles[i].UrpGUID == null)
                    {
                        t.shaderProfiles[i].URP =AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(t.shaderProfiles[i].UrpGUID));
                        if (t.shaderProfiles[i].URP == null)
                        {
                            t.shaderProfiles[i].UrpGUID = null;
                        }
                    }
                    if (t.shaderProfiles[i].HDRP == null && t.shaderProfiles[i].HdrpGUID == null)
                    {
                        t.shaderProfiles[i].HDRP =AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(t.shaderProfiles[i].HdrpGUID));
                        if (t.shaderProfiles[i].HDRP == null)
                        {
                            t.shaderProfiles[i].HdrpGUID = null;
                        }
                    }

                    // Get guid for assigned shader objects
                    if (t.shaderProfiles[i].Builtin != null)
                    {
                        string guid;
                        long localID;
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(t.shaderProfiles[i].Builtin, out guid, out localID);
                        t.shaderProfiles[i].BuiltinGUID = guid;
                    }
                    if (t.shaderProfiles[i].URP != null)
                    {
                        string guid;
                        long localID;
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(t.shaderProfiles[i].URP, out guid, out localID);
                        t.shaderProfiles[i].UrpGUID = guid;
                    }
                    if (t.shaderProfiles[i].HDRP != null)
                    {
                        string guid;
                        long localID;
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(t.shaderProfiles[i].HDRP, out guid, out localID);
                        t.shaderProfiles[i].HdrpGUID = guid;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}