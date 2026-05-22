using log4net.Util;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessSceneSystem
{
	private GameObject _gameObjectBootstrapGameSceneSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	public GameSceneManager GameSceneManager { get; private set; }

	private GameObject _canvasLoadingScreen;

	private GameObject _textLoadingReady;
	private TMP_Text _textSceneName;
	private TMP_Text _textSceneDescription;
	private Slider _sliderLoadingStatus;
	private Image _imageLoadingScreen;

	public BootstrapSubProcessSceneSystem(Bootstrap bootstrap, GameController gameController, GameObject canvasLoadingScreen)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_canvasLoadingScreen = canvasLoadingScreen;
	}

	public IEnumerator InitializeSceneSystem()
	{
		_gameObjectBootstrapGameSceneSystem = new GameObject("Bootstrap_GameSceneSystem");
		GameSceneManager = _gameObjectBootstrapGameSceneSystem.AddComponent<GameSceneManager>();

		_textLoadingReady = _bootstrap.FindDeepGameObject(_canvasLoadingScreen, "TextLoadingScreenReady");
		_textSceneName = _canvasLoadingScreen.transform.Find("TextSceneName").GetComponent<TMP_Text>();
		_textSceneDescription = _canvasLoadingScreen.transform.Find("TextLoadingScreenDescription").GetComponent<TMP_Text>();
		_sliderLoadingStatus = _canvasLoadingScreen.transform.Find("TextLoadingScreenDescription").GetComponent<Slider>();
		_imageLoadingScreen = _canvasLoadingScreen.transform.Find("ImageLoadingScreen").GetComponent<Image>();


		GameSceneManager.Initialize(_gameController, _canvasLoadingScreen, _textLoadingReady, _textSceneName, _textSceneDescription, _sliderLoadingStatus, _imageLoadingScreen);

		ServiceLocator.Register("GameSceneManager", GameSceneManager);

		Debug.Log("SCENE SYSTEM INITIALIZED");
		yield break;
	}
}