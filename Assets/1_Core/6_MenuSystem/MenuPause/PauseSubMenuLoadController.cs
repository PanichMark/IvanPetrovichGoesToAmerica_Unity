using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	public event Action<int> OnRequestLoadSaveFileConfirmation;

	private LocalizationManager _localizationManager;
	private SaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private GameScenesList _gameScenesList;

	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;

	private GameObject _canvasPauseSubMenuLoad;

	private TextMeshProUGUI _textComponentPauseSubMenuLoad;

	private GameObject[] _buttonsLoadGameFile;
	private Button[] _buttonsComponentsLoadGameFile;
	private TextMeshProUGUI[] _textComponentsGameFileDateAndTime;
	private TextMeshProUGUI[] _textComponentsGameFileSceneName;
	private Image[] _imagesComponentsSceneGameFile;

	private GameObject _buttonClosePauseSubMenuLoad;
	private Button _buttonComponentClosePauseSubMenuLoad;
	private TextMeshProUGUI _textButtonComponentClosePauseSubMenuLoad;

	private bool _isPauseSubMenuLoadOpened;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		SaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameScenesList gameScenesList,
		GameObject canvasPauseSubMenuLoad,
		ViewModelPauseSubMenuLoad viewModelPauseSubMenuLoad)
	{
		_bootstrap = bootstrap;
		_gameScenesList	= gameScenesList;
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_viewModelPauseSubMenuLoad = viewModelPauseSubMenuLoad;

		_textComponentPauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextPauseSubMenuLoad.GetComponent<TextMeshProUGUI>();

		_buttonsLoadGameFile = _viewModelPauseSubMenuLoad.ButtonsLoadGameFile;
		_buttonsComponentsLoadGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];

		_textComponentsGameFileDateAndTime = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imagesComponentsSceneGameFile = new Image[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			int slot = i + 1;

			_buttonsComponentsLoadGameFile[i] = _buttonsLoadGameFile[i].GetComponent<Button>();
			_buttonsComponentsLoadGameFile[i].onClick.AddListener(() => OnRequestLoadSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuLoad.TextGameFileDateAndTime[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuLoad.TextGameFileSceneName[i].GetComponent<TextMeshProUGUI>();
			_imagesComponentsSceneGameFile[i] = _viewModelPauseSubMenuLoad.ImageSceneGameFile[i].GetComponent<Image>();
		}

		_buttonClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.ButtonClosePauseSubMenuLoad;
		_buttonComponentClosePauseSubMenuLoad = _buttonClosePauseSubMenuLoad.GetComponent<Button>();
		_buttonComponentClosePauseSubMenuLoad.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextButtonClosePauseSubMenuLoad.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileSaved += RefreshButtonLabelsAndVisibility; 
		_saveLoadController.OnSafeFileDelete += RefreshButtonLabelsAndVisibility;

		_pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideLoadSubMenuCanvas;

		Debug.Log("PauseSubMenuLoadController");
	}

	public void ShowLoadSubMenuCanvas()
	{
		_isPauseSubMenuLoadOpened = true;
		_canvasPauseSubMenuLoad.SetActive(true);

		RefreshButtonLabelsAndVisibility();
	}

	public void HideLoadSubMenuCanvas()
	{
		if (_isPauseSubMenuLoadOpened)
		{
			_isPauseSubMenuLoadOpened = false;
			_canvasPauseSubMenuLoad.SetActive(false);
			Debug.Log("New Load SubMenu closed");
		}
	}

	public void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int safeFileIndex = 0; safeFileIndex < extendedSaveInfos.Length; safeFileIndex++)
		{
			string currentDateAndTime = extendedSaveInfos[safeFileIndex].SavefileDateAndTime;
			string currentSceneNameSystem = extendedSaveInfos[safeFileIndex].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentSceneNameSystem))
			{
				_buttonsLoadGameFile[safeFileIndex].SetActive(true);

				_textComponentsGameFileDateAndTime[safeFileIndex].text = currentDateAndTime;
				_textComponentsGameFileSceneName[safeFileIndex].text = _localizationManager.GetLocalizedString(currentSceneNameSystem);

				for (int sceneDataIndex = 0; sceneDataIndex < _gameScenesList.GameScenes.Count; sceneDataIndex++)
				{
					if (_gameScenesList.GameScenes[sceneDataIndex].GameScene.ToString() == currentSceneNameSystem)
					{
						_imagesComponentsSceneGameFile[safeFileIndex].sprite = _gameScenesList.GameScenes[sceneDataIndex].SceneLoadingScreenImage;
					}
				}
			}
			else
			{
				_buttonsLoadGameFile[safeFileIndex].SetActive(false);
			}
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentPauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuLoad_TextPauseSubMenuLoad");

		_textButtonComponentClosePauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuLoad_ButtonClosePauseSubMenuLoad");
	}
}