using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _GameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameMissions _allMissions;
	private LocalizationManager _localizationManager;
	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(Bootstrap bootstrap, BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem, GameMissions allMissions)
	{
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_allMissions = allMissions;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_GameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");

		_missionsManager = _GameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions);

		yield break;
	}

	public void ChangeLanguage()
	{
		_localizationManager = _bootstrap.LocalizationManager;
		_missionsManager.ChangeLanguage(_localizationManager);
	}
}
