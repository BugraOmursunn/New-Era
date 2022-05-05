using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering;

public class BarManager : MonoBehaviour
{
	[SerializeField] private SlotsData slotsData;

	[CanBeNull]
	private BarItemData m_BarItemData;

	[SerializeField] private Transform[] buttons;

	[SerializeField] private List<BarCooldownData> barCooldownData;
	private void OnEnable()
	{
		InputEventManager.BarIndexPressed += BarSkillPressed;
	}
	private void OnDisable()
	{
		InputEventManager.BarIndexPressed -= BarSkillPressed;
	}
	private void Awake()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
			{
				BarCooldownData newData = new BarCooldownData();
				
				switch (slotsData.barItems[i].barActionType)
				{
					case BarActionTypes.Empty:
						break;
					case BarActionTypes.Weapon:
						newData.barName = slotsData.barItems[i].weaponData.weaponType.ToString();
						newData.barDefaultCooldown = slotsData.barItems[i].weaponData.cooldown;
						break;
					case BarActionTypes.Skill:
						newData.barName = slotsData.barItems[i].skillData.skillName.ToString();
						newData.barDefaultCooldown = slotsData.barItems[i].skillData.cooldown;
						break;
					case BarActionTypes.Item:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				
				barCooldownData.Add(newData);
			}
		}
		
		//m_Press1 = playerInput.actions["Press1"];
		//m_Press1.performed += OnPress1;
		//m_DrawWeapon = playerInputs.Player.DrawWeapon;
	}
	private void Start()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = slotsData.barItems[i].icon;
		}
	}

	private void BarSkillPressed(int index)
	{
		m_BarItemData = slotsData.barItems[index];
		if (m_BarItemData != null)
		{
			InputEventManager.InteractionHandler(m_BarItemData);
		}
	}

	[Button]
	private void OnValidate()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = slotsData.barItems[i].icon;
		}
	}
}
