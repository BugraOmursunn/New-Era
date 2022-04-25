using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    /// <summary>
    /// Flora Detail Object
    /// </summary>
    public class DetailObject : CoreDetailObjectData
    {
        // public DetailObjectData m_detailObject = new DetailObjectData();
        private FloraTerrainTile m_terrainTile;
        private void OnEnable()
        {
            FloraGlobals.onRefreshDetailObject += RefreshAll;
            if (!FloraGlobals.DetailData.Contains(this))
                FloraGlobals.DetailData.Add(this);
        }
        private void Start()
        {
            StartCoroutine(InitWait());
        }

        private DetailOverrideData m_overrideData;

        public DetailOverrideData OverrideData
        {
            get 
            {
                if (m_overrideData == null)
                {
                    m_overrideData = m_terrainTile.m_detailObjectList.Find(x => x.DetailScriptableObject == DetailScriptableObject);
                }
                return m_overrideData;
            }
        }

        private void LateUpdate()
        {
            //Debug.Log("From LateUpdate, our object is" + DetailScriptableObject.name + " and our source index is: " + DetailScriptableObject.SourceDataIndex);
            if (m_initSuccessful && m_terrainTile.CoreFloraTerrainData.IsVisible && 
                !DetailScriptableObject.DisableDraw)
            {
                UpdateLocalCameraCells();
                UpdateGPUCells(OverrideData.SourceDataIndex);
                Draw();
            }
        }
        private void OnDisable()
        {
            m_initSuccessful = false;
            CleanReleaseData();
            FloraGlobals.onRefreshDetailObject -= RefreshAll;
            if (FloraGlobals.DetailData.Contains(this))
                FloraGlobals.DetailData.Remove(this);
        }
        private void OnDrawGizmos()
        {
           if (Application.isPlaying && Application.isEditor && m_initSuccessful && m_terrainTile.CoreFloraTerrainData.IsVisible && m_terrainTile.DrawDebugInfo)
           {
               DebugDraw();
           }
        }
        
        public bool SupportsInstancing()
        {
            CleanReleaseData();
            if (SystemInfo.supportsInstancing == false)
                return false;
            if (TerrainTileData == null)
            {
                m_terrainTile = GetComponentInParent<FloraTerrainTile>() as FloraTerrainTile;
                TerrainTileData = m_terrainTile;
                if (TerrainTileData == null)
                    return false;
            }
            return true;
        }
        public bool Init()
        {
            SetInitSuccessful(false);
            if (!SupportsInstancing())
                return false;
            CoreFloraTerrainData pwTerrainData = m_terrainTile.CoreFloraTerrainData;
            CoreCameraCellData cameraCellData = m_terrainTile.CameraCellData;
            if (!cameraCellData.IsReady || !pwTerrainData.IsReady || DetailScriptableObject.Mat == null || DetailScriptableObject.Mesh == null)
            {
                SetInitSuccessful(false);
                return false;
            }
            if(DetailScriptableObject.SourceDataType == SourceDataType.Splat)
            {
                if (DetailScriptableObject.SourceDataIndex > pwTerrainData.SplatPrototypesCount) DetailScriptableObject.SourceDataIndex = pwTerrainData.SplatPrototypesCount;
            }
            
            return InitPool();
        }

        IEnumerator InitWait()
        {
            yield return null;
            yield return null;
            Init();
        }

        public void RefreshAll()
        {
            if(m_terrainTile != null)
            {
                if (Application.isPlaying && m_terrainTile.CameraCellData.Count != 0)
                    Init();
            }

        }
    }
}