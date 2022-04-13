using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
[CreateAssetMenu(fileName = "MinimapData", menuName = "ScriptableObjects/MinimapData_ScriptableObject", order = 1)]

public class MinimapData : ScriptableObject
{
    [Serializable]
    public class MinimapIcon
    {
        public IconTypes iconType;
        public Sprite iconSprite;
    }
    public MinimapIcon[] minimapIcons;
    public Vector3 iconOffset;
}
