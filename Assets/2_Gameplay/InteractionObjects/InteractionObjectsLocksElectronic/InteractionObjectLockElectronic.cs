using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Подключаем пространство имен UI

public class InteractionObjectLockElectronic : MonoBehaviour, IInteractable
{
	public bool WasUnlocked {  get; private set; }

	public string InteractionObjectNameSystem => throw new NotImplementedException();

	public string InteractionObjectNameUI => "Электронный замок";

	public string InteractionHintMessageMain => "Выскрыть?";

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
	private LocalizationManager localizationManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		canvasLockpickElectronicMenu = ServiceLocator.Resolve<GameObject>("CanvasLockpickElectronicMenu");
		buttonExitLockpickElectronicMenu = ServiceLocator.Resolve<Button>("ExitLockpickElectronic");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		buttonsLockElectrical = ServiceLocator.Resolve<GameObject[]>("buttonsLockElectrical");

		Debug.Log(buttonExitLockpickElectronicMenu);
		buttonExitLockpickElectronicMenu.onClick.AddListener(OnClosePuzzle);// Присваиваем обработчик

		menuManager.OnOpenPauseMenu += HidePuzzleCanvas;
		menuManager.OnClosePauseMenu += ShowPuzzleCanvas;
		gameSceneManager.OnBeginLoadMainMenuScene += OnClosePuzzle;
		gameSceneManager.OnBeginLoadGameplayScene += OnClosePuzzle;
		//buttonExitLockpickElectronicMenu.OnClick = () => CloseLockElectricalMenu();
	}



	private void OnClosePuzzle()
	{
		if (IsPuzzleActive)
		{
			IsPuzzleActive = false;
			canvasLockpickElectronicMenu.SetActive(false);
			gameObject.tag = "Interactable";
			menuManager.CloseLockpickMenu();
		}
	}

	private void ShowPuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickElectronicMenu.SetActive(true);
		}
	}

	private void HidePuzzleCanvas()
	{
		if (IsPuzzleActive)
		{
			canvasLockpickElectronicMenu.SetActive(false);
		}
	}

	public void Interact()
	{
		menuManager.OpenLockpickMenu();
		IsPuzzleActive = true;
		//buttonExitLockpickElectronicMenu.onClick.RemoveAllListeners();      // Удаляем предыдущие события
	


		ShowPuzzleCanvas();
	}
}
