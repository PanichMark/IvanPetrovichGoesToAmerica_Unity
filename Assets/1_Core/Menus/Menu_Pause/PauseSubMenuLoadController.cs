using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PauseMenuController pauseMenuController;
	private SaveLoadController saveLoadController;
	private GameObject canvasPauseSubMenuLoad;
	private bool isPauseSubMenuLoadOpened;
	private GameObject buttonClosePauseSubMenuLoad;
	//private Button CloseLoadSubMenuButton;

	
	private GameObject[] buttonsLoadGame;


	private Text[] currentDateAndTimeTexts;
	private Text[] currentSceneNameUITexts;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, SaveLoadController saveLoadController, GameObject canvasPauseSubMenuLoad, GameObject[] buttonsLoadGame, GameObject buttonClosePauseSubMenuLoad)

	{
		this.buttonClosePauseSubMenuLoad = buttonClosePauseSubMenuLoad;
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		this.saveLoadController = saveLoadController;
		this.buttonsLoadGame = buttonsLoadGame;
		//this.buttonsDeleteGame = buttonsDeleteGame;


		this.buttonsLoadGame[0].GetComponent<Button>().onClick.AddListener(() => StartCoroutine(this.saveLoadController.LoadGame(1)));
		this.buttonsLoadGame[1].GetComponent<Button>().onClick.AddListener(() => StartCoroutine(this.saveLoadController.LoadGame(2)));
		this.buttonsLoadGame[2].GetComponent<Button>().onClick.AddListener(() => StartCoroutine(this.saveLoadController.LoadGame(3)));
		this.buttonsLoadGame[3].GetComponent<Button>().onClick.AddListener(() => StartCoroutine(this.saveLoadController.LoadGame(4)));
		this.buttonsLoadGame[4].GetComponent<Button>().onClick.AddListener(() => StartCoroutine(this.saveLoadController.LoadGame(5)));

	
		this.buttonClosePauseSubMenuLoad.GetComponent<Button>().onClick.AddListener(() => this.pauseMenuController.ClosePauseSubMenu());

		this.pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideLoadSubMenuCanvas;
		this.saveLoadController.OnSafeFileDelete += RefreshLoadButtonLabels;

		// Формируем массивы текстовых компонентов
		currentDateAndTimeTexts = new Text[this.buttonsLoadGame.Length]; // Столько же элементов, сколько кнопок
		currentSceneNameUITexts = new Text[this.buttonsLoadGame.Length];

		for (int i = 0; i < this.buttonsLoadGame.Length; i++)
		{
			Transform buttonTransform = this.buttonsLoadGame[i].transform;
			currentDateAndTimeTexts[i] = buttonTransform.Find("Text_CurrentDateAndTime")?.GetComponent<Text>();
			currentSceneNameUITexts[i] = buttonTransform.Find("Text_CurrentSceneNameUI")?.GetComponent<Text>();
		}

		Debug.Log("LoadSubMenu Initialized");
	}


		
	


	
	public void ShowLoadSubMenuCanvas()
	{
		isPauseSubMenuLoadOpened = true;
		canvasPauseSubMenuLoad.gameObject.SetActive(true);
		RefreshLoadButtonLabels();
	}
	public void HideLoadSubMenuCanvas()
	{
		if (isPauseSubMenuLoadOpened)
		{
			isPauseSubMenuLoadOpened = false;
			canvasPauseSubMenuLoad.gameObject.SetActive(false);
			Debug.Log("LoadSubMenu closed");
		}
	}

	public void RefreshLoadButtonLabels()
	{
		var extendedSaveInfos = saveLoadController.GetExtendedSaveInfo();

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

