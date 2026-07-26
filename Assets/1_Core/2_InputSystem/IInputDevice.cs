using System.Collections.Generic;
using UnityEngine;

public interface IInputDevice
{
	float CameraAxisX();
	float CameraAxisY();
	float CameraScroll();
	bool GetKeyPauseMenu();
	bool GetKeyUp();
	bool GetKeyDown();
	bool GetKeyRight();
	bool GetKeyLeft();
	bool GetKeyChangeCameraView();
	bool GetKeyChangeCameraShoulder();
	bool GetKeyHideWeapons();
	bool GetKeyReload();
	bool GetKeyRun();
	bool GetKeyJump();
	bool GetKeyJumpBeingHeld();
	bool GetKeyCrouch();
	bool GetKeyLegKick();
	bool GetKeyInteract();
	bool GetKeySkipCutscene();
	bool GetKeyRightHandWeaponWheel();
	bool GetKeyLeftHandWeaponWheel();
	bool GetKeyRightHandWeaponAttack();
	bool GetKeyLeftHandWeaponAttack();
	bool GetKeyRightHandWeaponAttackReleased();
	bool GetKeyLeftHandWeaponAttackReleased();
	string GetNameOfKey(InputControlsEnum actionName);

	IEnumerable<(InputControlsEnum action, KeyCode key)> GetCurrentKeyBindings();
	IReadOnlyDictionary<InputControlsEnum, KeyCode> CurrentKeyboardKeyBindings { get; }
	IReadOnlyDictionary<InputControlsEnum, KeyCode> GetDefaultKeyBindings();
	void RebindKey(InputControlsEnum actionName, KeyCode newKey);
}