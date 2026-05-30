using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private GameObject _playerCameraGameObject;
	private GameObject _canvasHUDmission;
	private MissionGoalMarkerManager _missionGoalMarkerManager;
	private GameObject _imageMissionGoalMarker;
	private GameMissions _allMissions;
	private LocalizationManager _localizationManager;
	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameMissions allMissions,
		GameObject canvasHUDmission,
		GameObject playerCameraGameObject)
	{
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_allMissions = allMissions;
		_canvasHUDmission = canvasHUDmission;
		_playerCameraGameObject = playerCameraGameObject;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_gameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");

		_imageMissionGoalMarker = _bootstrap.FindDeepGameObject(_canvasHUDmission, "MissionGoalMarker");
		
		_missionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionGoalMarkerManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionGoalMarkerManager>();

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions);
		_missionGoalMarkerManager.Initialize(_missionsManager, _playerCameraGameObject, _imageMissionGoalMarker);

		yield break;
	}

	public void ChangeLanguage()
	{
		_localizationManager = _bootstrap.LocalizationManager;
		_missionsManager.ChangeLanguage(_localizationManager);
	}
}
