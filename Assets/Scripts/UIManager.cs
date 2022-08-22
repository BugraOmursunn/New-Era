using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField] private GameObject canvas;
	private void Awake()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			canvas.SetActive(false);
	}
}
