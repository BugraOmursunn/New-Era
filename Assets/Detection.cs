using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		other.GetComponent<IInteraction>().TriggerInteraction();
	}
}
