using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameTypes gameType;

	private void OnEnable()
	{
		EventManager.gameType = () => gameType;
	}
}
