using System;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmActionMenuController : MonoBehaviour
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
		
		_pauseSubMenuSettingsController.OnRequestSaveSettingsConfirmation += HandleShowForSaveSettings;
		_pauseSubMenuSettingsController.OnRequestResetSettingsConfirmation += HandleShowForResetSettings;

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

	private void HandleShowForRewriteSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Перезаписать игру в слоте " + slot + " ?";

		_onAcceptAction = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForLoadSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Загрузить игру из слота " + slot + " ?";

		_onAcceptAction = () => StartCoroutine(_saveLoadController.LoadGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettings()
	{
		_confirmationTextComponent.text = "Сохранить настройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.SaveSettings();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForResetSettings()
	{
		_confirmationTextComponent.text = "Сбросить настройки?";
		_onAcceptAction = () => _pauseSubMenuSettingsController.ResetSettings();
		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForDeleteSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Удалить игру в слоте " + slot + " ?";

		_onAcceptAction = () => _saveLoadController.DeleteGame(slot);

		_pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForNewSaveFile(int slot)
	{
		_targetSlot = slot;
		_confirmationTextComponent.text = "Создать новое сохранение?"; 
	
		_onAcceptAction = () => StartCoroutine(_saveLoadController.SaveGame(slot));

		_pauseMenuController.OpenPauseConfirmMenu();
	}
	
	private void HandleShowForExitToMainMenu()
	{
		_menuManager.OpenConfirmationOnExitToMainMenu();

		_confirmationTextComponent.text = "Выйти в главное меню?";

		_onAcceptAction = () => StartCoroutine(_gameSceneManager.LoadMainMenuScene());

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

		_pauseMenuController.ClosePauseConfirmMenu(); 
	}
}