using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	// Event для вызова меню паузы
	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnOpenPauseMenu;
	public event OpenPauseMenuEventHandler OnClosePauseMenu;
	public delegate void OpenPauseSubMenuEventHandler();
	public event OpenPauseSubMenuEventHandler OnOpenPauseSubMenu;
	public event OpenPauseSubMenuEventHandler OnClosePauseSubMenu;
	public IInputDevice inputDevice;
	private IGameController gameController;


	// Стек для хранения уровней открытых меню
	public Stack<int> menuLevelStack = new Stack<int>();
	// Конструктор принимает зависимость
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
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;

		if (inputDevice.GetKeyPauseMenu()) // Нажата кнопка паузы
		{
			if (menuLevelStack.Count >= 2)
			{
				// Если открыто субменю, перейдем на предыдущий уровень
				menuLevelStack.Pop(); // Убираем текущий уровень (submenu)
				OnClosePauseSubMenu?.Invoke();
				OpenPauseMenu();       // Возвращаемся к меню паузы

			}
			else if (menuLevelStack.Count == 1)
			{
				// Только основное меню открыто, закрываем его
				ClosePauseMenu();
			}
			else
			{
				// Ничего не открыто, открываем меню паузы
				OpenPauseMenu();
			}
		}
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
			CloseWeaponWheelMenu(true);
		// Заполнение стека на уровне 1
		menuLevelStack.Push(1);
		OnOpenPauseMenu?.Invoke(); // Вызвать событие открытия меню паузы
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
		// Удаляем верхний элемент стека
		if (menuLevelStack.Count > 0)
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

	// Реализация открытия меню взаимодействия с исключением заданного объекта
	public void OpenInteractionMenu()
	{
		Time.timeScale = 0f;
		
		OpenAnyMenu();
	}

	// Реализация закрытия меню взаимодействия и возобновления нормального течения времени
	public void CloseInteractionMenu()
	{
		
		Time.timeScale = 1f;
		CloseAnyMenu();
	}
}
