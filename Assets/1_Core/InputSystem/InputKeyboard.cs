using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputKeyboard : IInputDevice
{
	private GameController gameController;
	public InputKeyboard(GameController gameController)
	{
		this.gameController = gameController;
		_keyPauseMenu = KeyCode.Alpha1;
		Debug.Log("InputKeyboard Initialized");
	}
	

	private float lastPressTime = 0f;
	private bool isKeyInteractBeingHeld = false;

	private bool isRightHandWeaponWheelOpened = false;
	private bool isLeftHandWeaponWheelOpened = false;

	private KeyCode _keyPauseMenu;

	private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>()
	{
		{"MoveForward", KeyCode.W},
		{"MoveBackward", KeyCode.S},
		{"MoveRight", KeyCode.D},
		{"MoveLeft", KeyCode.A},
		{"Run", KeyCode.LeftShift},
		{"Jump", KeyCode.Space},
		{"Crouch", KeyCode.LeftControl},
		{"Interact", KeyCode.F},

		{"ChangeCameraView", KeyCode.V},
		{"ChangeCameraShoulder", KeyCode.C},
		{"RightHandWeaponWheel", KeyCode.E},
		{"LeftHandWeaponWheel", KeyCode.Q},
		{"RightHandWeaponAttack", KeyCode.Mouse1},
		{"LeftHandWeaponAttack", KeyCode.Mouse0},
		{"Reload", KeyCode.R},
		{"LegKick", KeyCode.Mouse2},
	};




	public IEnumerable<(string action, KeyCode key)> GetCurrentBindings()
	{
		return keyBindings.Select(kvp => (kvp.Key, kvp.Value));
	}

	public KeyCode GetBinding(string actionName)
	{
		return keyBindings.ContainsKey(actionName) ? keyBindings[actionName] : KeyCode.None;
	}

	public void RebindKey(string actionName, KeyCode newKey)
	{
		if (!keyBindings.ContainsKey(actionName))
			Debug.LogError($"Нет такого действия '{actionName}'.");
		else
			keyBindings[actionName] = newKey;
	}

	public bool GetKeyPauseMenu()
	{
		if (Input.GetKeyDown(_keyPauseMenu) && gameController.IsPauseMenuAvailable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]) && gameController.IsPlayerControllable)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveForward"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]) && gameController.IsPlayerControllable)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveBackward"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]) && gameController.IsPlayerControllable)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveRight"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]) && gameController.IsPlayerControllable)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveLeft"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(keyBindings["ChangeCameraView"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(keyBindings["ChangeCameraShoulder"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}


	public bool GetKeyHideWeapons()
	{
		if (!isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(keyBindings["Interact"]) && gameController.IsPlayerControllable)
			{
				lastPressTime = Time.time;
				//Debug.Log("1111");
				isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(keyBindings["Interact"]) && gameController.IsPlayerControllable) // отпущена кнопка
		{
			isKeyInteractBeingHeld = false;
			//Debug.Log("2222");
		}
		else if (isKeyInteractBeingHeld && Time.time >= lastPressTime + 0.5f) // проверяем реальный временной промежуток
		{
			isKeyInteractBeingHeld = false;
			//Debug.Log("3333");
			return true;
		}
		return false;
	}

	public bool GetKeyReload()
	{
		if (Input.GetKeyDown(keyBindings["Reload"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(keyBindings["Run"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(keyBindings["Jump"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(keyBindings["Jump"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(keyBindings["Crouch"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(keyBindings["LegKick"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyInteract()
	{
		if (isKeyInteractBeingHeld && Time.time > lastPressTime + 0.01f)
		{
			return false; // Игнорируем нажатие, если идёт задержка для HideWeapons
		}

		if (Input.GetKeyDown(keyBindings["Interact"]) && gameController.IsPlayerControllable)
		{
			isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public string GetNameOfKeyInteract()
	{
		return keyBindings["Interact"].ToString();
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.GetKey(keyBindings["RightHandWeaponWheel"]) && !isLeftHandWeaponWheelOpened && gameController.IsPlayerControllable)
		{
			isRightHandWeaponWheelOpened = true;
			//Debug.Log("RIGHT");
			return true;
		}
		else
		{
			isRightHandWeaponWheelOpened = false;
			return false;
		}
	}


	public bool GetKeyLeftHandWeaponWheel()
	{
		if (Input.GetKey(keyBindings["LeftHandWeaponWheel"]) && !isRightHandWeaponWheelOpened && gameController.IsPlayerControllable)
		{
			isLeftHandWeaponWheelOpened = true;
			//Debug.Log("LEFT");
			return true;
			
		}
		else
		{
			isLeftHandWeaponWheelOpened = false;
			return false;
		}
	}

	public bool GetKeyRightHandWeaponAttack()
	{
		if (Input.GetKeyDown(keyBindings["RightHandWeaponAttack"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(keyBindings["LeftHandWeaponAttack"]) && gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public string GetNameOfKeyRightHandWeaponAttack()
	{
		return keyBindings["RightHandWeaponAttack"].ToString();
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return keyBindings["LeftHandWeaponAttack"].ToString();
	}
}

