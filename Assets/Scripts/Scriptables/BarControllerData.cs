using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarControllerData", menuName = "ScriptableObjects/BarControllerData_ScriptableObject", order = 2)]
public class BarControllerData : ScriptableObject
{
	public SlotData[] slots = new SlotData[10];
}
