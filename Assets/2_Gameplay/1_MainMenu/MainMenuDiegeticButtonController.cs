using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDiegeticButtonController : MonoBehaviour
{
	private PlayerMovementController _playerMovementController;
	private PlayerCameraController _playerCameraController;
	private PlayerHealthController _playerResourcesHealthManager;
	private PlayerManaController _playerResourcesManaManager;
	private PlayerMoneyController _playerResourcesMoneyManager;
	private PlayerWeaponController _weaponController;
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private Material _defaultMaterial;     
	private Material _hoverMaterial;
	private MenuBackgroundController _menuBackgroundController;
	private static List<MainMenuDiegeticButtonController> _instances = new List<MainMenuDiegeticButtonController>();
	private PlayerCameraVolumeController _playerCameraBlurFilter;
	private MainMenuReadNewsController _mainMenuReadNews;
	private PauseMenuController _pauseMenuController;
	private GameController _gameController;
	private Renderer _renderer;
	private GameScenesManager _gameSceneManager;
	private Collider _collider;
	private JsonSaveLoadController _saveLoadController;
	private MenuManager _menuManager;
	private Bootstrap _bootstrap;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private KeyCode _keyPauseMenu;
	private CutsceneController _cutsceneNewGame;
	[SerializeField] private MainMenuDiegeticButtonsEnum _mainMenuDiegeticButtonsEnum;
	private MainMenuChooseMissionController _mainMenuChooseMissionController;
	public bool IsCutsceneNewGamePlaying { get; private set; }
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private MainMenuCanvasController _mainMenuCanvasController;
	private PauseSubMenuSettingsGameDifficultyController _pauseSubMenuSettingsGameDifficultyController;

	public void Initialize(
		Bootstrap bootstrap,
		MainMenuChooseMissionController mainMenuChooseMissionController,
		MainMenuReadNewsController mainMenuReadNews,
		CutsceneController cutsceneNewGame,
		Material hoverMaterial,
		PlayerMovementController playerMovementController,
		PlayerCameraController playerCameraController,
		PlayerHealthController playerResourcesHealthManager,
		PlayerManaController playerResourcesManaManager,
		PlayerMoneyController playerResourcesMoneyManager,
		PlayerWeaponController weaponController,
		PlayerWeaponAmmoController playerResourcesAmmoManager)
	{
		_bootstrap = bootstrap;
		_playerMovementController = playerMovementController;
		_playerCameraController = playerCameraController;
		_playerResourcesHealthManager = playerResourcesHealthManager;
		_playerResourcesManaManager = playerResourcesManaManager;
		_playerResourcesMoneyManager = playerResourcesMoneyManager;
		_weaponController = weaponController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;

		_cutsceneNewGame = cutsceneNewGame;
		_instances.Add(this);
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>();
		_collider = GetComponent<Collider>();
		_renderer = GetComponent<Renderer>();
		_defaultMaterial = _renderer.material;
		_mainMenuCanvasController = GameObject.Find("MainMenuCanvasController").GetComponent<MainMenuCanvasController>();
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>();
		_hoverMaterial = hoverMaterial;
		_menuBackgroundController = ServiceLocator.Resolve<MenuBackgroundController>();
		_keyPauseMenu = ServiceLocator.Resolve<KeyCode>();
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>();
		_pauseMenuController = ServiceLocator.Resolve<PauseMenuController>();
		_gameController = ServiceLocator.Resolve<GameController>();
		_saveLoadController = ServiceLocator.Resolve<JsonSaveLoadController>();
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_playerCameraBlurFilter = ServiceLocator.Resolve<PlayerCameraVolumeController>();
		_pauseSubMenuSettingsController = ServiceLocator.Resolve<PauseSubMenuSettingsController>();
		_pauseSubMenuSettingsGameDifficultyController = ServiceLocator.Resolve<PauseSubMenuSettingsGameDifficultyController>();

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
			_gameController.DeactivateMainMenuOrEndGameTitlesActive();
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
			_cutsceneNewGame.TriggerCutscene(null);
			IsCutsceneNewGamePlaying = true;
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.TestScene)
		{
			Debug.Log("TEST SCENE");
			DisableAllColliders();
			_gameController.DeactivateMainMenuOrEndGameTitlesActive();
			StartCoroutine(LoadTestScene());
		}
		if (_mainMenuDiegeticButtonsEnum == MainMenuDiegeticButtonsEnum.EndGameTitles)
		{
			Debug.Log("END GAME TITLES SCENE");
			_gameController.DeactivateMainMenuOrEndGameTitlesActive();
			StartCoroutine(LoadEndGameTitlesScene());
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

	private IEnumerator LoadTestScene()
	{
		gameObject.transform.SetParent(null);

		DontDestroyOnLoad(gameObject);

		//yield return StartCoroutine(_saveLoadController.NewGame());
	

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(GameScenesGameplayEnum.Scene_System_Test));

		_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		ApplyTestSceneResourcesConfig();

		Destroy(gameObject);
	}

	private IEnumerator LoadEndGameTitlesScene()
	{
		gameObject.transform.SetParent(null);

		DontDestroyOnLoad(gameObject);

		//yield return StartCoroutine(_saveLoadController.NewGame());
		
		yield return StartCoroutine(_gameSceneManager.LoadEndGameTitlesScene());
		_playerMovementController.SetPlayerPosition(new Vector3(0, 0, -20));
		_playerCameraStateMachineController.SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
		Destroy(gameObject);
	}

	private void ApplyTestSceneResourcesConfig()
	{
		var resources = _bootstrap.GameData.GameMissionsList.MissionTest.MissionResources;

		Debug.Log($"PlayerPosition: {resources.PlayerTransform.PlayerPosition}");
		_playerMovementController.SetPlayerPosition(resources.PlayerTransform.PlayerPosition);

		Debug.Log($"PlayerRotationY: {resources.PlayerTransform.PlayerRotationY}");
		_playerMovementController.SetPlayerRotationY(resources.PlayerTransform.PlayerRotationY);
		_playerCameraController.SetCameraRotationY(resources.PlayerTransform.PlayerRotationY);

		Debug.Log($"PlayerHealth: {resources.PlayerHealth}");
		_playerResourcesHealthManager.ConfigApplyPlayerHealth(resources.PlayerHealth);

		Debug.Log($"PlayerHealingItems: {resources.PlayerHealingItems}");
		_playerResourcesHealthManager.ConfigApplyPlayerHealingItems(resources.PlayerHealingItems);

		Debug.Log($"PlayerMana: {resources.PlayerMana}");
		_playerResourcesManaManager.ConfigApplyPlayerMana(resources.PlayerMana);

		Debug.Log($"PlayerManaReplenishItems: {resources.PlayerManaReplenishItems}");
		_playerResourcesManaManager.ConfigApplyPlayerManaReplenishItems(resources.PlayerManaReplenishItems);

		Debug.Log($"PlayerMoney: {resources.PlayerMoney}");
		_playerResourcesMoneyManager.ConfigApplyPlayerMoney(resources.PlayerMoney);

		var weapons = resources.WeaponsToUnlock;
		for (int j = 0; j < weapons.Length; j++)
		{
			Debug.Log($"Weapon_{j}: {weapons[j].WeaponPrefab.name}");
			_weaponController.UnlockWeapon(weapons[j].WeaponPrefab);
		}

		var ammo = resources.Ammo;
		for (int j = 0; j < ammo.Length; j++)
		{
			Debug.Log($"Ammo_{j}: {ammo[j].AmmoType} x{ammo[j].StartAmount}");
			_playerResourcesAmmoManager.ConfigApplyPlayerAmmo(ammo[j].AmmoType, ammo[j].StartAmount);
		}
	}
}