using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubSystemSaveLoad
{
	private GameObject dataSaveLoadControllerGameObject;
	public SaveLoadController saveLoadController {  get; private set; }


	private GameSceneManager gameSceneManager;
	private GameController gameController;

	public BootstrapSubSystemSaveLoad(GameSceneManager gameSceneManager,
	GameController gameController)
	{
		this.gameSceneManager = gameSceneManager;
		this.gameController = gameController;
	}

	public IEnumerator InitializeSaveLoadSystem()
	{
		//loadingStatusText.text = "Saving System";

		dataSaveLoadControllerGameObject = new GameObject("DataSaveLoadController");
		saveLoadController = dataSaveLoadControllerGameObject.AddComponent<SaveLoadController>();
		saveLoadController.Initialize(gameSceneManager, gameController);

		ServiceLocator.Register("SaveLoadController", saveLoadController);

		Debug.Log("SAVE SYSTEM INITIALIZED");
		yield break;
	}
}
