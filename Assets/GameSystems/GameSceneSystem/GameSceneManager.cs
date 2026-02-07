using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
	private GameController gameController;
	private GameObject canvasLoadingScreen;
	private TMP_Text loadingScreenText;
	public void Initialize(GameController gameController, GameObject canvasLoadingScreen, TMP_Text loadingScreenText)
	{
		this.gameController = gameController;	
		this.canvasLoadingScreen = canvasLoadingScreen;	
		this.loadingScreenText = loadingScreenText;
		Debug.Log("GameSceneManager Initialized");
	}


	public IEnumerator LoadScene(GameScenesEnum scene)
	{
		gameController.SceneLoadBegan();
		canvasLoadingScreen.SetActive(true);
		loadingScreenText.text = "Подготовка к загрузке...";
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		


		Time.timeScale = 0f; // Устанавливаем таймскейл на паузу

		string sceneName = scene.ToString(); // Преобразуем перечисление в название сцены

		// Проверка и выгрузка предыдущей сцены
		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); // Вторая сцена в индексе - первая загруженная дополнительная сцена

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				loadingScreenText.text = "Выгрузка предыдущей сцены...";
				Debug.Log("Начало выгрузки сцены: " + loadedScene.name);

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded); // Ждем завершения выгрузки

				Debug.Log("Завершение выгрузки сцены: " + loadedScene.name);
				loadingScreenText.text = "Предыдущая сцена выгружена.";
			}
		}

		// Старт загрузки новой сцены
		loadingScreenText.text = "Начало загрузки сцены: " + sceneName;
		Debug.Log("Начало загрузки сцены: " + sceneName);

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Асинхронная загрузка новой сцены

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f); // Нормализуем прогресс загрузки
			loadingScreenText.text = $"Загрузка... {progress * 100:F1}%"; // Показываем процент загрузки
			yield return null; // Пауза на один кадр
		}
		//yield return new WaitForSecondsRealtime(0.05f);
		Debug.Log("Завершение загрузки сцены: " + sceneName);
		loadingScreenText.text = "Нажмите любую клавишу";

		// Ждем нажатия любой клавиши
		yield return new WaitWhile(() => !Input.anyKeyDown);

		canvasLoadingScreen.SetActive(false);
		gameController.SceneLoadEnded();
		Time.timeScale = 1f; // Возвращаем нормальный таймскейл
		Debug.Log($"SceneLoaded {scene}");

		yield break;
	}



	public IEnumerator LoadMainMenuScene()
	{
		gameController.SceneLoadBegan();
		//canvasLoadingScreen.SetActive(true);
		//loadingScreenText.text = "Загрузка";
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 0f; // Устанавливаем таймскейл на паузу

		//string sceneName = scene.ToString(); // Преобразуем перечисление в название сцены

	

		AsyncOperation operation = SceneManager.LoadSceneAsync("SceneMainMenu", LoadSceneMode.Additive); // Загрузка новой сцены асинхронно

		while (!operation.isDone)
		{
			yield return null; // Пауза на один кадр
		}

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//loadingScreenText.text = "Нажмите любую кнопку";

		// Ждём нажатия любой клавиши
		//yield return new WaitWhile(() => !Input.anyKeyDown);

		//canvasLoadingScreen.SetActive(false);
		//gameController.SceneLoadEnded();
		Time.timeScale = 1f; // Возвращаем нормальный таймскейл
							 //IsItFirstTimeLoading = false; ;
							 //Debug.Log($"SceneLoaded {scene}");
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		yield break;
	}
}