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
	public string InteractionHintAction => throw new NotImplementedException();
	public string InteractionHintMessageAdditional => throw new NotImplementedException();
	private bool IsPuzzleActive;
	public bool IsInteractionHintMessageAdditionalActive => throw new NotImplementedException();

	private Button buttonExitLockpickElectronicMenu;
	private GameObject[] buttonsLockElectrical;
	private MenuManager menuManager;
	private GameObject canvasLockpickElectronicMenu;
	private SaveLoadController saveLoadController;
	private GameSceneManager gameSceneManager;

	private List<int> alarmIndices;
	private int movesLeft = 5;

	void Start()
	{
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasLockpickElectronicMenu = ServiceLocator.Resolve<GameObject>("CanvasLockpickElectronicMenu");
		buttonExitLockpickElectronicMenu = ServiceLocator.Resolve<Button>("ExitLockpickElectronic");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		buttonsLockElectrical = ServiceLocator.Resolve<GameObject[]>("buttonsLockElectrical");

		buttonExitLockpickElectronicMenu.onClick.AddListener(CloseElectronicLockPuzzle);

		menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		menuManager.OnClosePauseMenu += ShowPuzzleCanvas;

		gameSceneManager.OnBeginLoadMainMenuScene += CloseElectronicLockPuzzle;
		gameSceneManager.OnBeginLoadGameplayScene += CloseElectronicLockPuzzle;
	}

	private void CloseElectronicLockPuzzle()
	{
		if (IsPuzzleActive)
		{
			IsPuzzleActive = false;
			canvasLockpickElectronicMenu.SetActive(false);
			menuManager.CloseInteractionMenu();
			movesLeft = 4;
			Debug.Log("CLOSE PUZZLE");
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (IsPuzzleActive)
			canvasLockpickElectronicMenu.SetActive(true);
	}

	private void HidePuzzleCanvas()
	{
		if (IsPuzzleActive)
			canvasLockpickElectronicMenu.SetActive(false);
	}

	public void Interact()
	{
		if (!IsPuzzleActive)
		{
			IsPuzzleActive = true;
			menuManager.OpenInteractionMenu();
			InitializeButtons();
			ShowPuzzleCanvas();
		}
	}

	private void InitializeButtons()
	{
		movesLeft = 5;

		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.grey;
			button.colors = colors;
			button.interactable = true;
			button.onClick?.RemoveAllListeners();
		}

		UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

		List<int> indices = Enumerable.Range(0, buttonsLockElectrical.Length).ToList();

		alarmIndices = new List<int>();
		while (alarmIndices.Count < 4)
		{
			int index = UnityEngine.Random.Range(0, indices.Count);
			alarmIndices.Add(indices[index]);
			indices.RemoveAt(index);
		}

		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.onClick.AddListener(() => OnButtonClicked(button));
		}
	}

	private void OnButtonClicked(Button clickedButton)
	{
		int buttonIndex = Array.IndexOf(buttonsLockElectrical, clickedButton.gameObject);

		if (alarmIndices.Contains(buttonIndex))
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

		movesLeft--;

		Debug.Log(movesLeft);

		if (movesLeft <= 0)
			StartCoroutine(SuccessfulyUnlockedFlashing());
	}

	private IEnumerator SuccessfulyUnlockedFlashing()
	{
		Debug.Log("SUCCESS FLASH");

		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.interactable = false;
		}

		for (int i = 0; i < 5; i++)
		{
			foreach (var buttonObj in buttonsLockElectrical)
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

		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			button.interactable = false;
		}

		for (int i = 0; i < 5; i++)
		{
			foreach (var buttonObj in buttonsLockElectrical)
			{
				Button button = buttonObj.GetComponent<Button>();
				ColorBlock colors = button.colors;
				colors.disabledColor = (i % 2 == 0) ? Color.red : Color.grey;
				button.colors = colors;
			}

			yield return new WaitForSecondsRealtime(0.1f);
		}

		foreach (var buttonObj in buttonsLockElectrical)
		{
			Button button = buttonObj.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.normalColor = Color.grey;
			button.colors = colors;
			button.interactable = true;
		}

		movesLeft = 5;
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
		menuManager.CloseInteractionMenu();
		CloseElectronicLockPuzzle();
	}

	private List<Button> GetAvailableAlarmButtons()
	{
		List<Button> result = new List<Button>();

		foreach (int index in alarmIndices)
		{
			Button button = buttonsLockElectrical[index].GetComponent<Button>();

			if (button.interactable)
				result.Add(button);
		}

		return result;
	}
}