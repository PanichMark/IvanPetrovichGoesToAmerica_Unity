using UnityEngine;

public interface IWeaponWheelMenuController
{
	void CreateWheel();

	void RecreateWheel();

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed);

	void OnWeaponUnlocked(GameObject weaponPrefab);

	void ShowWeaponName();

	void RestrictWeaponWheelWhilePickable();
	void UnrestrictWeaponWheelWhilePickable();


	void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerInteractionController playerInteractionController,
		PlayerWeaponAmmoController playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		GameObject weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel,
		GameObject PlayerCamera);
}
