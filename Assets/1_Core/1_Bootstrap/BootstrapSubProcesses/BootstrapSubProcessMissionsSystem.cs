using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private WorldToUISpace _worldToUISpace;

	private GameObject _canvasHUDmission;

	private GameMissions _allMissions;
	private LocalizationManager _localizationManager;
	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameMissions allMissions,
		GameObject canvasHUDmission)
	{
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_allMissions = allMissions;
		_canvasHUDmission = canvasHUDmission;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_gameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");


		_missionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();
		_worldToUISpace = _gameObjectBootstrapMissionsSystem.AddComponent<WorldToUISpace>();

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions);

		yield break;
	}

	public void ChangeLanguage()
	{
		_localizationManager = _bootstrap.LocalizationManager;
		_missionsManager.ChangeLanguage(_localizationManager);
	}
}
