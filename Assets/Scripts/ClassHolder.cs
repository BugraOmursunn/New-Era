using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
[Serializable]
public class SpellData
{
	public SpellType Type;
	public bool isChild;
	public GameObject Prefab;
	public AnimationClip Animation;
	public string AnimTriggerName;
	public float vfxActivationTime;
	public Vector3 offSet;
	
	[SuffixLabel("seconds ", true)]
	public float cooldown;
	[SuffixLabel("seconds ", true)]
	public float castTime;
}
[Serializable]
public class WeaponData
{
	public WeaponType Type;
	public bool is2Hand;
	public GameObject Prefab;
	public float cooldown;
	public float castTime;
	public string drawAnimName;
	public string sheetAnimName;
}

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
