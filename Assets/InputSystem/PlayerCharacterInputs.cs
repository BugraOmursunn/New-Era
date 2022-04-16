using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterInputs : MonoBehaviour
{
	private PlayerInputs playerInputs;

	[Header("Character Input Values")]
	private InputAction m_move;
	private InputAction m_look;
	private InputAction m_jump;
	private InputAction m_sprint;
	
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool isSprintPressing;
	public bool sprint;
	
	[Header("Movement Settings")]
	public bool analogMovement;

	#if !UNITY_IOS || !UNITY_ANDROID
	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;
	#endif

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

		m_move = playerInputs.Player.Move;
		m_move.started += _ => OnMove(m_move.ReadValue<Vector2>());
		m_move.performed += _ => OnMove(m_move.ReadValue<Vector2>());
		m_move.canceled += _ => OnMove(m_move.ReadValue<Vector2>());
		m_move.Enable();

		m_look = playerInputs.Player.Look;
		m_look.started += _ => OnLook(m_look.ReadValue<Vector2>());
		m_look.performed += _ => OnLook(m_look.ReadValue<Vector2>());
		m_look.canceled += _ => OnLook(m_look.ReadValue<Vector2>());
		m_look.Enable();
		
		m_jump = playerInputs.Player.Jump;
		m_jump.performed += _ => OnJump(m_jump.IsPressed());
		m_jump.Enable();
		
		m_sprint = playerInputs.Player.Sprint;
		m_sprint.started += _ => OnSprint(m_sprint.IsPressed());
		m_sprint.performed += _ => OnSprint(m_sprint.IsPressed());
		m_sprint.canceled += _ => OnSprint(m_sprint.IsPressed());
		m_sprint.Enable();
		
		
	}

	private void OnMove(Vector2 value)
	{
		MoveInput(value);
		
		if (isSprintPressing == true && move.y > 0)
		{
			SprintInput(true);
		}
		else
		{
			SprintInput(false);
		}
	}

	private void OnLook(Vector2 value)
	{
		if (cursorInputForLook)
		{
			LookInput(value);
		}
	}

	private void OnJump(bool value)
	{
		JumpInput(value);
	}

	private void OnSprint(bool value)
	{
		isSprintPressing = value;
		
		if (isSprintPressing == true && move.y > 0)
		{
			SprintInput(true);
		}
		else
		{
			SprintInput(false);
		}
	}


	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	#if !UNITY_IOS || !UNITY_ANDROID
	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}
	
	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
	#endif
}
