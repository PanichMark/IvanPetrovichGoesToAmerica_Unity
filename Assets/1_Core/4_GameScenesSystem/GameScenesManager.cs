using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScenesManager : MonoBehaviour, ISaveLoad
{
	public bool IsWaitingForGameplayDataToLoad {  get; private set; }
	public bool HasLoadedGameplayScene { get; private set; }
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private GameObject _canvasLoadingScreen;
	private TMP_Text _textComponentLoadingReady;
	private GameObject _textLoadingReady;
	private GameObject _textMissionName;
	private TMP_Text _textComponentMissionName;
	private GameObject _textSceneName;
	private TMP_Text _textComponentSceneName;
	private GameObject _textSceneDescription;
	private TMP_Text _textComponentSceneDescription;
	private GameObject _sliderLoadingStatus;
	private Slider _sliderComponentLoadingStatus;
	private GameObject _imageLoadingScreen;
	private Image _imageComponentLoadingScreen;
	private GameScenesList _gameScenesList;
	public delegate void LoadSceneHandler();
	public event LoadSceneHandler OnBeginLoadingMainMenuScene;
	public event LoadSceneHandler OnEndLoadingMainMenuScene;
	public event LoadSceneHandler OnBeginLoadingGameplayScene;
	public event LoadSceneHandler OnEndLoadingGameplayScene;

	//private bool _isInitialSceneLoad;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameScenesList gameScenesList,
		GameObject canvasLoadingScreen,
		ViewModelSceneLoadingScreen viewModelSceneLoadingScreen)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
		_textLoadingReady = viewModelSceneLoadingScreen.TextLoadingIsReady;
		_gameScenesList = gameScenesList;
		_textComponentLoadingReady = _textLoadingReady.GetComponent<TMP_Text>();
		_canvasLoadingScreen = canvasLoadingScreen;
		_textSceneName = viewModelSceneLoadingScreen.TextSceneName;
		_textComponentSceneName = _textSceneName.GetComponent<TMP_Text>();
		_textSceneDescription = viewModelSceneLoadingScreen.TextSceneDescription;
		_textComponentSceneDescription = _textSceneDescription.GetComponent<TMP_Text>();
		_sliderLoadingStatus = viewModelSceneLoadingScreen.SliderSceneLoadingStatus;
		_sliderComponentLoadingStatus = _sliderLoadingStatus.GetComponent<Slider>();
		_imageLoadingScreen = viewModelSceneLoadingScreen.ImageScene;
		_imageComponentLoadingScreen = viewModelSceneLoadingScreen.ImageScene.GetComponent<Image>();

		_textMissionName = viewModelSceneLoadingScreen.TextMissionName;
		_textComponentMissionName = viewModelSceneLoadingScreen.TextMissionName.GetComponent<TMP_Text>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		//_isInitialSceneLoad = true;
		Debug.Log("GameSceneManager Initialized");
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	}

	public IEnumerator LoadGameplayScene(GameScenesSystemEnum scene)
	{
		Debug.Log($"Loading scene {scene} Started Initial");

		HasLoadedGameplayScene = false;
		_gameController.GameplaySceneLoadBegan();

		//if (_isInitialSceneLoad == false)
		//{
		IsWaitingForGameplayDataToLoad = true;
		//}

		OnBeginLoadingGameplayScene?.Invoke();
		_canvasLoadingScreen.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		Time.timeScale = 0f;

		string sceneName = scene.ToString();
		string missionName = null;

		Sprite spriteToUse = null;

		int currentSceneData = 0;

		for (currentSceneData = 0; currentSceneData < _gameScenesList.GameScenes.Count; currentSceneData++)
		{
			if (_gameScenesList.GameScenes[currentSceneData].GameScene.ToString() == sceneName)
			{
				spriteToUse = _gameScenesList.GameScenes[currentSceneData].SceneLoadingScreenImage;
				missionName = _gameScenesList.GameScenes[currentSceneData].GameMissionName.ToString();
				break;
			}
		}

		_imageComponentLoadingScreen.sprite = spriteToUse;

		_textComponentMissionName.text = _localizationManager.GetLocalizedString(missionName);
		_textComponentSceneName.text = _localizationManager.GetLocalizedString(sceneName);

		_sliderLoadingStatus.SetActive(true);
		_sliderComponentLoadingStatus.value = 0f;
		_textLoadingReady.SetActive(false);
		_textSceneName.SetActive(true);
		_textSceneDescription.SetActive(true);

		TextAsset descriptionTextAsset = null;

		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			descriptionTextAsset = _gameScenesList.GameScenes[currentSceneData].SceneDescription_RU;
		}
		if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
		{
			descriptionTextAsset = _gameScenesList.GameScenes[currentSceneData].SceneDescription_EN;
		}

		//TextAsset descriptionTextAsset = Resources.Load<TextAsset>($"Texts/Texts_Descriptions/Texts_Descriptions_Scenes/{descriptionFileName}");

		if (descriptionTextAsset != null)
		{
			_textComponentSceneDescription.text = descriptionTextAsset.text;
		}
		else
		{
			_textComponentSceneDescription.text = ($"SCENE DESCRIPTION FOR \"{_localizationManager.CurrentLanguage}\" NOT FOUND");
			Debug.LogWarning($"SCENE DESCRIPTION FOR \"{_localizationManager.CurrentLanguage}\" NOT FOUND");
		}

		if (SceneManager.sceneCount > 1)
		{
			Scene loadedScene = SceneManager.GetSceneAt(1);

			if (loadedScene.isLoaded && loadedScene.buildIndex != SceneManager.GetActiveScene().buildIndex)
			{
				float unloadProgress = 0f;
				float unloadTarget = 0.25f;

				Debug.Log($"Unloading scene {loadedScene.name} Started");

				AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadedScene);
				var unloadingSceneName = loadedScene.name;
				while (!unloadOperation.isDone)
				{
					unloadProgress = Mathf.Lerp(0, unloadTarget, unloadOperation.progress);
					_sliderComponentLoadingStatus.value = unloadProgress;
					yield return null;
				}
				_sliderComponentLoadingStatus.value = unloadTarget;

				Debug.Log($"Unloading scene {unloadingSceneName} Ended");
			}
		}

		Debug.Log($"Loading scene {sceneName} Started");

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		float loadProgress = 0.25f;
		float loadTarget = 0.5f;

		while (!operation.isDone)
		{
			float loadProgressForSlider = Mathf.Lerp(loadProgress, loadTarget, operation.progress / 0.9f);

			_sliderComponentLoadingStatus.value = loadProgressForSlider;

			yield return null;
		}

		HasLoadedGameplayScene = true;



		Debug.Log($"Loading scene {sceneName} Ended");

		OnEndLoadingGameplayScene?.Invoke();

		_gameController.GameplaySceneLoadEnded();

		_gameController.BlockInput();

		//if (_isInitialSceneLoad == false)
		//{
		yield return new WaitWhile(() => IsWaitingForGameplayDataToLoad == true);
		//}

		_sliderComponentLoadingStatus.value = 1f;
		_sliderLoadingStatus.SetActive(false);
		_textLoadingReady.SetActive(true);
		_textComponentLoadingReady.text = _localizationManager.GetLocalizedString("UI_LoadingScreen_LoadingIsReady");

		yield return new WaitWhile(() => !Input.anyKeyDown);

		_canvasLoadingScreen.SetActive(false);

		_gameController.UnblockInput();

		Time.timeScale = 1f;

		//_isInitialSceneLoad = false;

		Debug.Log($"Loading scene {sceneName} Ended Initial");

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
		_imageComponentLoadingScreen.sprite = spriteToUse;

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

	public IEnumerator SaveData(GameData data)
	{
		data.Scene = SceneManager.GetSceneAt(1).name;
		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		yield return null;
	}

	public void ApplyGameplayDataLoadingFinished() 
	{
		Debug.Log("LOADED GAMEPLAY AFTER LOAD!");
		IsWaitingForGameplayDataToLoad = false;
	}

	// Добавь этот публичный метод в класс GameScenesManager
	public void SetLoadingSliderValue(float value)
	{
		_sliderComponentLoadingStatus.value = value;
	}
}