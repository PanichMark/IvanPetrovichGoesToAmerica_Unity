using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PauseMenuController pauseMenuController;
	private SaveLoadController dataPersistenceManager;
	private GameObject canvasPauseSubMenuLoad;
	private SaveLoadController saveLoadController;

	private Button CloseLoadSubMenuButton;

	
	private GameObject[] buttonsLoadGame;

	private Text[] currentDateAndTimeTexts;
	private Text[] currentSceneNameUITexts;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, SaveLoadController saveLoadController, GameObject canvasPauseSubMenuLoad, GameObject[] buttonsLoadGame)

	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		this.saveLoadController = saveLoadController;
		this.buttonsLoadGame = buttonsLoadGame;

		this.buttonsLoadGame[0].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.LoadGame(1));
		this.buttonsLoadGame[1].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.LoadGame(2));
		this.buttonsLoadGame[2].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.LoadGame(3));
		this.buttonsLoadGame[3].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.LoadGame(4));
		this.buttonsLoadGame[4].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.LoadGame(5));


		this.pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideLoadSubMenuCanvas;
		Debug.Log("LoadSubMenu Initialized");
	}

	/*
		void Start()
	{
	

		CloseLoadSubMenuButton.onClick.AddListener(CloseLoadSubMenu);

		// Добавляем обработчики кликов на каждую кнопку
		for (int i = 0; i < LoadGameButtons.Length; i++)
		{
			int index = i + 1; // Индекс сохранения начинается с 1
			LoadGameButtons[i].onClick.AddListener(() =>
			{
				dataPersistenceManager.LoadGame(index); // Передаем индекс сохранения
			});
		}

		// Формируем массивы текстовых компонентов
		currentDateAndTimeTexts = new Text[LoadGameButtons.Length]; // Столько же элементов, сколько кнопок
		currentSceneNameUITexts = new Text[LoadGameButtons.Length];

		for (int i = 0; i < LoadGameButtons.Length; i++)
		{
			Transform buttonTransform = LoadGameButtons[i].transform;
			currentDateAndTimeTexts[i] = buttonTransform.Find("Text_CurrentDateAndTime")?.GetComponent<Text>();
			currentSceneNameUITexts[i] = buttonTransform.Find("Text_CurrentSceneNameUI")?.GetComponent<Text>();
		}
	}
	*/


	/*
	private void Update()
	{
		if (InputManager.Instance.GetKeyPauseMenu() && LoadSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseLoadSubMenu();
		}
	}
	*/
	public void ShowLoadSubMenuCanvas()
	{
		canvasPauseSubMenuLoad.gameObject.SetActive(true);
	}
	public void HideLoadSubMenuCanvas()
	{
		canvasPauseSubMenuLoad.gameObject.SetActive(false);
		Debug.Log("LoadSubMenu closed");
	}

	public void RefreshLoadButtonLabels()
	{
		var extendedSaveInfos = dataPersistenceManager.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			var (currentDataAndTime, currentSceneNameUI, currentSceneNameSystem) = extendedSaveInfos[i];

			if (!string.IsNullOrEmpty(currentSceneNameSystem)) // Проверяем наличие сцены
			{
				buttonsLoadGame[i].gameObject.SetActive(true);

				// Обновляем текстовую информацию
				currentSceneNameUITexts[i].text = currentDataAndTime;
				currentDateAndTimeTexts[i].text = currentSceneNameUI;

				// Включаем компоненты
				currentSceneNameUITexts[i].gameObject.SetActive(true);
				currentDateAndTimeTexts[i].gameObject.SetActive(true);

				// Формирование имени файла иконки
				string currentSceneBackgroundImage = $"{currentSceneNameSystem}";

				// Загрузка спрайта иконки
				Sprite sprite = Resources.Load<Sprite>($"Sprites/{currentSceneBackgroundImage}");

				if (sprite != null)
				{
					// Активируем изображение и устанавливаем нужный спрайт
					buttonsLoadGame[i].transform.Find("Level_Image").gameObject.SetActive(true);
					buttonsLoadGame[i].transform.Find("Level_Image").GetComponent<Image>().sprite = sprite;
				}
				else
				{
					Debug.LogError("Failed to load Scene Background Image");
				}
			}
			else
			{
				// Скрываем ненужные элементы интерфейса
				buttonsLoadGame[i].gameObject.SetActive(false);
			}
		}
	}
}

