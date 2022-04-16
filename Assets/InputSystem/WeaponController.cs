using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
	private PlayerInputs playerInputs;
	
	[SerializeField] private Transform hand;

	[SerializeField] private Transform weapon;
	[SerializeField] private Vector3 handAttachPos;
	[SerializeField] private Vector3 handAttachRot;

	[SerializeField] private Transform sheath;
	[SerializeField] private Vector3 sheathAttachPos;
	[SerializeField] private Vector3 sheathAttachRot;

	private bool isDraw;

	private InputAction m_attack;
	private void OnEnable()
	{
		playerInputs.Enable();
		InputEventManager.Draw2HWeapon += Draw2HWeapon;
	}
	private void OnDisable()
	{
		playerInputs.Disable();
		InputEventManager.Draw2HWeapon -= Draw2HWeapon;
	}
	private void Awake()
	{
		playerInputs = new PlayerInputs();
		
		m_attack = playerInputs.Player.Attack;
		m_attack.performed += _ => OnAttack();
		m_attack.Enable();
	}
	private void Draw2HWeapon()
	{
		isDraw = !isDraw;
		if (isDraw)
		{
			this.GetComponent<Animator>().SetTrigger("DrawSword");
		}
		else
		{
			this.GetComponent<Animator>().SetTrigger("SheatSword");
		}
		this.GetComponent<Animator>().SetBool("is2hDraw", isDraw);
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
		weapon.parent = hand;
		weapon.transform.localPosition = handAttachPos;
		weapon.transform.localRotation = Quaternion.Euler(handAttachRot);
	}

	public void AttackWeaponToSheath()
	{
		weapon.parent = sheath;
		weapon.transform.localPosition = sheathAttachPos;
		weapon.transform.localRotation = Quaternion.Euler(sheathAttachRot);
	}
}
