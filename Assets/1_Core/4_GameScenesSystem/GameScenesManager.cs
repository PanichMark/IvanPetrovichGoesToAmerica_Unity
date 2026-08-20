using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScenesManager : MonoBehaviour, ISaveLoad
{
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
		Debug.Log("GameSceneManager Initialized");
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	}

	public IEnumerator LoadGameplayScene(GameScenesSystemEnum scene)
	{
		_gameController.GameplaySceneLoadBegan();
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

		_gameController.GameplaySceneLoadEnded();

		_gameController.BlockInput();

		yield return new WaitWhile(() => !Input.anyKeyDown);

		_canvasLoadingScreen.SetActive(false);

		_gameController.UnblockInput();

		Time.timeScale = 1f;

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

	public void SaveData(ref GameData data)
	{
		data.Scene = SceneManager.GetSceneAt(1).name;
	}

	public void LoadData(GameData data)
	{

	}
}