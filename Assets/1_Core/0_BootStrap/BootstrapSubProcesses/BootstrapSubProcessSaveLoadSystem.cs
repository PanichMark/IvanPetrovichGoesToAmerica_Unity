using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _gameObjectBootstrapSaveLoadSystem;

	private GameController _gameController;
	private GameScenesManager _gameSceneManager;
	public SaveLoadController SaveLoadController { get; private set; }

	public BootstrapSubProcessSaveLoadSystem(GameController gameController, BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem)
	{
		_gameController = gameController;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
	}

	public IEnumerator InitializeSaveLoadSystem()
	{
		_gameObjectBootstrapSaveLoadSystem = new GameObject("Bootstrap_SaveLoadSystem");

		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.AddComponent<SaveLoadController>();

		SaveLoadController.Initialize(_gameSceneManager, _gameController);

		ServiceLocator.Register("SaveLoadController", SaveLoadController);

		yield break;
	}
}