using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ScriptableProcessor
{
	// public static T SelectScriptable<T>(T scriptableObject)
	// {
	// 	switch (scriptableObject)
	// 	{
	// 		case SpellData:
	// 			return (T)(object)((SpellData)(object)scriptableObject);
	// 			break;
	// 		case WeaponData:
	// 			return (T)(object)((WeaponData)(object)scriptableObject);
	// 			break;
	// 		default:
	// 			return scriptableObject;
	// 	}
	// }
	
	// public static bool SelectScriptable<T>(T scriptableObject)
	// {
	// 	bool isCastSuccessful = false;
	//
	// 	switch (scriptableObject)
	// 	{
	// 		case WeaponData:
	// 			WeaponData newW = ((WeaponData)(object)scriptableObject);
	// 			InputEventManager.DrawWeapon.Invoke(newW);
	// 			break;
	// 		case SpellData:
	// 			SpellData newS = ((SpellData)(object)scriptableObject);
	// 			isCastSuccessful = InputEventManager.CastSpell.Invoke(newS);
	// 			break;
	// 		default:
	// 			isCastSuccessful = true;
	// 			break;
	// 	}
	// 	return isCastSuccessful;
	//}
	
}
