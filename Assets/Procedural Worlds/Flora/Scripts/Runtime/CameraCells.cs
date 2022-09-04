using System;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    public class CameraCells : CoreCameraCellsObjectData
    {
        /// <summary>
        /// Add Camera Cells to Flora Globals for Tracking
        /// </summary>
        private void OnEnable()
        {
            if (!FloraGlobals.DetailData.Contains(this))
            {
                FloraGlobals.DetailData.Add(this);
            }
        }
        
        /// <summary>
        /// Remove Camera Cells from Flora Global Tracking
        /// </summary>
        private void OnDisable()
        {
            if (FloraGlobals.DetailData.Contains(this))
            {
                FloraGlobals.DetailData.Remove(this);
            }
        }
    }
}
