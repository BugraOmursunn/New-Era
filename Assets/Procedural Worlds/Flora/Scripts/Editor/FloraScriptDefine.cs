using System;
using System.IO;
using UnityEditor;

namespace ProceduralWorlds.Flora
{
    /// <summary>
    /// static function to check that the script define is present in the project
    /// </summary>
    [InitializeOnLoad]
    public static class FloraScriptDefine
    {
        static FloraScriptDefine()
        {
            //Adds the detailer script define
            bool updateScripting = false;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!symbols.Contains("FLORA_PRESENT"))
            {

                //Only perform check for variant limit if standalone - if Gaia exists, we can do this during the Gaia Maintenance process
                if (GetGaiaDirectory() == "")
                {

                    if (!EditorPrefs.HasKey("UnityEditor.ShaderGraph.VariantLimit") || EditorPrefs.GetInt("UnityEditor.ShaderGraph.VariantLimit") < 256)
                    {
                        if (EditorUtility.DisplayDialog("Increase Shader Graph Variant Limit", "The shader graph variant limit is set below 256 for this machine in the unity preferences. Flora needs the limit to be increased to 256 or higher for certain features to work correctly across all rendering pipelines. Do you want to increase the limit to 256 now? (Recommended) \r\n\r\n This will trigger a restart of the unity editor.", "Yes, set to 256", "No thanks"))
                        {
                            EditorPrefs.SetInt("UnityEditor.ShaderGraph.VariantLimit", 256);
                            EditorApplication.OpenProject(Environment.CurrentDirectory);
                        }
                    }
                }

                AssetDatabase.ImportAsset(GetFloraDirectory() + "/Content Resources/Shaders");

                updateScripting = true;
                if (symbols.Length > 0)
                {
                    symbols += ";FLORA_PRESENT";
                }
                else
                {
                    symbols += "FLORA_PRESENT";
                }
            }

            if (updateScripting)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }



        /// <summary>
        /// Return the Gaia directory in the project
        /// </summary>
        /// <returns>String containing the Gaia directory</returns>
        public static string GetGaiaDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string gaiaDirectory = "";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Gaia_ReadMe", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Gaia_ReadMe.txt")
                {
                    gaiaDirectory = path.Replace("/Gaia_ReadMe.txt", "");
                }
            }
#endif
            return gaiaDirectory;
        }

        /// <summary>
        /// Return the Flora directory in the project (if flora exists, otherwise returns null)
        /// </summary>
        /// <returns>String containing the Gaia directory</returns>
        public static string GetFloraDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string floraDirectory = "";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Flora_ReadMe", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Flora_ReadMe.txt")
                {
                    floraDirectory = path.Replace("/Flora_ReadMe.txt", "");
                }
            }
#endif
            return floraDirectory;
        }

        public static void AddNavMeshDesign()
        {
            //Adds the detailer script define
            bool updateScripting = false;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!symbols.Contains("FLORA_NAV_MESH"))
            {
                updateScripting = true;
                if (symbols.Length > 0)
                {
                    symbols += ";FLORA_NAV_MESH";
                }
                else
                {
                    symbols += "FLORA_NAV_MESH";
                }
            }

            if (updateScripting)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }
    }
}