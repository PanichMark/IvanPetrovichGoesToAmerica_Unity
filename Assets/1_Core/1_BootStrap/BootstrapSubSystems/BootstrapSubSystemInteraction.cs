using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BootstrapSubSystemInteraction
{
	private Bootstrap bootstrap;
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;
	private GameController gameController;
	private GameSceneManager gameSceneManager;
	private IInputDevice inputDevice;
	private PlayerCameraController playerCameraController;
	private PlayerBehaviour playerBehaviour;


	private GameObject interactionControllerGameObject;
	public InteractionController interactionController {  get; private set; }
	private InteractionAnimationController interactionAnimationController;
	private InteractionFirstPersonRender interactionFirstPersonRender;
	private GameObject[] buttonsLockElectrical;
	private TextMeshProUGUI mainInteractionText;
	private TextMeshProUGUI additionalInteractionText;
	private Button buttonExitReadNoteMenu;
	private Button buttonExitLockpickMechanicalMenu;
	private Button buttonExitLockpickElectronicMenu;
	private TextMeshProUGUI readableText;
	private Image backgroundBlack;
	private TextMeshProUGUI[] itemsTexts;
	private Image[] itemsImages;
	private Image imageNewspaper;
	private TextMeshProUGUI NPCphrasesText;
	private TextMeshProUGUI NPCdialogueText;
	private Button buttonDialogueYes;
	private Button buttonDialogueNo;

	private GameObject canvasHUDInteraction;
	private GameObject canvasReadNoteMenu;
	private GameObject canvasLockpickMechanicalMenu;
	private GameObject canvasLockpickElectronicMenu;
	private GameObject canvasDialogueMenu;

	public BootstrapSubSystemInteraction(Bootstrap bootstrap,
	BootstrapSubSystemMenu bootstrapSubSystemMenu,
	GameController gameController,
	GameSceneManager gameSceneManager,
	IInputDevice inputDevice,
	PlayerCameraController playerCameraController,
	PlayerBehaviour playerBehaviour,
	GameObject canvasHUDInteraction,
	GameObject canvasReadNoteMenu,
	GameObject canvasLockpickMechanicalMenu,
	GameObject canvasLockpickElectronicMenu,
	GameObject canvasDialogueMenu)
	{
		this.bootstrap = bootstrap;
		this.bootstrapSubSystemMenu = bootstrapSubSystemMenu;
		this.gameController = gameController;
		this.gameSceneManager = gameSceneManager;
		this.inputDevice = inputDevice;
		this.playerCameraController	= playerCameraController;
		this.playerBehaviour = playerBehaviour;
		this.canvasHUDInteraction = canvasHUDInteraction;
		this.canvasReadNoteMenu = canvasReadNoteMenu;
		this.canvasLockpickMechanicalMenu = canvasLockpickMechanicalMenu;
		this.canvasLockpickElectronicMenu = canvasLockpickElectronicMenu;
		this.canvasDialogueMenu = canvasDialogueMenu;
	}

	public IEnumerator InitializeInteractionSystem()
	{
		//loadingStatusText.text = "Interaction System";

		interactionControllerGameObject = new GameObject("InteractionController");

		interactionController = interactionControllerGameObject.AddComponent<InteractionController>();
		interactionAnimationController = interactionControllerGameObject.AddComponent<InteractionAnimationController>();
		interactionFirstPersonRender = interactionControllerGameObject.AddComponent<InteractionFirstPersonRender>();

		// Элементы HUD
		mainInteractionText = canvasHUDInteraction.transform.Find("mainInteractionText").GetComponent<TextMeshProUGUI>();
		additionalInteractionText = canvasHUDInteraction.transform.Find("additionalInteractionText").GetComponent<TextMeshProUGUI>();
		NPCphrasesText = canvasHUDInteraction.transform.Find("NPCphrases").GetComponent<TextMeshProUGUI>();
		NPCdialogueText = canvasDialogueMenu.transform.Find("NPCdialogue").GetComponent<TextMeshProUGUI>();

		itemsTexts = new TextMeshProUGUI[]
		{
			canvasHUDInteraction.transform.Find("Item1text").GetComponent<TextMeshProUGUI>(),
			canvasHUDInteraction.transform.Find("Item2text").GetComponent<TextMeshProUGUI>(),
			canvasHUDInteraction.transform.Find("Item3text").GetComponent<TextMeshProUGUI>()
		};
		itemsImages = new Image[]
		{
			canvasHUDInteraction.transform.Find("Image1Icon").GetComponent<Image>(),
			canvasHUDInteraction.transform.Find("Image2Icon").GetComponent<Image>(),
			canvasHUDInteraction.transform.Find("Image3Icon").GetComponent<Image>()
		};

		buttonsLockElectrical = new GameObject[]
		{
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton1"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton2"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton3"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton4"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton5"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton6"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton7"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton8"),
			bootstrap.FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton9"),
		};
		buttonExitReadNoteMenu = canvasReadNoteMenu.transform.Find("ExitReadNote").GetComponent<Button>();
		imageNewspaper = canvasReadNoteMenu.transform.Find("ReadableImage").GetComponent<Image>();
		readableText = canvasReadNoteMenu.transform.Find("ReadableText").GetComponent<TextMeshProUGUI>();
		backgroundBlack = canvasReadNoteMenu.transform.Find("BackgroundBlack").GetComponent<Image>();
		buttonExitLockpickMechanicalMenu = canvasLockpickMechanicalMenu.transform.Find("ExitLockpickMechanical").GetComponent<Button>();
		buttonExitLockpickElectronicMenu = canvasLockpickElectronicMenu.transform.Find("ExitLockpickElectronic").GetComponent<Button>();
		buttonDialogueYes = canvasDialogueMenu.transform.Find("buttonYes").GetComponent<Button>();
		buttonDialogueNo = canvasDialogueMenu.transform.Find("buttonNo").GetComponent<Button>();

		// Инициализация взаимодействия
		interactionController.Initialize(gameController, gameSceneManager, inputDevice, bootstrapSubSystemMenu.menuManager, playerCameraController, playerBehaviour, canvasHUDInteraction, mainInteractionText,
			additionalInteractionText, itemsTexts, itemsImages);

		//interactionAnimationController.Initialize(playerGameObject, interactionController);
		//interactionFirstPersonRender.Initialize(gameSceneManager, playerCameraController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHandRightParent, playerHandLeftParent, interactionController);

		ServiceLocator.Register("ExitReadNote", buttonExitReadNoteMenu);
		ServiceLocator.Register("ExitLockpickMechanical", buttonExitLockpickMechanicalMenu);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ExitLockpickElectronic", buttonExitLockpickElectronicMenu);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);
		ServiceLocator.Register("buttonsLockElectrical", buttonsLockElectrical);
		ServiceLocator.Register("NPCphrases", NPCphrasesText);
		ServiceLocator.Register("NPCdialogueText", NPCdialogueText);
		ServiceLocator.Register("buttonDialogueYes", buttonDialogueYes);
		ServiceLocator.Register("buttonDialogueNo", buttonDialogueNo);
		ServiceLocator.Register("CanvasLockpickMechanicalMenu", canvasLockpickMechanicalMenu);
		ServiceLocator.Register("CanvasLockpickElectronicMenu", canvasLockpickElectronicMenu);
		ServiceLocator.Register("CanvasReadNoteMenu", canvasReadNoteMenu);
		ServiceLocator.Register("CanvasDialogueMenu", canvasDialogueMenu);

		Debug.Log("INTERACTION SYSTEM INITIALIZED");
		yield break;
	}

}
