using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesContainer", menuName = "ScriptableObjects/ResourcesContainer", order = 5)]
public class ResourcesContainer : ScriptableObject
{
	[SerializeField] private GameObject damageIndicator;
	public static Func<GameObject> DamageIndicator;
	
	
	private void OnEnable()
	{
		DamageIndicator = () => damageIndicator;
	}
}
