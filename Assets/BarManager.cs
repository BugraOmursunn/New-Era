using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
public class BarManager : MonoBehaviour
{
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private BarControllerData barControllerData;
	[SerializeField] private Transform[] buttons;

	private InputAction m_Press1;
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
		m_Press1.performed += OnPress1;
		m_Press1.Enable();
		
		
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

	private void OnPress1(InputAction.CallbackContext context)
	{
		InputeventManager.InteractionHandler(barControllerData.slots.barItems[0].barInteractionType);
	}
	
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
		
