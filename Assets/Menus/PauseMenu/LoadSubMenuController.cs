using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSubMenuController : MonoBehaviour
{
	PauseMenuController pauseMenuController;

	public Canvas LoadSubMenuCanvas;

	public Button CloseLoadSubMenuButton;

	public Button[] LoadGameButtons; // Массив всех кнопок загрузки игры

	private Text[] currentDateAndTimeTexts;
	private Text[] currentSceneNameUITexts;

	void Start()
	{
		pauseMenuController = GetComponent<PauseMenuController>();

		CloseLoadSubMenuButton.onClick.AddListener(CloseLoadSubMenu);

		// Добавляем обработчики кликов на каждую кнопку
		for (int i = 0; i < LoadGameButtons.Length; i++)
		{
			int index = i + 1; // Индекс сохранения начинается с 1
			LoadGameButtons[i].onClick.AddListener(() =>
			{
				DataPersistenceManager.Instance.LoadGame(index); // Передаем индекс сохранения
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

	private void Update()
	{
		if (InputManager.Instance.GetKeyPauseMenu() && LoadSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseLoadSubMenu();
		}
	}

	public void CloseLoadSubMenu()
	{
		LoadSubMenuCanvas.gameObject.SetActive(false);
		pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);
		Debug.Log("LoadSubMenu closed");
	}

	public void RefreshLoadButtonLabels()
	{
		var extendedSaveInfos = DataPersistenceManager.Instance.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			var (currentDataAndTime, currentSceneNameUI, currentSceneNameSystem) = extendedSaveInfos[i];

			if (!string.IsNullOrEmpty(currentSceneNameSystem)) // Проверяем наличие сцены
			{
				LoadGameButtons[i].gameObject.SetActive(true);

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
					LoadGameButtons[i].transform.Find("Level_Image").gameObject.SetActive(true);
					LoadGameButtons[i].transform.Find("Level_Image").GetComponent<Image>().sprite = sprite;
				}
				else
				{
					Debug.LogError("Failed to load Scene Background Image");
				}
			}
			else
			{
				// Скрываем ненужные элементы интерфейса
				LoadGameButtons[i].gameObject.SetActive(false);
			}
		}
	}
}