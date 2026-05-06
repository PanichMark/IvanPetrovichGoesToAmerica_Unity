using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuConfirmActionController : MonoBehaviour
{
	
	// --- Поля для компонентов ---
	private GameObject canvasPauseSubMenuConfirm;
	private GameObject buttonConfirm;
	private GameObject buttonCancel;

	// --- Данные для текущего действия ---
	private Action onAcceptAction; // Что сделать при "Принять"
	private int targetSlot; // Для какого слота действие

	// --- Ссылки на контроллеры, от которых мы зависим ---
	private SaveLoadController saveLoadController;
	private PauseSubMenuSaveController saveController;
	private PauseSubMenuLoadController loadController;

	// --- Метод для внедрения зависимостей (DI) ---
	public void Initialize(
		GameObject canvasPauseSubMenuConfirm,
		GameObject buttonAccept,
		GameObject buttonCancel,
		SaveLoadController saveLoadController,
		PauseSubMenuSaveController saveController,
		PauseSubMenuLoadController loadController)
	{
		this.canvasPauseSubMenuConfirm = canvasPauseSubMenuConfirm;
		this.buttonConfirm = buttonAccept;
		this.buttonCancel = buttonCancel;
		this.saveLoadController = saveLoadController;
		this.saveController = saveController;
		this.loadController = loadController;

		this.buttonConfirm.GetComponent<Button>().onClick.AddListener(() => ExecuteCancel());
		this.buttonCancel.GetComponent<Button>().onClick.AddListener(() => ExecuteAccept());

		// --- НОВОЕ: Подписываемся на события от Save и Load контроллеров ---
		// Когда SaveController хочет спросить подтверждение, он вызовет это событие
		this.saveController.OnRequestSaveFileConfirmation += HandleShowForSave;

		// Когда LoadController хочет спросить подтверждение, он вызовет это событие
		this.loadController.OnRequestLoadFileConfirmation += HandleShowForLoad;


		this.saveController.OnRequestDeleteFileConfirmation += HandleShowForDelete;
	}

	public void HideCanvasConfirmAction()
	{
		canvasPauseSubMenuConfirm.SetActive(false);
		onAcceptAction = null;
	}

	// --- Методы-реакции на события от других контроллеров ---
	// Этот метод вызовется из SaveController
	private void HandleShowForSave(int slot)
	{
		targetSlot = slot;
		//textMessage.text = "Сохранить игру в слот " + slot + "?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => StartCoroutine(saveLoadController.SaveGame(slot));

		canvasPauseSubMenuConfirm.SetActive(true);
	}

	// Этот метод вызовется из LoadController
	private void HandleShowForLoad(int slot)
	{
		targetSlot = slot;
		//textMessage.text = "Загрузить игру из слота " + slot + "?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => StartCoroutine(saveLoadController.LoadGame(slot));

		canvasPauseSubMenuConfirm.SetActive(true);
	}

	private void HandleShowForDelete(int slot)
	{
		targetSlot = slot;
		//textMessage.text = "Загрузить игру из слота " + slot + "?";

		// Задаем действие, которое нужно выполнить при "Принять"
		onAcceptAction = () => saveLoadController.DeleteGame(slot);

		canvasPauseSubMenuConfirm.SetActive(true);
	}

	private void ExecuteAccept()
	{
		onAcceptAction?.Invoke(); // Выполняем действие (Сохранить или Загрузить)
		HideCanvasConfirmAction();

		// После выполнения действия нужно разблокировать интерфейс в Save/Load контроллере.
		// Для этого можно вызвать еще одно событие или передать ссылку на метод разблокировки.
		// Это остается на ваше усмотрение.
	}

	private void ExecuteCancel()
	{
		HideCanvasConfirmAction();

		// При отмене нужно сообщить Save/Load контроллеру, чтобы он разблокировал кнопки.
	}
	
}