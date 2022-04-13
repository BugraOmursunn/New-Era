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

	private bool isDraw;
	public void OnDrawWeapon()
	{
		isDraw = !isDraw;
		this.GetComponent<Animator>().SetBool("DrawSword",isDraw);
		Debug.Log("Test");
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
