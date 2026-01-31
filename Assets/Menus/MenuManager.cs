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

	public IInputDevice inputDevice;
	private GameController gameController;
	private SaveLoadController saveLoadController;


	public Stack<int> PauseMenuLevel = new Stack<int>();

	public void Initialize(IInputDevice inputDevice, GameController gameController, SaveLoadController saveLoadController)
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
		this.saveLoadController.OnSafeFileLoad += CloseInteractionMenu;
		this.saveLoadController.OnSafeFileLoad += CloseReadNoteMenu;
		this.saveLoadController.OnSafeFileLoad += CloseLockpickMenu;
		Debug.Log("MenuManager Initialized");
	}
	private bool _isInitialized = false;
	
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }

	public bool IsInteractionMenuOpened { get; private set; }


	void Update()
	{
		if (!_isInitialized)
			return;

		//Debug.Log(PauseMenuLevel.Count);

		if (inputDevice.GetKeyPauseMenu())
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
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
		{
			CloseWeaponWheelMenu();
		}
		PauseMenuLevel.Push(1);
		OnOpenPauseMenu?.Invoke(); 
		Debug.Log("PauseMenu opened");
		OpenAnyMenu();
		gameController.MakePlayerNonControllable();

		IsPauseMenuOpened = true;

		Time.timeScale = 0f;
	}

	public void ClosePauseMenu()
	{

		OnClosePauseMenu?.Invoke();
		Debug.Log("PauseMenu closed");

		IsPauseMenuOpened = false;
		PauseMenuLevel.Pop();

		if (!IsInteractionMenuOpened)
		{
			gameController.MakePlayerControllable();
		
			CloseAnyMenu();
			Time.timeScale = 1f;
		}
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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void CloseAnyMenu()
	{
		IsAnyMenuOpened = false;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

	}

	public void OpenInteractionMenu()
	{
		OnOpenInteractionMenu?.Invoke();

		Time.timeScale = 0f;
		gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		IsInteractionMenuOpened = true;
		Debug.Log("InteractionMenu opened");
	}

	public void CloseInteractionMenu()
	{
		OnCloseInteractionMenu?.Invoke();
		Time.timeScale = 1f;
		CloseAnyMenu();
		IsInteractionMenuOpened = false;
		gameController.MakePlayerControllable();
		Debug.Log("InteractionMenu closed");

	}

	public void OpenReadNoteMenu()
	{
		OnOpenReadNoteMenu?.Invoke();
	}
	public void CloseReadNoteMenu()
	{
	
			OnCloseLockpickMenu?.Invoke();

		gameController.MakePlayerControllable();
		CloseAnyMenu();
	}
	public void OpenLockpickMenu()
	{
		OnOpenLockpickMenu?.Invoke();
	}
	public void CloseLockpickMenu()
	{
		
			OnCloseLockpickMenu?.Invoke();
		gameController.MakePlayerControllable();

		CloseAnyMenu();
	}
}
