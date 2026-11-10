using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;
	private GameObject _playerCameraGameObject;
	private MissionGoalMarkerController _missionGoalMarkerManager;
	private LocalizationManager _localizationManager;
	public MissionsManager MissionsManager { get; private set; }
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private GameMissionsList _gameMissions;
	private MissionMapsController _missionMapsController;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessSaveLoadSystem bootstrapSubProcessSaveLoad,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameObject playerCameraGameObject)
	{
		_bootstrapSubProcessSaveLoadSystem = bootstrapSubProcessSaveLoad;
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_playerCameraGameObject = playerCameraGameObject;
		_gameMissions = _bootstrap.GameData.GameMissionsList;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapMissionsSystem = new GameObject("Bootstrap_MissionsSystem");
		
		MissionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();

		_missionGoalMarkerManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionGoalMarkerController>();

		_missionMapsController = _gameObjectBootstrapMissionsSystem.AddComponent<MissionMapsController>();

		MissionsManager.Initialize(
			_localizationManager,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessSaveLoadSystem.SaveLoadController,
			_bootstrapSubProcessMenuSystem.HUDmissionsController,
			_bootstrapSubProcessMenuSystem.JournalMenuSectionMissionInfoController,
			_gameMissions);

		_missionGoalMarkerManager.Initialize(
			_bootstrap,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			MissionsManager, 
			_playerCameraGameObject,
			_bootstrapSubProcessMenuSystem.ViewModelHUDMission.ImageMissionGoalMarker);

		_missionMapsController.Initialize(
			MissionsManager);

		ServiceLocator.Register<MissionsManager>(MissionsManager);
		ServiceLocator.Register<MissionMapsController>(_missionMapsController);

		yield break;
	}
}
