using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponAttachmentData : MonoBehaviour
{
	public Vector3 handAttachPos;
	public Vector3 handAttachRot;
	public Vector3 sheathAttachPos;
	public Vector3 sheathAttachRot;

	public List<HandData> handData;
}
