using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;

public class SpawnManager : MonoBehaviour
{
	[SerializeField] private GameTypes gameType;
	public GameObject playerPrefab;

	void Start()
	{
		Spawn();
	}
	
	[Button]
	public void Spawn()
	{
		//for single, multi selection
		GameObject player;

		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
				break;
			case GameTypes.MultiPlayer:
				player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
				break;
		}
	}
}
