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
	}
}
