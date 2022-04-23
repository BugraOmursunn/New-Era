using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillData
{
	public SkillName skillName;
	public GameObject skillPrefab;
	public bool isChild;
	public float activationTime;
	
}
[Serializable]
public class WeaponData
{
	public WeaponType weaponType;
	
	[Space(10)]
	public GameObject weaponPrefab;
	public Vector3 handAttachPos;
	public Vector3 handAttachRot;
	public Vector3 sheathAttachPos;
	public Vector3 sheathAttachRot;
	
}
