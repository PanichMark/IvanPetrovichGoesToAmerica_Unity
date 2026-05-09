using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubSystemScene
{
	public GameSceneManager gameSceneManager {  get; private set; }
	private TMP_Text loadingScreenText;
	private TMP_Text sceneNameText;
	private Image sceneLoadingScreenImage;
	private GameObject gameSceneManagerGameObject;
	private GameObject canvasLoadingScreen;
	private GameController gameController;


	public BootstrapSubSystemScene(GameController gameController, GameObject canvasLoadingScreen)
	{
		this.gameController = gameController;
		this.canvasLoadingScreen = canvasLoadingScreen;
	}

	public IEnumerator InitializeSceneSystem()
	{
		gameSceneManagerGameObject = new GameObject("GameSceneManager");
		gameSceneManager = gameSceneManagerGameObject.AddComponent<GameSceneManager>();
		loadingScreenText = canvasLoadingScreen.transform.Find("LoadingScreenText").GetComponent<TMP_Text>();
		sceneNameText = canvasLoadingScreen.transform.Find("SceneName").GetComponent<TMP_Text>();
		sceneLoadingScreenImage = canvasLoadingScreen.transform.Find("BackgroundImage").GetComponent<Image>();
		gameSceneManager.Initialize(gameController, canvasLoadingScreen, loadingScreenText, sceneNameText, sceneLoadingScreenImage);

		ServiceLocator.Register("GameSceneManager", gameSceneManager);

		yield break;
	}
}
