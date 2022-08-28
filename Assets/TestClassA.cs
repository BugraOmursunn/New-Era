using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClassA : MonoBehaviour, IInteraction
{
	public void TriggerInteraction()
	{
		Debug.Log("ClassA");
	}
}
