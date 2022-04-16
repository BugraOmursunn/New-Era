using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
	[SerializeField] private Transform hand;

	[SerializeField] private Transform weapon;
	[SerializeField] private Vector3 handAttachPos;
	[SerializeField] private Vector3 handAttachRot;

	[SerializeField] private Transform sheath;
	[SerializeField] private Vector3 sheathAttachPos;
	[SerializeField] private Vector3 sheathAttachRot;

	private void OnEnable()
	{
		InputeventManager.Draw2HWeapon += Draw2HWeapon;
	}
	private void OnDisable()
	{
		InputeventManager.Draw2HWeapon -= Draw2HWeapon;
	}
	private bool isDraw;
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
		this.GetComponent<Animator>().SetBool("isWeaponDraw", isDraw);
	}
	
	public void OnAttack()
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
