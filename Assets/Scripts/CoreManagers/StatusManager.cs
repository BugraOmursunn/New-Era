using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
	private CharacterData characterData;
	private CharacterData.CharacterStats currentCharacterStats;
	[SerializeField] private Image hpFillImg;
	[SerializeField] private Image mpFillImg;

	private void OnEnable()
	{
		if (EventManager.IsGameMine.Invoke() == false) return;

		EventManager.RefreshCharacterStats += RefreshStats;
	}
	private void OnDisable()
	{
		if (EventManager.IsGameMine.Invoke() == false) return;

		EventManager.RefreshCharacterStats -= RefreshStats;
	}

	private void Start()
	{
		characterData = EventManager.GetCharacterData.Invoke();
		currentCharacterStats = EventManager.GetCurrentCharacterStats.Invoke();
		RefreshStats();
	}
	private void RefreshStats()
	{
		var initialStats = characterData.characterStats;
		var currentStats = currentCharacterStats;

		hpFillImg.DOFillAmount(currentStats.Health / initialStats.Health, 0.2f);
		mpFillImg.DOFillAmount(currentStats.Mana / initialStats.Mana, 0.2f);
	}
}
