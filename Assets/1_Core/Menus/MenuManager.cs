using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public delegate void MenuEventHandler();
	public event MenuEventHandler OnOpenPauseMenu;
	public event MenuEventHandler OnClosePauseMenu;
	public event MenuEventHandler OnOpenWeaponWheelMenu;
	public event MenuEventHandler OnCloseWeaponWheelMenu;
	public event MenuEventHandler OnOpenInteractionHUD;
	public event MenuEventHandler OnCloseInteractionHUD;
	public event MenuEventHandler OnOpenInteractionMenu;
	public event MenuEventHandler OnCloseInteractionMenu;
	public event MenuEventHandler OnOpenDialogueMenu;
	public event MenuEventHandler OnCloseDialogueMenu;
	public event MenuEventHandler OnOpenCutsceneMenu;
	public event MenuEventHandler OnCloseCutsceneMenu;
	public event MenuEventHandler OnClosePauseMenuDuringOpenedDialogueMenu;
	public event MenuEventHandler OnClosePauseMenuDuringOpenedCutsceneMenu;
	public event MenuEventHandler OnOpenAnyMenu;
	public event MenuEventHandler OnCloseAnyMenu;
	public event MenuEventHandler OnOpenConfirmationOnExitToMainMenu;
	public event MenuEventHandler OnCloseConfirmationOnExitToMainMenu;

	private bool _isInitialized = false;
	public bool IsConfirmationOnExitToMainMenuOpened { get; private set; }
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }
	public bool IsDialogueMenuOpened { get; private set; }

	public bool IsInteractionHUDOpened { get; private set; }

	public bool IsInteractionMenuOpened { get; private set; }

	public bool IsCutsceneMenuOpened {  get; private set; }
	private IInputDevice _inputDevice;
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;

	public Stack<int> PauseMenuLevel = new Stack<int>();

	public void Initialize(GameController gameController, IInputDevice inputDevice, GameSceneManager gameSceneManager)
	{
		_inputDevice = inputDevice;
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_isInitialized = true;
		_gameController.OnPlayerDeath += OpenPauseMenu;

		_gameSceneManager.OnBeginLoadingGameplayScene += ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseCutsceneMenu;

		_gameSceneManager.OnBeginLoadingMainMenuScene += ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseCutsceneMenu;

		Debug.Log("MenuManager Initialized");
	}

	void Update()
	{
		if (!_isInitialized)
			return;

		if (_inputDevice.GetKeyPauseMenu() && !_gameController.IsMainMenuOpen)
		{
			if (PauseMenuLevel.Count == 0)
			{
				OpenPauseMenu();
			}
			else if (PauseMenuLevel.Count == 1)
			{
				if (!_gameController.IsPlayerDead)
				{
					ClosePauseMenu();
					//Debug.Log("BRUH!");
				}
				if (IsDialogueMenuOpened)
				{
					OnClosePauseMenuDuringOpenedDialogueMenu?.Invoke();
				}
				if (IsCutsceneMenuOpened)
				{
					OnClosePauseMenuDuringOpenedCutsceneMenu?.Invoke();
				}
			}
			else if (PauseMenuLevel.Count == 2 && IsConfirmationOnExitToMainMenuOpened == true)
			{
				
				CloseConfirmationOnExitToMainMenu();
			}
		}
		//Debug.Log(PauseMenuLevel.Count);
		//Debug.Log(IsConfirmationOnExitToMainMenuOpened);
	}

	public void OpenConfirmationOnExitToMainMenu()
	{
		IsConfirmationOnExitToMainMenuOpened = true;
		OnOpenConfirmationOnExitToMainMenu?.Invoke();
	}

	public void CloseConfirmationOnExitToMainMenu()
	{
		IsConfirmationOnExitToMainMenuOpened = false;
		OnCloseConfirmationOnExitToMainMenu?.Invoke();
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
		{
			CloseWeaponWheelMenu();
		}

		PauseMenuLevel.Push(1);
		OnOpenPauseMenu?.Invoke();
		IsPauseMenuOpened = true;
		OpenAnyMenu();
		_gameController.MakePlayerNonControllable();

		Time.timeScale = 0f;

		Debug.Log("PauseMenu opened");
	}

	public void ClosePauseMenu()
	{
		OnClosePauseMenu?.Invoke();
		
		IsPauseMenuOpened = false;
		if (PauseMenuLevel.Count > 0)
			PauseMenuLevel.Pop();

		if (IsInteractionMenuOpened || IsDialogueMenuOpened || IsCutsceneMenuOpened)
		{
			
		}
		else
		{
			CloseAnyMenu();
			_gameController.MakePlayerControllable();
			Time.timeScale = 1f;
		}

		if(!IsPauseMenuOpened && IsCutsceneMenuOpened)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		Debug.Log("PauseMenu closed");
	}

	
	public void OpenCutsceneMenu()
	{
		IsCutsceneMenuOpened = true;
		OpenAnyMenu();

		OnOpenCutsceneMenu?.Invoke();
		Debug.Log("CutsceneMenu opened");
	}

	public void CloseCutsceneMenu()
	{
		CloseAnyMenu();
		
		OnCloseCutsceneMenu?.Invoke();
		IsCutsceneMenuOpened = false;
		Debug.Log("CutsceneMenu closed");
	}
	public void OpenWeaponWheelMenu()
	{
		OpenAnyMenu();
		IsWeaponWheelMenuOpened = true;
		OnOpenWeaponWheelMenu?.Invoke();

		Debug.Log("WeaponWheelMenu opened");
	}

	public void CloseWeaponWheelMenu()
	{
		CloseAnyMenu();
		IsWeaponWheelMenuOpened = false;
		OnCloseWeaponWheelMenu?.Invoke();
		Debug.Log("WeaponWheelMenu closed");
	}

	public void OpenAnyMenu()
	{
		IsAnyMenuOpened = true;

		if (IsDialogueMenuOpened || IsCutsceneMenuOpened)
		{

		}
		else
		{
			OnOpenAnyMenu?.Invoke();
		}

		if (!_gameController.IsMainMenuOpen)
		{
			CloseInteractionHUD();

			if (IsCutsceneMenuOpened && IsPauseMenuOpened)
			{
				
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			if(!IsCutsceneMenuOpened)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

	public void CloseAnyMenu()
	{
		Debug.Log("CloseAnyMenu");
		IsAnyMenuOpened = false;
	
		OnCloseAnyMenu?.Invoke();
		if (!_gameController.IsMainMenuOpen)
		{
			OpenInteractionHUD();

			if (IsCutsceneMenuOpened && !IsPauseMenuOpened)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (!IsCutsceneMenuOpened)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (IsDialogueMenuOpened)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

	public void OpenInteractionHUD()
	{
		IsInteractionHUDOpened = true;
		OnOpenInteractionHUD?.Invoke();
		
		Debug.Log("InteractionHUD opened");
	}

	public void CloseInteractionHUD()
	{
		OnCloseInteractionHUD?.Invoke();

		IsInteractionHUDOpened = false;

		Debug.Log("InteractionHUD closed");
	}

	public void OpenInteractionMenu()
	{
		IsInteractionMenuOpened = true;
		Time.timeScale = 0;
		_gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenInteractionMenu?.Invoke();
		
		Debug.Log("InteractionMenu opened");
	}
	public void CloseInteractionMenu()
	{
		IsInteractionMenuOpened = false;
		Time.timeScale = 1;
		OnCloseInteractionMenu?.Invoke();
		_gameController.MakePlayerControllable();
		CloseAnyMenu();
		Debug.Log("InteractionMenu closed");
	}
	public void OpenDialogueMenu()
	{
		IsDialogueMenuOpened = true;
		Time.timeScale = 0;
		_gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenDialogueMenu?.Invoke();
		
		Debug.Log("DialogueMenu opened");
	}

	public void CloseDialogueMenu()
	{
		IsDialogueMenuOpened = false;
		Time.timeScale = 1;
		OnCloseDialogueMenu?.Invoke();
		_gameController.MakePlayerControllable();
		CloseAnyMenu();
		Debug.Log("DialogueMenu closed");
	}
}