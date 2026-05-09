using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour, ISaveLoad
{
	private GameController _gameController;
	private LocalizationManager _localizationManager;

	private GameObject _canvasLoadingScreen;
	private TMP_Text _textLoadingScreenStatus;
	private TMP_Text _textSceneName;
	private Image _imageLoadingScreen;
	
	public delegate void LoadSceneHandler();
	public event LoadSceneHandler OnBeginLoadMainMenuScene;
	public event LoadSceneHandler OnEndLoadMainMenuScene;
	public event LoadSceneHandler OnBeginLoadGameplayScene;
	public event LoadSceneHandler OnEndLoadGameplayScene;

	public void Initialize(GameController gameController, GameObject canvasLoadingScreen, TMP_Text textLoadingScreenStatus, TMP_Text textSceneName, Image imageLoadingScreen)
	{
		_gameController = gameController;	

		_canvasLoadingScreen = canvasLoadingScreen;	
		_textLoadingScreenStatus = textLoadingScreenStatus;
		_textSceneName = textSceneName;
		_imageLoadingScreen = imageLoadingScreen;

		Debug.Log("GameSceneManager Initialized");
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		this._localizationManager = localizationManager;
	}
	public IEnumerator LoadGameplayScene(GameScenesEnum scene)
	{

		_gameController.SceneLoadBegan();
		OnBeginLoadGameplayScene?.Invoke();
		_canvasLoadingScreen.SetActive(true);
		_textLoadingScreenStatus.text = "Подготовка к загрузке...";
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		Time.timeScale = 0f;

		string sceneName = scene.ToString(); 

		Sprite spriteToUse = Resources.Load<Sprite>($"Sprites/{sceneName}");

		_imageLoadingScreen.sprite = spriteToUse;
		_textSceneName.text = _localizationManager.GetLocalizedString(sceneName);

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				_textLoadingScreenStatus.text = "Выгрузка предыдущей сцены...";
				Debug.Log($"Scene_{loadedScene.name} UNloading started");

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log($"Scene_{loadedScene.name} UNloading ended");
				_textLoadingScreenStatus.text = "Предыдущая сцена выгружена.";
			}
		}

		_textLoadingScreenStatus.text = "Начало загрузки сцены: " + sceneName;
		Debug.Log($"{sceneName} loading started");

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			_textLoadingScreenStatus.text = $"Загрузка... {progress * 100:F1}%";
			yield return null;
		}
	
		Debug.Log($"{sceneName} loading ended");
		_textLoadingScreenStatus.text = "Нажмите любую клавишу";
		OnEndLoadGameplayScene?.Invoke();
	
		yield return new WaitWhile(() => !Input.anyKeyDown);

		_canvasLoadingScreen.SetActive(false);
		
		Time.timeScale = 1f; 
		_gameController.SceneLoadEnded();
		Debug.Log($"SceneLoaded {scene}");

		yield break;
	}

	public IEnumerator LoadMainMenuScene()
	{
		_gameController.OpenMainMenu();
		OnBeginLoadMainMenuScene?.Invoke();
		_canvasLoadingScreen.SetActive(true);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 0f; 

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{

				_textLoadingScreenStatus.text = "Выгрузка предыдущей сцены...";
				Debug.Log("Начало выгрузки сцены: " + loadedScene.name);

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log("Завершение выгрузки сцены: " + loadedScene.name);
				_textLoadingScreenStatus.text = "Предыдущая сцена выгружена.";
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
		_gameController.OpenMainMenu();
		Debug.Log("Scene_MainMenu loading ended");
	
		_canvasLoadingScreen.SetActive(false);
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