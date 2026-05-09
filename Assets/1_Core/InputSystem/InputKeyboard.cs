using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class InputKeyboard : IInputDevice
{
	private readonly Dictionary<string, KeyCode> _initialBindingsSnapshot;

	private GameController _gameController;

	public InputKeyboard(GameController gameController, KeyCode keyPauseMenu)
	{
		_gameController = gameController;
		_keyPauseMenu = keyPauseMenu;
		_initialBindingsSnapshot = new Dictionary<string, KeyCode>(keyBindings);
		Debug.Log("InputKeyboard Initialized");
	}
	
	private float _lastTimeSinceKeyHideWeaponsWasHeld = 0f;
	private float _lastTimeSinceKeySkipCutsceneWasHeld = 0f;
	private bool _isKeyInteractBeingHeld = false;
	private bool _isKeySkipCutsceneBeingHeld = false;

	private bool _isRightHandWeaponWheelOpened = false;
	private bool _isLeftHandWeaponWheelOpened = false;

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

	public IReadOnlyDictionary<string, KeyCode> CurrentBindings
	{
		get
		{
			// Возвращаем копию, обернутую в ReadOnlyDictionary.
			// Теперь внешний код может только читать (foreach, ContainsKey, TryGetValue),
			// но не может изменять значения.
			return new ReadOnlyDictionary<string, KeyCode>(keyBindings);
		}
	}
	// Реализация нового метода из интерфейса
	public IReadOnlyDictionary<string, KeyCode> GetDefaultBindings()
	{
		// Теперь мы берем копию из нашего "снимка", сделанного при инициализации.
		// Это гарантирует, что мы всегда получаем НАЧАЛЬНЫЕ значения.
		var copyOfInitialBindings = new Dictionary<string, KeyCode>(_initialBindingsSnapshot);
		return new ReadOnlyDictionary<string, KeyCode>(copyOfInitialBindings);
	}

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
		if (Input.GetKeyDown(_keyPauseMenu) && _gameController.IsPauseMenuAvailable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveForward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveBackward"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveRight"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.GetKey(keyBindings["MoveLeft"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.GetKeyDown(keyBindings["ChangeCameraView"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.GetKeyDown(keyBindings["ChangeCameraShoulder"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}


	public bool GetKeyHideWeapons()
	{
		if (!_isKeyInteractBeingHeld)
		{
			if (Input.GetKeyDown(keyBindings["Interact"]) && _gameController.IsPlayerControllable)
			{
				_lastTimeSinceKeyHideWeaponsWasHeld = Time.time;
				//Debug.Log("1111");
				_isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(keyBindings["Interact"]) && _gameController.IsPlayerControllable) // отпущена кнопка
		{
			_isKeyInteractBeingHeld = false;
			//Debug.Log("2222");
		}
		else if (_isKeyInteractBeingHeld && Time.time >= _lastTimeSinceKeyHideWeaponsWasHeld + 0.5f) // проверяем реальный временной промежуток
		{
			_isKeyInteractBeingHeld = false;
			//Debug.Log("3333");
			return true;
		}
		return false;
	}

	public bool GetKeyReload()
	{
		if (Input.GetKeyDown(keyBindings["Reload"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.GetKey(keyBindings["Run"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.GetKeyDown(keyBindings["Jump"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.GetKey(keyBindings["Jump"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.GetKeyDown(keyBindings["Crouch"]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.GetKeyDown(keyBindings["LegKick"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyInteract()
	{
		if (_isKeyInteractBeingHeld && Time.time > _lastTimeSinceKeyHideWeaponsWasHeld + 0.01f)
		{
			return false; // Игнорируем нажатие, если идёт задержка для HideWeapons
		}

		if (Input.GetKeyDown(keyBindings["Interact"]) && _gameController.IsPlayerControllable)
		{
			_isKeyInteractBeingHeld = false;
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
		if (Input.GetKey(keyBindings["RightHandWeaponWheel"]) && !_isLeftHandWeaponWheelOpened && _gameController.IsPlayerControllable)
		{
			_isRightHandWeaponWheelOpened = true;
			//Debug.Log("RIGHT");
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
		if (Input.GetKey(keyBindings["LeftHandWeaponWheel"]) && !_isRightHandWeaponWheelOpened && _gameController.IsPlayerControllable)
		{
			_isLeftHandWeaponWheelOpened = true;
			//Debug.Log("LEFT");
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
		if (Input.GetKeyDown(keyBindings["RightHandWeaponAttack"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.GetKeyDown(keyBindings["LeftHandWeaponAttack"]) && _gameController.IsPlayerControllable)
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

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(keyBindings["RightHandWeaponAttack"]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}
	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.GetKeyUp(keyBindings["LeftHandWeaponAttack"]) && _gameController.IsPlayerControllable)
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
				//Debug.Log("1111");
				_isKeySkipCutsceneBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(KeyCode.Space)) // отпущена кнопка
		{
			_isKeySkipCutsceneBeingHeld = false;
			//Debug.Log("2222");
		}
		else if (_isKeySkipCutsceneBeingHeld && Time.unscaledTime >= _lastTimeSinceKeySkipCutsceneWasHeld + 0.5f) // проверяем реальный временной промежуток
		{
			_isKeySkipCutsceneBeingHeld = false;
			//Debug.Log("3333");
			return true;
		}
		return false;
	}

}

