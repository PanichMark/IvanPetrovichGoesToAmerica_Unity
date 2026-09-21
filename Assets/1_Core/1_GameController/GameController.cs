using UnityEngine;
using System.Collections;
public class GameController
{
	public bool IsPlayerControllable { get; private set; }
	public bool IsPlayerAbleToMove { get; private set; }
	public bool IsPlayerMovementRestrictedByCarryingNonThrowable { get; private set; }
	public bool IsPlayerDead { get; private set; }
	public bool IsPlayerPlunging { get; private set; }
	public bool IsMainMenuOrEndGameTitlesActive {  get; private set; }
	public bool IsPauseMenuAvailable { get; private set; }
	public bool IsGameAbleToSave { get; private set; }

	public delegate void SaveGameAvailabilityHandler();
	public event SaveGameAvailabilityHandler OnSaveGameAvailable;
	public event SaveGameAvailabilityHandler OnSaveGameUnavailable;

	public delegate void PlayerDeathHandler();
	public event PlayerDeathHandler OnPlayerEarlyDeath;
	public event PlayerDeathHandler OnPlayerLateDeath;
	public event PlayerDeathHandler OnPlayerRevive;

	public delegate void MainMenuEventHandler();
	public event MainMenuEventHandler OnActivateMainMenuEndGameTitlesActive;
	public event MainMenuEventHandler OnDeactivateMainMenuEndGameTitlesActive;

	public GameController()
	{
		IsPlayerAbleToMove = true;
		IsGameAbleToSave = true;

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
		IsPauseMenuAvailable = false;

		OnPlayerEarlyDeath?.Invoke();
	
		yield return new WaitForSecondsRealtime(1f);

		IsPauseMenuAvailable = true;
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

	private void ActivateMainMenuOrEndGameTitlesActive()
	{
		IsMainMenuOrEndGameTitlesActive = true;
		OnActivateMainMenuEndGameTitlesActive?.Invoke();
		MakePlayerNonControllable();
		Debug.Log("MainMenu opened");
	}

	public void DeactivateMainMenuOrEndGameTitlesActive()
	{
		IsMainMenuOrEndGameTitlesActive = false;
		OnDeactivateMainMenuEndGameTitlesActive?.Invoke();
		Debug.Log("MainMenu closed");
	}

	public void GameplaySceneLoadBegan()
	{
		if (IsPlayerDead)
		{
			IsPlayerDead = false;
			OnPlayerRevive?.Invoke();
		}

		IsPauseMenuAvailable = false;
		MakePlayerNonControllable();
	}

	public void GameplaySceneLoadEnded()
	{
		IsPauseMenuAvailable = true;
		MakePlayerControllable();
	}

	public void MainMenuOrEndGameTitlesSceneLoadBegan()
	{
		if (IsPlayerDead)
		{
			IsPlayerDead = false;
			OnPlayerRevive?.Invoke();
		}

		IsPauseMenuAvailable = false;
		MakePlayerNonControllable();
	}

	public void MainMenuOrEndGameTitlesSceneLoadEnded()
	{
		ActivateMainMenuOrEndGameTitlesActive();
		IsPauseMenuAvailable = true;
	}

	public void BlockInput()
	{
		IsPlayerControllable = false;
	}

	public void UnblockInput()
	{
		IsPlayerControllable = true;
	}

	public void MakeGameSavable()
	{
		IsGameAbleToSave = true;
		OnSaveGameAvailable?.Invoke();
	}

	public void MakeGameUnsavable()
	{
		IsGameAbleToSave = false;
		OnSaveGameUnavailable?.Invoke();
	}

	public void RestrictPlayerMovementWhileCarryingNonThrowable()
	{
		IsPlayerMovementRestrictedByCarryingNonThrowable = true;
	}

	public void UnrestrictPlayerMovementWhileCarryingNonThrowable()
	{
		IsPlayerMovementRestrictedByCarryingNonThrowable = false;
	}
}