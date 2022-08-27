using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BarCooldownData
{
	public bool isReady;
	public string barName;
	public float barDefaultCooldown;
	public float barCurrentCooldown;
	public float castTime;
}


[Serializable]
public class HandData
{
	[Serializable]
	public class FingerData
	{
		public Transform finger;
	}

	public Transform hand;
	public float positionWeight;
	public float rotationWeight;
	public float maintainRelativePositionWeight;

	public List<FingerData> fingers;
}
