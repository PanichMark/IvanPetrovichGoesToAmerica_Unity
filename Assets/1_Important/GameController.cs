using UnityEngine;
public class GameController
{
	private bool _isPlayerControllable = true;
	private bool _isPlayerDead = false;
	private bool _isGameAbleToSave = true;

	public bool IsPlayerControllable => _isPlayerControllable;
	public bool IsPlayerDead => _isPlayerDead;
	public bool IsGameAbleToSave => _isGameAbleToSave;

	public GameController()
	{
		Debug.Log("GameController Initialized");
	}

	public void PlayerIsDead()
	{
		_isPlayerDead = true;
	}

	public void MakePlayerNonControllable()
	{
		_isPlayerControllable = false;
	}

	public void MakePlayerControllable()
	{
		_isPlayerControllable = true;
	}
}