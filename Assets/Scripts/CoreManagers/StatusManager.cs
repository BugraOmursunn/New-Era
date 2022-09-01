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
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		EventManager.RefreshCharacterStats += RefreshStats;
	}
	private void OnDisable()
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

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
