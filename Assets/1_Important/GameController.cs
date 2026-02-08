using Unity.VisualScripting;
using UnityEngine;
public class GameController
{
	public bool IsMainMenuOpen {  get; private set; }
	public bool IsPauseMenuAvailable { get; private set; }
	public bool IsPlayerControllable { get; private set; }
	public bool IsPlayerDead { get; private set; }
	public bool IsGameAbleToSave { get; private set; }

	public GameController()
	{
		Debug.Log("GameController Initialized");
	}

	
	
	public void PlayerIsDead()
	{
		IsPlayerDead = true;
	}

	public void MakePlayerNonControllable()
	{
		IsPlayerControllable = false;
	}

	public void MakePlayerControllable()
	{
		IsPlayerControllable = true;
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
		Debug.Log("Open MAINMENU");
	}
	public void CloseMainMenu()
	{
		IsMainMenuOpen = false;
		Debug.Log("Close MAINMENU");

	}
}