using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameTypes gameType;
	private void Awake()
	{
		EventManager.gameType = () => gameType;
	}

	private void Start()
	{
		Cursor.visible = true;
	}
}
