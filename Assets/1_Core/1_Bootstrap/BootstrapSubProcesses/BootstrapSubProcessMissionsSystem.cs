using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private GameObject _GameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameMissions _allMissions;

	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem, GameMissions allMissions)
	{
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_allMissions = allMissions;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_GameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");

		_missionsManager = _GameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionsManager.Initialize(_bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions);

		yield break;
	}
}
