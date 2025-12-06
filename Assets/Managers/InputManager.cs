using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public PlayerMovementController playerMovementController;

	public static InputManager Instance { get; private set; }

	private float lastPressTime = 0f;
	private bool isKeyInteractBeingHeld = false;

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

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		_keyPauseMenu = KeyCode.Alpha1;
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
		if (Input.GetKeyDown(_keyPauseMenu))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]))
		{
			return false;
		}
		else if (MenuManager.IsPlayerControllable &&
				 Input.GetKey(keyBindings["MoveForward"]) &&
				 playerMovementController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["MoveForward"]) &&
			Input.GetKey(keyBindings["MoveBackward"]))
		{
			return false;
		}
		else if (MenuManager.IsPlayerControllable &&
				 Input.GetKey(keyBindings["MoveBackward"]) &&
				 playerMovementController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]))
		{
			return false;
		}
		else if (MenuManager.IsPlayerControllable &&
				 Input.GetKey(keyBindings["MoveRight"]) &&
				 playerMovementController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["MoveRight"]) &&
			Input.GetKey(keyBindings["MoveLeft"]))
		{
			return false;
		}
		else if (MenuManager.IsPlayerControllable &&
				 Input.GetKey(keyBindings["MoveLeft"]) &&
				 playerMovementController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["ChangeCameraView"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["ChangeCameraShoulder"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyEnterCutscene()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["EnterCutscene"]) &&
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
			if (MenuManager.IsPlayerControllable &&
				Input.GetKeyDown(keyBindings["Interact"]) &&
				!MenuManager.IsWeaponWheelMenuOpened)
			{
				lastPressTime = Time.time;
				isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(keyBindings["Interact"])) // отпущена кнопка
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
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["Reload"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["Run"]) &&
			playerMovementController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["Jump"]) &&
			playerMovementController.IsPlayerGrounded &&
			playerMovementController.IsPlayerAbleToMove &&
			playerMovementController.IsPlayerAbleToStandUp)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["Jump"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["Crouch"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["LegKick"]) &&
			playerMovementController.IsPlayerGrounded &&
			!playerMovementController.IsPLayerSliding &&
			playerMovementController.CurrentPlayerMovementStateType != "PlayerLedgeClimbing")
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

		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["Interact"]))
		{
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
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["RightHandWeaponWheel"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponWheel()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKey(keyBindings["LeftHandWeaponWheel"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRightHandWeaponAttack()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["RightHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (MenuManager.IsPlayerControllable &&
			Input.GetKeyDown(keyBindings["LeftHandWeaponAttack"]))
		{
			return true;
		}
		else return false;
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return keyBindings["LeftHandWeaponAttack"].ToString();
	}
}