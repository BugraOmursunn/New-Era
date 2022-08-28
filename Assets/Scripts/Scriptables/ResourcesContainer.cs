using System;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesContainer", menuName = "ScriptableObjects/ResourcesContainer", order = 5)]
public class ResourcesContainer : ScriptableObject
{
	[SerializeField] private GameObject singleDamageIndicator;
	[SerializeField] private GameObject multiDamageIndicator;

	public static Func<Transform, GameTypes, GameObject> DamageIndicator;

	private void OnEnable()
	{
		DamageIndicator = ReturnPrefab;
	}

	private GameObject ReturnPrefab(Transform transform, GameTypes gameType)
	{
		switch (gameType)
		{
			case GameTypes.SinglePlayer:
				return Instantiate(singleDamageIndicator, transform.position, Quaternion.identity);
				break;
			case GameTypes.MultiPlayer:
				return PhotonNetwork.Instantiate(multiDamageIndicator.name, transform.position, Quaternion.identity);
			default:
				return null;
		}
	}
}
