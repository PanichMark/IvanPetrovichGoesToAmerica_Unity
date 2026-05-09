using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessSceneSystem
{
	public GameSceneManager GameSceneManager { get; private set; }
	private TMP_Text _textLoadingScreenStatus;
	private TMP_Text _textSceneName;
	private Image _imageLoadingScreen;
	private GameObject _gameSceneManagerGameObject;
	private GameObject _canvasLoadingScreen;
	private GameController _gameController;

	public BootstrapSubProcessSceneSystem(GameController gameController, GameObject canvasLoadingScreen)
	{
		_gameController = gameController;
		_canvasLoadingScreen = canvasLoadingScreen;
	}

	public IEnumerator InitializeSceneSystem()
	{
		_gameSceneManagerGameObject = new GameObject("GameSceneManager");
		GameSceneManager = _gameSceneManagerGameObject.AddComponent<GameSceneManager>();

		_textLoadingScreenStatus = _canvasLoadingScreen.transform.Find("TextLoadingScreenStatus").GetComponent<TMP_Text>();
		_textSceneName = _canvasLoadingScreen.transform.Find("TextSceneName").GetComponent<TMP_Text>();
		_imageLoadingScreen = _canvasLoadingScreen.transform.Find("ImageLoadingScreen").GetComponent<Image>();

		GameSceneManager.Initialize(_gameController, _canvasLoadingScreen, _textLoadingScreenStatus, _textSceneName, _imageLoadingScreen);

		ServiceLocator.Register("GameSceneManager", GameSceneManager);

		yield break;
	}
}