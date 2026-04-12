using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// 
/// 
/// НЕ РАБОТАЕТ СЛОВАРЬ
/// ПОНЯТЬ КАКИЕ КНОПКИ КОНТРОЛЛЕРА OLD INPUT SYSTEM!!!!!!!!!
/// 
/// 
/// </summary>
public class InputGamepad : IInputDevice
{
	public InputGamepad()
	{
		_keyPauseMenu = KeyCode.Alpha1;
		Debug.Log("InputController Initialized");
	}


	private float lastPressTime = 0f;
	private bool isKeyInteractBeingHeld = false;

	private bool isRightHandWeaponWheelOpened = false;
	private bool isLeftHandWeaponWheelOpened = false;

	private KeyCode _keyPauseMenu;

	private Dictionary<string, string> controllerBindings = new Dictionary<string, string>()
{
	{"MoveForward", "Vertical"},             // Джойстик вперёд (positive)
    {"MoveBackward", "Vertical"},            // Джойстик назад (negative)
    {"MoveRight", "Horizontal"},             // Джойстик вправо (positive)
    {"MoveLeft", "Horizontal"},              // Джойстик влево (negative)
    {"Run", "Y"},                           // RB на Xbox или R2 на PlayStation
    {"Jump", "A"},                           // A на Xbox или X на PlayStation
    {"Crouch", "B"},                         // B на Xbox или O на PlayStation
    {"Interact", "Y"},                       // A на Xbox или X на PlayStation
    {"ChangeCameraView", "Y"},              // LB на Xbox или L1 на PlayStation
    {"ChangeCameraShoulder", "Y"},          // RB на Xbox или R1 на PlayStation
    {"RightHandWeaponWheel", "Y"},          // RT на Xbox или R2 на PlayStation
    {"LeftHandWeaponWheel", "Y"},           // LT на Xbox или L2 на PlayStation
    {"RightHandWeaponAttack", "Y"},         // RT на Xbox или R2 на PlayStation
    {"LeftHandWeaponAttack", "Y"},          // LT на Xbox или L2 на PlayStation
    {"Reload", "Y"},                         // Y на Xbox или Triangle на PlayStation
    {"LegKick", "Y"}                         // X на Xbox или Square на PlayStation
};


	private Dictionary<string, KeyCode> bruh = new Dictionary<string, KeyCode>()
	{
		{"MoveForward", KeyCode.A},
	};

	public IEnumerable<(string action, KeyCode key)> GetCurrentBindings()
	{
		return bruh.Select(kvp => (kvp.Key, kvp.Value));
	}

	public KeyCode GetBinding(string actionName)
	{
		return controllerBindings.ContainsKey(actionName) ? bruh[actionName] : KeyCode.None;
	}

	public void RebindKey(string actionName, KeyCode newKey)
	{
		if (!bruh.ContainsKey(actionName))
			Debug.LogError($"Нет такого действия '{actionName}'.");
		else
			bruh[actionName] = newKey;
	}







	public bool GetKeyPauseMenu()
	{
		if (Input.GetKeyDown(_keyPauseMenu))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (Input.GetKey(controllerBindings["MoveForward"]) &&
			Input.GetKey(controllerBindings["MoveBackward"]))
		{
			return false;
		}
		else if (Input.GetKey(controllerBindings["MoveForward"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(controllerBindings["MoveForward"]) &&
			Input.GetKey(controllerBindings["MoveBackward"]))
		{
			return false;
		}
		else if (Input.GetKey(controllerBindings["MoveBackward"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(controllerBindings["MoveRight"]) &&
			Input.GetKey(controllerBindings["MoveLeft"]))
		{
			return false;
		}
		else if (Input.GetKey(controllerBindings["MoveRight"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(controllerBindings["MoveRight"]) &&
			Input.GetKey(controllerBindings["MoveLeft"]))
		{
			return false;
		}
		else if (Input.GetKey(controllerBindings["MoveLeft"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(controllerBindings["ChangeCameraView"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(controllerBindings["ChangeCameraShoulder"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyEnterCutscene()
	{
		if (Input.GetKeyDown(controllerBindings["EnterCutscene"]) &&
			false /* вероятно, сюда должна подставляться дополнительная переменная или условие */)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyHideWeapons()
	{
		if (!isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(controllerBindings["Interact"]))
			{
				lastPressTime = Time.time;
				isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(controllerBindings["Interact"])) // отпущена кнопка
		{
			isKeyInteractBeingHeld = false;
		}
		else if (isKeyInteractBeingHeld && Time.time >= lastPressTime + 0.5f) // удержано дольше полсекунды
		{
			isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public bool GetKeyReload()
	{
		if (Input.GetKeyDown(controllerBindings["Reload"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(controllerBindings["Run"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(controllerBindings["Jump"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(controllerBindings["Jump"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(controllerBindings["Crouch"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(controllerBindings["LegKick"]))
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

		if (Input.GetKeyDown(controllerBindings["Interact"]))
		{
			return true;
		}
		return false;
	}

	public string GetNameOfKeyInteract()
	{
		return controllerBindings["Interact"].ToString();
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.GetKey(controllerBindings["RightHandWeaponWheel"]) && !isLeftHandWeaponWheelOpened)
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
		if (Input.GetKey(controllerBindings["LeftHandWeaponWheel"]) && !isRightHandWeaponWheelOpened)
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
		if (Input.GetKeyDown(controllerBindings["RightHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(controllerBindings["LeftHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public string GetNameOfKeyRightHandWeaponAttack()
	{
		return controllerBindings["RightHandWeaponAttack"].ToString();
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return controllerBindings["LeftHandWeaponAttack"].ToString();
	}

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(controllerBindings["RightHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}
	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(controllerBindings["LeftHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}
}

