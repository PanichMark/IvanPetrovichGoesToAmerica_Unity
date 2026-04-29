using System.Collections.Generic;
using UnityEngine;

public interface IInputDevice
{
	
	// Получение состояния клавиш
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
	bool GetKeyRightHandWeaponWheel();
	bool GetKeyLeftHandWeaponWheel();
	bool GetKeyRightHandWeaponAttack();
	bool GetKeyLeftHandWeaponAttack();
	bool GetKeyRightHandWeaponAttackReleased();
	bool GetKeyLeftHandWeaponAttackReleased();


	// Информация о ключах
	string GetNameOfKeyInteract();
	string GetNameOfKeyRightHandWeaponAttack();

	string GetNameOfKeyLeftHandWeaponAttack();

	// Дополнительные полезные методы
	IEnumerable<(string action, KeyCode key)> GetCurrentBindings();
	IReadOnlyDictionary<string, KeyCode> CurrentBindings { get; }

	IReadOnlyDictionary<string, KeyCode> GetDefaultBindings();
	KeyCode GetBinding(string actionName);
	void RebindKey(string actionName, KeyCode newKey);
}

