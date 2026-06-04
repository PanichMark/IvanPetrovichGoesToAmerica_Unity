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
	private GameObject _textSceneName;
	private TMP_Text _textComponentSceneName;
	private GameObject _textSceneDescription;
	private TMP_Text _textComponentSceneDescription;
	private GameObject _sliderLoadingStatus;
	private Slider _sliderComponentLoadingStatus;
	private Image _imageLoadingScreen;
	
	public delegate void LoadSceneHandler();
	public event LoadSceneHandler OnBeginLoadingMainMenuScene;
	public event LoadSceneHandler OnEndLoadingMainMenuScene;
	public event LoadSceneHandler OnBeginLoadingGameplayScene;
	public event LoadSceneHandler OnEndLoadingGameplayScene;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameObject canvasLoadingScreen,
		GameObject textLoadingReady,
		GameObject textSceneName,
		GameObject textSceneDescription,
		GameObject sliderLoadingStatus,
		Image imageLoadingScreen)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
		_textLoadingReady = textLoadingReady;
		_textComponentLoadingReady = _textLoadingReady.GetComponent<TMP_Text>();
		_canvasLoadingScreen = canvasLoadingScreen;	
		_textSceneName = textSceneName;
		_textComponentSceneName = _textSceneName.GetComponent<TMP_Text>();
		_textSceneDescription = textSceneDescription;
		_textComponentSceneDescription = _textSceneDescription.GetComponent<TMP_Text>();
		_sliderLoadingStatus = sliderLoadingStatus;
		_sliderComponentLoadingStatus = _sliderLoadingStatus.GetComponent<Slider>();
		_imageLoadingScreen = imageLoadingScreen;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		Debug.Log("GameSceneManager Initialized");
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
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
		
		_textComponentSceneName.text = _localizationManager.GetLocalizedString(sceneName);

		_sliderLoadingStatus.SetActive(true);
		_sliderComponentLoadingStatus.value = 0f;
		_textLoadingReady.SetActive(false);
		_textSceneName.SetActive(true);
		_textSceneDescription.SetActive(true);

		string languageSuffix = _localizationManager.GetLanguageSuffix();

		string descriptionFileName = $"Text_Description_{sceneName}_{languageSuffix}";

		TextAsset descriptionTextAsset = Resources.Load<TextAsset>($"Texts/Texts_Descriptions/Texts_Descriptions_Scenes/{descriptionFileName}");

		if (descriptionTextAsset != null)
		{
			_textComponentSceneDescription.text = descriptionTextAsset.text;
		}
		else
		{
			_textComponentSceneDescription.text = ($"SCENE DESCRIPTION FOR \"{languageSuffix}\" NOT FOUND");
			Debug.LogWarning($"SCENE DESCRIPTION FOR \"{languageSuffix}\" NOT FOUND");
		}

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1);

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				float unloadProgress = 0f;
				float unloadTarget = 0.5f; 

				Debug.Log($"Scene_{loadedScene.name} UNloading started");

				AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadedScene);

				while (!unloadOperation.isDone)
				{
					unloadProgress = Mathf.Lerp(0, unloadTarget, unloadOperation.progress);
					_sliderComponentLoadingStatus.value = unloadProgress;
					yield return null;
				}
				_sliderComponentLoadingStatus.value = unloadTarget;

				Debug.Log($"Scene_{loadedScene.name} UNloading ended");
			}
		}
		
		Debug.Log($"{sceneName} loading started");

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 

		while (!operation.isDone)
		{
			float loadProgressForSlider = Mathf.Lerp(0.5f, 1f, operation.progress / 0.9f);

			_sliderComponentLoadingStatus.value = loadProgressForSlider;

			yield return null;
		}

		_sliderComponentLoadingStatus.value = 1f;
		_sliderLoadingStatus.SetActive(false);
		_textLoadingReady.SetActive(true);
		_textComponentLoadingReady.text = _localizationManager.GetLocalizedString("UI_LoadingScreen_LoadingIsReady");

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

		_sliderLoadingStatus.SetActive(false);
		_textSceneName.SetActive(false);
		_textSceneDescription.SetActive(false);
		_textLoadingReady.SetActive(false);

		Sprite spriteToUse = Resources.Load<Sprite>("Sprites/Sprites_LoadingScreens/Scene_0_MainMenu");
		_imageLoadingScreen.sprite = spriteToUse;

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
		OnEndLoadingMainMenuScene?.Invoke();
		_gameController.MainMenuSceneLoadEnded();
		Debug.Log("Scene_MainMenu loading ended");
	
		_canvasLoadingScreen.SetActive(false);
		yield break;
	}

	public void SaveData(ref GameData data)
	{
		data.CurrentSceneNameSystem = SceneManager.GetSceneAt(1).name;
	}

	public void LoadData(GameData data)
	{

	}
}