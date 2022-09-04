using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : Character
{
	[field: SerializeField] public override CharacterData CharacterData { get; set; }
	[field: SerializeField] public override Animator CharAnimator { get; set; }
	[field: SerializeField] public override bool IsDead { get; set; }

	[SerializeField] private CharacterData.CharacterStats _currentCharacterStats = new CharacterData.CharacterStats();
	private PhotonView view;

	private void OnEnable()
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		_currentCharacterStats.Health = CharacterData.characterStats.Health;
		_currentCharacterStats.Mana = CharacterData.characterStats.Mana;
		_currentCharacterStats.Stamina = CharacterData.characterStats.Stamina;

		EventManager.GetCharacterData = () => CharacterData;
		EventManager.GetCurrentCharacterStats = () => _currentCharacterStats;
		EventManager.GetCharacterData = () => CharacterData;
	}

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

		if (_currentCharacterStats.Health > 0 && IsDead == false)
		{
			_currentCharacterStats.Health += damage;
			EventManager.RefreshCharacterStats?.Invoke();

			if (_currentCharacterStats.Health <= 0)
			{
				IsDead = true;
			}

			CharAnimator.SetTrigger(_currentCharacterStats.Health > 0 ? CharacterData.GetHitAnim : CharacterData.DieAnim);
		}

		var transformPosition = this.transform.position;
		GameObject newDamageIndicator = GameTypePrefabManager.ReturnGameTypeSelectionPrefab(CharacterData.DamageIndicator,
			new Vector3(transformPosition.x, transformPosition.y + 1f, transformPosition.z),
			Quaternion.identity);

		newDamageIndicator.GetComponent<DamageIndicator>().Instantiate(damage);
	}
}
