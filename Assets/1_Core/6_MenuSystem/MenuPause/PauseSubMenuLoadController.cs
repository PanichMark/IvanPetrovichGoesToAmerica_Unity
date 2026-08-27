using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	public event Action<int> OnRequestLoadSaveFileConfirmation;

	private LocalizationManager _localizationManager;
	private JsonSaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private GameScenesList _gameScenesList;
	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;

	private GameObject _canvasPauseSubMenuLoad;

	private TextMeshProUGUI _textComponentPauseSubMenuLoad;

	private GameObject[] _buttonsLoadGameFile;
	private Button[] _buttonsComponentsLoadGameFile;
	private TextMeshProUGUI[] _textComponentsGameFileDateAndTime;
	private TextMeshProUGUI[] _textComponentsGameFileMissionName;
	private TextMeshProUGUI[] _textComponentsGameFileSceneName;
	private Image[] _imagesComponentsSceneGameFile;

	private GameObject _buttonClosePauseSubMenuLoad;
	private Button _buttonComponentClosePauseSubMenuLoad;
	private TextMeshProUGUI _textButtonComponentClosePauseSubMenuLoad;

	private GameObject _scrollbar;
	private Scrollbar _scrollbarComponent;
	private float _scrollbarHandleSize = 0.11f;
	private GameObject _scrollbarHandle;

	private bool _isPauseSubMenuLoadOpened;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		JsonSaveLoadController saveLoadController,
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
		_textComponentsGameFileMissionName = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imagesComponentsSceneGameFile = new Image[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			int slot = i + 1;

			_buttonsComponentsLoadGameFile[i] = _buttonsLoadGameFile[i].GetComponent<Button>();
			_buttonsComponentsLoadGameFile[i].onClick.AddListener(() => OnRequestLoadSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuLoad.TextGameFileDateAndTime[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileMissionName[i] = _viewModelPauseSubMenuLoad.TextGameFileMissionName[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuLoad.TextGameFileSceneName[i].GetComponent<TextMeshProUGUI>();
			_imagesComponentsSceneGameFile[i] = _viewModelPauseSubMenuLoad.ImageSceneGameFile[i].GetComponent<Image>();
		}

		_buttonClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.ButtonClosePauseSubMenuLoad;
		_buttonComponentClosePauseSubMenuLoad = _buttonClosePauseSubMenuLoad.GetComponent<Button>();
		_buttonComponentClosePauseSubMenuLoad.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextButtonClosePauseSubMenuLoad.GetComponent<TextMeshProUGUI>();

		_scrollbar = _viewModelPauseSubMenuLoad.Scrollbar;
		_scrollbarComponent = _viewModelPauseSubMenuLoad.Scrollbar.GetComponent<Scrollbar>();
		Canvas.willRenderCanvases += EnforceFixedHandleSize;
		_scrollbarHandle = _viewModelPauseSubMenuLoad.ScrollbarHandle;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileSaved += RefreshButtonLabelsAndVisibility; 
		_saveLoadController.OnSafeFileDelete += RefreshButtonLabelsAndVisibility;

		_pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideLoadSubMenuCanvas;

		Debug.Log("PauseSubMenuLoadController");
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
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

		int activeLoadButtonsCount = 0;

		for (int safeFileIndex = 0; safeFileIndex < extendedSaveInfos.Length; safeFileIndex++)
		{
			string currentDateAndTime = extendedSaveInfos[safeFileIndex].SavefileDateAndTime;
			string currentMissionNameSystem = extendedSaveInfos[safeFileIndex].SafeFileMissionNameSystem;
			string currentSceneNameSystem = extendedSaveInfos[safeFileIndex].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentSceneNameSystem))
			{
				_buttonsLoadGameFile[safeFileIndex].SetActive(true);

				activeLoadButtonsCount++;

				_textComponentsGameFileDateAndTime[safeFileIndex].text = currentDateAndTime;
				_textComponentsGameFileMissionName[safeFileIndex].text = _localizationManager.GetLocalizedString(currentMissionNameSystem);
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

		_scrollbarHandle.gameObject.SetActive(activeLoadButtonsCount > 6);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentPauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Load_TextPauseSubMenuLoad");

		_textButtonComponentClosePauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Load_ButtonClosePauseSubMenuLoad");
	}
}