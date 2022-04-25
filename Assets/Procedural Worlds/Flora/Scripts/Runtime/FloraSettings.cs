using System.Collections.Generic;
using UnityEngine;
namespace ProceduralWorlds.Flora
{
    [CreateAssetMenu(fileName = "FloraSettings", menuName = "Procedural Worlds/Flora/Flora Settings", order = 0)]
    
    public class FloraSettings : ScriptableObject
    {
        /// <summary>
        /// Scriptable object that holds cross references for shader switching
        /// to support the multiple renderpiplines.
        /// </summary>
        [System.Serializable]
        public class ShaderProfiles
        {
            public Object Builtin;
            public Object URP;
            public Object HDRP;
            
           [HideInInspector]public string BuiltinGUID;
           [HideInInspector]public string UrpGUID;
           [HideInInspector]public string HdrpGUID;
        }
        
        [SerializeField]
        public List<ShaderProfiles> shaderProfiles;
    }
    
    
}