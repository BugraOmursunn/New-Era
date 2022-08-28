using System;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	[SerializeField] private GameObject damageIndicator;

	public static Func<Transform, GameObject> DamageIndicator;

	private GameTypes gameType;
	private void OnEnable()
	{
		DamageIndicator = ReturnPrefab;
	}

	private void Start()
	{
		gameType = EventManager.gameType.Invoke();
	}
	private GameObject ReturnPrefab(Transform transform)
	{
		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				return Instantiate(damageIndicator, transform.position, Quaternion.identity);
				break;
			case GameTypes.MultiPlayer:
				return PhotonNetwork.Instantiate(damageIndicator.name, transform.position, Quaternion.identity);
			default:
				return null;
		}
	}
}
