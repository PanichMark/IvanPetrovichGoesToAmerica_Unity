using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDiegeticButtonController : MonoBehaviour
{
	public Material defaultMaterial;     
	public Material hoverMaterial;     

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
	private KeyCode _keyCloseMenu;

	[SerializeField] private GameObject _DiegeticText;

	void Awake()
	{
		_instances.Add(this);
		_keyCloseMenu = ServiceLocator.Resolve<KeyCode>("KeyPauseMenu");
	}

	void OnDestroy()
	{
		_instances.Remove(this);
		_menuManager.OnCloseAnyMenu -= () => _DiegeticText.SetActive(true);
		_mainMenuReadNews.OnCloseMainMenuReadNews -= () => _DiegeticText.SetActive(true);
		_mainMenuReadNews.OnCloseMainMenuReadNews -= EnableAllColliders;
		_mainMenuReadNews.OnCloseMainMenuReadNews -= _playerCameraBlurFilter.DeactivateCameraBlur;
		_pauseMenuController.OnClosePauseSubMenu -= EnableAllColliders;
	}

	void Start()
	{
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_renderer = GetComponent<Renderer>();
		_pauseMenuController = ServiceLocator.Resolve<PauseMenuController>("PauseMenuController");
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_collider = GetComponent<Collider>();
		_saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_mainMenuReadNews = ServiceLocator.Resolve<MainMenuReadNewsController>("MainMenuReadNews");
		_playerCameraBlurFilter = ServiceLocator.Resolve<PlayerCameraBlurFilter>("PlayerCameraBlurFilter");


		_mainMenuReadNews.OnCloseMainMenuReadNews += EnableAllColliders;
		_mainMenuReadNews.OnCloseMainMenuReadNews += () =>
		{
			if (_DiegeticText != null)
			{
				_DiegeticText.SetActive(true);
			}
		};
		_mainMenuReadNews.OnCloseMainMenuReadNews += _playerCameraBlurFilter.DeactivateCameraBlur;

		_pauseMenuController.OnClosePauseSubMenu += EnableAllColliders;

		_menuManager.OnCloseAnyMenu += () =>
		{
			if (_DiegeticText != null)
			{
				_DiegeticText.SetActive(true);
			}
		};
	}

	private void Update()
	{
		if (Input.GetKeyDown(_keyCloseMenu) && _menuManager.PauseMenuLevel.Count == 1)
		{
			_menuManager.CloseAnyMenu();
			_DiegeticText.SetActive(true);
			_pauseMenuController.ClosePauseSubMenu();
		}

		if (Input.GetKeyDown(_keyCloseMenu) && _mainMenuReadNews.IsMainMenuReadNewsOpened)
		{
			_mainMenuReadNews.HideCanvasMainMenuReadNews();
			_playerCameraBlurFilter.DeactivateCameraBlur();
		}
	}

	void OnMouseEnter()
	{
		_renderer.material = hoverMaterial;
	}

	void OnMouseExit()
	{
		_renderer.material = defaultMaterial;
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
			_DiegeticText.SetActive(false);
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenLoadSubMenu();
		}
		else if (this.name == "ExitGame")
		{
			Debug.Log("EXIT GAME");
			Application.Quit();
		}
		else if (this.name == "Options")
		{
			Debug.Log("OPEN OPTIONS");
			_DiegeticText.SetActive(false);
			DisableAllColliders();
			_menuManager.OpenAnyMenu();
			_pauseMenuController.OpenSettingsSubMenu();
		}
		else if (this.name == "ReadNews")
		{
			Debug.Log("OPEN NEWS");
			_DiegeticText.SetActive(false);
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