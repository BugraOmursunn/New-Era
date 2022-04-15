using UnityEngine;
using UnityEngine.InputSystem;

public static class InputInitializer
{
	public static PlayerInput Initialize(this PlayerInput input)
	{
		var instance = PlayerInput.Instantiate(input.gameObject, controlScheme: "KeyboardMouse", pairWithDevices: new InputDevice[] { Keyboard.current, Mouse.current });

		instance.transform.parent = input.transform.parent;
		instance.transform.position = input.transform.position;

		//Object.Destroy(input.gameObject);
		return instance;
	}
}
