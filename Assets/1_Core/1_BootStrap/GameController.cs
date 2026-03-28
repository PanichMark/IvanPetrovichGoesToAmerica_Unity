using Unity.VisualScripting;
using UnityEngine;
public class GameController
{
	public bool IsMainMenuOpen {  get; private set; }
	public bool IsPauseMenuAvailable { get; private set; }
	public bool IsPlayerControllable { get; private set; }
	public bool IsPlayerDead { get; private set; }
	public bool IsGameAbleToSave { get; private set; }

	public bool IsPlayerPlunging { get; private set; }

	public bool IsPlayerAbleToMove { get; private set;}

	public delegate void MainMenuEventHandler();
	public event MainMenuEventHandler OnOpenMainMenu;

	public GameController()
	{
		IsPlayerAbleToMove = true;
		Debug.Log("GameController Initialized");
	}

	public void StartPlunging()
	{
		IsPlayerAbleToMove = false;
		IsPlayerPlunging = true;
	}
	public void StopPlunging()
	{
		IsPlayerAbleToMove = true;
		IsPlayerPlunging = false;
	}
	public void PlayerIsDead()
	{
		IsPlayerDead = true;
	}

	public void MakePlayerControllable()
	{
		IsPlayerControllable = true;
	}

	public void MakePlayerNonControllable()
	{
		IsPlayerControllable = false;
	}



	public void SceneLoadBegan()
	{
		IsPauseMenuAvailable = false;
		MakePlayerNonControllable();
	}

	public void SceneLoadEnded()
	{
		IsPauseMenuAvailable = true;
		MakePlayerControllable();
	}

	public void OpenMainMenu()
	{
		IsMainMenuOpen = true;
		OnOpenMainMenu?.Invoke();
		MakePlayerNonControllable();
		Debug.Log("Open MAINMENU");
	}
	public void CloseMainMenu()
	{
		IsMainMenuOpen = false;
		Debug.Log("Close MAINMENU");

	}
}