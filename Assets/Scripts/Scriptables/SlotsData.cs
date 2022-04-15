using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotData", menuName = "BarItems/SlotData", order = 2)]
public class SlotsData : ScriptableObject
{
	public BarItemData[] barItems;
}
