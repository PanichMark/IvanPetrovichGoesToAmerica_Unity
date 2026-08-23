using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDiegeticButtonController : MonoBehaviour
{
	private Material _defaultMaterial;     
	private Material _hoverMaterial;
	private MenuBackgroundController _menuBackgroundController;
	private static List<MainMenuDiegeticButtonController> _instances = new List<MainMenuDiegeticButtonController>();
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private MainMenuReadNewsController _mainMenuReadNews;
	private PauseMenuController _pauseMenuController;
	private GameController _gameController;
	private Renderer _renderer;
	private GameScenesManager _gameSceneManager;
	private Collider _collider;
	private SaveLoadController _saveLoadController;
	private MenuManager _menuManager;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private KeyCode _keyPauseMenu;
	private ICutscene _cutsceneNewGame;
	[SerializeField] private MainMenuDiegeticButtonsEnum _mainMenuDiegeticButtonsEnum;
	private MainMenuChooseMissionController _mainMenuChooseMissionController;
	public bool IsCutsceneNewGamePlaying { get; private set; }
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private MainMenuCanvasController _mainMenuCanvasController;
	private PauseSubMenuSettingsGameDifficultyController _pauseSubMenuSettingsGameDifficultyController;

	public void Initialize(
		MainMenuChooseMissionController mainMenuChooseMissionController,
		MainMenuReadNewsController mainMenuReadNews,
		Material hoverMaterial)
	{
		_instances.Add(this);

		_collider = GetComponent<Collider>();
		_renderer = GetComponent<Renderer>();
		_defaultMaterial = _renderer.material;
		_mainMenuCanvasController = GameObject.Find("MainMenuCanvasController").GetComponent<MainMenuCanvasController>();
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
		_hoverMaterial = hoverMaterial;
		//_cutsceneNewGame = GameObject.Find("CutsceneNewGame").GetComponent<ICutscene>();
		_menuBackgroundController = ServiceLocator.Resolve<MenuBackgroundController>("MenuBackgroundController");
		_keyPauseMenu = ServiceLocator.Resolve<KeyCode>("KeyPauseMenu");
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>("GameSceneManager");
		_pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_playerCameraBlurFilter = ServiceLocator.Resolve<PlayerCameraBlurFilter>("PlayerCameraBlurFilter");
		_pauseSubMenuSettingsController = ServiceLocator.Resolve<PauseSubMenuSettingsController>("PauseSubMenuSettingsController");
		_pauseSubMenuSettingsGameDifficultyController = ServiceLocator.Resolve<PauseSubMenuSettingsGameDifficultyController>("PauseSubMenuSettingsGameDifficultyController");

		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ChooseMission)
		{
			_mainMenuChooseMissionController = mainMenuChooseMissionController;

			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission += EnableAllColliders;
			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission += _playerCameraBlurFilter.DeactivateCameraBlur;
			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission += _menuBackgroundController.HideCanvasMenuBackground;
		}

		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ReadNews)
		{
			_mainMenuReadNews = mainMenuReadNews;

			_mainMenuReadNews.OnCloseMainMenuReadNews += EnableAllColliders;
			_mainMenuReadNews.OnCloseMainMenuReadNews += _playerCameraBlurFilter.DeactivateCameraBlur;
			_mainMenuReadNews.OnCloseMainMenuReadNews += _menuBackgroundController.HideCanvasMenuBackground;
		}

		_pauseMenuController.OnCloseAnyPauseSubMenu += EnableAllColliders;

		Debug.Log($"MainMenu DiegeticButon-{_mainMenuDiegeticButtonsEnum} Initialized");
	}

	void OnDestroy()
	{
		_instances.Remove(this);

		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ReadNews)
		{
			_mainMenuReadNews.OnCloseMainMenuReadNews -= _playerCameraBlurFilter.DeactivateCameraBlur;
			_mainMenuReadNews.OnCloseMainMenuReadNews -= EnableAllColliders;
			_mainMenuReadNews.OnCloseMainMenuReadNews -= _playerCameraBlurFilter.DeactivateCameraBlur;
		}

		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ChooseMission)
		{
			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission -= EnableAllColliders;
			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission -= _playerCameraBlurFilter.DeactivateCameraBlur;
			_mainMenuChooseMissionController.OnCloseMainMenuChooseMission -= _menuBackgroundController.HideCanvasMenuBackground;
		}

		_pauseMenuController.OnCloseAnyPauseSubMenu -= EnableAllColliders;

		if (IsCutsceneNewGamePlaying)
		{
			_gameController.CloseMainMenu();
			_menuManager.OpenInteractionHUD();
		}
	}

	private void Update()
	{
		if (!IsCutsceneNewGamePlaying)
		{
			if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.NewGame)
			{
				if (Input.GetKeyDown(_keyPauseMenu) && _menuManager.PauseMenuLevel.Count == 1)
				{
					_menuManager.CloseAnyMenu();
					_mainMenuCanvasController.ShowMainMenuCanvas();
					_pauseMenuController.ClosePauseSubMenu();
				}
				if (Input.GetKeyDown(_keyPauseMenu) && _menuManager.PauseMenuLevel.Count == 2)
				{
					if (!_pauseMenuController.IsPauseConfirmMenuOpened && !_pauseSubMenuSettingsGameDifficultyController.IsChooseGameDifficultyMenuOpened)
					{
						_pauseSubMenuSettingsController.ShowSettingsSubMenuCanvas();
						_menuManager.PopPauseMenuLevel();
					}
					if (_pauseMenuController.IsPauseConfirmMenuOpened)
					{
						_pauseMenuController.ClosePauseConfirmMenu();
					}
					if (_pauseSubMenuSettingsGameDifficultyController.IsChooseGameDifficultyMenuOpened)
					{
						_pauseSubMenuSettingsGameDifficultyController.HideMenuGameDifficulty();
						_pauseSubMenuSettingsController.ShowSettingsSubMenuCanvas();

						_menuManager.PopPauseMenuLevel();
					}
				}
			}
			if (Input.GetKeyDown(_keyPauseMenu) &&
			((_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ChooseMission) ||
			 (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ReadNews)))
			{
				if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ChooseMission && _mainMenuChooseMissionController.IsMainMenuChooseMissionOpened && _menuManager.PauseMenuLevel.Count == 1)
				{
					_mainMenuChooseMissionController.HideCanvasMainMenuChooseMission();
				}
				if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ReadNews && _mainMenuReadNews.IsMainMenuReadNewsOpened)
				{
					_mainMenuReadNews.HideCanvasMainMenuReadNews();
				}
			}
		}
	}

	void OnMouseEnter()
	{
		_renderer.material = _hoverMaterial;
	}

	void OnMouseExit()
	{
		_renderer.material = _defaultMaterial;
	}

	void OnMouseDown()
	{
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.NewGame)
		{
			Debug.Log("START NEW GAME");
			DisableAllColliders();
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.Locked;
			_mainMenuCanvasController.HideGameVersionCanvas();
			StartCoroutine(StartNewGame());
			_cutsceneNewGame.TriggerCutscene();
			IsCutsceneNewGamePlaying = true;
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.TestScene)
		{
			Debug.Log("TEST SCENE");
			DisableAllColliders();
			_gameController.CloseMainMenu();
			StartCoroutine(LoadTestScene());
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.EndGameTitles)
		{

		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.LoadGame)
		{
			Debug.Log("OPEN LOAD GAME");
			_menuBackgroundController.ShowCanvasMenuBackground();
			_mainMenuCanvasController.HideMainMenuCanvas();
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenLoadSubMenu();
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.Settings)
		{
			Debug.Log("OPEN SETTINGS");
			_menuBackgroundController.ShowCanvasMenuBackground();
			_mainMenuCanvasController.HideMainMenuCanvas();
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenSettingsSubMenu();
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ChooseMission)
		{
			Debug.Log("CHOOSE MISSION");
			_menuBackgroundController.ShowCanvasMenuBackground();
			_mainMenuCanvasController.HideMainMenuCanvas();
			_mainMenuChooseMissionController.ShowCanvasMainMenuChooseMission();
			DisableAllColliders();
			_menuManager.PushPauseMenuLevel();
			_playerCameraBlurFilter.ActivateCameraBlur();
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ReadNews)
		{
			Debug.Log("OPEN NEWS");
			_menuBackgroundController.ShowCanvasMenuBackground();
			_mainMenuCanvasController.HideMainMenuCanvas();
			_mainMenuReadNews.ShowCanvasMainMenuReadNews();
			DisableAllColliders();
			_playerCameraBlurFilter.ActivateCameraBlur();
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.ExitGame)
		{
			Debug.Log("EXIT GAME");
			Application.Quit();
		}
	}

	public void EnableAllColliders()
	{
		foreach (var instance in _instances)
		{
			var colliderInstance = instance._collider;
			colliderInstance.enabled = true;
		}
	}

	private void DisableAllColliders()
	{
		foreach (var instance in _instances)
		{
			var colliderInstance = instance._collider;
			colliderInstance.enabled = false;
		}
	}

	IEnumerator LoadTestScene()
	{
		gameObject.transform.SetParent(null);

		DontDestroyOnLoad(gameObject);

		//yield return StartCoroutine(_saveLoadController.NewGame());
		_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(GameScenesSystemEnum.Scene_0_Test));
	
		Destroy(gameObject);
	}

	IEnumerator StartNewGame()
	{
		yield return null;
		//yield return StartCoroutine(_saveLoadController.NewGame());
	}
}