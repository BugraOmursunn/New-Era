using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
public class InputHandler : MonoBehaviour
{
	private PlayerInputs playerInputs;

	private InputAction m_attack;

	private int m_PressedButtonIndex;
	private InputAction m_Press1;
	private InputAction m_Press2;
	private InputAction m_Press3;
	private InputAction m_Press4;
	private InputAction m_Press5;
	private InputAction m_Press6;

	private InputAction m_rotateMouseWheel;
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

		m_attack = playerInputs.Player.Attack;
		m_attack.performed += _ => InputEventManager.Attack.Invoke();
		m_attack.Enable();

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

		m_rotateMouseWheel = playerInputs.Player.MouseWheel;
		m_rotateMouseWheel.performed += MouseWheel;
		m_rotateMouseWheel.Enable();
	}

	private void BarSkillPressed(InputAction.CallbackContext context)
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		if (context.action == playerInputs.Player.Press1)
			m_PressedButtonIndex = 0;

		if (context.action == playerInputs.Player.Press2)
			m_PressedButtonIndex = 1;

		if (context.action == playerInputs.Player.Press3)
			m_PressedButtonIndex = 2;

		if (context.action == playerInputs.Player.Press4)
			m_PressedButtonIndex = 3;

		if (context.action == playerInputs.Player.Press5)
			m_PressedButtonIndex = 4;

		if (context.action == playerInputs.Player.Press6)
			m_PressedButtonIndex = 5;

		InputEventManager.BarIndexPressed.Invoke(m_PressedButtonIndex);
	}
	private void MouseWheel(InputAction.CallbackContext context)
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		InputEventManager.ChangeCameraZoomValue.Invoke(context.ReadValue<float>());
	}
}
