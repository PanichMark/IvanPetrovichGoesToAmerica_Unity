using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour, ISaveLoad
{
	private GameController gameController;
	private LocalizationManager localizationManager;

	private GameObject canvasLoadingScreen;
	private TMP_Text loadingScreenText;
	private TMP_Text sceneNameText;
	private Image sceneLoadingScreenImage;
	
	public delegate void LoadSceneHandler();
	public event LoadSceneHandler OnBeginLoadMainMenuScene;
	public event LoadSceneHandler OnEndLoadMainMenuScene;
	public event LoadSceneHandler OnBeginLoadGameplayScene;
	public event LoadSceneHandler OnEndLoadGameplayScene;

	public void Initialize(GameController gameController, GameObject canvasLoadingScreen, TMP_Text loadingScreenText, TMP_Text sceneNameText, Image sceneLoadingScreenImage)
	{
		this.gameController = gameController;	

		this.canvasLoadingScreen = canvasLoadingScreen;	
		this.loadingScreenText = loadingScreenText;
		this.sceneNameText = sceneNameText;
		this.sceneLoadingScreenImage = sceneLoadingScreenImage;

		Debug.Log("GameSceneManager Initialized");
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		this.localizationManager = localizationManager;
	}
	public IEnumerator LoadScene(GameScenesEnum scene)
	{

		gameController.SceneLoadBegan();
		OnBeginLoadGameplayScene?.Invoke();
		canvasLoadingScreen.SetActive(true);
		loadingScreenText.text = "Подготовка к загрузке...";
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		Time.timeScale = 0f;

		string sceneName = scene.ToString(); 

		Sprite spriteToUse = Resources.Load<Sprite>($"Sprites/{sceneName}");

		sceneLoadingScreenImage.sprite = spriteToUse;
		sceneNameText.text = localizationManager.GetLocalizedString(sceneName);

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				loadingScreenText.text = "Выгрузка предыдущей сцены...";
				Debug.Log($"Scene_{loadedScene.name} UNloading started");

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log($"Scene_{loadedScene.name} UNloading ended");
				loadingScreenText.text = "Предыдущая сцена выгружена.";
			}
		}

		loadingScreenText.text = "Начало загрузки сцены: " + sceneName;
		Debug.Log($"{sceneName} loading started");

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			loadingScreenText.text = $"Загрузка... {progress * 100:F1}%";
			yield return null;
		}
	
		Debug.Log($"{sceneName} loading ended");
		loadingScreenText.text = "Нажмите любую клавишу";
		OnEndLoadGameplayScene?.Invoke();
	
		yield return new WaitWhile(() => !Input.anyKeyDown);

		canvasLoadingScreen.SetActive(false);
		
		Time.timeScale = 1f; 
		gameController.SceneLoadEnded();
		Debug.Log($"SceneLoaded {scene}");

		yield break;
	}

	public IEnumerator LoadMainMenuScene()
	{
		gameController.OpenMainMenu();
		OnBeginLoadMainMenuScene?.Invoke();
		canvasLoadingScreen.SetActive(true);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 0f; 

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{

				loadingScreenText.text = "Выгрузка предыдущей сцены...";
				Debug.Log("Начало выгрузки сцены: " + loadedScene.name);

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log("Завершение выгрузки сцены: " + loadedScene.name);
				loadingScreenText.text = "Предыдущая сцена выгружена.";
			}
		}
		Debug.Log("Scene_MainMenu loading started");
		AsyncOperation operation = SceneManager.LoadSceneAsync("Scene_0_MainMenu", LoadSceneMode.Additive);

		while (!operation.isDone)
		{
			yield return null; 
		}

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		Time.timeScale = 1f; 
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		OnEndLoadMainMenuScene?.Invoke();
		gameController.OpenMainMenu();
		Debug.Log("Scene_MainMenu loading ended");
	
		canvasLoadingScreen.SetActive(false);
		yield break;
	}

	public void SaveData(ref GameData data)
	{
		data.CurrentSceneNameSystem = SceneManager.GetSceneAt(1).name;
		data.CurrentSceneNameUI = SceneManager.GetSceneAt(1).name;
	}

	public void LoadData(GameData data)
	{
	}
}