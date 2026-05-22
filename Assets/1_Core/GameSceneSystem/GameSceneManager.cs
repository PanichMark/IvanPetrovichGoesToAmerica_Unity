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
	private TMP_Text _textComponentLoadingReady;
	private GameObject _textLoadingReady;
	private TMP_Text _textSceneName;
	private TMP_Text _textSceneDescription;
	private Slider _sliderLoadingStatus;
	private Image _imageLoadingScreen;
	
	public delegate void LoadSceneHandler();
	public event LoadSceneHandler OnBeginLoadingMainMenuScene;
	public event LoadSceneHandler OnEndLoadingMainMenuScene;
	public event LoadSceneHandler OnBeginLoadingGameplayScene;
	public event LoadSceneHandler OnEndLoadingGameplayScene;

	public void Initialize(GameController gameController,
		GameObject canvasLoadingScreen,
		GameObject textComponentLoadingReady,
		TMP_Text textSceneName,
		TMP_Text textSceneDescription,
		Slider sliderLoadingStatus,
		Image imageLoadingScreen)
	{
		_gameController = gameController;	

		_canvasLoadingScreen = canvasLoadingScreen;	
		_textSceneName = textSceneName;
		_textSceneDescription = textSceneDescription;
		_sliderLoadingStatus = sliderLoadingStatus;
		_imageLoadingScreen = imageLoadingScreen;

		Debug.Log("GameSceneManager Initialized");
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	}
	public IEnumerator LoadGameplayScene(GameScenesEnum scene)
	{
		_gameController.GameplaySceneLoadBegan();
		OnBeginLoadingGameplayScene?.Invoke();
		_canvasLoadingScreen.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		Time.timeScale = 0f;

		string sceneName = scene.ToString(); 

		Sprite spriteToUse = Resources.Load<Sprite>($"Sprites/Sprites_LoadingScreens/{sceneName}");

		_imageLoadingScreen.sprite = spriteToUse;
		
		_textSceneName.text = _localizationManager.GetLocalizedString(sceneName);

		string languageSuffix = "RU";
		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			languageSuffix = "RU";
		}
		else if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
		{
			languageSuffix = "EN";
		}

		string descriptionFileName = $"Text_Description_{sceneName}_{languageSuffix}";

		TextAsset descriptionTextAsset = Resources.Load<TextAsset>($"Texts/Texts_Descriptions/Texts_Descriptions_Scenes/{descriptionFileName}");

		if (descriptionTextAsset != null)
		{
			_textSceneDescription.text = descriptionTextAsset.text;
		}
		else
		{
			_textSceneDescription.text = ($"SCENE DESCRIPTION FOR \"{languageSuffix}\" NOT FOUND");
			Debug.LogWarning($"SCENE DESCRIPTION FOR \"{languageSuffix}\" NOT FOUND");
		}

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				
				Debug.Log($"Scene_{loadedScene.name} UNloading started");

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log($"Scene_{loadedScene.name} UNloading ended");
				
			}
		}

		
		Debug.Log($"{sceneName} loading started");

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			
			yield return null;
		}
	
		Debug.Log($"{sceneName} loading ended");
		
		OnEndLoadingGameplayScene?.Invoke();
	
		yield return new WaitWhile(() => !Input.anyKeyDown);

		_canvasLoadingScreen.SetActive(false);
		
		Time.timeScale = 1f; 
		_gameController.GameplaySceneLoadEnded();
		Debug.Log($"SceneLoaded {scene}");

		yield break;
	}

	public IEnumerator LoadMainMenuScene()
	{
		_gameController.MainMenuSceneLoadBegan();
		OnBeginLoadingMainMenuScene?.Invoke();
		_canvasLoadingScreen.SetActive(true);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 0f; 

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1); 

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{

				
				Debug.Log("Начало выгрузки сцены: " + loadedScene.name);

				SceneManager.UnloadSceneAsync(loadedScene);
				yield return new WaitUntil(() => !loadedScene.isLoaded);

				Debug.Log("Завершение выгрузки сцены: " + loadedScene.name);
			}
		}
		Debug.Log("Scene_MainMenu loading started");
		AsyncOperation operation = SceneManager.LoadSceneAsync("Scene0_MainMenu", LoadSceneMode.Additive);

		while (!operation.isDone)
		{
			yield return null; 
		}

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		Time.timeScale = 1f; 
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		OnEndLoadingMainMenuScene?.Invoke();
		_gameController.MainMenuSceneLoadEnded();
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