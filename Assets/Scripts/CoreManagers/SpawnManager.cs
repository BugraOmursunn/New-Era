using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;

public class SpawnManager : MonoBehaviour
{
	public GameObject playerPrefab;

	void Start()
	{
		Spawn();
	}
	[Button]
	public void Spawn()
	{
		//for single, multi selection
		GameObject player = GameTypePrefabManager.ReturnGameTypeSelectionPrefab(playerPrefab, Vector3.zero, Quaternion.identity);

		// GameTypes gameType = EventManager.gameType.Invoke();
		//
		// switch (gameType)
		// {
		// 	case GameTypes.SinglePlayer:
		// 		GameObject sPlayer = Instantiate(singlePlayerPlayerPrefab, Vector3.zero, Quaternion.identity);
		// 		break;
		// 	case GameTypes.MultiPlayer:
		// 		GameObject playerPackage = PhotonNetwork.Instantiate(multiPlayerPlayerPrefab.name, Vector3.zero, Quaternion.identity);
		// 		break;
		// 	default:
		// 		throw new ArgumentOutOfRangeException();
		// }
	}
}
