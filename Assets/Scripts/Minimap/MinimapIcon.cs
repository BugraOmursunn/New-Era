using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
	[SerializeField] private MinimapData minimapData;
	[SerializeField] private IconTypes iconType;
	private GameObject m_MinimapIcon;

	private void Awake()
	{
		CreateMinimapIcon();
	}

	private void CreateMinimapIcon()
	{
		m_MinimapIcon = new GameObject();
		m_MinimapIcon.transform.position = this.transform.position + minimapData.iconOffset;
		m_MinimapIcon.transform.parent = this.transform;
		m_MinimapIcon.transform.name= "MinimapIcon";
		m_MinimapIcon.transform.tag = "MinimapIcon";
		m_MinimapIcon.layer = 6;
		m_MinimapIcon.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

		m_MinimapIcon.AddComponent<SpriteRenderer>();
		m_MinimapIcon.GetComponent<SpriteRenderer>().sprite = minimapData.minimapIcons[(int)iconType].iconSprite;
	}
}
