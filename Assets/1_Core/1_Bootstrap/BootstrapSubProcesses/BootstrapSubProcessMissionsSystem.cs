using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private GameObject _GameObjectBootstrapMissionsSystem;

	private GameMissions _allMissions;

	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(GameMissions allMissions)
	{
		_allMissions = allMissions;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_GameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");

		_missionsManager = _GameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionsManager.Initialize(_allMissions);

		yield break;
	}
}
