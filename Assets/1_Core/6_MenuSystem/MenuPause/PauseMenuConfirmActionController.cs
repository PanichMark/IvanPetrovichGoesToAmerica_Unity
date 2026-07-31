using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuConfirmActionController : MonoBehaviour
{
	public delegate void PlayerCameraStateMenuEventHandler();
	public event PlayerCameraStateMenuEventHandler OnSetPlayerCameraToFirstPerson;

	private LocalizationManager _localizationManager;
	private GameScenesManager _gameSceneManager;
	private SaveLoadController _saveLoadController;
	private MenuManager _menuManager;
	private GameController _gameController;
	private PauseMenuController _pauseMenuController;
	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private PauseSubMenuLoadController _pauseSubMenuLoadController;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private PauseSubMenuSettingsSectionControlsController _pauseSubMenuSettingsSectionControlsController;
	private PauseSubMenuSettingsSectionGraphicsController _pauseSubMenuSettingsSectionGraphicsController;
	private PauseSubMenuSettingsSectionAudioController _pauseSubMenuSettingsSectionAudioController;

	private GameObject _canvasPauseSubMenuConfirm;

	private GameObject _textActionMessage;
	private TextMeshProUGUI _textComponentActionMessage;

	private GameObject _buttonConfirmAction;
	private Button _buttonComponentConfirmAction;
	private GameObject _textButtonConfirmAction;
	private TextMeshProUGUI _textButtonComponentConfirmAction;

	private GameObject _buttonCancelAction;
	private Button _buttonComponentCancelAction;
	private GameObject _textButtonCancelAction;
	private TextMeshProUGUI _textButtonComponentCancelAction;

	private string _textConfirmCreateNewGameFile;
	private string _textConfirmRewriteGameFile;
	private string _textConfirmDeleteGameFile;
	private string _textConfirmLoadGameFile;

	private string _textConfirmExitToMainMenu;

	private string _textSettingsGeneral;
	private string _textSettingsControls;
	private string _textSettingsGraphics;
	private string _textSettingsAudio;

	private string _textConfirmSaveSettings;
	private string _textConfirmResetSettings;

	private string _textLoadEpisode;

	private Action _actionOnAccept;

	private Color _confirmDefaultColor;
	private Color _confirmHighlightedColor;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		SaveLoadController saveLoadController,
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		PauseSubMenuSaveController saveController,
		PauseSubMenuLoadController loadController,
		PauseSubMenuSettingsController pauseSubMenuSettingsController,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PauseSubMenuSettingsSectionControlsController pauseSubMenuSettingsSectionControlsController,
		PauseSubMenuSettingsSectionGraphicsController pauseSubMenuSettingsSectionGraphicsController,
		PauseSubMenuSettingsSectionAudioController pauseSubMenuSettingsSectionAudioController,
		GameObject canvasPauseSubMenuConfirm,
		ViewModelPauseMenuConfirmAction viewModelPauseMenuConfirmAction)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_saveLoadController = saveLoadController;
		_menuManager = menuManager;

		_pauseMenuController = pauseMenuController;
		_pauseSubMenuSaveController = saveController;
		_pauseSubMenuLoadController = loadController;
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_pauseSubMenuSettingsSectionControlsController = pauseSubMenuSettingsSectionControlsController;
		_pauseSubMenuSettingsSectionGraphicsController = pauseSubMenuSettingsSectionGraphicsController;
		_pauseSubMenuSettingsSectionAudioController = pauseSubMenuSettingsSectionAudioController;

		_canvasPauseSubMenuConfirm = canvasPauseSubMenuConfirm;

		_textActionMessage = viewModelPauseMenuConfirmAction.TextActionMessage;
		_textComponentActionMessage = viewModelPauseMenuConfirmAction.TextActionMessage.GetComponent<TextMeshProUGUI>();

		_buttonConfirmAction = viewModelPauseMenuConfirmAction.ButtonConfirmAction;
		_buttonComponentConfirmAction = viewModelPauseMenuConfirmAction.ButtonConfirmAction.GetComponent<Button>();
		_buttonComponentConfirmAction.onClick.AddListener(() => ExecuteAccept());
		_textButtonConfirmAction = viewModelPauseMenuConfirmAction.TextButtonConfirmAction;
		_textButtonComponentConfirmAction = viewModelPauseMenuConfirmAction.TextButtonConfirmAction.GetComponent<TextMeshProUGUI>();

		_buttonCancelAction = viewModelPauseMenuConfirmAction.ButtonCancelAction;
		_buttonComponentCancelAction = viewModelPauseMenuConfirmAction.ButtonCancelAction.GetComponent<Button>();
		_buttonComponentCancelAction.onClick.AddListener(() => ExecuteCancel());
		_textButtonCancelAction = viewModelPauseMenuConfirmAction.TextButtonCancelAction;
		_textButtonComponentCancelAction = viewModelPauseMenuConfirmAction.TextButtonCancelAction.GetComponent<TextMeshProUGUI>();

		_confirmDefaultColor = _buttonComponentConfirmAction.colors.normalColor;
		_confirmHighlightedColor = _buttonComponentConfirmAction.colors.highlightedColor;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseSubMenuSaveController.OnRequestNewSaveFileConfirmation += HandleShowForNewSaveFile;
		_pauseSubMenuSaveController.OnRequestRewriteSaveFileConfirmation += HandleShowForRewriteSaveFile;
		_pauseSubMenuSaveController.OnRequestDeleteSaveFileConfirmation += HandleShowForDeleteSaveFile;
		_pauseSubMenuLoadController.OnRequestLoadSaveFileConfirmation += HandleShowForLoadSaveFile;

		_pauseMenuController.OnOpenConfirmMenu += ShowCanvasConfirmAction;
		_pauseMenuController.OnCloseConfirmMenu += HideCanvasConfirmAction;
		_pauseMenuController.OnExitToMainMenu += HandleShowForExitToMainMenu;

		_pauseSubMenuSettingsController.OnRequestSaveSettingsGeneralConfirmation += HandleShowForSaveSettingsGeneral;
		_pauseSubMenuSettingsController.OnRequestResetSettingsGeneralConfirmation += HandleShowForResetSettingsGeneral;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsControlsConfirmation += HandleShowForSaveSettingsControls;
		_pauseSubMenuSettingsController.OnRequestResetSettingsControlsConfirmation += HandleShowForResetSettingsControls;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsGraphicsConfirmation += HandleShowForSaveSettingsGraphics;
		_pauseSubMenuSettingsController.OnRequestResetSettingsGraphicsConfirmation += HandleShowForResetSettingsGraphics;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsAudioConfirmation += HandleShowForSaveSettingsAudio;
		_pauseSubMenuSettingsController.OnRequestResetSettingsAudioConfirmation += HandleShowForResetSettingsAudio;
	}

	public void ShowCanvasConfirmAction()
	{
		_canvasPauseSubMenuConfirm.SetActive(true);
	}

	public void HideCanvasConfirmAction()
	{
		ChangeConfirmButtonColorToDefault();
		_canvasPauseSubMenuConfirm.SetActive(false);
		_actionOnAccept = null;
	}

	private void HandleShowForNewSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmCreateNewGameFile}?";

		_actionOnAccept = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForRewriteSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmRewriteGameFile} {slot} ?";

		_actionOnAccept = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForDeleteSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmDeleteGameFile} {slot} ?";

		_actionOnAccept = () => _saveLoadController.DeleteGame(slot);

		_pauseMenuController.OpenPauseConfirmMenu();

		ChangeConfirmButtonColorToRed();
	}

	private void HandleShowForLoadSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmLoadGameFile} {slot} ?";

		_actionOnAccept = () => StartCoroutine(_saveLoadController.LoadGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForExitToMainMenu()
	{
		ChangeConfirmButtonColorToRed();

		_menuManager.OpenConfirmationOnExitToMainMenu();

		_textComponentActionMessage.text = $"{_textConfirmExitToMainMenu}";

		_actionOnAccept = () => 
		{
			_menuManager.CloseConfirmationOnExitToMainMenu();
			StartCoroutine(_gameSceneManager.LoadMainMenuScene()); 
		};

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsGeneral()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsGeneral}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGeneralController.SaveSettingsGeneral();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGeneral()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsGeneral}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGeneralController.ResetSettingsGeneral();
		ChangeConfirmButtonColorToRed();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsControls()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsControls}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionControlsController.SaveSettingsControls();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsControls()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsControls}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionControlsController.ResetSettingsControls();
		ChangeConfirmButtonColorToRed();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsGraphics()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsGraphics}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGraphicsController.SaveSettingsGraphics();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGraphics()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsGraphics}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGraphicsController.ResetSettingsGraphics();
		ChangeConfirmButtonColorToRed();
		_pauseMenuController.OpenPauseConfirmMenu();
	}


	private void HandleShowForSaveSettingsAudio()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsAudio}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsAudio()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsAudio}?";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionAudioController.ResetSettingsAudio();
		ChangeConfirmButtonColorToRed();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	public void HandleShowForChooseEpisode(GameScenesEnum sceneToLoad, string episodeMessage)
	{
		_textComponentActionMessage.text = $"{_textLoadEpisode} {episodeMessage}?";
		_actionOnAccept = () =>
		{
			_gameController.CloseMainMenu();
			StartCoroutine(_gameSceneManager.LoadGameplayScene(sceneToLoad));
			OnSetPlayerCameraToFirstPerson?.Invoke();
		};
		_pauseMenuController.OpenPauseConfirmMenu();
	}
	
	private void ExecuteAccept()
	{
		_actionOnAccept?.Invoke();
		_pauseMenuController.ClosePauseConfirmMenu();

		ChangeConfirmButtonColorToDefault();
	}

	private void ExecuteCancel()
	{
		ChangeConfirmButtonColorToDefault();

		if (_menuManager.IsConfirmationOnExitToMainMenuOpened)
		{
			_menuManager.CloseConfirmationOnExitToMainMenu();
		}
		else
		{
			_pauseMenuController.ClosePauseConfirmMenu();
		}
	}

	private void ChangeConfirmButtonColorToRed()
	{
		ColorBlock colors = _buttonComponentConfirmAction.colors;
		colors.normalColor = new Color(0.576f, 0f, 0f);
		colors.highlightedColor = new Color(0.804f, 0f, 0f);
		colors.pressedColor = new Color(0.804f, 0f, 0f);
		_buttonComponentConfirmAction.colors = colors;
	}

	private void ChangeConfirmButtonColorToDefault()
	{
		ColorBlock colors = _buttonComponentConfirmAction.colors;

		colors.normalColor = _confirmDefaultColor;

		_buttonComponentConfirmAction.colors = colors;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textConfirmCreateNewGameFile = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextConfirmCreateNewGameFile");
		_textConfirmRewriteGameFile = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextConfirmRewriteGameFile");
		_textConfirmDeleteGameFile = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextConfirmDeleteGameFile");
		_textConfirmLoadGameFile = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextConfirmLoadGameFile");

		_textConfirmExitToMainMenu = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextConfirmExitToMainMenu");
	
		_textSettingsGeneral = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextSettingsGeneral");
		_textSettingsControls = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextSettingsControls");
		_textSettingsGraphics = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextSettingsGraphics");
		_textSettingsAudio = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextSettingsAudio");

		_textConfirmSaveSettings = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextSaveSettings");
		_textConfirmResetSettings = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_TextResetSettings");

		_textButtonComponentConfirmAction.text = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_ButtonConfirmAction");
		_textButtonComponentCancelAction.text = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_ButtonCancelAction");

		_textLoadEpisode = _localizationManager.GetLocalizedString("UI_Menu_PauseConfirmActionMenu_LoadEpisode");
	}
}