using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubSystemWeapon
{
	private GameObject weaponSystemGameObject;
	public PlayerWeaponController weaponController { get; private set; }
	private LegKickAttack legKickAttack;
	private WeaponAnimationController weaponAnimationController;
	private WeaponFirstPersonRender weaponFirstPersonRender;
	private GameObject firstPersonLeftHandWeaponSlotGameObject;
	private GameObject firstPersonRightHandWeaponSlotGameObject;
	private GameObject thirdPersonLeftHandWeaponSlotGameObject;
	private GameObject thirdPersonRightHandWeaponSlotGameObject;
	private WeaponWheelMenuController weaponWheelController;
	private GameObject canvasMenuWeaponWheel;
	private GameObject weaponWheelSegmentPrefab;
	private TextMeshProUGUI weaponText;
	private TextMeshProUGUI weaponWheelName;
	private Image weaponIconBig;
	private GameObject ChokeNPCtext;

	private BootstrapSubSystemScene bootstrapSubSystemScene;
	private GameController gameController;
	private BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems;
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;
	private IInputDevice inputDevice;
	private GameObject playerGameObject;
	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private BootstrapSubSystemInteraction bootstrapSubSystemInteraction;
	private CanvasHUDammoController canvasHUDammoController;
	private GameObject canvasHUDammo;
	private GameObject RightWeaponAmmoMagazine;
	private GameObject RightWeaponAmmoReserve;
	private GameObject RightWeaponAmmoSeparator;
	private GameObject LeftWeaponAmmoMagazine;
	private GameObject LeftWeaponAmmoReserve;
	private GameObject LeftWeaponAmmoSeparator;

	public BootstrapSubSystemWeapon(
		BootstrapSubSystemScene bootstrapSubSystemScene,
		GameController gameController,
		BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems,
		BootstrapSubSystemMenu bootstrapSubSystemMenu,
		IInputDevice inputDevice,
		GameObject playerGameObject,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		BootstrapSubSystemInteraction bootstrapSubSystemInteraction,
		CanvasHUDammoController canvasHUDammoController,
		GameObject canvasHUDammo,
		GameObject RightWeaponAmmoMagazine,
		GameObject RightWeaponAmmoReserve,
		GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine,
		GameObject LeftWeaponAmmoReserve,
		GameObject LeftWeaponAmmoSeparator,
		GameObject canvasMenuWeaponWheel)
	{
		this.bootstrapSubSystemScene = bootstrapSubSystemScene;
		this.gameController = gameController;
		this.bootstrapSubSystemPlayerSystems = bootstrapSubSystemPlayerSystems;
		this.bootstrapSubSystemMenu = bootstrapSubSystemMenu;
		this.inputDevice = inputDevice;
		this.playerGameObject = playerGameObject;
		this.playerResourcesAmmoManager = playerResourcesAmmoManager;
		this.bootstrapSubSystemInteraction = bootstrapSubSystemInteraction;
		this.canvasHUDammoController = canvasHUDammoController;
		this.canvasHUDammo = canvasHUDammo;

		this.RightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		this.RightWeaponAmmoReserve = RightWeaponAmmoReserve;
		this.RightWeaponAmmoSeparator = RightWeaponAmmoSeparator;

		this.LeftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		this.LeftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		this.LeftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;

		this.canvasMenuWeaponWheel = canvasMenuWeaponWheel;
	}

	public IEnumerator InitializeWeaponSystem()
	{
		weaponSystemGameObject = new GameObject("WeaponSystem");

		weaponController = weaponSystemGameObject.AddComponent<PlayerWeaponController>();
		legKickAttack = weaponSystemGameObject.AddComponent<LegKickAttack>();
		weaponWheelController = weaponSystemGameObject.AddComponent<WeaponWheelMenuController>();
		weaponAnimationController = weaponSystemGameObject.AddComponent<WeaponAnimationController>();
		weaponFirstPersonRender = weaponSystemGameObject.AddComponent<WeaponFirstPersonRender>();

		weaponWheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButtons/WeaponWheelButton");

		weaponText = canvasMenuWeaponWheel.transform.Find("Selected Weapon Name").GetComponent<TextMeshProUGUI>();
		weaponWheelName = canvasMenuWeaponWheel.transform.Find("WeaponWheel Hand").GetComponent<TextMeshProUGUI>();
		weaponIconBig = canvasMenuWeaponWheel.transform.Find("WeaponBig").GetComponent<Image>();

		firstPersonLeftHandWeaponSlotGameObject = GameObject.Find("Slot1.L");
		thirdPersonLeftHandWeaponSlotGameObject = GameObject.Find("Slot.L");
		firstPersonRightHandWeaponSlotGameObject = GameObject.Find("Slot1.R");
		thirdPersonRightHandWeaponSlotGameObject = GameObject.Find("Slot.R");

		ChokeNPCtext = canvasHUDammo.transform.Find("ChokingText").gameObject;

		weaponController.Initialize(inputDevice, bootstrapSubSystemMenu.menuManager, bootstrapSubSystemPlayerSystems.playerBehaviour, bootstrapSubSystemInteraction.interactionController);
		legKickAttack.Initialize(inputDevice, playerGameObject, bootstrapSubSystemPlayerSystems.playerMovementController);

		weaponWheelController.Initialize(
			inputDevice,
			bootstrapSubSystemMenu.menuManager,
			bootstrapSubSystemPlayerSystems.playerBehaviour,
			weaponController,
			weaponWheelSegmentPrefab,
			canvasMenuWeaponWheel,
			weaponText,
			weaponWheelName,
			weaponIconBig);

		weaponAnimationController.Initialize(
			playerGameObject,
			bootstrapSubSystemPlayerSystems.playerBehaviour,
			bootstrapSubSystemPlayerSystems.playerCameraController,
			weaponController,
			legKickAttack);

		weaponFirstPersonRender.Initialize(
			bootstrapSubSystemScene.gameSceneManager,
			bootstrapSubSystemPlayerSystems.playerCameraController,
			weaponController,
			bootstrapSubSystemPlayerSystems.playerFirstPersonHandRight,
			bootstrapSubSystemPlayerSystems.playerFirstPersonHandLeft,
			bootstrapSubSystemPlayerSystems.playerHandRightParent,
			bootstrapSubSystemPlayerSystems.playerHandLeftParent);

		canvasHUDammoController.Initialize(
			bootstrapSubSystemScene.gameSceneManager,
			gameController,
			bootstrapSubSystemMenu.menuManager,
			canvasHUDammo,
			weaponController,
			playerResourcesAmmoManager,
			bootstrapSubSystemPlayerSystems.playerBehaviour,
			RightWeaponAmmoMagazine,
			RightWeaponAmmoReserve,
			RightWeaponAmmoSeparator,
			LeftWeaponAmmoMagazine,
			LeftWeaponAmmoReserve,
			LeftWeaponAmmoSeparator);

		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("firstPersonLeftHandWeaponSlotGameObject", firstPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("firstPersonRightHandWeaponSlotGameObject", firstPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonLeftHandWeaponSlotGameObject", thirdPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonRightHandWeaponSlotGameObject", thirdPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("ChokeNPCtext", ChokeNPCtext);

		Debug.Log("WEAPON SYSTEM INITIALIZED");

		yield break;
	}
}