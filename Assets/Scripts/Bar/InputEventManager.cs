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
		}
	}
	
	public static Action Draw2HWeapon;
}
