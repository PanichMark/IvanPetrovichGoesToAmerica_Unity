using System.Collections;
using UnityEngine;

public class BootstrapSubProcessObjectPoolSystem
{
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private PlayerPrefsSettingsController _pauseSubMenuSettingsPlayerPrefs;
	private BootstrapSubProcessMenuSystem _subProcessMenuSystem;
	private GameObject _gameObjectBootstrapObjectPoolSystem;
	private ObjectPoolWeaponController _objectPoolWeaponController;

	public BootstrapSubProcessObjectPoolSystem(
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem)
	{
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapObjectPoolSystem = new GameObject("Bootstrap_ObjectPoolSystem");

		_objectPoolWeaponController = _gameObjectBootstrapObjectPoolSystem.AddComponent<ObjectPoolWeaponController>();

		_objectPoolWeaponController.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController);

		ServiceLocator.Register("ObjectPoolWeaponController", _objectPoolWeaponController);

		yield break;
	}
}