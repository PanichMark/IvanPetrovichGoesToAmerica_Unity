using UnityEngine;

[CreateAssetMenu(fileName = "GameCanvasesList", menuName = "Configs/GameData/GameCanvasesList")]
public class GameCanvasesList : ScriptableObject
{
	[Header("Bootstrap")]
	public GameObject CanvasBootstrapInitialization;
	public GameObject CanvasBootstrapChooseFirstLanguage;
	public GameObject CanvasBootstrapSignTermsAndConditions;

	[Header("Loading Screen")]
	public GameObject CanvasSceneLoadingScreen;

	[Header("Menu")]
	public GameObject CanvasMenuBackground;

	[Header("Pause Menu")]
	public GameObject CanvasPauseMenu;
	public GameObject CanvasPauseSubMenuSave;
	public GameObject CanvasPauseSubMenuLoad;
	public GameObject CanvasPauseSubMenuAppearance;
	public GameObject CanvasPauseSubMenuTutorial;
	public GameObject CanvasPauseSubMenuSettings;
	public GameObject CanvasPauseSubMenuSettingsGameDifficulty;
	public GameObject CanvasPauseMenuConfirmAction;

	[Header("Main Menu")]
	public GameObject CanvasMainMenuChooseMission;
	public GameObject CanvasMainMenuReadNews;

	[Header("HUDs")]
	public GameObject CanvasHUDinteraction;
	public GameObject CanvasHUDmission;
	public GameObject CanvasHUDhealthAndMana;
	public GameObject CanvasHUDammo;

	[Header("Weapon Wheel Menu")]
	public GameObject CanvasMenuWeaponWheel;

	[Header("Interaction Menu")]
	public GameObject CanvasMenuNote;
	public GameObject CanvasMenuLockpickMechanical;
	public GameObject CanvasMenuLockpickElectronic;
	public GameObject CanvasMenuDialogue;
	public GameObject CanvasMenuCutscene;
}
