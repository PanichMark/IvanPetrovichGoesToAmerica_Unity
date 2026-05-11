using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObjectLockElectronic : MonoBehaviour, IInteractable
{
	public bool WasUnlocked { get; private set; }
	public string InteractionObjectNameSystem => throw new NotImplementedException();
	public string InteractionObjectNameUI => "Электронный замок";
	public string InteractionHintMessageMain => "Взломать?";
	public string InteractionHintMessageAction => throw new NotImplementedException();
	public string InteractionHintMessageFail => throw new NotImplementedException();
	private bool _isPuzzleActive;
	public bool IsInteractionHintMessageFailActive => throw new NotImplementedException();

	private Button _buttonExitLockpickElectronicMenu;
	private GameObject[] _buttonsLockElectrical;
	private MenuManager _menuManager;
	private GameObject _canvasLockpickElectronicMenu;
	private SaveLoadController _saveLoadController;
	private GameSceneManager _gameSceneManager;

	private List<int> _alarmIndices;
	private int _movesLeft = 5;

	void Start()
	{
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_canvasLockpickElectronicMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuLockpickElectronic");
		_buttonExitLockpickElectronicMenu = ServiceLocator.Resolve<Button>("ButtonExitLockpickElectronicMenu");
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_buttonsLockElectrical = ServiceLocator.Resolve<GameObject[]>("ButtonsLockElectronic");

		_buttonExitLockpickElectronicMenu.onClick.AddListener(CloseElectronicLockPuzzle);

		_menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		_menuManager.OnClosePauseMenu += ShowPuzzleCanvas;

		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseElectronicLockPuzzle;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseElectronicLockPuzzle;
	}

	private void CloseElectronicLockPuzzle()
	{
		if (_isPuzzleActive)
		{
			_isPuzzleActive = false;
			_canvasLockpickElectronicMenu.SetActive(false);
			_menuManager.CloseInteractionMenu();
			_movesLeft = 4;
			Debug.Log("CLOSE PUZZLE");
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (_isPuzzleActive)
			_canvasLockpickElectronicMenu.SetActive(true);
	}

	private void HidePuzzleCanvas()
	{
		if (_isPuzzleActive)
			_canvasLockpickElectronicMenu.SetActive(false);
	}

	public void Interact()
	{
		if (!_isPuzzleActive)
		{
			_isPuzzleActive = true;
			_menuManager.OpenInteractionMenu();
			InitializeButtons();
			ShowPuzzleCanvas();
		}
	}

	private void InitializeButtons()
	{
		_movesLeft = 5;

		foreach (var buttonObj in _buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.grey;
			button.colors = colors;
			button.interactable = true;
			button.onClick?.RemoveAllListeners();
		}

		UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

		List<int> indices = Enumerable.Range(0, _buttonsLockElectrical.Length).ToList();

		_alarmIndices = new List<int>();
		while (_alarmIndices.Count < 4)
		{
			int index = UnityEngine.Random.Range(0, indices.Count);
			_alarmIndices.Add(indices[index]);
			indices.RemoveAt(index);
		}

		foreach (var buttonObj in _buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.onClick.AddListener(() => OnButtonClicked(button));
		}
	}

	private void OnButtonClicked(Button clickedButton)
	{
		int buttonIndex = Array.IndexOf(_buttonsLockElectrical, clickedButton.gameObject);

		if (_alarmIndices.Contains(buttonIndex))
		{
			Debug.Log("FAIL!");
			StartCoroutine(FlashAndResetButtons());
			return;
		}

		ColorBlock colors = clickedButton.colors;
		colors.disabledColor = Color.green;
		clickedButton.colors = colors;
		clickedButton.interactable = false;

		RevealAlarmButton();

		_movesLeft--;

		Debug.Log(_movesLeft);

		if (_movesLeft <= 0)
			StartCoroutine(SuccessfulyUnlockedFlashing());
	}

	private IEnumerator SuccessfulyUnlockedFlashing()
	{
		Debug.Log("SUCCESS FLASH");

		foreach (var buttonObj in _buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.interactable = false;
		}

		for (int i = 0; i < 5; i++)
		{
			foreach (var buttonObj in _buttonsLockElectrical)
			{
				Button button = buttonObj.GetComponent<Button>();
				ColorBlock colors = button.colors;
				colors.disabledColor = (i % 2 == 0) ? Color.green : Color.grey;
				button.colors = colors;
			}

			yield return new WaitForSecondsRealtime(0.1f);
		}

		EndPuzzle();
	}

	private IEnumerator FlashAndResetButtons()
	{
		Debug.Log("FLASH AND RESET BUTTONS");

		foreach (var buttonObj in _buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.interactable = false;
		}

		for (int i = 0; i < 5; i++)
		{
			foreach (var buttonObj in _buttonsLockElectrical)
			{
				Button button = buttonObj.GetComponent<Button>();
				ColorBlock colors = button.colors;
				colors.disabledColor = (i % 2 == 0) ? Color.red : Color.grey;
				button.colors = colors;
			}

			yield return new WaitForSecondsRealtime(0.1f);
		}

		foreach (var buttonObj in _buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.grey;
			button.colors = colors;
			button.interactable = true;
		}

		_movesLeft = 5;
	}

	private void RevealAlarmButton()
	{
		List<Button> availableAlarms = GetAvailableAlarmButtons();

		if (availableAlarms.Any())
		{
			int randomIndex = UnityEngine.Random.Range(0, availableAlarms.Count);
			Button revealedButton = availableAlarms[randomIndex];

			ColorBlock colors = revealedButton.colors;
			colors.disabledColor = Color.red;
			revealedButton.colors = colors;
			revealedButton.interactable = false;
		}
	}

	private void EndPuzzle()
	{
		WasUnlocked = true;
		_menuManager.CloseInteractionMenu();
		CloseElectronicLockPuzzle();
	}

	private List<Button> GetAvailableAlarmButtons()
	{
		List<Button> result = new List<Button>();

		foreach (int index in _alarmIndices)
		{
			Button button = _buttonsLockElectrical[index].GetComponent<Button>();

			if (button.interactable)
				result.Add(button);
		}

		return result;
	}
}