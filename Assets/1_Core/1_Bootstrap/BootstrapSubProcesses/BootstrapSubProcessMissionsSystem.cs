using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private GameObject _playerCameraGameObject;
	private MissionGoalMarkerManager _missionGoalMarkerManager;
	private GameMissions _allMissions;
	private LocalizationManager _localizationManager;
	private MissionsManager _missionsManager;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameObject playerCameraGameObject)
	{
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_playerCameraGameObject = playerCameraGameObject;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_gameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");
		_allMissions = (GameMissions)Resources.Load("GameMissions");

		_missionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionGoalMarkerManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionGoalMarkerManager>();

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions, _bootstrapSubProcessMenuSystem.ViewModelPauseMenu);
		_missionGoalMarkerManager.Initialize(_missionsManager, _playerCameraGameObject, _bootstrapSubProcessMenuSystem.ViewModelHUDMission.ImageMissionGoalMarker);

		yield break;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	
		_missionsManager.ChangeLanguage(_localizationManager);
	}
}
