using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : Character, IPunObservable
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

		EventManager.GetCharacterData = () => CharacterData;
		EventManager.GetCurrentCharacterStats = () => _currentCharacterStats;
		EventManager.GetCharacterData = () => CharacterData;
		EventManager.DecreaseMana = DecreaseMana;
		EventManager.IsHaveEnoughMana = IsHaveEnoughMana;
	}

	private void Start()
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			// if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			// 	return;

			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(Initialize), RpcTarget.All);
		}
		else
		{
			Initialize();
		}
	}

	[PunRPC]
	private void Initialize()
	{
		_currentCharacterStats.Health = CharacterData.characterStats.Health;
		_currentCharacterStats.Mana = CharacterData.characterStats.Mana;
		_currentCharacterStats.Stamina = CharacterData.characterStats.Stamina;
		_currentCharacterStats.healthRegen = CharacterData.characterStats.healthRegen;
		_currentCharacterStats.manaRegen = CharacterData.characterStats.manaRegen;
		_currentCharacterStats.staminaRegen = CharacterData.characterStats.staminaRegen;
		EventManager.RefreshCharacterStats?.Invoke();
	}

	[PunRPC]
	//sync health
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			object data = _currentCharacterStats.Health;
			// We own this player: send the others our data
			stream.SendNext(data);
		}
		else
		{
			// Network player, receive data
			var data = stream.ReceiveNext();
			if (data != null)
			{
				_currentCharacterStats.Health = (float)data;
			}
		}
	}


	public override void GetDamage(float damage)
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.SinglePlayer)
		{
			HealthProcessor(damage);
		}
		else if (gameType == GameTypes.MultiPlayer)
		{
			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(HealthProcessor), RpcTarget.All, damage);
		}
	}

	[PunRPC]
	public override void HealthProcessor(float damage)
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
		Debug.Log(_currentCharacterStats.Health);
	}

	public override void DecreaseMana(float amount)
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.SinglePlayer)
		{
			ManaProcessor(amount);
		}
		else if (gameType == GameTypes.MultiPlayer)
		{
			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(ManaProcessor), RpcTarget.All, amount);
		}
	}

	[PunRPC]
	public override void ManaProcessor(float amount)
	{
		_currentCharacterStats.Mana -= amount;
		EventManager.RefreshCharacterStats?.Invoke();
	}

	private bool IsHaveEnoughMana(float requiredMana)
	{
		return _currentCharacterStats.Mana >= requiredMana;
	}

	private float tempTime;
	private void Update()
	{
		tempTime += Time.deltaTime;
		if (_currentCharacterStats.Health > 0 && IsDead == false && tempTime >= 1f)
		{
			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(RegenProcessor), RpcTarget.All);
			tempTime = 0;
		}
	}
	[PunRPC]
	public override void RegenProcessor()
	{
		_currentCharacterStats.Health += Mathf.Clamp(_currentCharacterStats.healthRegen, 0, CharacterData.characterStats.Health);
		_currentCharacterStats.Mana += Mathf.Clamp(_currentCharacterStats.manaRegen, 0, CharacterData.characterStats.Mana);

		EventManager.RefreshCharacterStats?.Invoke();
	}
}
