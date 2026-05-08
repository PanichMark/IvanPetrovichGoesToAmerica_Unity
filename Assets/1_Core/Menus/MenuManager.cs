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

	public bool IsCutsceneMenuOpened {  get; private set; }
	public IInputDevice inputDevice;
	private GameController gameController;
	private GameSceneManager gameSceneManager;


	public Stack<int> PauseMenuLevel = new Stack<int>();

	public void Initialize(IInputDevice inputDevice, GameController gameController, GameSceneManager gameSceneManager)
	{
		this.inputDevice = inputDevice;
		this.gameController = gameController;
		this.gameSceneManager = gameSceneManager;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_isInitialized = true;
		this.gameController.OnPlayerDeath += OpenPauseMenu;

		this.gameSceneManager.OnBeginLoadGameplayScene += ClosePauseMenu;
		this.gameSceneManager.OnBeginLoadGameplayScene += CloseWeaponWheelMenu;
		this.gameSceneManager.OnBeginLoadGameplayScene += CloseInteractionHUD;
		this.gameSceneManager.OnBeginLoadGameplayScene += CloseInteractionMenu;
		this.gameSceneManager.OnBeginLoadGameplayScene += CloseDialogueMenu;
		this.gameSceneManager.OnBeginLoadGameplayScene += CloseCutsceneMenu;


		this.gameSceneManager.OnBeginLoadMainMenuScene += ClosePauseMenu;
		this.gameSceneManager.OnBeginLoadMainMenuScene += CloseWeaponWheelMenu;
		this.gameSceneManager.OnBeginLoadMainMenuScene += CloseInteractionHUD;
		this.gameSceneManager.OnBeginLoadMainMenuScene += CloseInteractionMenu;
		this.gameSceneManager.OnBeginLoadMainMenuScene += CloseDialogueMenu;
		this.gameSceneManager.OnBeginLoadMainMenuScene += CloseCutsceneMenu;
		Debug.Log("MenuManager Initialized");
	}
	private bool _isInitialized = false;
	
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }
	public bool IsDialogueMenuOpened { get; private set; }

	public bool IsInteractionHUDOpened { get; private set; }
	

	public bool IsInteractionMenuOpened { get; private set; }
	void Update()
	{
		if (!_isInitialized)
			return;

		//Debug.Log(PauseMenuLevel.Count);

		if (inputDevice.GetKeyPauseMenu() && !gameController.IsMainMenuOpen)
		{
			if (PauseMenuLevel.Count == 0)
			{
				OpenPauseMenu();
			}
			else if (PauseMenuLevel.Count == 1)
			{
				if (!gameController.IsPlayerDead)
				{
					ClosePauseMenu();
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
			//Debug.Log(gameController.IsMainMenuOpen);
		}
		//Debug.Log(Time.deltaTime);
		//Debug.Log(PauseMenuLevel.Count);
		//Debug.Log(IsCutsceneMenuOpened);
		//Debug.Log(IsInteractionMenuOpened);
		//Debug.Log(IsDialogueMenuOpened);
		//Debug.Log(IsAnyMenuOpened);
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
		gameController.MakePlayerNonControllable();

		

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
			gameController.MakePlayerControllable();
			Time.timeScale = 1f;
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
		//Debug.Log("Opened any menu");
		IsAnyMenuOpened = true;
		//Debug.Log("--- ANY MENU ---");
		if (IsDialogueMenuOpened || IsCutsceneMenuOpened)
		{
		
			//Debug.Log("--- BLUR ---");
			
			
		}
		else
		{
			OnOpenAnyMenu?.Invoke();
		}

		if (!gameController.IsMainMenuOpen)
		{
			CloseInteractionHUD();

			//Debug.Log(IsPauseMenuOpened);
			//Debug.Log(IsCutsceneMenuOpened);

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
		if (!gameController.IsMainMenuOpen)
		{
			OpenInteractionHUD();

			//Debug.Log(IsPauseMenuOpened);
			//Debug.Log(IsCutsceneMenuOpened);


			//Debug.Log(IsPauseMenuOpened);
			//Debug.Log(IsDialogueMenuOpened);
			//Debug.Log(IsCutsceneMenuOpened);

			if (IsCutsceneMenuOpened && !IsPauseMenuOpened)
			{
				//Debug.Log("1");
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (!IsCutsceneMenuOpened)
			{
				//Debug.Log("2");
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (IsDialogueMenuOpened)
			{
				//Debug.Log("3");
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
		gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenInteractionMenu?.Invoke();
		
		Debug.Log("InteractionMenu opened");
	}
	public void CloseInteractionMenu()
	{
		IsInteractionMenuOpened = false;
		Time.timeScale = 1;
		OnCloseInteractionMenu?.Invoke();
		gameController.MakePlayerControllable();
		CloseAnyMenu();
		Debug.Log("InteractionMenu closed");

	}
	public void OpenDialogueMenu()
	{
		IsDialogueMenuOpened = true;
		Time.timeScale = 0;
		gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenDialogueMenu?.Invoke();
		
		Debug.Log("DialogueMenu opened");
	}

	public void CloseDialogueMenu()
	{
		IsDialogueMenuOpened = false;
		Time.timeScale = 1;
		OnCloseDialogueMenu?.Invoke();
		gameController.MakePlayerControllable();
		CloseAnyMenu();
		Debug.Log("DialogueMenu closed");
	
	}




}
