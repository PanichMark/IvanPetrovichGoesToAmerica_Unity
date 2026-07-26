using System.Collections;
using UnityEngine;

public class BootstrapSubProcessPlayerPrefsSystem
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerPrefsSettingsController _pauseSubMenuSettingsPlayerPrefs;
	private BootstrapSubProcessMenuSystem _subProcessMenuSystem;
	private GameObject _gameObjectBootstrapPlayerPrefsSystem;

	public BootstrapSubProcessPlayerPrefsSystem(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		BootstrapSubProcessMenuSystem subProcessMenuSystem)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_subProcessMenuSystem = subProcessMenuSystem;
	}

	public IEnumerator InitializePlayerPrefsSystem()
	{
		_gameObjectBootstrapPlayerPrefsSystem = new GameObject("Bootstrap_MenuSystem");

		_pauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapPlayerPrefsSystem.AddComponent<PlayerPrefsSettingsController>();

		_pauseSubMenuSettingsPlayerPrefs.Initialize(
			_bootstrap,
			_inputDevice,
			_subProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_subProcessMenuSystem.PauseSubMenuSettingsSectionControlsController,
			_subProcessMenuSystem.PauseSubMenuSettingsSectionGraphicsController,
			_subProcessMenuSystem.PauseSubMenuSettingsSectionAudioController);

		yield break;
	}
}
