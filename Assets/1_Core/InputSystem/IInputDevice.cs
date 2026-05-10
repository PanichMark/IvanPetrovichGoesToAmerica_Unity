using System.Collections.Generic;
using UnityEngine;

public interface IInputDevice
{
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
	string GetNameOfKeyInteract();
	string GetNameOfKeyRightHandWeaponAttack();
	string GetNameOfKeyLeftHandWeaponAttack();

	IEnumerable<(string action, KeyCode key)> GetCurrentKeyBindings();
	IReadOnlyDictionary<string, KeyCode> CurrentKeyboardKeyBindings { get; }
	IReadOnlyDictionary<string, KeyCode> GetDefaultKeyBindings();
	void RebindKey(string actionName, KeyCode newKey);
}