using System;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    /// <summary>
    /// Global detail manager, sets up static variables used to control and influence performance
    /// </summary>
    public class FloraGlobalManager : MonoBehaviour
    {
        public FloraGlobalData Settings => m_settings;
        #region Definitions
        [SerializeField] protected FloraGlobalData m_settings = new FloraGlobalData();
        [NonSerialized] protected FloraGlobalData m_oldSettings = new FloraGlobalData();
        #endregion
        #region Methods
        private void OnEnable()
        {
            StoreGlobals();
            SetGlobals();
        }
        private void OnDisable() => ResetGlobals();
        public void SetGlobals()
        {
            FloraGlobals.Settings = new FloraGlobalData(m_settings);
            FloraGlobals.Settings.ChangeCheck();
        }
        public void GetGlobals() => m_settings = new FloraGlobalData(FloraGlobals.Settings);
        public void StoreGlobals() => m_oldSettings = new FloraGlobalData(FloraGlobals.Settings);
        public void ResetGlobals()
        {
            FloraGlobals.Settings = new FloraGlobalData();
            FloraGlobals.Settings.ChangeCheck();
        }
        #endregion
    }
}