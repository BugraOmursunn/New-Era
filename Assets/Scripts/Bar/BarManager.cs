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

	private void BarSkillPressed(InputAction.CallbackContext context)
	{
		if (context.action == m_Press1)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[0], 0);

		if (context.action == m_Press2)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[1], 1);
		
		if (context.action == m_Press3)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[2], 2);
		
		if (context.action == m_Press4)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[3], 3);
		
		if (context.action == m_Press5)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[4], 4);
		
		if (context.action == m_Press6)
			InputEventManager.InteractionHandler(barControllerData.slots.barItems[5], 5);
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
