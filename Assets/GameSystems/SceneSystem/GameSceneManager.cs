using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
	private GameController gameController;
	private GameObject canvasLoadingScreen;
	private TMP_Text loadingScreenText;
	private bool IsItFirstTimeLoading;
	public void Initialize(GameController gameController, GameObject canvasLoadingScreen, TMP_Text loadingScreenText)
	{
		this.gameController = gameController;	
		this.canvasLoadingScreen = canvasLoadingScreen;	
		this.loadingScreenText = loadingScreenText;
		IsItFirstTimeLoading = true;
		Debug.Log("GameSceneManager Initialized");
	}


	// Метод-корутина для загрузки сцены в аддитивном режиме
	public IEnumerator LoadScene(ScenesEnum scene)
	{
		gameController.SceneLoadBegan();
		canvasLoadingScreen.SetActive(true);
		loadingScreenText.text = "Загрузка";

		Time.timeScale = 0f; // Устанавливаем таймскейл на паузу

		string sceneName = scene.ToString(); // Преобразуем перечисление в название сцены

		// НЕ УДАЛЯТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		if (!IsItFirstTimeLoading)
		{
			// Проверяем наличие активной аддитивной сцены
			Scene loadedScene = SceneManager.GetSceneAt(1); // Вторая сцена в индексе - первая загруженная дополнительная сцена

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				// Выгрузка предыдущей дополнительной сцены
				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded); // Ждем завершения выгрузки
			}
		}

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Загрузка новой сцены асинхронно

		while (!operation.isDone)
		{
			yield return null; // Пауза на один кадр
		}

		loadingScreenText.text = "Нажмите любую кнопку";

		// Ждём нажатия любой клавиши
		yield return new WaitWhile(() => !Input.anyKeyDown);

		canvasLoadingScreen.SetActive(false);
		gameController.SceneLoadEnded();
		Time.timeScale = 1f; // Возвращаем нормальный таймскейл
		IsItFirstTimeLoading = false; ;
		Debug.Log($"SceneLoaded {scene}");
	}
}