using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
public class BarManager : MonoBehaviour
{
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private BarControllerData barControllerData;
	[SerializeField] private BarItemData barItemData;
	[SerializeField] private Transform[] buttons;

	private InputAction m_Press1;
	private InputAction m_Press2;
	private InputAction m_Press3;
	private InputAction m_Press4;
	private InputAction m_Press5;
	private InputAction m_Press6;
	//private InputAction m_DrawWeapon;

	[SerializeField] private PlayerInputs playerInputs;

	private void OnEnable()
	{
		playerInputs.Enable();
	}
	private void OnDisable()
	{
		playerInputs.Disable();
	}
	private void Awake()
	{
		playerInputs = new PlayerInputs();

		m_Press1 = playerInputs.Player.Press1;
		m_Press1.performed += BarSkillPressed;
		m_Press1.Enable();

		m_Press2 = playerInputs.Player.Press2;
		m_Press2.performed += BarSkillPressed;
		m_Press2.Enable();

		m_Press3 = playerInputs.Player.Press3;
		m_Press3.performed += BarSkillPressed;
		m_Press3.Enable();

		m_Press4 = playerInputs.Player.Press4;
		m_Press4.performed += BarSkillPressed;
		m_Press4.Enable();

		m_Press5 = playerInputs.Player.Press5;
		m_Press5.performed += BarSkillPressed;
		m_Press5.Enable();

		m_Press6 = playerInputs.Player.Press6;
		m_Press6.performed += BarSkillPressed;
		m_Press6.Enable();

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

	private int m_PressedButtonIndex;
	private void BarSkillPressed(InputAction.CallbackContext context)
	{
		if (context.action == m_Press1)
			m_PressedButtonIndex = 0;

		if (context.action == m_Press2)
			m_PressedButtonIndex = 1;

		if (context.action == m_Press3)
			m_PressedButtonIndex = 2;

		if (context.action == m_Press4)
			m_PressedButtonIndex = 3;

		if (context.action == m_Press5)
			m_PressedButtonIndex = 4;

		if (context.action == m_Press6)
			m_PressedButtonIndex = 5;

		barItemData = barControllerData.slots.barItems[m_PressedButtonIndex];
		
		switch (barItemData.barActionType)
		{
			case BarActionTypes.Weapon:
				InputEventManager.InteractionHandler(barItemData, m_PressedButtonIndex);
				break;
			case BarActionTypes.Skill:
				InputEventManager.InteractionHandler(barItemData, m_PressedButtonIndex);
				break;
			case BarActionTypes.Empty:
				break;
			case BarActionTypes.Item:
				break;
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
