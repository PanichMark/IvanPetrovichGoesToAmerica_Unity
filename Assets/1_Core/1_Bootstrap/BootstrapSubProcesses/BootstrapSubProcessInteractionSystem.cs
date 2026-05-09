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
	private GameObject[] _buttonsLockElectronic;
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
	private TextMeshProUGUI _textPhrase;
	private TextMeshProUGUI _textDialogue;
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

		_textMainInteraction = _canvasHUDInteraction.transform.Find("TextMainInteraction").GetComponent<TextMeshProUGUI>();
		_textAdditionalInteraction = _canvasHUDInteraction.transform.Find("TextAdditionalInteraction").GetComponent<TextMeshProUGUI>();
		_textPhrase = _canvasHUDInteraction.transform.Find("TextPhrase").GetComponent<TextMeshProUGUI>();
		_textDialogue = _canvasMenuDialogue.transform.Find("TextDialogue").GetComponent<TextMeshProUGUI>();

		_textsGainedItems = new TextMeshProUGUI[]
		{
			_canvasHUDInteraction.transform.Find("TextGainedItem1").GetComponent<TextMeshProUGUI>(),
			_canvasHUDInteraction.transform.Find("TextGainedItem2").GetComponent<TextMeshProUGUI>(),
			_canvasHUDInteraction.transform.Find("TextGainedItem3").GetComponent<TextMeshProUGUI>()
		};

		_imagesGainedItems = new Image[]
		{
			_canvasHUDInteraction.transform.Find("ImageGainedItem1").GetComponent<Image>(),
			_canvasHUDInteraction.transform.Find("ImageGainedItem2").GetComponent<Image>(),
			_canvasHUDInteraction.transform.Find("ImageGainedItem3").GetComponent<Image>()
		};

		_buttonsLockElectronic = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic1"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic2"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic3"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic4"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic5"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic6"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic7"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic8"),
			_bootstrap.FindDeepGameObject(_canvasMenuLockpickElectronic, "ButtonLockElectronic9")

		};

		_buttonExitReadNoteMenu = _canvasMenuNote.transform.Find("ButtonExitReadNoteMenu").GetComponent<Button>();
		_imageNote = _canvasMenuNote.transform.Find("ImageNote").GetComponent<Image>();
		_textNote = _canvasMenuNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();
		_imageNoteBlackBackground = _canvasMenuNote.transform.Find("ImageNoteBlackBackground").GetComponent<Image>();

		_buttonExitLockpickMechanicalMenu = _canvasMenuLockpickMechanical.transform.Find("ButtonExitLockpickMechanicalMenu").GetComponent<Button>();
		_buttonExitLockpickElectronicMenu = _canvasMenuLockpickElectronic.transform.Find("ButtonExitLockpickElectronicMenu").GetComponent<Button>();

		_buttonDialogueYes = _canvasMenuDialogue.transform.Find("ButtonDialogueYes").GetComponent<Button>();
		_buttonDialogueNo = _canvasMenuDialogue.transform.Find("ButtonDialogueNo").GetComponent<Button>();

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

		ServiceLocator.Register("ButtonExitReadNoteMenu", _buttonExitReadNoteMenu);
		ServiceLocator.Register("ButtonExitLockpickMechanicalMenu", _buttonExitLockpickMechanicalMenu);
		ServiceLocator.Register("ImageNote", _imageNote);
		ServiceLocator.Register("ButtonExitLockpickElectronicMenu", _buttonExitLockpickElectronicMenu);
		ServiceLocator.Register("TextNote", _textNote);
		ServiceLocator.Register("ImageNoteBlackBackground", _imageNoteBlackBackground);

		ServiceLocator.Register("ButtonsLockElectronic", _buttonsLockElectronic);

		ServiceLocator.Register("TextNPCphrases", _textPhrase);
		ServiceLocator.Register("TextDialogue", _textDialogue);

		ServiceLocator.Register("ButtonDialogueYes", _buttonDialogueYes);
		ServiceLocator.Register("ButtonDialogueNo", _buttonDialogueNo);

		ServiceLocator.Register("CanvasMenuLockpickMechanical", _canvasMenuLockpickMechanical);
		ServiceLocator.Register("CanvasMenuLockpickElectronic", _canvasMenuLockpickElectronic);

		ServiceLocator.Register("CanvasMenuNote", _canvasMenuNote);

		ServiceLocator.Register("CanvasMenuDialogue", _canvasMenuDialogue);

		Debug.Log("INTERACTION SYSTEM INITIALIZED");

		yield break;
	}
}