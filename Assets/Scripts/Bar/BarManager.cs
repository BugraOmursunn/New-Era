using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class BarManager : MonoBehaviour
{
	[SerializeField] private BarControllerData barControllerData;
	
	[CanBeNull]
	private BarItemData m_BarItemData;

	[SerializeField] private Transform[] buttons;


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
		//m_Press1 = playerInput.actions["Press1"];
		//m_Press1.performed += OnPress1;
		//m_DrawWeapon = playerInputs.Player.DrawWeapon;
	}
	private void Start()
	{
		for (int i = 0; i < barControllerData.slots.barItems.Length; i++)
		{
			if (barControllerData.slots.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = barControllerData.slots.barItems[i].icon;
		}
	}

	private void BarSkillPressed(int index)
	{
		m_BarItemData = barControllerData.slots.barItems[index];
		if (m_BarItemData != null)
		{
			InputEventManager.InteractionHandler(m_BarItemData);
		}
	}
	// private void OnPress2(InputAction.CallbackContext context)
	// {
	// 	Debug.Log(((KeyControl)context.control).keyCode);
	// 	Debug.Log(context.action.name);
	// }

	[Button]
	private void OnValidate()
	{
		for (int i = 0; i < barControllerData.slots.barItems.Length; i++)
		{
			if (barControllerData.slots.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = barControllerData.slots.barItems[i].icon;
		}
	}
}
