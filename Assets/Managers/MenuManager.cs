using UnityEngine;

public class MenuManager : MonoBehaviour
{
	private IInputDevice inputDevice;

	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		IsPlayerControllable = true;
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_isInitialized = true;
		Debug.Log("MenuManager Initialized");
	}
	private bool _isInitialized = false;
	public bool IsPlayerControllable { get; private set; }
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }




	void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;

		if (inputDevice.GetKeyPauseMenu())
		{
			if (!IsPauseMenuOpened)
				OpenPauseMenu();
			else
				ClosePauseMenu();
		}
	}

	public void OpenPauseMenu()
	{
		if (IsWeaponWheelMenuOpened)
			CloseWeaponWheelMenu(true);

		Debug.Log("PauseMenu opened");
		OpenAnyMenu();

		IsPlayerControllable = false;
		IsPauseMenuOpened = true;

		Time.timeScale = 0f;
	}

	public void ClosePauseMenu()
	{
		Debug.Log("PauseMenu closed");
		CloseAnyMenu();

		IsPlayerControllable = true;
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
