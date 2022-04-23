using UnityEngine;
using System;

public static class InputEventManager
{
	public static void InteractionHandler(BarItemData barItem)
	{
		switch (barItem.barActionType)
		{
			case BarActionTypes.Weapon:
				InputEventManager.DrawWeapon.Invoke(barItem.weaponData);
				break;
			case BarActionTypes.Skill:
				InputEventManager.CastSkill.Invoke(barItem.skillData);
				break;
		}
	}

	public static Action<WeaponData> DrawWeapon;
	public static Action<SkillData> CastSkill;
}
