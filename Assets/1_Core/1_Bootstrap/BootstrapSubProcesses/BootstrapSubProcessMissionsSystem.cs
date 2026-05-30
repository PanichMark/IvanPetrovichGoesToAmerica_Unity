using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private GameObject _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private WorldToUISpace _worldToUISpace;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private GameObject _playerCameraGameObject;
	private GameObject _canvasHUDmission;
	private RectTransform _canvasRectTransform;
	private Camera _playerCamera;
	private MissionObjectiveMarker _missionObjectiveMarker;
	private GameObject _UImarker;
	private RectTransform _UImarkerRectTransform;
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


		_canvasRectTransform = _canvasHUDmission.GetComponent<RectTransform>();
		//_playerCamera = _bootstrap.FindDeepGameObject(_playerCameraGameObject, "ButtonPauseMenuExitToMainMenu")
		_UImarker = _bootstrap.FindDeepGameObject(_canvasHUDmission, "MissionMarker");
		_UImarkerRectTransform = _UImarker.GetComponent<RectTransform>();
		Debug.Log(_playerCameraGameObject);
		_playerCamera = _playerCameraGameObject.GetComponent<Camera>();
		

		_missionsManager = _gameObjectBootstrapMissionsSystem.AddComponent<MissionsManager>();
		_worldToUISpace = _UImarker.AddComponent<WorldToUISpace>();
		_missionObjectiveMarker = _gameObjectBootstrapMissionsSystem.AddComponent<MissionObjectiveMarker>();

		Canvas canvasComponent = _canvasHUDmission.GetComponent<Canvas>();
	
		canvasComponent.worldCamera = _playerCamera;

		_missionsManager.Initialize(_localizationManager, _bootstrapSubProcessMenuSystem.PauseMenuController, _allMissions);
		_worldToUISpace.Initialize(_canvasRectTransform, _playerCamera);
		_missionObjectiveMarker.Initialize(_missionsManager, _playerCameraGameObject, _canvasRectTransform, _worldToUISpace);

		yield break;
	}

	public void ChangeLanguage()
	{
		_localizationManager = _bootstrap.LocalizationManager;
		_missionsManager.ChangeLanguage(_localizationManager);
	}
}
