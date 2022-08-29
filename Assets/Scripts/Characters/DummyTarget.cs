using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DummyTarget : Character
{
	[field: SerializeField] public override CharacterData CharacterData { get; set; }
	[field: SerializeField] public CharacterData.CharacterStats CurrentCharacterStats { get; set; }
	[field: SerializeField] public override Animator CharAnimator { get; set; }

	[field: SerializeField] public override bool IsDead { get; set; }

	private PhotonView view;

	public override void GetDamage(float damage)
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
	public override void DamageProcessor(float damage)
	{
		if (IsDead == true)
			return;

		if (CurrentCharacterStats.Health > 0 && IsDead == false)
		{
			CurrentCharacterStats.Health += damage;

			if (CurrentCharacterStats.Health <= 0)
			{
				IsDead = true;
				Invoke(nameof(Resurrection), 3f);
			}

			CharAnimator.SetTrigger(CurrentCharacterStats.Health > 0 ? CharacterData.GetHitAnim : CharacterData.DieAnim);
		}

		
		GameObject newDamageIndicator = GameTypePrefabManager.ReturnGameTypeSelectionPrefab(CharacterData.DamageIndicator, this.transform.position, Quaternion.identity);
		newDamageIndicator.GetComponent<DamageIndicator>().Instantiate(damage);
	}

	private void Resurrection()
	{
		CharAnimator.SetTrigger("Resurrection");
		CurrentCharacterStats.Health = 10;
		IsDead = false;
	}
}
