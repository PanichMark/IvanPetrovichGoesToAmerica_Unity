using UnityEngine;

public interface IWeaponWheelMenuController
{
	void CreateWheel();

	void RecreateWheel();

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed);

	void OnWeaponUnlocked(GameObject weaponPrefab);

	void ShowWeaponName();

	void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		GameObject weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel,
		GameObject PlayerCamera);
}
