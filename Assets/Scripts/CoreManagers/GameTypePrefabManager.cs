using System;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class GameTypePrefabManager : MonoBehaviour
{
	public static Func<GameObject, Vector3, Quaternion, GameObject> ReturnGameTypeSelectionPrefab;

	private void OnEnable()
	{
		ReturnGameTypeSelectionPrefab = GameTypeSelection;
	}

	private GameObject GameTypeSelection(GameObject _gameObject, Vector3 pos, Quaternion rot)
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				return Instantiate(_gameObject, pos, rot);
				break;
			case GameTypes.MultiPlayer:
				return PhotonNetwork.Instantiate(_gameObject.name, pos, rot);
			default:
				return null;
		}
	}
}
