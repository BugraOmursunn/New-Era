using UnityEngine;
using System;

public static class InputEventManager
{
	#region General Region
	public static Action<int> BarIndexPressed;
	#endregion

	#region Weapon Region
	public static Action Attack;
	public static Func<bool> IsDrawSword2h;
	public static Action<float> EnableWeaponTrail;
	#endregion
	
	#region Bottom Bar Interaction Region
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
	#endregion
	
}
