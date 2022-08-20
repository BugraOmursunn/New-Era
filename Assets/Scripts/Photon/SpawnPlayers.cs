using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
	public GameObject playerPackagePrefab;
	private void Start()
	{
		//GameObject playerPackage = Instantiate(playerPackagePrefab, Vector3.zero, Quaternion.identity);
		GameObject playerPackage = PhotonNetwork.Instantiate(playerPackagePrefab.name, Vector3.zero, Quaternion.identity);
	}
}
