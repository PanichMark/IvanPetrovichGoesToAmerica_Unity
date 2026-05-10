using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _gameObjectBootstrapSaveLoadSystem;
	public SaveLoadController SaveLoadController { get; private set; }

	private GameSceneManager _gameSceneManager;
	private GameController _gameController;

	public BootstrapSubProcessSaveLoadSystem(GameSceneManager gameSceneManager, GameController gameController)
	{
		_gameSceneManager = gameSceneManager;
		_gameController = gameController;
	}

	public IEnumerator InitializeSaveLoadSystem()
	{
		_gameObjectBootstrapSaveLoadSystem = new GameObject("Bootstrap_SaveLoadSystem");
		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.AddComponent<SaveLoadController>();
		SaveLoadController.Initialize(_gameSceneManager, _gameController);

		ServiceLocator.Register("SaveLoadController", SaveLoadController);

		Debug.Log("SAVE SYSTEM INITIALIZED");
		yield break;
	}
}