using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseSubMenuSaveController : MonoBehaviour
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;
	
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PauseMenuController _pauseMenuController;
	private bool _isPauseSubMenuSaveOpened;
	private GameObject _canvasPauseSubMenuSave;
	private SaveLoadController _saveLoadController;

	private GameObject _buttonClosePauseSubMenuSave;
	private GameObject[] _buttonsRewriteGame;
	private GameObject[] _buttonsDeleteGame;
	private GameObject _buttonSaveNewGame;

	private Text[] _currentDateAndTimeTexts;
	private Text[] _currentSceneNameUITexts;

	public void Initialize(
		IInputDevice inputDevice,
		SaveLoadController saveLoadController,
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuSave,
		GameObject buttonSaveNewGame,
		GameObject[] buttonsRewriteGame,
		GameObject[] buttonsDeleteGame,
		GameObject buttonClosePauseSubMenuSave)
	{
		_buttonClosePauseSubMenuSave = buttonClosePauseSubMenuSave;
		_buttonSaveNewGame = buttonSaveNewGame;
		_pauseMenuController = pauseMenuController;
		_menuManager = menuManager;
		_inputDevice = inputDevice;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_buttonsRewriteGame = buttonsRewriteGame;
		_buttonsDeleteGame = buttonsDeleteGame;
		_saveLoadController = saveLoadController;

		for (int i = 0; i < buttonsRewriteGame.Length; i++)
		{
			int slot = i + 1;
			int capturedSlot = slot; 
			_buttonsRewriteGame[i].GetComponent<Button>().onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(capturedSlot));
			_buttonsDeleteGame[i].GetComponent<Button>().onClick.AddListener(() => OnRequestDeleteSaveFileConfirmation?.Invoke(capturedSlot));
		}

		_buttonClosePauseSubMenuSave.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_currentDateAndTimeTexts = new Text[buttonsRewriteGame.Length];
		_currentSceneNameUITexts = new Text[buttonsRewriteGame.Length];

		for (int i = 0; i < buttonsRewriteGame.Length; i++)
		{
			Transform buttonTransform = buttonsRewriteGame[i].transform;
			_currentDateAndTimeTexts[i] = buttonTransform.Find("Text_CurrentDateAndTime")?.GetComponent<Text>();
			_currentSceneNameUITexts[i] = buttonTransform.Find("Text_CurrentSceneNameUI")?.GetComponent<Text>();

			if (_currentDateAndTimeTexts[i] != null) _currentDateAndTimeTexts[i].gameObject.SetActive(false);
			if (_currentSceneNameUITexts[i] != null) _currentSceneNameUITexts[i].gameObject.SetActive(false);

			Transform iconTransform = buttonTransform.Find("Level_Image");
			if (iconTransform != null) iconTransform.gameObject.SetActive(false);
		}

		_buttonSaveNewGame.GetComponent<Button>().onClick.AddListener(() =>
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
		foreach (var buttonObj in _buttonsRewriteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		foreach (var buttonObj in _buttonsDeleteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		Button newSaveButton = _buttonSaveNewGame.GetComponent<Button>();
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
		foreach (var buttonObj in _buttonsRewriteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		foreach (var buttonObj in _buttonsDeleteGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		Button newSaveButton = _buttonSaveNewGame.GetComponent<Button>();
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

		if (_buttonSaveNewGame.activeSelf != hasEmptySlot)
		{
			_buttonSaveNewGame.SetActive(hasEmptySlot);
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
				_buttonsRewriteGame[i].SetActive(true);
				_buttonsDeleteGame[i].SetActive(true);

				_currentDateAndTimeTexts[i].text = currentDataAndTime;
				_currentSceneNameUITexts[i].text = currentSceneNameUI;

				_currentDateAndTimeTexts[i].gameObject.SetActive(true);
				_currentSceneNameUITexts[i].gameObject.SetActive(true);

				string imagePath = $"Sprites/Sprites_LoadingScreens/{currentSceneNameSystem}";
				Sprite sprite = Resources.Load<Sprite>(imagePath);
				Transform imageTransform = _buttonsRewriteGame[i].transform.Find("Level_Image");

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
				_buttonsRewriteGame[i].SetActive(false);
				_buttonsDeleteGame[i].SetActive(false);

				if (_currentDateAndTimeTexts[i] != null) _currentDateAndTimeTexts[i].gameObject.SetActive(false);
				if (_currentSceneNameUITexts[i] != null) _currentSceneNameUITexts[i].gameObject.SetActive(false);

				Transform imageTransform = _buttonsRewriteGame[i].transform.Find("Level_Image");
				if (imageTransform != null) imageTransform.gameObject.SetActive(false);
			}
		}
	}

	private void ShowSaveSubMenuCanvas()
	{
		_isPauseSubMenuSaveOpened = true;
		_canvasPauseSubMenuSave.SetActive(true);

		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;
		_buttonSaveNewGame.SetActive(hasEmptySlot);
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