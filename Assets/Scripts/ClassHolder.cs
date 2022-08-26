using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillData
{
	public SkillType skillType;
	public bool isChild;
	public GameObject skillPrefab;
	public AnimationClip skillAnimation;
	public string skillAnimationTriggerName;
	public float vfxActivationTime;
	public Vector3 offSet;
	public float cooldown;
	public float castTime;
}
[Serializable]
public class WeaponData
{
	public WeaponType weaponType;
	public bool is2Hand;
	public GameObject weaponPrefab;
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
