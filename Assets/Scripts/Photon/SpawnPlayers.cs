using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
	public GameObject playerPrefab;
	public GameObject canvasPanel;
	//public GameObject managers;
	//public GameObject virtualCam;
	private void Start()
	{
		GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
		player.transform.parent = canvasPanel.transform;
		// managers.GetComponent<SkillController>().playerAnimator = player.GetComponent<Animator>();
		// managers.GetComponent<MinimapManager>().target = player.transform;
		// virtualCam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
		// virtualCam.SetActive(true);
		// managers.SetActive(true);
	}
}
