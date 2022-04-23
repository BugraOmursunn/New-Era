using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	
	private WeaponData weaponData;

	private bool isDraw;
	

	[SerializeField] private Transform hand;

	[SerializeField] private bool isHaveSword2h;
	[SerializeField] private Transform sword2hSpawnPos;
	[SerializeField] private GameObject sword2hPrefab;
	
	private Transform sword2hParent;
	private Transform sword2hObject;
	
	private void OnEnable()
	{
		InputEventManager.DrawWeapon += DrawWeapon;
		InputEventManager.Attack += OnAttack;
		InputEventManager.IsDrawSword2h += IsDrawSword2h;
	}
	private void OnDisable()
	{
		InputEventManager.DrawWeapon -= DrawWeapon;
		InputEventManager.Attack -= OnAttack;
		InputEventManager.IsDrawSword2h -= IsDrawSword2h;
	}
	private void Awake()
	{
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
			InputEventManager.EnableWeaponTrail.Invoke(3);
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
	private bool IsDrawSword2h()
	{
		return isDraw;
	}
}
