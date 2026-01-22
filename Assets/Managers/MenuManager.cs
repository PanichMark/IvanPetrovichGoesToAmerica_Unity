using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	
	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnOpenPauseMenu;
	public event OpenPauseMenuEventHandler OnClosePauseMenu;

	public IInputDevice inputDevice;
	private IGameController gameController;



	public Stack<int> menuLevelStack = new Stack<int>();

	public void Initialize(IInputDevice inputDevice, IGameController gameController)
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

		//Debug.Log(menuLevelStack.Count);

		if (inputDevice.GetKeyPauseMenu())
		{
			if (menuLevelStack.Count == 0)
			{
				OpenPauseMenu();   
			}
			else if (menuLevelStack.Count == 1)
			{
				ClosePauseMenu();
			}
		}
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
		{
			CloseWeaponWheelMenu(true);
		}
		menuLevelStack.Push(1);
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
		menuLevelStack.Pop();
		IsPauseMenuOpened = false;

		Time.timeScale = 1f;
	}

	public void OpenWeaponWheelMenu(string handType)
	{
		OpenAnyMenu();
		IsWeaponWheelMenuOpened = true;

		if (handType == "right")
			Debug.Log("Right WeaponWheelMenu opened");
		else if (handType == "left")
			Debug.Log("Left WeaponWheelMenu opened");
	}

	public void CloseWeaponWheelMenu(bool IsItRightWeaponWheelMenu)
	{
		CloseAnyMenu();
		IsWeaponWheelMenuOpened = false;

		if (IsItRightWeaponWheelMenu)
			Debug.Log("Right WeaponWheelMenu closed");
		else
			Debug.Log("Left WeaponWheelMenu closed");
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
