using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PauseSubMenuSaveController : MonoBehaviour
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;

	private SaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;
	
	private GameObject _canvasPauseSubMenuSave;

	private GameObject _buttonClosePauseSubMenuSave;

	private GameObject _buttonCreateNewGameFile;
	private GameObject[] _buttonsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;
	private TextMeshProUGUI[] _textComponentsGameFileDateAndTime;
	private TextMeshProUGUI[] _textComponentsGameFileSceneName;
	private Image[] _imagesComponentsSceneGameFile;

	private bool _isPauseSubMenuSaveOpened;

	public void Initialize(
		SaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSave,
		ViewModelPauseSubMenuSave viewModelPauseSubMenuSave)
	{
		_viewModelPauseSubMenuSave = viewModelPauseSubMenuSave;

		_buttonClosePauseSubMenuSave = _viewModelPauseSubMenuSave.ButtonClosePauseSubMenuSave;
		_buttonCreateNewGameFile = _viewModelPauseSubMenuSave.ButtonCreateNewGameFile;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_buttonsRewriteGameFile = _viewModelPauseSubMenuSave.ButtonsRewriteGameFile;
		_buttonsDeleteGameFile = _viewModelPauseSubMenuSave.ButtonsDeleteGameFile;
		_saveLoadController = saveLoadController;

		for (int i = 0; i < _buttonsRewriteGameFile.Length; i++)
		{
			_buttonsRewriteGameFile[i].GetComponent<Button>().onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(i + 1));
			_buttonsDeleteGameFile[i].GetComponent<Button>().onClick.AddListener(() => OnRequestDeleteSaveFileConfirmation?.Invoke(i + 1));
		}

		_buttonClosePauseSubMenuSave.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_textComponentsGameFileDateAndTime = new TextMeshProUGUI[_buttonsRewriteGameFile.Length];
		_textComponentsGameFileSceneName = new TextMeshProUGUI[_buttonsRewriteGameFile.Length];

		for (int i = 0; i < _buttonsRewriteGameFile.Length; i++)
		{
			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuSave.TextGameFileDateAndTime[i].GetComponent<TextMeshProUGUI>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuSave.TextGameFileSceneName[i].GetComponent<TextMeshProUGUI>();

			//_imagesComponentsSceneGameFile[i] = viewModelPauseSubMenuSave.ImageSceneGameFile[i].GetComponent<Image>();

			//Transform imageTransform = _buttonsRewriteGameFile[i].transform.Find("Level_Image");
			//if (imageTransform != null) imageTransform.gameObject.SetActive(false);
		}

		_buttonCreateNewGameFile.GetComponent<Button>().onClick.AddListener(() =>
		{
			int slotToUse = FindFirstEmptySlot();
			if (slotToUse != -1)
			{
				OnRequestNewSaveFileConfirmation?.Invoke(slotToUse);
			}
		});

		_pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSaveSubMenuCanvas;

		_saveLoadController.OnSafeFileDelete += UpdateAllUIElements;
		_saveLoadController.OnSafeFileSaved += UpdateAllUIElements;

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SaveSubMenu Initialized");
	}

	private void DisableButtons()
	{
		foreach (var buttonObj in _buttonsRewriteGameFile)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		foreach (var buttonObj in _buttonsDeleteGameFile)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		Button newSaveButton = _buttonCreateNewGameFile.GetComponent<Button>();
		if (newSaveButton != null)
		{
			newSaveButton.interactable = false;
		}

		Button closeButton = _buttonClosePauseSubMenuSave.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var buttonObj in _buttonsRewriteGameFile)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		foreach (var buttonObj in _buttonsDeleteGameFile)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		Button newSaveButton = _buttonCreateNewGameFile.GetComponent<Button>();
		if (newSaveButton != null)
		{
			newSaveButton.interactable = true;
		}

		Button closeButton = _buttonClosePauseSubMenuSave.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = true;
		}
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
				_buttonsDeleteGameFile[i].SetActive(true);
				//Debug.Log(_textComponentsGameFileDateAndTime[i]);
				_textComponentsGameFileDateAndTime[i].text = currentDataAndTime;
				_textComponentsGameFileSceneName[i].text = currentSceneNameUI;

				_textComponentsGameFileDateAndTime[i].gameObject.SetActive(true);
				_textComponentsGameFileSceneName[i].gameObject.SetActive(true);

				string imagePath = $"Sprites/Sprites_LoadingScreens/{currentSceneNameSystem}";
				Sprite sprite = Resources.Load<Sprite>(imagePath);
				Transform imageTransform = _viewModelPauseSubMenuSave.ImageSceneGameFile[i].GetComponent<Transform>();

				if (imageTransform != null)
				{
					imageTransform.gameObject.SetActive(sprite != null);
					if (sprite != null)
					{
						imageTransform.GetComponent<Image>().sprite = sprite;
					}
				}
			}
			else 
			{
				//_buttonsRewriteGameFile[i].SetActive(false);
				//_buttonsDeleteGameFile[i].SetActive(false);

				//_textComponentsGameFileDateAndTime[i].gameObject.SetActive(false);
				//_textComponentsGameFileSceneName[i].gameObject.SetActive(false);



				//Transform imageTransform = _buttonsRewriteGameFile[i].transform.Find("Level_Image");
				//if (imageTransform != null) imageTransform.gameObject.SetActive(false);
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
}