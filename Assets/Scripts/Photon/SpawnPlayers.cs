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
	public GameObject managers;
	private void Start()
	{
		//GameObject playerPackage = Instantiate(playerPackagePrefab, Vector3.zero, Quaternion.identity);
		GameObject playerPackage = PhotonNetwork.Instantiate(playerPackagePrefab.name, Vector3.zero, Quaternion.identity);
		managers.GetComponent<SkillController>().playerAnimator = playerPackage.transform.GetChild(2).GetComponent<Animator>();
		managers.GetComponent<MinimapManager>().target = playerPackage.transform.GetChild(2);
		managers.SetActive(true);
	}
}
