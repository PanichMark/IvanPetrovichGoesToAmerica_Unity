using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuChooseMissionController : MonoBehaviour
{
	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	public delegate void MainMenuChooseMissionHandler();
	public event MainMenuChooseMissionHandler OnCloseMainMenuChooseMission;
	private ViewModelMainMenuChooseMission _viewModelMainMenuChooseMission;
	private MenuManager _menuManager;

	private PlayerMovementController _playerMovementController;
	private PlayerCameraController _playerCameraController;
	private PlayerHealthController _playerResourcesHealthManager;
	private PlayerManaController _playerResourcesManaManager;
	private PlayerMoneyController _playerResourcesMoneyManager;
	private PlayerWeaponController _weaponController;
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;


	private LocalizationManager _localizationManager;
	private GameScenesList _gameScenesList;
	private GameMissionsList _gameMissionsList;

	private GameObject _textMainMenuChooseMission;
	private TextMeshProUGUI _textComponentMainMenuChooseMission;

	private GameObject[] _buttonsMissions;
	private Button[] _buttonsComponentsMissions;
	private GameObject[] _imagesMissions;
	private Image[] _imagesComponentsMissions;
	private GameObject[] _textsMissionsNames;
	private TextMeshProUGUI[] _textsComponentsMissionsNames;
	private GameObject[] _textsScenesNames;
	private TextMeshProUGUI[] _textsComponentsScenesNames;

	private GameObject _buttonCloseMainMenuChooseMission;
	private Button _buttonComponentCloseMainMenuChooseMission;
	private GameObject _textButtonCloseMainMenuChooseMission;
	private TextMeshProUGUI _textComponentButtonCloseMainMenuChooseMission;
	private GameObject _canvasChooseMissionMenu;
	
	public bool IsMainMenuChooseMissionOpened { get; private set; }
	public void Initialize(Bootstrap bootstrap,
		PlayerMovementController playerMovementController,
		PlayerCameraController playerCameraController,
		PlayerHealthController playerResourcesHealthManager,
		PlayerManaController playerResourcesManaManager,
		PlayerMoneyController playerResourcesMoneyManager,
		PlayerWeaponController weaponController,
		PlayerWeaponAmmoController playerResourcesAmmoManager)
	{
		_playerMovementController = playerMovementController;
		_playerCameraController = playerCameraController;
		_playerResourcesHealthManager = playerResourcesHealthManager;
		_playerResourcesManaManager = playerResourcesManaManager;
		_playerResourcesMoneyManager = playerResourcesMoneyManager;
		_weaponController = weaponController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;

		_canvasChooseMissionMenu = bootstrap._canvasMainMenuChooseMission;
		_pauseMenuConfirmActionController = ServiceLocator.Resolve<PauseMenuConfirmActionController>();
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_viewModelMainMenuChooseMission = ServiceLocator.Resolve<ViewModelMainMenuChooseMission>();
		_gameScenesList = ServiceLocator.Resolve<GameScenesList>();
		_gameMissionsList = ServiceLocator.Resolve<GameMissionsList>();

		_textComponentMainMenuChooseMission = _viewModelMainMenuChooseMission.TextMainMenuChooseMission.GetComponent<TextMeshProUGUI>();
		_textComponentMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextChooseDemoEpisode");

		_buttonsComponentsMissions = new Button[_viewModelMainMenuChooseMission.Missions.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.Missions.Length; i++)
		{
			_buttonsComponentsMissions[i] = _viewModelMainMenuChooseMission.Missions[i].GetComponent<Button>();
		}
		_buttonsComponentsMissions[0].onClick.AddListener(() => _pauseMenuConfirmActionController.HandleShowForChooseEpisode(GameScenesGameplayEnum.Scene_0_Church, _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextPrologue")));
		_buttonsComponentsMissions[1].onClick.AddListener(() => _pauseMenuConfirmActionController.HandleShowForChooseEpisode(GameScenesGameplayEnum.Scene_0_RevenueHouse, _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameScene.ToString())));
		_buttonsComponentsMissions[2].onClick.AddListener(() => _pauseMenuConfirmActionController.HandleShowForChooseEpisode(GameScenesGameplayEnum.Scene_0_InnerYard, _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameScene.ToString())));

		_imagesComponentsMissions = new Image[_viewModelMainMenuChooseMission.Missions.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.Missions.Length; i++)
		{
			_imagesComponentsMissions[i] = _viewModelMainMenuChooseMission.Missions[i].GetComponent<Image>();
		}
		_imagesComponentsMissions[0].sprite = _gameScenesList.GameScenes[3].SceneLoadingScreenImage;
		_imagesComponentsMissions[1].sprite = _gameScenesList.GameScenes[5].SceneLoadingScreenImage;
		_imagesComponentsMissions[2].sprite = _gameScenesList.GameScenes[6].SceneLoadingScreenImage;

		_textsComponentsMissionsNames = new TextMeshProUGUI[_viewModelMainMenuChooseMission.TextsMissionsNames.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.TextsMissionsNames.Length; i++)
		{
			_textsComponentsMissionsNames[i] = _viewModelMainMenuChooseMission.TextsMissionsNames[i].GetComponent<TextMeshProUGUI>();
		}
		_textsComponentsMissionsNames[0].text =  _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[3].SceneGameMission.MissionName.ToString());
		_textsComponentsMissionsNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].SceneGameMission.MissionName.ToString());
		_textsComponentsMissionsNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].SceneGameMission.MissionName.ToString());

		_textsComponentsScenesNames = new TextMeshProUGUI[_viewModelMainMenuChooseMission.TextsScenesNames.Length];
		for (int i = 0; i < _viewModelMainMenuChooseMission.TextsScenesNames.Length; i++)
		{
			_textsComponentsScenesNames[i] = _viewModelMainMenuChooseMission.TextsScenesNames[i].GetComponent<TextMeshProUGUI>();
		}
		_textsComponentsScenesNames[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextPrologue");
		_textsComponentsScenesNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameScene.ToString());
		_textsComponentsScenesNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameScene.ToString());

		_buttonComponentCloseMainMenuChooseMission = _viewModelMainMenuChooseMission.ButtonCloseMainMenuChooseMission.GetComponent<Button>();
		_buttonComponentCloseMainMenuChooseMission.onClick.AddListener(() => HideCanvasMainMenuChooseMission());

		_textComponentButtonCloseMainMenuChooseMission = _viewModelMainMenuChooseMission.TextButtonCloseMainMenuChooseMission.GetComponent<TextMeshProUGUI>();
		_textComponentButtonCloseMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextButtonClose");

		_pauseMenuConfirmActionController.OnChooseMission += ApplyMissionResourcesData;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		Debug.Log("MainMenuChooseMissionController Initialized");
	}

	private void OnDestroy()
	{
		HideCanvasMainMenuChooseMission();
	}

	public void ShowCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = true;
		_canvasChooseMissionMenu.SetActive(true);

		Debug.Log("Show ChooseMission");
	}

	public void HideCanvasMainMenuChooseMission()
	{
		IsMainMenuChooseMissionOpened = false;
		OnCloseMainMenuChooseMission?.Invoke();
		_canvasChooseMissionMenu.SetActive(false);

		if (_menuManager.PauseMenuLevel.Count > 0)
		{
			_menuManager.PopPauseMenuLevel();
		}

		Debug.Log("Hide ChooseMission");
	}

	private void ApplyMissionResourcesData(GameScenesGameplayEnum missionScene)
	{
		for (int i = 3; i < _gameScenesList.GameScenes.Count; i++)
		{
			var sceneData = _gameScenesList.GameScenes[i];

			if (sceneData.GameScene == (GameScenesSystemEnum)((int)missionScene + 2))
			{
				var resources = sceneData.SceneGameMission.MissionResources;

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

				break;
			}
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextChooseDemoEpisode");

		_textsComponentsMissionsNames[0].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[3].SceneGameMission.MissionName.ToString());
		_textsComponentsMissionsNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].SceneGameMission.MissionName.ToString());
		_textsComponentsMissionsNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].SceneGameMission.MissionName.ToString());

		_textsComponentsScenesNames[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextPrologue");
		_textsComponentsScenesNames[1].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[5].GameScene.ToString());
		_textsComponentsScenesNames[2].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[6].GameScene.ToString());

		_textComponentButtonCloseMainMenuChooseMission.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ChooseMission_TextButtonClose");
	}
}
