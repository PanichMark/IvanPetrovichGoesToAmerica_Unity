using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuLoadController : MonoBehaviour
{
	public event Action<int> OnRequestLoadSaveFileConfirmation;

	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PauseMenuController _pauseMenuController;
	private SaveLoadController _saveLoadController;
	private GameObject _canvasPauseSubMenuLoad;
	private bool _isPauseSubMenuLoadOpened;
	private GameObject _buttonClosePauseSubMenuLoad;

	private GameObject[] _buttonsLoadGame;

	private Text[] _currentDateAndTimeTexts;
	private Text[] _currentSceneNameUITexts;

	public void Initialize(
		IInputDevice inputDevice,
		SaveLoadController saveLoadController,
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuLoad,
		GameObject[] buttonsLoadGame,
		GameObject buttonClosePauseSubMenuLoad)
	{
		_buttonClosePauseSubMenuLoad = buttonClosePauseSubMenuLoad;
		_pauseMenuController = pauseMenuController;
		_menuManager = menuManager;
		_inputDevice = inputDevice;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_saveLoadController = saveLoadController;
		_buttonsLoadGame = buttonsLoadGame;

		for (int i = 0; i < buttonsLoadGame.Length; i++)
		{
			int slot = i + 1;
			_buttonsLoadGame[i].GetComponent<Button>().onClick.AddListener(() => OnRequestLoadSaveFileConfirmation?.Invoke(slot));
		}

		_buttonClosePauseSubMenuLoad.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideLoadSubMenuCanvas;
		_saveLoadController.OnSafeFileDelete += RefreshLoadButtonLabels;

		_currentDateAndTimeTexts = new Text[_buttonsLoadGame.Length]; 
		_currentSceneNameUITexts = new Text[_buttonsLoadGame.Length];

		for (int i = 0; i < this._buttonsLoadGame.Length; i++)
		{
			Transform buttonTransform = _buttonsLoadGame[i].transform;
			_currentDateAndTimeTexts[i] = buttonTransform.Find("Text_CurrentDateAndTime")?.GetComponent<Text>();
			_currentSceneNameUITexts[i] = buttonTransform.Find("Text_CurrentSceneNameUI")?.GetComponent<Text>();
		}

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("LoadSubMenu Initialized");
	}

	private void DisableButtons()
	{
		foreach (var buttonObj in _buttonsLoadGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = false;
			}
		}

		Button closeButton = _buttonClosePauseSubMenuLoad.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var buttonObj in _buttonsLoadGame)
		{
			Button button = buttonObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;
			}
		}

		Button closeButton = _buttonClosePauseSubMenuLoad.GetComponent<Button>();
		if (closeButton != null)
		{
			closeButton.interactable = true;
		}
	}

	public void ShowLoadSubMenuCanvas()
	{
		_isPauseSubMenuLoadOpened = true;
		_canvasPauseSubMenuLoad.gameObject.SetActive(true);
		RefreshLoadButtonLabels();
	}

	public void HideLoadSubMenuCanvas()
	{
		if (_isPauseSubMenuLoadOpened)
		{
			_isPauseSubMenuLoadOpened = false;
			_canvasPauseSubMenuLoad.gameObject.SetActive(false);
			Debug.Log("LoadSubMenu closed");
		}
	}

	public void RefreshLoadButtonLabels()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			var (currentDataAndTime, currentSceneNameUI, currentSceneNameSystem) = extendedSaveInfos[i];

			if (!string.IsNullOrEmpty(currentSceneNameSystem)) 
			{
				_buttonsLoadGame[i].gameObject.SetActive(true);

				_currentSceneNameUITexts[i].text = currentDataAndTime;
				_currentDateAndTimeTexts[i].text = currentSceneNameUI;

				_currentSceneNameUITexts[i].gameObject.SetActive(true);
				_currentDateAndTimeTexts[i].gameObject.SetActive(true);

				string currentSceneBackgroundImage = $"{currentSceneNameSystem}";
				Sprite sprite = Resources.Load<Sprite>($"Sprites/Sprites_LoadingScreens/{currentSceneBackgroundImage}");

				if (sprite != null)
				{
					_buttonsLoadGame[i].transform.Find("Level_Image").gameObject.SetActive(true);
					_buttonsLoadGame[i].transform.Find("Level_Image").GetComponent<Image>().sprite = sprite;
				}
				else
				{
					Debug.LogError("Failed to load Scene Background Image");
				}
			}
			else
			{
				_buttonsLoadGame[i].gameObject.SetActive(false);
			}
		}
	}
}

