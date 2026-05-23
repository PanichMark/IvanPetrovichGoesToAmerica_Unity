using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuConfirmActionController : MonoBehaviour
{
	private PauseMenuController _pauseMenuController;
	private GameObject _canvasPauseSubMenuConfirm;
	private GameObject _buttonConfirm;
	private GameObject _buttonCancel;
	private MenuManager _menuManager;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private GameSceneManager _gameSceneManager;
	private Action _onAcceptAction;
	private int _targetSlot; 
	private GameObject _textShowConfirmationMessage;
	private Text _confirmationTextComponent;

	private SaveLoadController _saveLoadController;
	private PauseSubMenuSaveController _saveController;
	private PauseSubMenuLoadController _loadController;

	public void Initialize(
		GameSceneManager gameSceneManager,
		SaveLoadController saveLoadController,
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		PauseSubMenuSaveController saveController,
		PauseSubMenuLoadController loadController,
		PauseSubMenuSettingsController pauseSubMenuSettingsController,
		GameObject canvasPauseSubMenuConfirm,
		GameObject buttonAccept,
		GameObject buttonCancel,
		GameObject textShowConfirmationMessage)
	{
		_gameSceneManager = gameSceneManager;
		_pauseMenuController = pauseMenuController;
		_menuManager = menuManager;
		_canvasPauseSubMenuConfirm = canvasPauseSubMenuConfirm;
		_buttonConfirm = buttonAccept;
		_buttonCancel = buttonCancel;
		_saveLoadController = saveLoadController;
		_saveController = saveController;
		_loadController = loadController;
		_textShowConfirmationMessage = textShowConfirmationMessage;
		_confirmationTextComponent = textShowConfirmationMessage.GetComponent<Text>();
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;
		_buttonConfirm.GetComponent<Button>().onClick.AddListener(() => ExecuteAccept());
		_buttonCancel.GetComponent<Button>().onClick.AddListener(() => ExecuteCancel());

		_saveController.OnRequestRewriteSaveFileConfirmation += HandleShowForRewriteSaveFile;
	
		_loadController.OnRequestLoadSaveFileConfirmation += HandleShowForLoadSaveFile;
		_saveController.OnRequestNewSaveFileConfirmation += HandleShowForNewSaveFile;
		
		_pauseSubMenuSettingsController.OnRequestSaveSettingsGeneralConfirmation += HandleShowForSaveSettingsGeneral;
		_pauseSubMenuSettingsController.OnRequestResetSettingsGeneralConfirmation += HandleShowForResetSettingsGeneral;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsControlsConfirmation += HandleShowForSaveSettingsControls;
		_pauseSubMenuSettingsController.OnRequestResetSettingsControlsConfirmation += HandleShowForResetSettingsControls;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsGraphicsConfirmation += HandleShowForSaveSettingsGraphics;
		_pauseSubMenuSettingsController.OnRequestResetSettingsGraphicsConfirmation += HandleShowForResetSettingsGraphics;
		_pauseSubMenuSettingsController.OnRequestSaveSettingsAudioConfirmation += HandleShowForSaveSettingsAudio;
		_pauseSubMenuSettingsController.OnRequestResetSettingsAudioConfirmation += HandleShowForResetSettingsAudio;

		_saveController.OnRequestDeleteSaveFileConfirmation += HandleShowForDeleteSaveFile;

		_pauseMenuController.OnOpenConfirmMenu += ShowCanvasConfirmAction;
		_pauseMenuController.OnCloseConfirmMenu += HideCanvasConfirmAction;

		_pauseMenuController.OnExitToMainMenu += HandleShowForExitToMainMenu;
	}

	public void ShowCanvasConfirmAction()
	{
		_canvasPauseSubMenuConfirm.SetActive(true);
	}

	public void HideCanvasConfirmAction()
	{
		_canvasPauseSubMenuConfirm.SetActive(false);
		_onAcceptAction = null;
	}

	private void HandleShowForNewSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Создать новое сохранение?";

		_onAcceptAction = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForRewriteSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Перезаписать игру в слоте " + slot + " ?";

		_onAcceptAction = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForDeleteSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Удалить игру в слоте " + slot + " ?";

		_onAcceptAction = () => _saveLoadController.DeleteGame(slot);

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForLoadSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Загрузить игру из слота " + slot + " ?";

		_onAcceptAction = () => StartCoroutine(_saveLoadController.LoadGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForExitToMainMenu()
	{
		_menuManager.OpenConfirmationOnExitToMainMenu();

		_confirmationTextComponent.text = "Выйти в главное меню?";

		_onAcceptAction = () => 
		{
			_menuManager.CloseConfirmationOnExitToMainMenu();
			StartCoroutine(_gameSceneManager.LoadMainMenuScene()); 
		};

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsGeneral()
	{
		_confirmationTextComponent.text = "Сохранить общие настройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.SaveSettingsGeneral();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGeneral()
	{
		_confirmationTextComponent.text = "Сбросить общие настройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.ResetSettingsGeneral();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsControls()
	{
		_confirmationTextComponent.text = "Сохранить настройки управления?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.SaveSettingsControls();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsControls()
	{
		_confirmationTextComponent.text = "Сбросить настройки управления?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.ResetSettingsControls();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettingsGraphics()
	{
		_confirmationTextComponent.text = "Сохранить настройки графики?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.SaveSettingsGraphics();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsGraphics()
	{
		_confirmationTextComponent.text = "Сбросить настройки графики?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.ResetSettingsGraphics();
		_pauseMenuController.OpenPauseConfirmMenu();
	}


	private void HandleShowForSaveSettingsAudio()
	{
		_confirmationTextComponent.text = "Сохранить аудио настройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.SaveSettingsAudio();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettingsAudio()
	{
		_confirmationTextComponent.text = "Сбросить аудионастройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.ResetSettingsAudio();
		_pauseMenuController.OpenPauseConfirmMenu();
	}
	
	private void ExecuteAccept()
	{
		_onAcceptAction?.Invoke();
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
}