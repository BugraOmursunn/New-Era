using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
	[SerializeField] private GameTypes gameType;
	public GameObject[] playerPrefabs;

	void Start()
	{
		Spawn();
	}

	[Button]
	public void Spawn()
	{
		//for single, multi selection
		GameObject player;
		GameObject spawnPlayer = playerPrefabs[Random.Range(0, playerPrefabs.Length)];
		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				player = Instantiate(spawnPlayer, Vector3.zero, Quaternion.identity);
				break;
			case GameTypes.MultiPlayer:
				player = PhotonNetwork.Instantiate(spawnPlayer.name, Vector3.zero, Quaternion.identity);
				break;
		}
	}
}
