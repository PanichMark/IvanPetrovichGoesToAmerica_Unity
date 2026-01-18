using UnityEngine;

public class MenuManager : MonoBehaviour
{
	// Event для вызова меню паузы
	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnOpenPauseMenu;
	public event OpenPauseMenuEventHandler OnClosePauseMenu;
	public IInputDevice inputDevice;
	private IGameController gameController;
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

		if (inputDevice.GetKeyPauseMenu()) // Любое нажатие
		{
			if (!IsPauseMenuOpened)
				OpenPauseMenu(); // Открывать, если меню не открыто
			else ClosePauseMenu();
		}
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
			CloseWeaponWheelMenu(true);

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
