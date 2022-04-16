using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BarControllerData", menuName = "BarItems/BarControllerData", order = 0)]
public class BarControllerData : ScriptableObject
{
	public SlotsData slots;
}
