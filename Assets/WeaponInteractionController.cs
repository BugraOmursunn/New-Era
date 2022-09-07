using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInteractionController : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out IDamageAble damageAble) == false)
			return;

		if (damageAble != this.transform.GetComponentInParent<IDamageAble>())
			damageAble.GetDamage(-50);
		
	}
}
