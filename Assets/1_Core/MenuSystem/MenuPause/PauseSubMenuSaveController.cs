using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PauseSubMenuSaveController : MonoBehaviour
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;

	private LocalizationManager _localizationManager;
	private SaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;

	private GameObject _canvasPauseSubMenuSave;

	private TextMeshProUGUI _textComponentPauseSubMenuSave;

	private GameObject _buttonCreateNewGameFile;
	private Button _buttonComponentCreateNewGameFile;
	private TextMeshProUGUI _textButtonComponentCreateNewGameFile;

	private GameObject[] _buttonsRewriteGameFile;
	private Button[] _buttonsComponentsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private Button[] _buttonsComponentsDeleteGameFile;
	private TextMeshProUGUI[] _textButtonsComponentsDeleteGameFile;

	private TextMeshProUGUI[] _textComponentsGameFileDateAndTime;
	private TextMeshProUGUI[] _textComponentsGameFileSceneName;
	private Image[] _imagesComponentsSceneGameFile;

	private GameObject _buttonClosePauseSubMenuSave;
	private Button _buttonComponentClosePauseSubMenuSave;
	private TextMeshProUGUI _textButtonComponentClosePauseSubMenuSave;

	private bool _isPauseSubMenuSaveOpened;

	public void Initialize(
		LocalizationManager localizationManager,
		SaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSave,
		ViewModelPauseSubMenuSave viewModelPauseSubMenuSave)
	{
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_viewModelPauseSubMenuSave = viewModelPauseSubMenuSave;

		_textComponentPauseSubMenuSave = _viewModelPauseSubMenuSave.TextPauseSubMenuSave.GetComponent<TextMeshProUGUI>();

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

		_buttonsRewriteGameFile = new GameObject[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];
		_buttonsRewriteGameFile = _viewModelPauseSubMenuSave.ButtonsRewriteGameFile;
		_buttonsComponentsRewriteGameFile = new Button[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];

		_textComponentsGameFileDateAndTime = new TextMeshProUGUI[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];
		_imagesComponentsSceneGameFile = new Image[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];

		_buttonsDeleteGameFile = new GameObject[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];
		_buttonsDeleteGameFile = _viewModelPauseSubMenuSave.ButtonsDeleteGameFile;
		_buttonsComponentsDeleteGameFile = new Button[_viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length];
		_textButtonsComponentsDeleteGameFile = new TextMeshProUGUI[_viewModelPauseSubMenuSave.ButtonsDeleteGameFile.Length];

		for (int i = 0; i < _viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length; i++)
		{
			int slot = i + 1;

			_buttonsComponentsRewriteGameFile[i] = _buttonsRewriteGameFile[i].GetComponent<Button>();
			_buttonsComponentsRewriteGameFile[i].onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuSave.TextGameFileDateAndTime[i].GetComponent<TextMeshProUGUI>();
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

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileDelete += UpdateAllUIElements;
		_saveLoadController.OnSafeFileSaved += UpdateAllUIElements;

		_pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSaveSubMenuCanvas;
		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SaveSubMenu Initialized");
	}

	private void DisableButtons()
	{
		foreach (var button in _buttonsComponentsRewriteGameFile)
		{
			button.interactable = false;
		}
		foreach (var button in _buttonsComponentsDeleteGameFile)
		{
			button.interactable = false;
		}
		_buttonComponentCreateNewGameFile.interactable = false;
		_buttonComponentClosePauseSubMenuSave.interactable = false;
	}

	private void EnableButtons()
	{
		foreach (var button in _buttonsComponentsRewriteGameFile)
		{
			button.interactable = true;
		}
		foreach (var button in _buttonsComponentsDeleteGameFile)
		{
			button.interactable = true;
		}
		_buttonComponentCreateNewGameFile.interactable = true;
		_buttonComponentClosePauseSubMenuSave.interactable = true;
	}

	private int FindFirstEmptySlot()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			if (string.IsNullOrEmpty(extendedSaveInfos[i].SafefileSceneSystemNameForIcon)) 
				return i + 1;
		}
		return -1;
	}

	public void UpdateAllUIElements()
	{
		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;

		if (_buttonCreateNewGameFile.activeSelf != hasEmptySlot)
		{
			_buttonCreateNewGameFile.SetActive(hasEmptySlot);
		}
	}

	private void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			string currentDataAndTime = extendedSaveInfos[i].SavefileDateTimeForUI;
			string currentSceneNameUI = extendedSaveInfos[i].SafefileSceneUINameForUI;
			string currentSceneNameSystem = extendedSaveInfos[i].SafefileSceneSystemNameForIcon;

			if (!string.IsNullOrEmpty(currentSceneNameSystem))
			{
				_buttonsRewriteGameFile[i].SetActive(true);

				_textComponentsGameFileDateAndTime[i].text = currentDataAndTime;
				_textComponentsGameFileSceneName[i].text = _localizationManager.GetLocalizedString(currentSceneNameUI);

				Sprite sprite = Resources.Load<Sprite>($"Sprites/Sprites_LoadingScreens/{currentSceneNameSystem}");

				_imagesComponentsSceneGameFile[i].sprite = sprite;
			}
			else
			{
				_buttonsRewriteGameFile[i].SetActive(false);
			}
		}
	}

	private void ShowSaveSubMenuCanvas()
	{
		_isPauseSubMenuSaveOpened = true;
		_canvasPauseSubMenuSave.SetActive(true);

		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;
		_buttonCreateNewGameFile.SetActive(hasEmptySlot);
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

		_textComponentPauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSUbMenuSave_TextPauseSubMenuSave");

		_textButtonComponentCreateNewGameFile.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSUbMenuSave_ButtonCreateNewGameFile");

		for (int i = 0; i < _viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length; i++)
		{
			_textButtonsComponentsDeleteGameFile[i].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSUbMenuSave_ButtonDeleteGameFile");
		}

		_textButtonComponentClosePauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSUbMenuSave_ButtonClosePauseSubMenuSave");
	}
}