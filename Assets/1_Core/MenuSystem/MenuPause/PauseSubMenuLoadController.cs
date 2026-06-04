using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	public event Action<int> OnRequestLoadSaveFileConfirmation;

	private LocalizationManager _localizationManager;
	private SaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

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
		LocalizationManager localizationManager,
		SaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuLoad,
		ViewModelPauseSubMenuLoad viewModelPauseSubMenuLoad)
	{
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_viewModelPauseSubMenuLoad = viewModelPauseSubMenuLoad;

		_textComponentPauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextPauseSubMenuLoad.GetComponent<TextMeshProUGUI>();

		_buttonsLoadGameFile = _viewModelPauseSubMenuLoad.ButtonsLoadGameFile;
		_buttonsComponentsLoadGameFile = new Button[_viewModelPauseSubMenuLoad.ButtonsLoadGameFile.Length];

		_textComponentsGameFileDateAndTime = new TextMeshProUGUI[_viewModelPauseSubMenuLoad.ButtonsLoadGameFile.Length];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_viewModelPauseSubMenuLoad.ButtonsLoadGameFile.Length];
		_imagesComponentsSceneGameFile = new Image[_viewModelPauseSubMenuLoad.ButtonsLoadGameFile.Length];

		for (int i = 0; i < _viewModelPauseSubMenuLoad.ButtonsLoadGameFile.Length; i++)
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
		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("PauseSubMenuLoadController");
	}

	private void DisableButtons()
	{
		foreach (var button in _buttonsComponentsLoadGameFile)
		{
			button.interactable = false;
		}

		_buttonComponentClosePauseSubMenuLoad.interactable = false;
	}

	private void EnableButtons()
	{
		foreach (var button in _buttonsComponentsLoadGameFile)
		{
			button.interactable = true;
		}

		_buttonComponentClosePauseSubMenuLoad.interactable = true;
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

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			string currentDateAndTime = extendedSaveInfos[i].SavefileDateAndTime;
			string currentSceneNameSystem = extendedSaveInfos[i].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentSceneNameSystem))
			{
				_buttonsLoadGameFile[i].SetActive(true);

				_textComponentsGameFileDateAndTime[i].text = currentDateAndTime;
				_textComponentsGameFileSceneName[i].text = _localizationManager.GetLocalizedString(currentSceneNameSystem);

				Sprite spriteScene = Resources.Load<Sprite>($"Sprites/Sprites_LoadingScreens/{currentSceneNameSystem}");

				_imagesComponentsSceneGameFile[i].sprite = spriteScene;
			}
			else
			{
				_buttonsLoadGameFile[i].SetActive(false);
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