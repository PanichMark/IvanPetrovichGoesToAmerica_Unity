using UnityEngine;
public class GameController
{

	public bool IsSceneLoading { get; private set; }
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
		IsSceneLoading = true;
		MakePlayerNonControllable();
	}

	public void SceneLoadEnded()
	{
		IsSceneLoading = false;
		MakePlayerControllable();
	}

}