using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
	public GameObject singlePlayerPlayerPrefab;
	public GameObject multiPlayerPlayerPrefab;

	void Start()
	{
		GameTypes gameType = EventManager.gameType.Invoke();

		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				GameObject sPlayer = Instantiate(singlePlayerPlayerPrefab, Vector3.zero, Quaternion.identity);
				break;
			case GameTypes.MultiPlayer:
				GameObject playerPackage = PhotonNetwork.Instantiate(multiPlayerPlayerPrefab.name, Vector3.zero, Quaternion.identity);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
