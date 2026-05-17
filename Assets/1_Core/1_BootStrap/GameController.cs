using UnityEngine;
using System.Collections;
public class GameController
{
	public bool IsPlayerControllable { get; private set; }
	public bool IsPlayerAbleToMove { get; private set;}
	public bool IsPlayerDead { get; private set; }
	public bool IsPlayerPlunging { get; private set; }
	public bool IsMainMenuOpen {  get; private set; }
	public bool IsPauseMenuAvailable { get; private set; }
	
	public delegate void PlayerDeathHandler();
	public event PlayerDeathHandler OnPlayerEarlyDeath;
	public event PlayerDeathHandler OnPlayerLateDeath;
	public event PlayerDeathHandler OnPlayerRevive;

	public delegate void MainMenuEventHandler();
	public event MainMenuEventHandler OnOpenMainMenu;

	public GameController()
	{
		IsPlayerAbleToMove = true;
		Debug.Log("GameController Initialized");
	}

	public void MakePlayerControllable()
	{
		IsPlayerControllable = true;
	}

	public void MakePlayerNonControllable()
	{
		IsPlayerControllable = false;
	}

	public IEnumerator PlayerHasDied()
	{
		IsPlayerDead = true;
		MakePlayerNonControllable();
		OnPlayerEarlyDeath?.Invoke();

		yield return new WaitForSecondsRealtime(1f);

		OnPlayerLateDeath?.Invoke();
	}

	public void PlayerStartedPlunging()
	{
		IsPlayerAbleToMove = false;
		IsPlayerPlunging = true;
	}
	public void PlayerStoppedPlunging()
	{
		IsPlayerAbleToMove = true;
		IsPlayerPlunging = false;
	}

	public void OpenMainMenu()
	{
		IsMainMenuOpen = true;
		OnOpenMainMenu?.Invoke();
		MakePlayerNonControllable();
		Debug.Log("MainMenu opened");
	}

	public void CloseMainMenu()
	{
		IsMainMenuOpen = false;
		Debug.Log("MainMeni closed");
	}

	public void SceneLoadBegan()
	{
		if (IsPlayerDead)
		{
			IsPlayerDead = false;
			OnPlayerRevive?.Invoke();
		}

		IsPauseMenuAvailable = false;
		MakePlayerNonControllable();
	}

	public void SceneLoadEnded()
	{
		IsPauseMenuAvailable = true;
		MakePlayerControllable();
	}
}