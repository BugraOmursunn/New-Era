using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Func<bool> IsGameMine;
    public static Func<GameTypes> gameType;
    public static Func<CharacterData> GetCharacterData;
    public static Func<CharacterData.CharacterStats> GetCurrentCharacterStats;
    public static Action RefreshCharacterStats;
}
