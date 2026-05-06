using System;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuConfirmActionController : MonoBehaviour
{
	private PauseMenuController pauseMenuController;
	// --- Поля для компонентов ---
	private GameObject canvasPauseSubMenuConfirm;
	private GameObject buttonConfirm;
	private GameObject buttonCancel;
	private MenuManager menuManager;
	private PauseSubMenuSettingsController pauseSubMenuSettingsController;
	// --- Данные для текущего действия ---
	private Action onAcceptAction; // Что сделать при "Принять"
	private int targetSlot; // Для какого слота действие
	private GameObject textShowConfirmationMessage;
	private Text confirmationTextComponent;
	// --- Ссылки на контроллеры, от которых мы зависим ---
	private SaveLoadController saveLoadController;
	private PauseSubMenuSaveController saveController;
	private PauseSubMenuLoadController loadController;

	// --- Метод для внедрения зависимостей (DI) ---
	public void Initialize(
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuConfirm,
		GameObject buttonAccept,
		GameObject buttonCancel,
		SaveLoadController saveLoadController,
		PauseSubMenuSaveController saveController,
		PauseSubMenuLoadController loadController,
		PauseSubMenuSettingsController pauseSubMenuSettingsController,
		GameObject textShowConfirmationMessage)
	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.canvasPauseSubMenuConfirm = canvasPauseSubMenuConfirm;
		this.buttonConfirm = buttonAccept;
		this.buttonCancel = buttonCancel;
		this.saveLoadController = saveLoadController;
		this.saveController = saveController;
		this.loadController = loadController;
		this.textShowConfirmationMessage = textShowConfirmationMessage;
		confirmationTextComponent = textShowConfirmationMessage.GetComponent<Text>();
		this.pauseSubMenuSettingsController = pauseSubMenuSettingsController;
		this.buttonConfirm.GetComponent<Button>().onClick.AddListener(() => ExecuteAccept());
		this.buttonCancel.GetComponent<Button>().onClick.AddListener(() => ExecuteCancel());

		// --- НОВОЕ: Подписываемся на события от Save и Load контроллеров ---
		// Когда SaveController хочет спросить подтверждение, он вызовет это событие
		this.saveController.OnRequestRewriteSaveFileConfirmation += HandleShowForRewriteSaveFile;

		// Когда LoadController хочет спросить подтверждение, он вызовет это событие
		this.loadController.OnRequestLoadSaveFileConfirmation += HandleShowForLoadSaveFile;
		this.saveController.OnRequestNewSaveFileConfirmation += HandleShowForNewSaveFile;
		
		
		this.pauseSubMenuSettingsController.OnRequestSaveSettingsConfirmation += HandleShowForSaveSettings;
		this.pauseSubMenuSettingsController.OnRequestResetSettingsConfirmation += HandleShowForResetSettings;

		this.saveController.OnRequestDeleteSaveFileConfirmation += HandleShowForDeleteSaveFile;

		this.pauseMenuController.OnOpenConfirmMenu += ShowCanvasConfirmAction;
		this.pauseMenuController.OnCloseConfirmMenu += HideCanvasConfirmAction;
	}

	public void ShowCanvasConfirmAction()
	{
		canvasPauseSubMenuConfirm.SetActive(true);
		//onAcceptAction = null;
	}
	public void HideCanvasConfirmAction()
	{
		canvasPauseSubMenuConfirm.SetActive(false);
		onAcceptAction = null;
	}


	// --- Методы-реакции на события от других контроллеров ---
	// Этот метод вызовется из SaveController
	private void HandleShowForRewriteSaveFile(int slot)
	{
		targetSlot = slot;
		confirmationTextComponent.text = "Перезаписать игру в слоте " + slot + " ?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => StartCoroutine(saveLoadController.SaveGame(slot));

		pauseMenuController.OpenPauseConfirmMenu();
	}

	// Этот метод вызовется из LoadController
	private void HandleShowForLoadSaveFile(int slot)
	{
		targetSlot = slot;
		confirmationTextComponent.text = "Загрузить игру из слота " + slot + " ?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => StartCoroutine(saveLoadController.LoadGame(slot));

		pauseMenuController.OpenPauseConfirmMenu();
	}

	private void HandleShowForSaveSettings()
	{
		confirmationTextComponent.text = "Сохранить настройки?";
		onAcceptAction = () => pauseSubMenuSettingsController.SaveSettings();
		pauseMenuController.OpenPauseConfirmMenu();
	}
	private void HandleShowForResetSettings()
	{
		confirmationTextComponent.text = "Сбросить настройки?";
		onAcceptAction = () => pauseSubMenuSettingsController.ResetSettings();
		pauseMenuController.OpenPauseConfirmMenu();
	}
	private void HandleShowForDeleteSaveFile(int slot)
	{
		targetSlot = slot;
		confirmationTextComponent.text = "Удалить игру в слоте " + slot + " ?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => saveLoadController.DeleteGame(slot);

		pauseMenuController.OpenPauseConfirmMenu();
	}
	// --- НОВЫЙ МЕТОД ---
	// Этот метод вызовется, когда пользователь нажмет "Новое сохранение"
	private void HandleShowForNewSaveFile(int slot)
	{
		//Debug.Log("BRUH!");
		targetSlot = slot;
		confirmationTextComponent.text = "Создать новое сохранение?"; 
		// *Если у вас есть текст, раскомментируйте эту строку*

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => StartCoroutine(saveLoadController.SaveGame(slot));

		pauseMenuController.OpenPauseConfirmMenu();
	}


	private void ExecuteAccept()
	{
		onAcceptAction?.Invoke(); // Выполняем действие (Сохранить или Загрузить)
		pauseMenuController.ClosePauseConfirmMenu(); // Вызывать при любом закрытии

		// После выполнения действия нужно разблокировать интерфейс в Save/Load контроллере.
		// Для этого можно вызвать еще одно событие или передать ссылку на метод разблокировки.
		// Это остается на ваше усмотрение.
	}

	private void ExecuteCancel()
	{
		pauseMenuController.ClosePauseConfirmMenu(); // Вызывать при любом закрытии

		// При отмене нужно сообщить Save/Load контроллеру, чтобы он разблокировал кнопки.
	}
	
}