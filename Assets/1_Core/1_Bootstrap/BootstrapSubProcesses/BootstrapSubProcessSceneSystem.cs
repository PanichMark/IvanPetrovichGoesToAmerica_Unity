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
	private LocalizationManager _localizationManager;
	public GameSceneManager GameSceneManager { get; private set; }

	private GameObject _canvasLoadingScreen;

	private GameObject _textLoadingReady;
	private GameObject _textSceneName;
	private GameObject _textSceneDescription;
	private GameObject _sliderLoadingStatus;
	private Image _imageLoadingScreen;

	public BootstrapSubProcessSceneSystem(Bootstrap bootstrap, GameController gameController, LocalizationManager localizationManager, GameObject canvasLoadingScreen)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_canvasLoadingScreen = canvasLoadingScreen;	
		_localizationManager = localizationManager;
	}

	public IEnumerator InitializeSceneSystem()
	{
		_gameObjectBootstrapGameSceneSystem = new GameObject("Bootstrap_GameSceneSystem");
		GameSceneManager = _gameObjectBootstrapGameSceneSystem.AddComponent<GameSceneManager>();

		_textLoadingReady = _bootstrap.FindDeepGameObject(_canvasLoadingScreen, "TextLoadingIsReady");
		_textSceneName = _bootstrap.FindDeepGameObject(_canvasLoadingScreen, "TextSceneName");
		_textSceneDescription = _bootstrap.FindDeepGameObject(_canvasLoadingScreen, "TextSceneDescription");
		_sliderLoadingStatus = _bootstrap.FindDeepGameObject(_canvasLoadingScreen, "SliderLoadingStatus");
		_imageLoadingScreen = _canvasLoadingScreen.transform.Find("ImageScene").GetComponent<Image>();


		GameSceneManager.Initialize(_gameController, _localizationManager, _canvasLoadingScreen, _textLoadingReady, _textSceneName, _textSceneDescription, _sliderLoadingStatus, _imageLoadingScreen);

		ServiceLocator.Register("GameSceneManager", GameSceneManager);

		yield break;
	}
}