using UnityEngine;
using System;

public static class InputEventManager
{
	public static void InteractionHandler(BarInteractableTypes interactionType)
	{
		switch (interactionType)
		{
			case BarInteractableTypes.Draw_2h_weapon:
				InputEventManager.Draw2HWeapon.Invoke();
				break;
			case BarInteractableTypes.Cast_2h_Skill1:
				InputEventManager.CastSkill1.Invoke();
				break;
		}
	}
	
	public static Action Draw2HWeapon;
	public static Action CastSkill1;
}
