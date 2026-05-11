using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessSceneSystem
{
	private GameObject _gameObjectBootstrapGameSceneSystem;

	private GameController _gameController;
	public GameSceneManager GameSceneManager { get; private set; }

	private GameObject _canvasLoadingScreen;

	private TMP_Text _textLoadingScreenStatus;
	private TMP_Text _textSceneName;
	private Image _imageLoadingScreen;

	public BootstrapSubProcessSceneSystem(GameController gameController, GameObject canvasLoadingScreen)
	{
		_gameController = gameController;
		_canvasLoadingScreen = canvasLoadingScreen;
	}

	public IEnumerator InitializeSceneSystem()
	{
		_gameObjectBootstrapGameSceneSystem = new GameObject("Bootstrap_GameSceneSystem");
		GameSceneManager = _gameObjectBootstrapGameSceneSystem.AddComponent<GameSceneManager>();

		_textLoadingScreenStatus = _canvasLoadingScreen.transform.Find("TextLoadingScreenStatus").GetComponent<TMP_Text>();
		_textSceneName = _canvasLoadingScreen.transform.Find("TextSceneName").GetComponent<TMP_Text>();
		_imageLoadingScreen = _canvasLoadingScreen.transform.Find("ImageLoadingScreen").GetComponent<Image>();

		GameSceneManager.Initialize(_gameController, _canvasLoadingScreen, _textLoadingScreenStatus, _textSceneName, _imageLoadingScreen);

		ServiceLocator.Register("GameSceneManager", GameSceneManager);

		Debug.Log("SCENE SYSTEM INITIALIZED");
		yield break;
	}
}