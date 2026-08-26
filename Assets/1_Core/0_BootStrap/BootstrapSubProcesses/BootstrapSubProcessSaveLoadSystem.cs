using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _gameObjectBootstrapSaveLoadSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private GameScenesManager _gameSceneManager;
	public PlayerPrefsSettingsController PauseSubMenuSettingsPlayerPrefs { get; private set; }
	private IInputDevice _inputDevice;
	public JsonSaveLoadController SaveLoadController { get; private set; }

	public BootstrapSubProcessSaveLoadSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapSaveLoadSystem = new GameObject("Bootstrap_SaveLoadSystem");


		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.AddComponent<JsonSaveLoadController>();
		PauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapSaveLoadSystem.AddComponent<PlayerPrefsSettingsController>();

		SaveLoadController.Initialize(
			_bootstrap,
			_gameSceneManager,
			_gameController);

		PauseSubMenuSettingsPlayerPrefs.Initialize(
			_bootstrap,
			_inputDevice);

		ServiceLocator.Register<SaveLoadController>(SaveLoadController);

		yield break;
	}
}