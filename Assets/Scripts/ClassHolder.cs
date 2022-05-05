using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillData
{
	public SkillName skillName;
	public bool isChild;
	public GameObject skillPrefab;
	public AnimationClip skillAnimation;
	public string skillAnimationTriggerName;
	public float vfxActivationTime;
	public Vector3 offSet;
	public float cooldown;
}
[Serializable]
public class WeaponData
{
	public WeaponType weaponType;
	public GameObject weaponPrefab;
	public Vector3 handAttachPos;
	public Vector3 handAttachRot;
	public Vector3 sheathAttachPos;
	public Vector3 sheathAttachRot;
	public float cooldown;
}

[Serializable]
public class BarCooldownData
{
	public string barName;
	public float barDefaultCooldown;
	public float barCurrentCooldown;
}
