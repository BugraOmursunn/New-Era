using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
	public static Func<CharacterData> GetCharacterData;
	public static Func<GameTypes> gameType;
	public static Func<CharacterData.CharacterStats> GetCurrentCharacterStats;
	public static Action RefreshCharacterStats;
	public static Action<float> DecreaseMana;
	public static Func<float, bool> IsHaveEnoughMana;
}
