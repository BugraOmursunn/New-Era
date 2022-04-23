using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
	private PlayerInputs playerInputs;
	private WeaponData weaponData;

	private bool isDraw;
	private InputAction m_attack;

	[SerializeField] private Transform hand;

	[SerializeField] private bool isHaveSword2h;
	[SerializeField] private Transform sword2hSpawnPos;
	[SerializeField] private GameObject sword2hPrefab;
	
	private Transform sword2hParent;
	private Transform sword2hObject;
	private void OnEnable()
	{
		playerInputs.Enable();
		InputEventManager.DrawWeapon += DrawWeapon;
	}
	private void OnDisable()
	{
		playerInputs.Disable();
		InputEventManager.DrawWeapon -= DrawWeapon;
	}
	private void Awake()
	{
		playerInputs = new PlayerInputs();

		m_attack = playerInputs.Player.Attack;
		m_attack.performed += _ => OnAttack();
		m_attack.Enable();

		if (isHaveSword2h)
		{
			GameObject sword2h = Instantiate(sword2hPrefab, sword2hSpawnPos);
			sword2hParent = sword2h.transform;
			sword2hObject = sword2h.transform.GetChild(0);
		}
	}
	private void DrawWeapon(WeaponData _weaponData)
	{
		weaponData = _weaponData;

		isDraw = !isDraw;
		switch (weaponData.weaponType)
		{
			case WeaponType.Empty:
				break;
			case WeaponType.Sword_1h_shield:
				break;
			case WeaponType.Sword_2h:
				this.GetComponent<Animator>().SetTrigger(isDraw ? "Draw2hSword" : "Sheath2hSword");
				this.GetComponent<Animator>().SetBool("isEmptyHand", !isDraw);
				this.GetComponent<Animator>().SetBool("is2hDraw", isDraw);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnAttack()
	{
		if (isDraw == true)
		{
			this.GetComponent<Animator>().SetTrigger("Attack");
		}
	}

	public void AttachWeaponToHand()
	{
		sword2hObject.parent = hand;
		sword2hObject.localPosition = weaponData.handAttachPos;
		sword2hObject.localRotation = Quaternion.Euler(weaponData.handAttachRot);
	}

	public void AttackWeaponToSheath()
	{
		sword2hObject.parent = sword2hParent.transform;
		sword2hObject.transform.localPosition = weaponData.sheathAttachPos;
		sword2hObject.localRotation = Quaternion.Euler(weaponData.sheathAttachRot);
	}
}
