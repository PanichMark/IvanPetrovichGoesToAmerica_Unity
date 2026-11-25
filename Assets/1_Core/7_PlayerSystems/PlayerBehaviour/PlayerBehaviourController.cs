using UnityEngine;
using System.Collections;

public class PlayerBehaviourController : MonoBehaviour, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;

	public delegate void OnPlayerEventHandler();
	public event OnPlayerEventHandler OnPlayerArmed;
	public event OnPlayerEventHandler OnPlayerDisarmed;
	private GameScenesManager _scenesManager;

	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; }

	private bool _wasPlayerSpottedInThisScene;
	private bool _wasPlayerSpottedOnMission;
	private int _timesPlayerSpottedGameTotal;

	private bool _werePeopleKilledInThisScene;
	private bool _werePeopleKilledOnMission;
	private int _peopleKilledGameTotal;

	private GameCityState _cityState;

	private const int _thresholdKilledPeopleForCityStateAgitatated = 10;
	private const int _thresholdKilledPeopleForCityStateCurfew = 30;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		GameScenesManager scenesManager)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_scenesManager = scenesManager;

		Debug.Log("PlayerBehaviourController Initialized");

		_scenesManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += () => { if (IsPlayerArmed) DisarmPlayer(); };

		_cityState = GameCityState.Normal;
	}

	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyHideWeapons())
		{
			DisarmPlayer();
		}

		/*
		Debug.Log("IS");
		Debug.Log(IsPlayerArmed);
		Debug.Log("WAS");
		Debug.Log(WasPlayerArmed);
		*/

		//Debug.Log(IsPlayerArmed);
	}

	public void OnPlayerSpotted()
	{
		_timesPlayerSpottedGameTotal++;
	}

	public void OnHumanNPCkilled()
	{
		_peopleKilledGameTotal++;

		if (_peopleKilledGameTotal >= _thresholdKilledPeopleForCityStateAgitatated)
		{
			_cityState = GameCityState.Agitatated;
		}
		if (_peopleKilledGameTotal >= _thresholdKilledPeopleForCityStateCurfew)
		{
			_cityState = GameCityState.Curfew;
		}
	}

	public void ArmPlayer()
	{
	//	Debug.Log("ARM!!!!");
		if (!IsPlayerArmed)
		{
			IsPlayerArmed = true;
			WasPlayerArmed = false;

			OnPlayerArmed?.Invoke();

			Debug.Log("Player Armed");
		}
	}

	public void DisarmPlayer()
	{
		//Debug.Log("DISARM!!!!");

		if (IsPlayerArmed)
		{
			IsPlayerArmed = false;
			WasPlayerArmed = true;

			OnPlayerDisarmed?.Invoke();

			Debug.Log("Player Disarmed");
		}
		else
		{
			WasPlayerArmed = false;
		}
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		IsPlayerArmed = data.PlayerBehaviour.IsPlayerArmed;
		WasPlayerArmed = data.PlayerBehaviour.WasPlayerArmed;
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		IsPlayerArmed = false;
		WasPlayerArmed = false;

		OnPlayerDisarmed?.Invoke();

		data.PlayerBehaviour.IsPlayerArmed = IsPlayerArmed;
		data.PlayerBehaviour.WasPlayerArmed = WasPlayerArmed;

		if (IsPlayerArmed)
		{
			ArmPlayer();
		}
		else
		{
			DisarmPlayer();
		}

		yield return null;
	}
}