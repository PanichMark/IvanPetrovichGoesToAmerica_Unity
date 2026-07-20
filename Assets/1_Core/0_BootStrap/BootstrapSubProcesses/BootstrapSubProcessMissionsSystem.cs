using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private GameObject _playerCameraGameObject;
	private MissionGoalMarkerController _missionGoalMarkerManager;
	private LocalizationManager _localizationManager;
	private MissionsManager _missionsManager;
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameObject playerCameraGameObject)
	{
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_playerCameraGameObject = playerCameraGameObject;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
	}

	public IEnumerator InitializeMissionsSystem()
	{
		_gameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");
		
		_missionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionGoalMarkerManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionGoalMarkerController>();

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessSceneSystem.GameSceneManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _bootstrapSubProcessMenuSystem.ViewModelPauseMenu);
		_missionGoalMarkerManager.Initialize(_bootstrapSubProcessSceneSystem.GameSceneManager, _missionsManager, _playerCameraGameObject, _bootstrapSubProcessMenuSystem.ViewModelHUDMission.ImageMissionGoalMarker);

		ServiceLocator.Register("MissionsManager", _missionsManager);

		yield break;
	}
}
