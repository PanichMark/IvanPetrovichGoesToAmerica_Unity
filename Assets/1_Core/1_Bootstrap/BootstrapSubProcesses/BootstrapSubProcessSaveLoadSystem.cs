using System.Collections;
using UnityEngine;

public class BootstrapSubProcessSaveLoadSystem
{
	private GameObject dataSaveLoadControllerGameObject;
	public SaveLoadController saveLoadController { get; private set; }

	private GameSceneManager gameSceneManager;
	private GameController gameController;

	public BootstrapSubProcessSaveLoadSystem(GameSceneManager gameSceneManager, GameController gameController)
	{
		this.gameSceneManager = gameSceneManager;
		this.gameController = gameController;
	}

	public IEnumerator InitializeSaveLoadSystem()
	{
		dataSaveLoadControllerGameObject = new GameObject("DataSaveLoadController");
		saveLoadController = dataSaveLoadControllerGameObject.AddComponent<SaveLoadController>();
		saveLoadController.Initialize(gameSceneManager, gameController);

		ServiceLocator.Register("SaveLoadController", saveLoadController);

		Debug.Log("SAVE SYSTEM INITIALIZED");
		yield break;
	}
}