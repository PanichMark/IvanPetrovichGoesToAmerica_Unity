using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class InputKeyboard : IInputDevice
{
	private readonly Dictionary<string, KeyCode> _initialKeyboardKeyBindings;

	private GameController _gameController;

	private float _lastTimeSinceKeyHideWeaponWasHeld;
	private float _lastTimeSinceKeySkipCutsceneWasHeld;

	private float _timeToHoldKeyHideWeapon = 0.5f;
	private float _timeToHoldKeySkipCutscene = 1;

	private bool _isKeyInteractBeingHeld;
	private bool _isKeySkipCutsceneBeingHeld;

	private bool _isRightHandWeaponWheelOpened;
	private bool _isLeftHandWeaponWheelOpened;

	private KeyCode _keyPauseMenu;

	public InputKeyboard(GameController gameController, KeyCode keyPauseMenu)
	{
		_gameController = gameController;
		_keyPauseMenu = keyPauseMenu;
		_initialKeyboardKeyBindings = new Dictionary<string, KeyCode>(_keyboardKeyBindings);

		Debug.Log("InputKeyboard Initialized");
	}

	private Dictionary<string, KeyCode> _keyboardKeyBindings = new Dictionary<string, KeyCode>()
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
		{"WeaponWheelRightHand", KeyCode.E},
		{"WeaponWheelLeftHand", KeyCode.Q},
		{"WeaponAttackRightHand", KeyCode.Mouse1},
		{"WeaponAttackLeftHand", KeyCode.Mouse0},
		{"WeaponReload", KeyCode.R},
		{"LegKick", KeyCode.Mouse2},
	};

	public IReadOnlyDictionary<string, KeyCode> CurrentKeyboardKeyBindings
	{
		get
		{
			return new ReadOnlyDictionary<string, KeyCode>(_keyboardKeyBindings);
		}
	}

	public IReadOnlyDictionary<string, KeyCode> GetDefaultKeyBindings()
	{
		var copyOfInitialBindings = new Dictionary<string, KeyCode>(_initialKeyboardKeyBindings);
		return new ReadOnlyDictionary<string, KeyCode>(copyOfInitialBindings);
	}

	public IEnumerable<(string action, KeyCode key)> GetCurrentKeyBindings()
	{
		return _keyboardKeyBindings.Select(kvp => (kvp.Key, kvp.Value));
	}

	public void RebindKey(string actionName, KeyCode newKey)
	{
		if (!_keyboardKeyBindings.ContainsKey(actionName))
			Debug.LogError($"Нет такого действия '{actionName}'.");
		else
			_keyboardKeyBindings[actionName] = newKey;
	}

	public bool GetKeyPauseMenu()
	{
		if (Input.GetKeyDown(_keyPauseMenu) && _gameController.IsPauseMenuAvailable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (Input.GetKey(_keyboardKeyBindings["MoveForward"]) &&
			Input.GetKey(_keyboardKeyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings["MoveForward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(_keyboardKeyBindings["MoveForward"]) &&
			Input.GetKey(_keyboardKeyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(_keyboardKeyBindings["MoveRight"]) &&
			Input.GetKey(_keyboardKeyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings["MoveRight"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(_keyboardKeyBindings["MoveRight"]) &&
			Input.GetKey(_keyboardKeyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["ChangeCameraView"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["ChangeCameraShoulder"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyHideWeapons()
	{
		if (!_isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(_keyboardKeyBindings["Interact"]) && _gameController.IsPlayerControllable)
			{
				_lastTimeSinceKeyHideWeaponWasHeld = Time.time;
			
				_isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(_keyboardKeyBindings["Interact"]) && _gameController.IsPlayerControllable) 
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
		if (Input.GetKeyDown(_keyboardKeyBindings["WeaponReload"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(_keyboardKeyBindings["Run"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["Jump"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(_keyboardKeyBindings["Jump"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["Crouch"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["LegKick"]) && _gameController.IsPlayerControllable)
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

		if (Input.GetKeyDown(_keyboardKeyBindings["Interact"]) && _gameController.IsPlayerControllable)
		{
			_isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public string GetNameOfKeyInteract()
	{
		return _keyboardKeyBindings["Interact"].ToString();
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.GetKey(_keyboardKeyBindings["WeaponWheelRightHand"]) && !_isLeftHandWeaponWheelOpened && _gameController.IsPlayerControllable)
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
		if (Input.GetKey(_keyboardKeyBindings["WeaponWheelLeftHand"]) && !_isRightHandWeaponWheelOpened && _gameController.IsPlayerControllable)
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
		if (Input.GetKeyDown(_keyboardKeyBindings["WeaponAttackRightHand"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings["WeaponAttackLeftHand"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public string GetNameOfKeyRightHandWeaponAttack()
	{
		return _keyboardKeyBindings["WeaponAttackRightHand"].ToString();
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return _keyboardKeyBindings["WeaponAttackLeftHand"].ToString();
	}

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_keyboardKeyBindings["WeaponAttackRightHand"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_keyboardKeyBindings["WeaponAttackLeftHand"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeySkipCutscene()
	{
		if (!_isKeySkipCutsceneBeingHeld)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				_lastTimeSinceKeySkipCutsceneWasHeld = Time.unscaledTime;
	
				_isKeySkipCutsceneBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(KeyCode.Space)) 
		{
			_isKeySkipCutsceneBeingHeld = false;
			
		}
		else if (_isKeySkipCutsceneBeingHeld && Time.unscaledTime >= _lastTimeSinceKeySkipCutsceneWasHeld + _timeToHoldKeySkipCutscene)
		{
			_isKeySkipCutsceneBeingHeld = false;
		
			return true;
		}
		return false;
	}

	public float CameraAxisX()
	{
		if (_gameController.IsPlayerControllable)
		{
			return Input.GetAxis("Mouse X");
		}
		else
		{
			return 0;
		}
	}

	public float CameraAxisY()
	{
		if (_gameController.IsPlayerControllable)
		{
			return Input.GetAxis("Mouse Y");
		}
		else
		{
			return 0;
		}
	}

	public float CameraScroll()
	{
		if (_gameController.IsPlayerControllable)
		{
			return Input.mouseScrollDelta.y;
		}
		else
		{
			return 0;
		}
	}
}