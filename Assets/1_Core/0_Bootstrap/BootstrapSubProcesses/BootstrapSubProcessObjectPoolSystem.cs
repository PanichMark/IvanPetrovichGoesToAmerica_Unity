using System.Collections;
using UnityEngine;

public class BootstrapSubProcessObjectPoolSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private PlayerPrefsSettingsController _pauseSubMenuSettingsPlayerPrefs;
	private BootstrapSubProcessMenuSystem _subProcessMenuSystem;
	private GameObject _gameObjectBootstrapObjectPoolSystem;
	private ObjectPoolWeaponController _objectPoolWeaponController;

	public BootstrapSubProcessObjectPoolSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapObjectPoolSystem = new GameObject("Bootstrap_ObjectPoolSystem");

		_objectPoolWeaponController = _gameObjectBootstrapObjectPoolSystem.AddComponent<ObjectPoolWeaponController>();

		_objectPoolWeaponController.Initialize(
			_bootstrap,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController);

		ServiceLocator.Register<IObjectPoolWeaponController>(_objectPoolWeaponController);

		yield break;
	}
}