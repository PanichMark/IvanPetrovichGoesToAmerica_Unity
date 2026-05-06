using System.Collections; // Для StartCoroutine
using UnityEngine;
using UnityEngine.UI;
using System;



public class PauseSubMenuSaveController : MonoBehaviour
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;

	// --- Поля для управления интерфейсом ---
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PauseMenuController pauseMenuController;
	private bool isPauseSubMenuSaveOpened;
	private GameObject canvasPauseSubMenuSave;
	private SaveLoadController saveLoadController;

	// --- Ссылки на кнопки (GameObjects) ---
	private GameObject buttonClosePauseSubMenuSave;
	private GameObject[] buttonsRewriteGame;
	private GameObject[] buttonsDeleteGame;
	private GameObject buttonSaveNewGame;

	// --- Поля для отображения информации о слотах ---
	private Text[] currentDateAndTimeTexts;
	private Text[] currentSceneNameUITexts;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, SaveLoadController saveLoadController,
		GameObject canvasPauseSubMenuSave, GameObject[] buttonsRewriteGame, GameObject[] buttonsDeleteGame,
		GameObject buttonClosePauseSubMenuSave, GameObject buttonSaveNewGame)
	{
		// Сохраняем ссылки на объекты
		this.buttonClosePauseSubMenuSave = buttonClosePauseSubMenuSave;
		this.buttonSaveNewGame = buttonSaveNewGame;
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		this.buttonsRewriteGame = buttonsRewriteGame;
		this.buttonsDeleteGame = buttonsDeleteGame;
		this.saveLoadController = saveLoadController;

		// 1. Инициализация слушателей кликов
		for (int i = 0; i < buttonsRewriteGame.Length; i++)
		{
			int slot = i + 1;
			int capturedSlot = slot; // Защита от замыкания в цикле
			this.buttonsRewriteGame[i].GetComponent<Button>().onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(capturedSlot));
			this.buttonsDeleteGame[i].GetComponent<Button>().onClick.AddListener(() => OnRequestDeleteSaveFileConfirmation?.Invoke(capturedSlot));
		}

		this.buttonClosePauseSubMenuSave.GetComponent<Button>().onClick.AddListener(() => pauseMenuController.ClosePauseSubMenu());

		// 2. Инициализация текстовых полей и иконок
		currentDateAndTimeTexts = new Text[buttonsRewriteGame.Length];
		currentSceneNameUITexts = new Text[buttonsRewriteGame.Length];

		for (int i = 0; i < buttonsRewriteGame.Length; i++)
		{
			Transform buttonTransform = buttonsRewriteGame[i].transform;
			currentDateAndTimeTexts[i] = buttonTransform.Find("Text_CurrentDateAndTime")?.GetComponent<Text>();
			currentSceneNameUITexts[i] = buttonTransform.Find("Text_CurrentSceneNameUI")?.GetComponent<Text>();

			// Скрываем элементы по умолчанию
			if (currentDateAndTimeTexts[i] != null) currentDateAndTimeTexts[i].gameObject.SetActive(false);
			if (currentSceneNameUITexts[i] != null) currentSceneNameUITexts[i].gameObject.SetActive(false);

			Transform iconTransform = buttonTransform.Find("Level_Image");
			if (iconTransform != null) iconTransform.gameObject.SetActive(false);
		}

		this.buttonSaveNewGame.GetComponent<Button>().onClick.AddListener(() =>
		{
			int slotToUse = FindFirstEmptySlot();
			if (slotToUse != -1)
			{
				OnRequestNewSaveFileConfirmation?.Invoke(slotToUse);
			}
		});

		// 3. Подписка на события
		this.pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideSaveSubMenuCanvas;

		// Подписываемся на событие удаления для обновления интерфейса
		this.saveLoadController.OnSafeFileDelete += UpdateAllUIElements;
		this.saveLoadController.OnSafeFileSaved += UpdateAllUIElements;

		this.menuManager.OnOpenConfirmMenu += DisableButtons;
		this.menuManager.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SaveSubMenu Initialized");
	}

	private void DisableButtons()
	{
		// Блокируем кнопки перезаписи слотов
		foreach (var buttonObj in buttonsRewriteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		// Блокируем кнопки удаления слотов
		foreach (var buttonObj in buttonsDeleteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		// Блокируем кнопку "Новое сохранение"
		Button newSaveButton = buttonSaveNewGame.GetComponent<Button>();
		if (newSaveButton != null)
		{
			newSaveButton.interactable = false;
		}

		// Блокируем кнопку закрытия подменю
		Button closeButton = buttonClosePauseSubMenuSave.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = false;
		}
	}

	private void EnableButtons()
	{
		// Разблокируем кнопки перезаписи слотов
		foreach (var buttonObj in buttonsRewriteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		// Разблокируем кнопки удаления слотов
		foreach (var buttonObj in buttonsDeleteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		// Разблокируем кнопку "Новое сохранение"
		Button newSaveButton = buttonSaveNewGame.GetComponent<Button>();
		if (newSaveButton != null)
		{
			newSaveButton.interactable = true;
		}

		// Разблокируем кнопку закрытия подменю
		Button closeButton = buttonClosePauseSubMenuSave.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = true;
		}
	}

	// --- Логика поиска пустого слота ---
	private int FindFirstEmptySlot()
	{
		var extendedSaveInfos = saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			// Обращаемся к элементам кортежа через .ItemN
			if (string.IsNullOrEmpty(extendedSaveInfos[i].Item3)) // Item3 - это currentSceneNameSystem
				return i + 1;
		}
		return -1;
	}

	// --- Метод обновления ВСЕХ элементов интерфейса ---
	public void UpdateAllUIElements()
	{
		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;

		// Обновляем состояние кнопки "Новое сохранение"
		if (buttonSaveNewGame.activeSelf != hasEmptySlot)
		{
			buttonSaveNewGame.SetActive(hasEmptySlot);
		}
	}

	// --- Метод обновления текстов и видимости кнопок слотов ---
	private void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			// Обращаемся к элементам кортежа через .ItemN
			string currentDataAndTime = extendedSaveInfos[i].Item1;
			string currentSceneNameUI = extendedSaveInfos[i].Item2;
			string currentSceneNameSystem = extendedSaveInfos[i].Item3;

			if (!string.IsNullOrEmpty(currentSceneNameSystem)) // ЕСЛИ СЛОТ ЗАНЯТ
			{
				buttonsRewriteGame[i].SetActive(true);
				buttonsDeleteGame[i].SetActive(true);

				currentDateAndTimeTexts[i].text = currentDataAndTime;
				currentSceneNameUITexts[i].text = currentSceneNameUI;

				currentDateAndTimeTexts[i].gameObject.SetActive(true);
				currentSceneNameUITexts[i].gameObject.SetActive(true);

				// Загрузка и показ иконки сцены
				string imagePath = $"Sprites/{currentSceneNameSystem}";
				Sprite sprite = Resources.Load<Sprite>(imagePath);
				Transform imageTransform = buttonsRewriteGame[i].transform.Find("Level_Image");

				if (imageTransform != null)
				{
					imageTransform.gameObject.SetActive(sprite != null);
					if (sprite != null)
					{
						imageTransform.GetComponent<Image>().sprite = sprite;
					}
				}
			}
			else // ЕСЛИ СЛОТ ПУСТ
			{
				buttonsRewriteGame[i].SetActive(false);
				buttonsDeleteGame[i].SetActive(false);

				if (currentDateAndTimeTexts[i] != null) currentDateAndTimeTexts[i].gameObject.SetActive(false);
				if (currentSceneNameUITexts[i] != null) currentSceneNameUITexts[i].gameObject.SetActive(false);

				Transform imageTransform = buttonsRewriteGame[i].transform.Find("Level_Image");
				if (imageTransform != null) imageTransform.gameObject.SetActive(false);
			}
		}
	}


	private void ShowSaveSubMenuCanvas()
	{
		isPauseSubMenuSaveOpened = true;
		canvasPauseSubMenuSave.SetActive(true);

		// Обновляем информацию о слотах и видимость кнопок "Сохранить/Удалить"
		RefreshButtonLabelsAndVisibility();

		// Обновляем состояние кнопки "Новое сохранение"
		bool hasEmptySlot = FindFirstEmptySlot() != -1;
		buttonSaveNewGame.SetActive(hasEmptySlot);


	
	}

	private void HideSaveSubMenuCanvas()
	{
		if (isPauseSubMenuSaveOpened)
		{
			isPauseSubMenuSaveOpened = false;
			canvasPauseSubMenuSave.SetActive(false);
			Debug.Log("SaveSubMenu closed");
		}
	}
}