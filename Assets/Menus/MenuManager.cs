using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	
	public delegate void OpenMenuEventHandler();
	public event OpenMenuEventHandler OnOpenPauseMenu;
	public event OpenMenuEventHandler OnClosePauseMenu;
	public event OpenMenuEventHandler OnOpenWeaponWheelMenu;
	public event OpenMenuEventHandler OnCloseWeaponWheelMenu;

	public IInputDevice inputDevice;
	private GameController gameController;



	public Stack<int> PauseMenuLevel = new Stack<int>();

	public void Initialize(IInputDevice inputDevice, GameController gameController)
	{
		this.inputDevice = inputDevice;
		this.gameController = gameController;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_isInitialized = true;
		Debug.Log("MenuManager Initialized");
	}
	private bool _isInitialized = false;
	
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }




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
		CloseAnyMenu();
		gameController.MakePlayerControllable();
		PauseMenuLevel.Pop();
		
		IsPauseMenuOpened = false;

		Time.timeScale = 1f;
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
		Time.timeScale = 0f;
		
		OpenAnyMenu();
	}

	public void CloseInteractionMenu()
	{
		
		Time.timeScale = 1f;
		CloseAnyMenu();
	}
}
