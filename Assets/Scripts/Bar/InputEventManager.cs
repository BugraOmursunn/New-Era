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

	public static bool InteractionHandler(ScriptableObject barItem)
	{
		bool isCastSuccessful;

		switch (barItem)
		{
			case WeaponData weaponData:
				InputEventManager.DrawWeapon.Invoke(weaponData);
				isCastSuccessful = true;
				break;
			case SpellData spellData:
				isCastSuccessful = InputEventManager.CastSpell.Invoke(spellData);
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

	public static Action<float> SetIsCasting;
	public static Func<bool> IsCastingContinue;

	#endregion
}
