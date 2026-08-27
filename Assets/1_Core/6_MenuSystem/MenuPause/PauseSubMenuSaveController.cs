using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PauseSubMenuSaveController : MonoBehaviour
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;

	private Bootstrap _bootstrap;
	private GameScenesList _gameScenesList;
	private LocalizationManager _localizationManager;
	private JsonSaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;

	private GameObject _canvasPauseSubMenuSave;

	private TextMeshProUGUI _textComponentPauseSubMenuSave;

	private GameObject _buttonBoxCreateNewGameFile;
	private GameObject _buttonCreateNewGameFile;
	private Button _buttonComponentCreateNewGameFile;
	private TextMeshProUGUI _textButtonComponentCreateNewGameFile;

	private GameObject[] _containersSaveGameFile;

	private GameObject[] _buttonsRewriteGameFile;
	private Button[] _buttonsComponentsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private Button[] _buttonsComponentsDeleteGameFile;
	private TextMeshProUGUI[] _textButtonsComponentsDeleteGameFile;

	private TextMeshProUGUI[] _textComponentsGameFileDateAndTime;
	private TextMeshProUGUI[] _textComponentsGameFileMissionName;
	private TextMeshProUGUI[] _textComponentsGameFileSceneName;
	private Image[] _imagesComponentsSceneGameFile;

	private GameObject _buttonClosePauseSubMenuSave;
	private Button _buttonComponentClosePauseSubMenuSave;
	private TextMeshProUGUI _textButtonComponentClosePauseSubMenuSave;

	private GameObject _scrollbar;
	private Scrollbar _scrollbarComponent;
	private float _scrollbarHandleSize = 0.11f;
	private GameObject _scrollbarHandle;

	private bool _isPauseSubMenuSaveOpened;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		JsonSaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameScenesList gameScenesList,
		GameObject canvasPauseSubMenuSave,
		ViewModelPauseSubMenuSave viewModelPauseSubMenuSave)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_gameScenesList = gameScenesList;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_viewModelPauseSubMenuSave = viewModelPauseSubMenuSave;

		_textComponentPauseSubMenuSave = _viewModelPauseSubMenuSave.TextPauseSubMenuSave.GetComponent<TextMeshProUGUI>();

		_buttonBoxCreateNewGameFile = _viewModelPauseSubMenuSave.ButtonBoxCreateNewGameFile;
		_buttonCreateNewGameFile = _viewModelPauseSubMenuSave.ButtonCreateNewGameFile;
		_buttonComponentCreateNewGameFile = _buttonCreateNewGameFile.GetComponent<Button>();
		_buttonComponentCreateNewGameFile.onClick.AddListener(() =>
		{
			int slotToUse = FindFirstEmptySlot();
			if (slotToUse != -1)
			{
				OnRequestNewSaveFileConfirmation?.Invoke(slotToUse);
			}
		});
		_textButtonComponentCreateNewGameFile = _viewModelPauseSubMenuSave.TextButtonCreateNewGameFile.GetComponent<TextMeshProUGUI>();

		_containersSaveGameFile = new GameObject[_bootstrap.GameData.NumberOfSafeFileSlots];
		_containersSaveGameFile = _viewModelPauseSubMenuSave.ContainersSaveGameFile;

		_buttonsRewriteGameFile = new GameObject[_bootstrap.GameData.NumberOfSafeFileSlots];
		_buttonsRewriteGameFile = _viewModelPauseSubMenuSave.ButtonsRewriteGameFile;
		_buttonsComponentsRewriteGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];

		_textComponentsGameFileDateAndTime = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileMissionName = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imagesComponentsSceneGameFile = new Image[_bootstrap.GameData.NumberOfSafeFileSlots];

		_buttonsDeleteGameFile = new GameObject[_bootstrap.GameData.NumberOfSafeFileSlots];
		_buttonsDeleteGameFile = _viewModelPauseSubMenuSave.ButtonsDeleteGameFile;
		_buttonsComponentsDeleteGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textButtonsComponentsDeleteGameFile = new TextMeshProUGUI[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			int slot = i + 1;

			_buttonsComponentsRewriteGameFile[i] = _buttonsRewriteGameFile[i].GetComponent<Button>();
			_buttonsComponentsRewriteGameFile[i].onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuSave.TextGameFileDateAndTime[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileMissionName[i] = _viewModelPauseSubMenuSave.TextGameFileMissionName[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuSave.TextGameFileSceneName[i].GetComponent<TextMeshProUGUI>();
			_imagesComponentsSceneGameFile[i] = _viewModelPauseSubMenuSave.ImageSceneGameFile[i].GetComponent<Image>();

			_buttonsComponentsDeleteGameFile[i] = _buttonsDeleteGameFile[i].GetComponent<Button>();
			_buttonsComponentsDeleteGameFile[i].onClick.AddListener(() => OnRequestDeleteSaveFileConfirmation?.Invoke(slot));
			_textButtonsComponentsDeleteGameFile[i] = _viewModelPauseSubMenuSave.TextButtonsDeleteGameFile[i].GetComponent<TextMeshProUGUI>();
		}

		_buttonClosePauseSubMenuSave = _viewModelPauseSubMenuSave.ButtonClosePauseSubMenuSave;
		_buttonComponentClosePauseSubMenuSave = _buttonClosePauseSubMenuSave.GetComponent<Button>();
		_buttonComponentClosePauseSubMenuSave.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuSave = _viewModelPauseSubMenuSave.TextButtonClosePauseSubMenuSave.GetComponent<TextMeshProUGUI>();

		_scrollbar = _viewModelPauseSubMenuSave.Scrollbar;
		_scrollbarComponent = _viewModelPauseSubMenuSave.Scrollbar.GetComponent<Scrollbar>();
		Canvas.willRenderCanvases += EnforceFixedHandleSize;
		_scrollbarHandle = _viewModelPauseSubMenuSave.ScrollbarHandle;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileDelete += UpdateAllUIElements;
		_saveLoadController.OnSafeFileSaved += UpdateAllUIElements;

		_pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSaveSubMenuCanvas;

		Debug.Log("PauseSubMenuSaveController Initialized");
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
	}

	private int FindFirstEmptySlot()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			if (string.IsNullOrEmpty(extendedSaveInfos[i].SafefileSceneNameSystem)) 
				return i + 1;
		}
		return -1;
	}

	public void UpdateAllUIElements()
	{
		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;

		if (_buttonBoxCreateNewGameFile.activeSelf != hasEmptySlot)
		{
			_buttonBoxCreateNewGameFile.SetActive(hasEmptySlot);
		}
	}

	private void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		int activeSaveButtonsCount = 0;

		for (int safeFileIndex = 0; safeFileIndex < extendedSaveInfos.Length; safeFileIndex++)
		{
			string currentDateAndTime = extendedSaveInfos[safeFileIndex].SavefileDateAndTime;
			string currentMissionNameSystem = extendedSaveInfos[safeFileIndex].SafeFileMissionNameSystem;
			string currentSceneNameSystem = extendedSaveInfos[safeFileIndex].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentSceneNameSystem))
			{
				_containersSaveGameFile[safeFileIndex].SetActive(true);

				activeSaveButtonsCount++;

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
				_containersSaveGameFile[safeFileIndex].SetActive(false);
			}
		}

		_scrollbarHandle.gameObject.SetActive(activeSaveButtonsCount > 5);
	}

	private void ShowSaveSubMenuCanvas()
	{
		_isPauseSubMenuSaveOpened = true;
		_canvasPauseSubMenuSave.SetActive(true);

		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;
		_buttonBoxCreateNewGameFile.SetActive(hasEmptySlot);
	}

	private void HideSaveSubMenuCanvas()
	{
		if (_isPauseSubMenuSaveOpened)
		{
			_isPauseSubMenuSaveOpened = false;
			_canvasPauseSubMenuSave.SetActive(false);
			Debug.Log("SaveSubMenu closed");
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentPauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_TextPauseSubMenuSave");

		_textButtonComponentCreateNewGameFile.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonCreateNewGameFile");

		for (int i = 0; i < _viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length; i++)
		{
			_textButtonsComponentsDeleteGameFile[i].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonDeleteGameFile");
		}

		_textButtonComponentClosePauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonClosePauseSubMenuSave");
	}
}