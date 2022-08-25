using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField] private GameObject canvas;

	private GameTypes gameType;
	private void Awake()
	{
		gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				canvas.SetActive(false);
		}
	}
}
