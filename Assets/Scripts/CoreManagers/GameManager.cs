using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameTypes gameType;
	[SerializeField] private bool isGameMine;
	private void Awake()
	{
		EventManager.gameType = () => gameType;
		if (EventManager.gameType.Invoke() == GameTypes.MultiPlayer && this.transform.parent.GetComponent<PhotonView>().IsMine == false)
		{
			isGameMine = false;
			EventManager.IsGameMine = () => false;
			return;
		}
		isGameMine = true;
		EventManager.IsGameMine = () => true;
	}

	private void Start()
	{
		Cursor.visible = true;
	}
}
