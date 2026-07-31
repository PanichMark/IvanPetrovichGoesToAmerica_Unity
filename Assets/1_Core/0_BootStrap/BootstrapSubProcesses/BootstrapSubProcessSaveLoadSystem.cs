using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _gameObjectBootstrapSaveLoadSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private GameScenesManager _gameSceneManager;
	public SaveLoadController SaveLoadController { get; private set; }

	public BootstrapSubProcessSaveLoadSystem(
		Bootstrap bootstrap,
		GameController gameController,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapSaveLoadSystem = new GameObject("Bootstrap_SaveLoadSystem");

		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.AddComponent<SaveLoadController>();

		SaveLoadController.Initialize(
			_bootstrap,
			_gameSceneManager,
			_gameController);

		ServiceLocator.Register("SaveLoadController", SaveLoadController);

		yield break;
	}
}