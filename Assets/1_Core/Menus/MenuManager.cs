using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	
	public delegate void OpenMenuEventHandler();
	public event OpenMenuEventHandler OnOpenPauseMenu;
	public event OpenMenuEventHandler OnClosePauseMenu;
	public event OpenMenuEventHandler OnOpenWeaponWheelMenu;
	public event OpenMenuEventHandler OnCloseWeaponWheelMenu;
	public event OpenMenuEventHandler OnOpenInteractionMenu;
	public event OpenMenuEventHandler OnCloseInteractionMenu;
	public event OpenMenuEventHandler OnOpenReadNoteMenu;
	public event OpenMenuEventHandler OnCloseReadNoteMenu;
	public event OpenMenuEventHandler OnOpenLockpickMenu;
	public event OpenMenuEventHandler OnCloseLockpickMenu;

	public event OpenMenuEventHandler OnOpenAnyMenu;
	public event OpenMenuEventHandler OnCloseAnyMenu;

	public IInputDevice inputDevice;
	private GameController gameController;
	private SaveLoadController saveLoadController;


	public Stack<int> PauseMenuLevel = new Stack<int>();

	public void Initialize(IInputDevice inputDevice, GameSceneManager gameSceneManager, GameController gameController, SaveLoadController saveLoadController)
	{
		this.inputDevice = inputDevice;
		this.gameController = gameController;
		this.saveLoadController = saveLoadController;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_isInitialized = true;
		this.saveLoadController.OnSafeFileLoad += ClosePauseMenu;
		this.saveLoadController.OnSafeFileLoad += CloseWeaponWheelMenu;
		this.saveLoadController.OnSafeFileLoad += CloseInteractionHUD;
		this.saveLoadController.OnSafeFileLoad += CloseReadNoteMenu;
		this.saveLoadController.OnSafeFileLoad += CloseLockpickMenu;
		Debug.Log("MenuManager Initialized");
	}
	private bool _isInitialized = false;
	
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }

	public bool IsInteractionMenuOpened { get; private set; }

	public bool IsReadNoteMenuOpened { get; private set; }
	public bool IsLockpickMenuOpened { get; private set; }
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
				ClosePauseMenu();
			}
		}
		//Debug.Log(PauseMenuLevel.Count);
		//Debug.Log(gameController.IsMainMenuOpen);
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
		{
			CloseWeaponWheelMenu();
		}
		PauseMenuLevel.Push(1);
		OnOpenPauseMenu?.Invoke(); 
		
		OpenAnyMenu();
		gameController.MakePlayerNonControllable();

		IsPauseMenuOpened = true;

		Time.timeScale = 0f;

		Debug.Log("PauseMenu opened");
	}

	public void ClosePauseMenu()
	{

		OnClosePauseMenu?.Invoke();
	

		IsPauseMenuOpened = false;
		if (PauseMenuLevel.Count > 0)
			PauseMenuLevel.Pop();

		if (!IsReadNoteMenuOpened && !IsLockpickMenuOpened)
		{
			gameController.MakePlayerControllable();

			

			CloseAnyMenu();
			Time.timeScale = 1f;
		}
		

		Debug.Log("PauseMenu closed");
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

		OnOpenAnyMenu?.Invoke();
		if (!gameController.IsMainMenuOpen)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseInteractionHUD();
		}
	}

	public void CloseAnyMenu()
	{
		IsAnyMenuOpened = false;
	
		OnCloseAnyMenu?.Invoke();
		if (!gameController.IsMainMenuOpen)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			OpenInteractionHUD();
			
		}
	}

	public void OpenInteractionHUD()
	{
		IsInteractionMenuOpened = true;
		OnOpenInteractionMenu?.Invoke();
		
		

		Debug.Log("InteractionHUD opened");
	}

	public void CloseInteractionHUD()
	{
		OnCloseInteractionMenu?.Invoke();

		IsInteractionMenuOpened = false;

		Debug.Log("InteractionHUD closed");

	}

	public void OpenReadNoteMenu()
	{
		IsReadNoteMenuOpened = true;
		Time.timeScale = 0;
		gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenReadNoteMenu?.Invoke();
		
		Debug.Log("ReadNoteMenu opened");
	}
	public void CloseReadNoteMenu()
	{
		IsReadNoteMenuOpened = false;
		Time.timeScale = 1;
		OnCloseReadNoteMenu?.Invoke();
		gameController.MakePlayerControllable();
		//OpenInteractionHUD();
		CloseAnyMenu();
		Debug.Log("ReadNoteMenu closed");

	}
	public void OpenLockpickMenu()
	{
		gameController.MakePlayerNonControllable();
		IsLockpickMenuOpened = true;
		Time.timeScale = 0;
		OnOpenLockpickMenu?.Invoke();
		OpenAnyMenu();
		
		Debug.Log("LockpickMenu opened");

	}
	public void CloseLockpickMenu()
	{
		IsLockpickMenuOpened = false;
		Time.timeScale = 1;
		OnCloseLockpickMenu?.Invoke();
		OpenInteractionHUD();
		gameController.MakePlayerControllable();
		CloseAnyMenu();
		Debug.Log("LockpickMenu closed");

	}
}
