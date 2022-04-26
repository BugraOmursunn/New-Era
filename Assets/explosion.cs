using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
	void Start()
	{
		ExplosionDamage(this.transform.position, 20);
	}
	void ExplosionDamage(Vector3 center, float radius)
	{
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		foreach (var hitCollider in hitColliders)
		{
			//hitCollider.SendMessage("AddDamage");
			Debug.Log(hitCollider.transform.name);
		}
	}
}
