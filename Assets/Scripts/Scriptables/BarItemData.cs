using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarItemData", menuName = "BarItems/BarItemData", order = 1)]
public class BarItemData : ScriptableObject
{
	public BarItemTypes itemType;
	public Sprite icon;
}
