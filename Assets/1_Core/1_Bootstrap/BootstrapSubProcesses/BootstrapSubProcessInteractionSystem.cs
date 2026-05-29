using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameSceneManager _gameSceneManager;

	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private GameObject _gameObjectBootstrapInteractionSystem;
	public InteractionController InteractionController { get; private set; }
	private InteractionAnimationController _interactionAnimationController;
	private InteractionFirstPersonRender _interactionFirstPersonRender;

	private KeysManager _keysManager;

	private GameObject _canvasHUDInteraction;
	private TextMeshProUGUI _textInteractionMessageMain;
	private TextMeshProUGUI _textInteractionMessageFail;
	private TextMeshProUGUI[] _textsGainedItems;
	private Image[] _imagesGainedItems;

	private GameObject _canvasMenuNote;
	private TextMeshProUGUI _textNote;
	private Image _imageNote;
	private Image _imageNoteBlackBackground;
	private Button _buttonCloseMenuNote;

	private GameObject _canvasMenuLockpickMechanical;
	private Button _buttonCloseMenuLockpickMechanical;

	private GameObject _canvasMenuLockpickElectronic;
	private GameObject[] _buttonsLockElectronic;
	private Button _buttonCloseMenuLockpickElectronic;

	private GameObject _canvasMenuDialogue;
	private TextMeshProUGUI _textPhraseLine;
	private TextMeshProUGUI _textDialogueLine;
	private Button _buttonDialogueYes;
	private Button _buttonDialogueNo;
	private GameObject _textDialogueYes;
	private GameObject _textDialogueNo;

	public BootstrapSubProcessInteractionSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameSceneManager gameSceneManager,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		GameObject canvasHUDInteraction,
		GameObject canvasMenuNote,
		GameObject canvasMenuLockpickMechanical,
		GameObject canvasMenuLockpickElectronic,
		GameObject canvasMenuDialogue)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_playerBehaviour = playerBehaviour;
		_playerCameraController = playerCameraController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_canvasHUDInteraction = canvasHUDInteraction;
		_canvasMenuNote = canvasMenuNote;
		_canvasMenuLockpickMechanical = canvasMenuLockpickMechanical;
		_canvasMenuLockpickElectronic = canvasMenuLockpickElectronic;
		_canvasMenuDialogue = canvasMenuDialogue;
	}

	public IEnumerator InitializeInteractionSystem()
	{
		_gameObjectBootstrapInteractionSystem = new GameObject("Bootstrap_InteractionSystem");

		InteractionController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionController>();
		_interactionAnimationController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionAnimationController>();
		_interactionFirstPersonRender = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionFirstPersonRender>();

		_textInteractionMessageMain = _canvasHUDInteraction.transform.Find("TextMainInteraction").GetComponent<TextMeshProUGUI>();
		_textInteractionMessageFail = _canvasHUDInteraction.transform.Find("TextAdditionalInteraction").GetComponent<TextMeshProUGUI>();
		_textPhraseLine = _canvasHUDInteraction.transform.Find("TextPhrase").GetComponent<TextMeshProUGUI>();
		_textDialogueLine = _canvasMenuDialogue.transform.Find("TextDialogue").GetComponent<TextMeshProUGUI>();

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

		_buttonCloseMenuNote = _canvasMenuNote.transform.Find("ButtonExitReadNoteMenu").GetComponent<Button>();
		_imageNote = _canvasMenuNote.transform.Find("ImageNote").GetComponent<Image>();
		_textNote = _canvasMenuNote.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();
		_imageNoteBlackBackground = _canvasMenuNote.transform.Find("ImageNoteBlackBackground").GetComponent<Image>();

		_buttonCloseMenuLockpickMechanical = _canvasMenuLockpickMechanical.transform.Find("ButtonExitLockpickMechanicalMenu").GetComponent<Button>();

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
		_buttonCloseMenuLockpickElectronic = _canvasMenuLockpickElectronic.transform.Find("ButtonExitLockpickElectronicMenu").GetComponent<Button>();

		_buttonDialogueYes = _canvasMenuDialogue.transform.Find("ButtonDialogueYes").GetComponent<Button>();
		_buttonDialogueNo = _canvasMenuDialogue.transform.Find("ButtonDialogueNo").GetComponent<Button>();
		_textDialogueYes = _bootstrap.FindDeepGameObject(_canvasMenuDialogue, "TextDialogueYes");
		_textDialogueNo = _bootstrap.FindDeepGameObject(_canvasMenuDialogue, "TextDialogueNo");

		InteractionController.Initialize(
			_gameController,
			_inputDevice,
			_localizationManager,
			_gameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_playerBehaviour,
			_playerCameraController,
			_playerCameraStateMachineController,
			_canvasHUDInteraction,
			_textInteractionMessageMain,
			_textInteractionMessageFail,
			_textsGainedItems,
			_imagesGainedItems);
		_keysManager = new KeysManager();

		ServiceLocator.Register("CanvasMenuNote", _canvasMenuNote);
		ServiceLocator.Register("TextNote", _textNote);
		ServiceLocator.Register("ImageNote", _imageNote);
		ServiceLocator.Register("ImageNoteBlackBackground", _imageNoteBlackBackground);
		ServiceLocator.Register("ButtonCloseReadNoteMenu", _buttonCloseMenuNote);

		ServiceLocator.Register("CanvasMenuLockpickMechanical", _canvasMenuLockpickMechanical);
		ServiceLocator.Register("ButtonCloseLockpickMechanicalMenu", _buttonCloseMenuLockpickMechanical);

		ServiceLocator.Register("CanvasMenuLockpickElectronic", _canvasMenuLockpickElectronic);
		ServiceLocator.Register("ButtonsLockElectronic", _buttonsLockElectronic);
		ServiceLocator.Register("ButtonCloseLockpickElectronicMenu", _buttonCloseMenuLockpickElectronic);

		ServiceLocator.Register("CanvasMenuDialogue", _canvasMenuDialogue);
		ServiceLocator.Register("TextPhraseLine", _textPhraseLine);
		ServiceLocator.Register("TextDialogueLine", _textDialogueLine);
		ServiceLocator.Register("ButtonDialogueYes", _buttonDialogueYes);
		ServiceLocator.Register("ButtonDialogueNo", _buttonDialogueNo);
		ServiceLocator.Register("TextDialogueYes", _textDialogueYes);
		ServiceLocator.Register("TextDialogueNo", _textDialogueNo);

		ServiceLocator.Register("KeysManager", _keysManager);

		Debug.Log("INTERACTION SYSTEM INITIALIZED");

		yield break;
	}
}