using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private IInputDevice _inputDevice;
	private PlayerCameraController _playerCameraController;
	private PlayerBehaviour _playerBehaviour;

	private GameObject interactionControllerGameObject;
	public InteractionController InteractionController { get; private set; }
	private InteractionAnimationController _interactionAnimationController;
	private InteractionFirstPersonRender _interactionFirstPersonRender;
	private GameObject[] _buttonsLockElectrical;
	private TextMeshProUGUI _textMainInteraction;
	private TextMeshProUGUI _textAdditionalInteraction;
	private Button _buttonExitReadNoteMenu;
	private Button _buttonExitLockpickMechanicalMenu;
	private Button _buttonExitLockpickElectronicMenu;
	private TextMeshProUGUI _textNote;
	private Image _imageNoteBlackBackground;
	private TextMeshProUGUI[] _textsGainedItems;
	private Image[] _imagesGainedItems;
	private Image _imageNote;
	private TextMeshProUGUI _textNPCphrases;
	private TextMeshProUGUI _textNPCdialogue;
	private Button _buttonDialogueYes;
	private Button _buttonDialogueNo;

	private GameObject _canvasHUDInteraction;
	private GameObject _canvasMenuNote;
	private GameObject _canvasMenuLockpickMechanical;
	private GameObject _canvasMenuLockpickElectronic;
	private GameObject _canvasMenuDialogue;

	public BootstrapSubProcessInteractionSystem(Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameController gameController,
		GameSceneManager gameSceneManager,
		IInputDevice inputDevice,
		PlayerCameraController playerCameraController,
		PlayerBehaviour playerBehaviour,
		GameObject canvasHUDInteraction,
		GameObject canvasMenuNote,
		GameObject canvasMenuLockpickMechanical,
		GameObject canvasMenuLockpickElectronic,
		GameObject canvasMenuDialogue)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_inputDevice = inputDevice;
		_playerCameraController = playerCameraController;
		_playerBehaviour = playerBehaviour;
		_canvasHUDInteraction = canvasHUDInteraction;
		_canvasMenuNote = canvasMenuNote;
		_canvasMenuLockpickMechanical = canvasMenuLockpickMechanical;
		_canvasMenuLockpickElectronic = canvasMenuLockpickElectronic;
		_canvasMenuDialogue = canvasMenuDialogue;
	}

	public IEnumerator InitializeInteractionSystem()
	{
		interactionControllerGameObject = new GameObject("InteractionController");

		InteractionController = interactionControllerGameObject.AddComponent<InteractionController>();
		_interactionAnimationController = interactionControllerGameObject.AddComponent<InteractionAnimationController>();
		_interactionFirstPersonRender = interactionControllerGameObject.AddComponent<InteractionFirstPersonRender>();

		_textMainInteraction = _canvasHUDInteraction.transform.Find("mainInteractionText").GetComponent<TextMeshProUGUI>();
		_textAdditionalInteraction = _canvasHUDInteraction.transform.Find("additionalInteractionText").GetComponent<TextMeshProUGUI>();
		_textNPCphrases = _canvasHUDInteraction.transform.Find("NPCphrases").GetComponent<TextMeshProUGUI>();
		_textNPCdialogue = _canvasMenuDialogue.transform.Find("NPCdialogue").GetComponent<TextMeshProUGUI>();

		_textsGainedItems = new TextMeshProUGUI[]
		{
			_canvasHUDInteraction.transform.Find("Item1text").GetComponent<TextMeshProUGUI>(),
			_canvasHUDInteraction.transform.Find("Item2text").GetComponent<TextMeshProUGUI>(),
			_canvasHUDInteraction.transform.Find("Item3text").GetComponent<TextMeshProUGUI>()
		};

		_imagesGainedItems = new Image[]
		{
			_canvasHUDInteraction.transform.Find("Image1Icon").GetComponent<Image>(),
			_canvasHUDInteraction.transform.Find("Image2Icon").GetComponent<Image>(),
			_canvasHUDInteraction.transform.Find("Image3Icon").GetComponent<Image>()
		};

		_buttonsLockElectrical = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton1"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton2"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton3"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton4"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton5"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton6"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton7"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton8"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ElectronicLockButton9")
		};

		_buttonExitReadNoteMenu = _canvasMenuNote.transform.Find("ExitReadNote").GetComponent<Button>();
		_imageNote = _canvasMenuNote.transform.Find("ReadableImage").GetComponent<Image>();
		_textNote = _canvasMenuNote.transform.Find("ReadableText").GetComponent<TextMeshProUGUI>();
		_imageNoteBlackBackground = _canvasMenuNote.transform.Find("BackgroundBlack").GetComponent<Image>();

		_buttonExitLockpickMechanicalMenu = _canvasMenuLockpickMechanical.transform.Find("ExitLockpickMechanical").GetComponent<Button>();
		_buttonExitLockpickElectronicMenu = _canvasMenuLockpickElectronic.transform.Find("ExitLockpickElectronic").GetComponent<Button>();

		_buttonDialogueYes = _canvasMenuDialogue.transform.Find("buttonYes").GetComponent<Button>();
		_buttonDialogueNo = _canvasMenuDialogue.transform.Find("buttonNo").GetComponent<Button>();

		InteractionController.Initialize(
			_gameController,
			_gameSceneManager,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_playerCameraController,
			_playerBehaviour,
			_canvasHUDInteraction,
			_textMainInteraction,
			_textAdditionalInteraction,
			_textsGainedItems,
			_imagesGainedItems);

		ServiceLocator.Register("ExitReadNote", _buttonExitReadNoteMenu);
		ServiceLocator.Register("ExitLockpickMechanical", _buttonExitLockpickMechanicalMenu);
		ServiceLocator.Register("ImageNewspaper", _imageNote);
		ServiceLocator.Register("ExitLockpickElectronic", _buttonExitLockpickElectronicMenu);
		ServiceLocator.Register("ReadableText", _textNote);
		ServiceLocator.Register("BackgroundBlack", _imageNoteBlackBackground);

		ServiceLocator.Register("buttonsLockElectrical", _buttonsLockElectrical);

		ServiceLocator.Register("NPCphrases", _textNPCphrases);
		ServiceLocator.Register("NPCdialogueText", _textNPCdialogue);

		ServiceLocator.Register("buttonDialogueYes", _buttonDialogueYes);
		ServiceLocator.Register("buttonDialogueNo", _buttonDialogueNo);

		ServiceLocator.Register("CanvasLockpickMechanicalMenu", _canvasMenuLockpickMechanical);
		ServiceLocator.Register("CanvasLockpickElectronicMenu", _canvasMenuLockpickElectronic);

		ServiceLocator.Register("CanvasReadNoteMenu", _canvasMenuNote);

		ServiceLocator.Register("CanvasDialogueMenu", _canvasMenuDialogue);

		Debug.Log("INTERACTION SYSTEM INITIALIZED");

		yield break;
	}
}