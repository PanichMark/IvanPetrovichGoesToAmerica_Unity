using UnityEngine;

public class MainMenuInitialize : MonoBehaviour
{
    [SerializeField] private MainMenuChooseMissionController _mainMenuChooseMissionController;
    [SerializeField] private MainMenuReadNewsController _readNewsController;
    [SerializeField] private MainMenuCanvasController _canvasController;
    [SerializeField] private MainMenuDiegeticButtonController[] _diegeticButtonController;
	[SerializeField] private CutsceneController _cutsceneNewGame;
	[SerializeField] private Material _diegeticButtonMaterial;

	private PlayerMovementController _playerMovementController;
	private PlayerCameraController _playerCameraController;
	private PlayerHealthController _playerResourcesHealthManager;
	private PlayerManaController _playerResourcesManaManager;
	private PlayerMoneyController _playerResourcesMoneyManager;
	private PlayerWeaponController _weaponController;
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private LocalizationManager _localizationManager;

	private Bootstrap _bootstrap;
    void Start()
    {
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_bootstrap = ServiceLocator.Resolve<Bootstrap>();
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>();
		_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>();
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerHealthController>();
		_playerResourcesManaManager = ServiceLocator.Resolve<PlayerManaController>();
		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerMoneyController>();
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>();
		_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerWeaponAmmoController>();

		_mainMenuChooseMissionController.Initialize(_bootstrap,
			_playerMovementController,
			_playerCameraController,
			_playerResourcesHealthManager,
			_playerResourcesManaManager,
			_playerResourcesMoneyManager,
			_weaponController,
			_playerResourcesAmmoManager);

		_readNewsController.Initialize(
			_bootstrap,
			_localizationManager);

		for (int i = 0; i < _diegeticButtonController.Length; i++)
		{
			_diegeticButtonController[i].Initialize(
				_bootstrap,
				_mainMenuChooseMissionController,
				_readNewsController,
				_cutsceneNewGame,
				_diegeticButtonMaterial,
				_playerMovementController,
				_playerCameraController,
				_playerResourcesHealthManager,
				_playerResourcesManaManager,
				_playerResourcesMoneyManager,
				_weaponController,
				_playerResourcesAmmoManager);
		}

		_canvasController.Initialize(_mainMenuChooseMissionController, _readNewsController);
	}
}
