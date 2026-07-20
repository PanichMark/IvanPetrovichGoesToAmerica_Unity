using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public InputGamepad(GameController gameController)
	{
		_gameController = gameController;
		_keyPauseMenu = KeyCode.JoystickButton7;
		_initialGamepadBindings = new Dictionary<string, string>(_gamepadBindings);

		Debug.Log("InputKeyboard Initialized");
	}

	private GameController _gameController;

	private readonly Dictionary<string, string> _initialGamepadBindings;

	private bool _isKeyInteractBeingHeld;
	private bool _isKeySkipCutsceneBeingHeld;

	private float _lastTimeSinceKeyHideWeaponWasHeld;
	private float _lastTimeSinceKeySkipCutsceneWasHeld;

	private float _timeToHoldKeyHideWeapon = 0.5f;
	private float _timeToHoldKeySkipCutscene = 1;

	private bool _isRightHandWeaponWheelOpened;
	private bool _isLeftHandWeaponWheelOpened;

	private KeyCode _keyPauseMenu;

	private Dictionary<string, string> _gamepadBindings = new Dictionary<string, string>()
	{
		//????
		{"MoveForward", "joystick button 2"},
		{"MoveBackward", "joystick button 2"},
		{"MoveRight", "joystick button 2"},
		{"MoveLeft", "joystick button 2"},
		//????

		//OK
		{"Run", "joystick button 8"}, //??? Кнопка левого джойстика

		//OK
		{"Jump", "joystick button 0"}, // A
		//OK
		{"Crouch", "joystick button 1"}, // B
		//OK
		{"Interact", "joystick button 3"}, // Y

		//?????
		{"ChangeCameraView", "joystick button 10"},
		{"ChangeCameraShoulder", "joystick button 10"},
		//??????

		//OK
		{"RightHandWeaponWheel", "joystick button 5"}, // R1
		//OK
		{"LeftHandWeaponWheel", "joystick button 4"}, // L1

		//???????
		{"RightHandWeaponAttack", "joystick button 10"}, // R2
		{"LeftHandWeaponAttack", "joystick button 10"}, // L2
		//???????

		//OK
		{"Reload", "joystick button 2"}, // X
		//OK
		{"LegKick", "joystick button 9"} // L3


		/*
		//????
		{"MoveForward", "Vertical"},
		{"MoveBackward", "Vertical"},
		{"MoveRight", "Horizontal"},
		{"MoveLeft", "Horizontal"},
		//????

		{"Run", "joystick button 4"}, //??? Кнопка левого джойстика

		{"Jump", "joystick button 14"}, // A
		{"Crouch", "joystick button 13"}, // B
		{"Interact", "joystick button 12"}, // Y

		{"ChangeCameraView", "joystick button 4"},
		{"ChangeCameraShoulder", "joystick button 5"},

		{"RightHandWeaponWheel", "joystick button 9"}, // R1
		{"LeftHandWeaponWheel", "joystick button 8"}, // L1

		{"RightHandWeaponAttack", "joystick button 11"}, // R2
		{"LeftHandWeaponAttack", "joystick button 10 "}, // L2

		{"Reload", "joystick button 15"}, // X
		{"LegKick", "joystick button 2"} //??? Кнопка правого джойстика
		*/
	};

	public IReadOnlyDictionary<string, KeyCode> CurrentKeyboardKeyBindings
	{
		get
		{
			return new ReadOnlyDictionary<string, KeyCode>(bruh);
		}
	}

	public IReadOnlyDictionary<string, KeyCode> GetDefaultKeyBindings()
	{
		return new ReadOnlyDictionary<string, KeyCode>(bruh);
	}

	private Dictionary<string, KeyCode> bruh = new Dictionary<string, KeyCode>()
	{
		{"MoveForward", KeyCode.A},
	};

	public IEnumerable<(string action, KeyCode key)> GetCurrentKeyBindings()
	{
		return bruh.Select(kvp => (kvp.Key, kvp.Value));
	}

	public KeyCode GetBinding(string actionName)
	{
		return _gamepadBindings.ContainsKey(actionName) ? bruh[actionName] : KeyCode.None;
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
		if (Input.GetKey(_gamepadBindings["MoveForward"]) &&
			Input.GetKey(_gamepadBindings["MoveBackward"]))
		{
			return false;
		}
		else if (Input.GetKey(_gamepadBindings["MoveForward"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(_gamepadBindings["MoveForward"]) &&
			Input.GetKey(_gamepadBindings["MoveBackward"]))
		{
			return false;
		}
		else if (Input.GetKey(_gamepadBindings["MoveBackward"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(_gamepadBindings["MoveRight"]) &&
			Input.GetKey(_gamepadBindings["MoveLeft"]))
		{
			return false;
		}
		else if (Input.GetKey(_gamepadBindings["MoveRight"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(_gamepadBindings["MoveRight"]) &&
			Input.GetKey(_gamepadBindings["MoveLeft"]))
		{
			return false;
		}
		else if (Input.GetKey(_gamepadBindings["MoveLeft"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(_gamepadBindings["ChangeCameraView"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(_gamepadBindings["ChangeCameraShoulder"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyHideWeapons()
	{
		if (!_isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(_gamepadBindings["Interact"]))
			{
				_lastTimeSinceKeyHideWeaponWasHeld = Time.time;
				_isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(_gamepadBindings["Interact"]))
		{
			_isKeyInteractBeingHeld = false;
		}
		else if (_isKeyInteractBeingHeld && Time.time >= _lastTimeSinceKeyHideWeaponWasHeld + _timeToHoldKeyHideWeapon)
		{
			_isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public bool GetKeyReload()
	{
		if (Input.GetKeyDown(_gamepadBindings["Reload"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(_gamepadBindings["Run"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(_gamepadBindings["Jump"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(_gamepadBindings["Jump"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(_gamepadBindings["Crouch"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(_gamepadBindings["LegKick"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyInteract()
	{
		if (_isKeyInteractBeingHeld && Time.time > _lastTimeSinceKeyHideWeaponWasHeld + 0.01f)
		{
			return false;
		}

		if (Input.GetKeyDown(_gamepadBindings["Interact"]))
		{
			return true;
		}
		return false;
	}

	public string GetNameOfKeyInteract()
	{
		return _gamepadBindings["Interact"].ToString();
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.GetKey(_gamepadBindings["RightHandWeaponWheel"]) && !_isLeftHandWeaponWheelOpened)
		{
			_isRightHandWeaponWheelOpened = true;
			return true;
		}
		else
		{
			_isRightHandWeaponWheelOpened = false;
			return false;
		}
	}


	public bool GetKeyLeftHandWeaponWheel()
	{
		if (Input.GetKey(_gamepadBindings["LeftHandWeaponWheel"]) && !_isRightHandWeaponWheelOpened)
		{
			_isLeftHandWeaponWheelOpened = true;
			return true;
		}
		else
		{
			_isLeftHandWeaponWheelOpened = false;
			return false;
		}
	}

	public bool GetKeyRightHandWeaponAttack()
	{
		if (Input.GetKeyDown(_gamepadBindings["RightHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(_gamepadBindings["LeftHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public string GetNameOfKeyRightHandWeaponAttack()
	{
		return _gamepadBindings["RightHandWeaponAttack"].ToString();
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return _gamepadBindings["LeftHandWeaponAttack"].ToString();
	}

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_gamepadBindings["RightHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}
	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_gamepadBindings["LeftHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}
	public bool GetKeySkipCutscene()
	{
		if (!_isKeySkipCutsceneBeingHeld)
		{
			if (Input.GetKeyDown(_gamepadBindings["Interact"]))
			{
				_lastTimeSinceKeySkipCutsceneWasHeld = Time.time;
				_isKeySkipCutsceneBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(_gamepadBindings["Interact"]))
		{
			_isKeySkipCutsceneBeingHeld = false;
		}
		else if (_isKeySkipCutsceneBeingHeld && Time.time >= _lastTimeSinceKeySkipCutsceneWasHeld + _timeToHoldKeySkipCutscene)
		{
			_isKeySkipCutsceneBeingHeld = false;
			return true;
		}
		return false;
	}

	public float CameraAxisX()
	{
		throw new System.NotImplementedException();
	}

	public float CameraAxisY()
	{
		throw new System.NotImplementedException();
	}

	public float CameraScroll()
	{
		throw new System.NotImplementedException();
	}
}