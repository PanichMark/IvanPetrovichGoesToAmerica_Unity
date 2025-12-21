using UnityEngine;

public class MenuManager : MonoBehaviour
{
	private IInputDevice inputDevice;


	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
		Debug.Log("MenuManager Initialized");
	}

	
	//PauseMenuController pauseMenuController;


	public bool IsPlayerControllable { get; private set; }
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }
	
	void Start()
    {
     
		//pauseMenuController = GetComponent<PauseMenuController>();

		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(false);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		IsPlayerControllable = true;
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
	}

	
	void Update()
    {
		if (inputDevice.GetKeyPauseMenu())
		{
			if (!IsPauseMenuOpened)
			{
				OpenPauseMenu();
			}
			else ClosePauseMenu();
		}
		//Debug.Log(IsPauseMenuOpened);
	}
	

	
    public void OpenPauseMenu()
    {
		if (IsWeaponWheelMenuOpened)
		{
			CloseWeaponWheelMenu(true);
		}

		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true); // Скрываем Canvas

		Debug.Log("PauseMenu opened");
		OpenAnyMenu();

        IsPlayerControllable = false;
		IsPauseMenuOpened = true;

		// Полностью останавливаем игру
		Time.timeScale = 0f;
	}
	public void ClosePauseMenu()
	{
		//if (pauseMenuController.PauseMenuCanvas.gameObject.activeInHierarchy)
		//{
			//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(false); // Скрываем Canvas

			Debug.Log("PauseMenu closed");
			CloseAnyMenu();

			IsPlayerControllable = true;
			IsPauseMenuOpened = false;

			// Возвращаем нормальное течение времени
			Time.timeScale = 1f;
		//}
	}
	

	public void OpenWeaponWheelMenu(string handType)
	{
		OpenAnyMenu();
		IsWeaponWheelMenuOpened = true;

		if (handType == "right")
		{
			Debug.Log("Right WeaponWheelMenu opened");
		}
		else if (handType == "left")
		{ 
			Debug.Log("Left WeaponWheelMenu opened");
		}

		//Time.timeScale = 0.2f;
	}

	public void CloseWeaponWheelMenu(bool IsItRightWeaponWheelMenu)
	{
		CloseAnyMenu();
		IsWeaponWheelMenuOpened = false;
		if (IsItRightWeaponWheelMenu)
		{
			Debug.Log("Right WeaponWheelMenu closed");
		}
		else
		{
			Debug.Log("Left WeaponWheelMenu closed");
		}

		//Time.timeScale = 1f;
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
}

