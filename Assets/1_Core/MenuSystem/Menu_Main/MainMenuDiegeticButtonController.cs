using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuDiegeticButtonController : MonoBehaviour
{
	private Material _defaultMaterial;     
	private Material _hoverMaterial;
	private GameObject _CanvasDiegeticText;

	private static List<MainMenuDiegeticButtonController> _instances = new List<MainMenuDiegeticButtonController>();
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private MainMenuReadNewsController _mainMenuReadNews;
	private PauseMenuController _pauseMenuController;
	private GameController _gameController;
	private Renderer _renderer;
	private GameSceneManager _gameSceneManager;
	private Collider _collider;
	private SaveLoadController _saveLoadController;
	private MenuManager _menuManager;
	private KeyCode _keyPauseMenu;

	void Start()
	{
		_instances.Add(this);

		_collider = GetComponent<Collider>();
		_renderer = GetComponent<Renderer>();
		_defaultMaterial = _renderer.material;
		_hoverMaterial = Resources.Load<Material>("Materials/Material_MainMenuDiegeticButton");

		_CanvasDiegeticText = GameObject.Find("CANVASES");

		_keyPauseMenu = ServiceLocator.Resolve<KeyCode>("KeyPauseMenu");
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_mainMenuReadNews = ServiceLocator.Resolve<MainMenuReadNewsController>("MainMenuReadNews");
		_playerCameraBlurFilter = ServiceLocator.Resolve<PlayerCameraBlurFilter>("PlayerCameraBlurFilter");

		_mainMenuReadNews.OnCloseMainMenuReadNews += EnableAllColliders;
		_mainMenuReadNews.OnCloseMainMenuReadNews += EnableDiegeticText;
		_mainMenuReadNews.OnCloseMainMenuReadNews += _playerCameraBlurFilter.DeactivateCameraBlur;
		_pauseMenuController.OnCloseAnyPauseSubMenu += EnableAllColliders;
		_menuManager.OnCloseAnyMenu += EnableDiegeticText;
	}

	private void EnableDiegeticText()
	{
		_CanvasDiegeticText.SetActive(true);
	}

	void OnDestroy()
	{
		_instances.Remove(this);

		_mainMenuReadNews.OnCloseMainMenuReadNews -= _playerCameraBlurFilter.DeactivateCameraBlur;
		_menuManager.OnCloseAnyMenu -= EnableDiegeticText;
		_mainMenuReadNews.OnCloseMainMenuReadNews -= EnableAllColliders;
		_mainMenuReadNews.OnCloseMainMenuReadNews -= _playerCameraBlurFilter.DeactivateCameraBlur;
		_pauseMenuController.OnCloseAnyPauseSubMenu -= EnableAllColliders;
	}

	private void Update()
	{
		if (name == "NewGame")
		{
			if (Input.GetKeyDown(_keyPauseMenu) && _menuManager.PauseMenuLevel.Count == 1)
			{
				_menuManager.CloseAnyMenu();
				_CanvasDiegeticText.SetActive(true);
				_pauseMenuController.ClosePauseSubMenu();
			}
			if (Input.GetKeyDown(_keyPauseMenu) && _menuManager.PauseMenuLevel.Count == 2)
			{
				_pauseMenuController.ClosePauseConfirmMenu();
			}

			if (Input.GetKeyDown(_keyPauseMenu) && _mainMenuReadNews.IsMainMenuReadNewsOpened)
			{
				_mainMenuReadNews.HideCanvasMainMenuReadNews();
				_menuManager.HideCanvasMenuBackground();
				_playerCameraBlurFilter.DeactivateCameraBlur();
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
		if (name == "NewGame")
		{
			Debug.Log("START NEW GAME");
			DisableAllColliders();
			_gameController.CloseMainMenu();
			StartCoroutine(StartNewGame());
		}
		else if (name == "LoadGame")
		{
			Debug.Log("OPEN LOAD GAME");
			_menuManager.ShowCanvasMenuBackground();
			_CanvasDiegeticText.SetActive(false);
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenLoadSubMenu();
		}
		else if (name == "ExitGame")
		{
			Debug.Log("EXIT GAME");
			Application.Quit();
		}
		else if (name == "Settings")
		{
			Debug.Log("OPEN SETTINGS");
			_menuManager.ShowCanvasMenuBackground();
			_CanvasDiegeticText.SetActive(false);
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenSettingsSubMenu();
		}
		else if (name == "ReadNews")
		{
			Debug.Log("OPEN NEWS");
			_menuManager.ShowCanvasMenuBackground();
			_CanvasDiegeticText.SetActive(false);
			_mainMenuReadNews.ShowCanvasMainMenuReadNews();
			DisableAllColliders();
			_playerCameraBlurFilter.ActivateCameraBlur();
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

	IEnumerator StartNewGame()
	{
		DontDestroyOnLoad(gameObject);

		yield return StartCoroutine(_saveLoadController.NewGame());
		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(GameScenesEnum.Scene_0_Test));
	
		Destroy(gameObject);
	}
}