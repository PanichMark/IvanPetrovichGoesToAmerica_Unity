using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class InputKeyboard : IInputDevice
{
	private readonly Dictionary<InputControlsEnum, KeyCode> _initialKeyboardKeyBindings;

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
		_initialKeyboardKeyBindings = new Dictionary<InputControlsEnum, KeyCode>(_keyboardKeyBindings);

		Debug.Log("InputKeyboard Initialized");
	}

	private Dictionary<InputControlsEnum, KeyCode> _keyboardKeyBindings = new Dictionary<InputControlsEnum, KeyCode>()
	{
		{ InputControlsEnum.MoveForward, KeyCode.W },
		{ InputControlsEnum.MoveBackward, KeyCode.S },
		{ InputControlsEnum.MoveRight, KeyCode.D },
		{ InputControlsEnum.MoveLeft, KeyCode.A },
		{ InputControlsEnum.Run, KeyCode.LeftShift },
		{ InputControlsEnum.Jump, KeyCode.Space },
		{ InputControlsEnum.Crouch, KeyCode.LeftControl },
		{ InputControlsEnum.Interact, KeyCode.F },
		{ InputControlsEnum.ChangeCameraView, KeyCode.V },
		{ InputControlsEnum.ChangeCameraShoulder, KeyCode.C },
		{ InputControlsEnum.WeaponWheelRightHand, KeyCode.E },
		{ InputControlsEnum.WeaponWheelLeftHand, KeyCode.Q },
		{ InputControlsEnum.WeaponAttackRightHand, KeyCode.Mouse1 },
		{ InputControlsEnum.WeaponAttackLeftHand, KeyCode.Mouse0 },
		{ InputControlsEnum.WeaponReload, KeyCode.R },
		{ InputControlsEnum.LegKick, KeyCode.Mouse2 }
	};

	public IReadOnlyDictionary<InputControlsEnum, KeyCode> CurrentKeyboardKeyBindings
	{
		get
		{
			return new ReadOnlyDictionary<InputControlsEnum, KeyCode>(_keyboardKeyBindings);
		}
	}

	public IReadOnlyDictionary<InputControlsEnum, KeyCode> GetDefaultKeyBindings()
	{
		var copyOfInitialBindings = new Dictionary<InputControlsEnum, KeyCode>(_initialKeyboardKeyBindings);
		return new ReadOnlyDictionary<InputControlsEnum, KeyCode>(copyOfInitialBindings);
	}

	public IEnumerable<(InputControlsEnum action, KeyCode key)> GetCurrentKeyBindings()
	{
		return _keyboardKeyBindings.Select(kvp => (kvp.Key, kvp.Value));
	}

	public void RebindKey(InputControlsEnum actionName, KeyCode newKey)
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
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveForward]) &&
			Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveForward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveForward]) &&
			Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveRight]) &&
			Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveRight]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveRight]) &&
			Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.ChangeCameraView]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.ChangeCameraShoulder]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyHideWeapons()
	{
		if (!_isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable)
			{
				_lastTimeSinceKeyHideWeaponWasHeld = Time.time;
			
				_isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable) 
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
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.WeaponReload]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.Run]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.Jump]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.Jump]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.Crouch]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.LegKick]) && _gameController.IsPlayerControllable && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
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

		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable)
		{
			_isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}


	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.WeaponWheelRightHand]) && !_isLeftHandWeaponWheelOpened && _gameController.IsPlayerControllable)
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
		if (Input.GetKey(_keyboardKeyBindings[InputControlsEnum.WeaponWheelLeftHand]) && !_isRightHandWeaponWheelOpened && _gameController.IsPlayerControllable)
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
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.WeaponAttackRightHand]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(_keyboardKeyBindings[InputControlsEnum.WeaponAttackLeftHand]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_keyboardKeyBindings[InputControlsEnum.WeaponAttackRightHand]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(_keyboardKeyBindings[InputControlsEnum.WeaponAttackLeftHand]) && _gameController.IsPlayerControllable)
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

	public string GetNameOfKey(InputControlsEnum actionName)
	{
		if (_keyboardKeyBindings.TryGetValue(actionName, out KeyCode key))
		{
			return key.ToString();
		}

		Debug.LogWarning($"[InputKeyboard] Не найдено действие '{actionName}' для получения имени клавиши.");
		return "?";
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