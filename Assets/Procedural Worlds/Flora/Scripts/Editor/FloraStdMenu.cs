// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using PWCommon5;

namespace ProceduralWorlds.Flora
{
    public class FloraStdMenu : Editor
    {
        /// <summary>
        /// Show tutorials
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Flora/Show Flora Tutorials...", false, 60)]
        public static void ShowTutorial()
        {
            Application.OpenURL(FloraApp.CONF.TutorialsLink);
        }

        /// <summary>
        /// Show support page
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Flora/Show Flora Support, Lodge a Ticket...", false, 61)]
        public static void ShowSupport()
        {
            Application.OpenURL(FloraApp.CONF.SupportLink);
        }

        /// <summary>
        /// Show review option
        /// </summary>
        [MenuItem("Window/" + PWConst.COMMON_MENU + "/Flora/Please Review Flora...", false, 62)]
        public static void ShowProductAssetStore()
        {
            Application.OpenURL(FloraApp.CONF.ASLink);
        }
    }
}
