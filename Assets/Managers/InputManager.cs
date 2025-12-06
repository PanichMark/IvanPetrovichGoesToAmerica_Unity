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
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["MoveForward"]);
	}

	public bool GetKeyDown()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["MoveBackward"]);
	}

	public bool GetKeyRight()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["MoveRight"]);
	}

	public bool GetKeyLeft()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["MoveLeft"]);
	}

	public bool GetKeyChangeCameraView()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["ChangeCameraView"]);
	}

	public bool GetKeyChangeCameraShoulder()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["ChangeCameraShoulder"]);
	}

	public bool GetKeyReload()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["Reload"]);
	}

	public bool GetKeyRun()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["Run"]) && playerMovementController.IsPlayerAbleToMove;
	}

	public bool GetKeyJump()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["Jump"])
			   && playerMovementController.IsPlayerGrounded && playerMovementController.IsPlayerAbleToMove
			   && playerMovementController.IsPlayerAbleToStandUp;
	}

	public bool GetKeyJumpBeingHeld()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["Jump"]);
	}

	public bool GetKeyCrouch()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["Crouch"]);
	}

	public bool GetKeyLegKick()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["LegKick"])
			   && playerMovementController.IsPlayerGrounded && !playerMovementController.IsPLayerSliding
			   && playerMovementController.CurrentPlayerMovementStateType != "PlayerLedgeClimbing";
	}

	public bool GetKeyHideWeapons()
	{
		if (!isKeyInteractBeingHeld)
		{
			if (MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["Interact"]) && !MenuManager.IsWeaponWheelMenuOpened)
			{
				lastPressTime = Time.time;
				isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.GetKeyUp(keyBindings["Interact"]))
		{
			isKeyInteractBeingHeld = false;
		}
		else if (isKeyInteractBeingHeld && Time.time >= lastPressTime + 0.5f)
		{
			isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public bool GetKeyInteract()
	{
		if (isKeyInteractBeingHeld && Time.time > lastPressTime + 0.01f)
			return false;

		if (MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["Interact"]))
			return true;

		return false;
	}

	public string GetNameOfKeyInteract()
	{
		return keyBindings["Interact"].ToString();
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["RightHandWeaponWheel"]);
	}

	public bool GetKeyLeftHandWeaponWheel()
	{
		return MenuManager.IsPlayerControllable && Input.GetKey(keyBindings["LeftHandWeaponWheel"]);
	}

	public bool GetKeyRightHandWeaponAttack()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["RightHandWeaponAttack"]);
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		return MenuManager.IsPlayerControllable && Input.GetKeyDown(keyBindings["LeftHandWeaponAttack"]);
	}

	public string GetNameOfKeyLeftHandWeaponAttack()
	{
		return keyBindings["LeftHandWeaponAttack"].ToString();
	}
}