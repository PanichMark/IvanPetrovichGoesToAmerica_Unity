using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuConfirmActionController : MonoBehaviour
{
	private LocalizationManager _localizationManager;
	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;
	private MenuManager _menuManager;

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

	private Action _actionOnAccept;

	public void Initialize(
		LocalizationManager localizationManager,
		GameSceneManager gameSceneManager,
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
		_canvasPauseSubMenuConfirm.SetActive(false);
		_actionOnAccept = null;
	}

	private void HandleShowForNewSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmCreateNewGameFile}";

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
	}

	private void HandleShowForLoadSaveFile(int slot)
	{
		_textComponentActionMessage.text = $"{_textConfirmLoadGameFile} {slot} ?";

		_actionOnAccept = () => StartCoroutine(_saveLoadController.LoadGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForExitToMainMenu()
	{
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
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsGeneral}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGeneralController.SaveSettingsGeneral();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGeneral()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsGeneral}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGeneralController.ResetSettingsGeneral();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsControls()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsControls}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionControlsController.SaveSettingsControls();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsControls()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsControls}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionControlsController.ResetSettingsControls();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsGraphics()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsGraphics}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGraphicsController.SaveSettingsGraphics();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGraphics()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsGraphics}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionGraphicsController.ResetSettingsGraphics();
		_pauseMenuController.OpenPauseConfirmMenu();
	}


	private void HandleShowForSaveSettingsAudio()
	{
		_textComponentActionMessage.text = $"{_textConfirmSaveSettings} {_textSettingsAudio}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsAudio()
	{
		_textComponentActionMessage.text = $"{_textConfirmResetSettings} {_textSettingsAudio}";
		_actionOnAccept = () => _pauseSubMenuSettingsSectionAudioController.ResetSettingsAudio();
		_pauseMenuController.OpenPauseConfirmMenu();
	}
	
	private void ExecuteAccept()
	{
		_actionOnAccept?.Invoke();
		_pauseMenuController.ClosePauseConfirmMenu(); 
	}

	private void ExecuteCancel()
	{
		if (_menuManager.IsConfirmationOnExitToMainMenuOpened)
		{
			_menuManager.CloseConfirmationOnExitToMainMenu();
		}
		else
		{
			_pauseMenuController.ClosePauseConfirmMenu();
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textConfirmCreateNewGameFile = _localizationManager.GetLocalizedString("");
		_textConfirmRewriteGameFile = _localizationManager.GetLocalizedString("");
		_textConfirmDeleteGameFile = _localizationManager.GetLocalizedString("");
		_textConfirmLoadGameFile = _localizationManager.GetLocalizedString("");

		_textConfirmExitToMainMenu = _localizationManager.GetLocalizedString("");
	
		_textSettingsGeneral = _localizationManager.GetLocalizedString("");
		_textSettingsControls = _localizationManager.GetLocalizedString("");
		_textSettingsGraphics = _localizationManager.GetLocalizedString("");
		_textSettingsAudio = _localizationManager.GetLocalizedString("");

		_textConfirmSaveSettings = _localizationManager.GetLocalizedString("");
		_textConfirmResetSettings = _localizationManager.GetLocalizedString("");

		_textButtonComponentConfirmAction.text = _localizationManager.GetLocalizedString("");
		_textButtonComponentCancelAction.text = _localizationManager.GetLocalizedString("");
	}
}