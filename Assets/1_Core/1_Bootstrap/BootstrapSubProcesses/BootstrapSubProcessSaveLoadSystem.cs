using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _gameObjectBootstrapSaveLoadSystem;

	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	public SaveLoadController SaveLoadController { get; private set; }

	public BootstrapSubProcessSaveLoadSystem(GameController gameController, GameSceneManager gameSceneManager)
	{
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
	}

	public IEnumerator InitializeSaveLoadSystem()
	{
		_gameObjectBootstrapSaveLoadSystem = new GameObject("Bootstrap_SaveLoadSystem");

		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.AddComponent<SaveLoadController>();

		SaveLoadController.Initialize(_gameSceneManager, _gameController);

		ServiceLocator.Register("SaveLoadController", SaveLoadController);

		Debug.Log("SAVELOAD SYSTEM INITIALIZED");
		yield break;
	}
}