using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessScenesSystem
{
	private GameObject _gameObjectBootstrapGameSceneSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private GameObject _canvasSceneLoadingScreen;
	public GameScenesManager GameSceneManager { get; private set; }
	private ViewModelSceneLoadingScreen _viewModelSceneLoadingScreen;
	public BootstrapSubProcessScenesSystem(
		Bootstrap bootstrap,
		GameController gameController,
		LocalizationManager localizationManager,
		GameObject canvasSceneLoadingScreen)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_localizationManager = localizationManager;
		_canvasSceneLoadingScreen = canvasSceneLoadingScreen;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapGameSceneSystem = new GameObject("Bootstrap_GameSceneSystem");
		GameSceneManager = _gameObjectBootstrapGameSceneSystem.AddComponent<GameScenesManager>();

		_viewModelSceneLoadingScreen = new ViewModelSceneLoadingScreen(_bootstrap, _canvasSceneLoadingScreen);

		GameSceneManager.Initialize(
			_gameController,
			_localizationManager,
			_bootstrap.GameData.GameScenesList,
			_canvasSceneLoadingScreen,
			_viewModelSceneLoadingScreen);

		ServiceLocator.Register<GameScenesManager>(GameSceneManager);

		yield break;
	}
}