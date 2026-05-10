using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject _saveLoadControllerGameObject;
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
		_saveLoadControllerGameObject = new GameObject("Bootstrap_SaveLoadSystem");
		SaveLoadController = _saveLoadControllerGameObject.AddComponent<SaveLoadController>();
		SaveLoadController.Initialize(_gameSceneManager, _gameController);

		ServiceLocator.Register("SaveLoadController", SaveLoadController);

		Debug.Log("SAVE SYSTEM INITIALIZED");
		yield break;
	}
}