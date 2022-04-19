using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BarItemData", menuName = "BarItems/BarItemData", order = 1)]
public class BarItemData : ScriptableObject
{
	public Sprite icon;

	public BarActionTypes barActionType;

	[ShowIf("barActionType", BarActionTypes.Skill)]
	public SkillData skillData;

	[ShowIf("barActionType", BarActionTypes.Weapon)]
	public WeaponType weaponType;
}
