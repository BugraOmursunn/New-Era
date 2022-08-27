using UnityEngine;
using System;
using JetBrains.Annotations;

public static class InputEventManager
{
	#region General Region

	public static Action<int> BarIndexPressed;

	#endregion

	#region Weapon Region

	public static Action Attack;
	public static Func<bool> IsDrawSword2h;
	public static Action EnableWeaponTrail;
	public static Action DisableWeaponTrail;

	#endregion

	#region Camera Region

	public static Action<float> ChangeCameraZoomValue;

	#endregion

	#region Bottom Bar Interaction Region

	public static bool InteractionHandler(BarItemData barItem)
	{
		bool isCastSuccessful;

		switch (barItem.Type)
		{
			case BarActionTypes.Weapon:
				InputEventManager.DrawWeapon.Invoke(barItem.weaponData);
				isCastSuccessful = true;
				break;
			case BarActionTypes.Spell:
				isCastSuccessful = InputEventManager.CastSpell.Invoke(barItem.spellData);
				break;
			default:
				isCastSuccessful = true;
				break;
		}
		return isCastSuccessful;
	}

	public static Action<WeaponData> DrawWeapon;
	public static Func<SpellData, bool> CastSpell;

	#endregion

	#region Bar Manager

	public static Func<bool> IsCastingContinue;

	#endregion
}
