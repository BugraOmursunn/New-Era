using UnityEngine;
using System;

public static class InputEventManager
{
	public static void InteractionHandler(BarItemData barItem, int skillIndex)
	{
		switch (barItem.itemType)
		{
			case BarItemTypes.Weapon:
				InputEventManager.Draw2HWeapon.Invoke();
				break;
			case BarItemTypes.Skill:
				InputEventManager.CastSkill.Invoke(skillIndex);
				break;
		}
	}

	public static Action Draw2HWeapon;
	public static Action<int> CastSkill;
}
