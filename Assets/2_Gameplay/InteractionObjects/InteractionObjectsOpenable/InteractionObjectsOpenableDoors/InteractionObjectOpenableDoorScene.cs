using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoorScene : InteractionObjectOpenableDoorUnbreakable
{
	private GameSceneManager _gameSceneManager;
	[SerializeField] private GameScenesEnum _targetScene;
	[SerializeField] private InteractionObjectOpenableDoorScenePlayerTransform _interactionObjectOpenableDoorScenePlayerTransform;
	private PlayerMovementController _playerMovementController;
	private string _interactionHintMessageScene;
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	private void Start()
	{
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");


		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_interactionHintMessageScene = _localizationManager.GetLocalizedString(_targetScene.ToString());

		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _electronicLockController.InteractionHintMessageMain;
			_electronicLockController.OnUnlockLock += UnlockDoor;
		}

		if (WasOpenableUnlocked ||
			(_mechanicalLockController == null && _electronicLockController == null)
			|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
			|| (_electronicLockController != null && _electronicLockController.WasUnlocked))
		{
			SetUnlockedDoorHintMessageMain();

			_interactionHintMessageMain = $"{InteractionHintMessageAction} {_interactionHintMessageScene}?";
		}

	}

	protected override void SetUnlockedDoorHintMessageMain()
	{
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GoToScene");
		
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	}

	protected override void UnlockDoor()
	{
		WasOpenableUnlocked = true;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GoToScene");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {_interactionHintMessageScene}?";
	}

	public override void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GoToScene");
		_interactionHintMessageScene = _localizationManager.GetLocalizedString(_targetScene.ToString());
		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _electronicLockController.InteractionHintMessageMain;
			_electronicLockController.OnUnlockLock += UnlockDoor;
		}

		if ((_mechanicalLockController == null && _electronicLockController == null)
			|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
			|| (_electronicLockController != null && _electronicLockController.WasUnlocked))
		{
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {_interactionHintMessageScene}?";
		}
	}

	protected override void PerformDoorInteraction()
	{
		StartCoroutine(LoadGameplayScene());
	}

	private IEnumerator LoadGameplayScene()
	{
		Debug.Log("LOADING: " + _targetScene);

		Transform parentTransform = transform.parent;

		DontDestroyOnLoad(parentTransform.gameObject);

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(_targetScene));

		_playerMovementController.SetPlayerPosition(_interactionObjectOpenableDoorScenePlayerTransform.PlayerPosition);
		_playerMovementController.SetPlayerRotation(_interactionObjectOpenableDoorScenePlayerTransform.PlayerRotation);

		Destroy(parentTransform.gameObject);
	}
}