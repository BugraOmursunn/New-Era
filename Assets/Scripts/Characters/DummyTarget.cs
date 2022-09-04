using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DummyTarget : MonoBehaviour, IDamageAble
{
	[SerializeField] private GameObject damageIndicatorPrefab;
	[SerializeField] private Animator charAnimator;
	[SerializeField] private float Health;
	private float currentHealth;
	private bool IsDead;
	private PhotonView view;

	private void Start()
	{
		currentHealth = Health;
	}
	public void GetDamage(float damage)
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.SinglePlayer)
		{
			DamageProcessor(damage);
		}
		else if (gameType == GameTypes.MultiPlayer)
		{
			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(DamageProcessor), RpcTarget.All, damage);
		}
	}

	[PunRPC]
	public void DamageProcessor(float damage)
	{
		if (IsDead == true)
			return;

		if (currentHealth > 0 && IsDead == false)
		{
			currentHealth += damage;

			if (currentHealth <= 0)
			{
				IsDead = true;
				Invoke(nameof(Resurrection), 3f);
			}

			charAnimator.SetTrigger(currentHealth > 0 ? "GetHit" : "Die");
		}


		var transformPosition = this.transform.position;
		GameObject newDamageIndicator = GameTypePrefabManager.ReturnGameTypeSelectionPrefab(damageIndicatorPrefab, 
			new Vector3(transformPosition.x, transformPosition.y + 1f, transformPosition.z), 
			Quaternion.identity);
		
		newDamageIndicator.GetComponent<DamageIndicator>().Instantiate(damage);
	}

	private void Resurrection()
	{
		charAnimator.SetTrigger("Resurrection");
		currentHealth = Health;
		IsDead = false;
	}
}
