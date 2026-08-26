using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectOpenableDoorUndestructable : InteractionObjectOpenableAbstract
{
	protected bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	[SerializeField] private int _doorOpenAngle = 90;
	protected string _interactionHintMessageMain;
	private KeysManager _keysManager;
	[SerializeField] private float _doorOpeningSpeed = 200f;
	[SerializeField] protected InteractionObjectKeyData _interactionObjectKeyData;
	[SerializeField] protected InteractionObjectLockMechanical _mechanicalLockController;
	[SerializeField] protected InteractionObjectLockElectronic _electronicLockController;
	[SerializeField] protected InteractionObjectElectricalPanel _electronicElectricalPanel;
	[SerializeField] private InteractionObjectChangeScene _changeScene;
	[SerializeField] protected bool _isLockedForever;
	[SerializeField] protected InteractionObjectOpenableDoorUndestructable _doorSibling;
	public override string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public override string InteractionHintMessageMain => _interactionHintMessageMain;
	private GameScenesManager _gameSceneManager;
	private Coroutine _currentAnimation;
	protected bool _isDoorDouble;
	private Quaternion _openedRotation;
	private Quaternion _closedRotation;
	private PlayerMovementController _playerMovementController;
	private string _interactionHintMessageFail;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;

	private IEnumerator LoadGameplayScene()
	{
		gameObject.transform.SetParent(null);
		DontDestroyOnLoad(gameObject);

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(_changeScene.SceneToLoad));

		_playerMovementController.SetPlayerPosition(_changeScene.PlayerPosition);
		_playerMovementController.SetPlayerRotationY(_changeScene.PlayerRotationY);
		
		Destroy(gameObject);
	}

	void Start()
	{
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>("GameSceneManager");
		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		Vector3 openedEulerAngles = new Vector3(0, _doorOpenAngle, 0);
		_openedRotation = Quaternion.Euler(openedEulerAngles);

		if (_doorSibling == null)
		{
			_isDoorDouble = false;
		}
		else
		{
			_isDoorDouble = true;	
		}

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (_isObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");

			transform.localRotation = _openedRotation;
		}

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedKey")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
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
			if (_electronicElectricalPanel != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedElectricalPanel")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null && _electronicElectricalPanel == null)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| _electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true))
			{
				SetUnlockedDoorHintMessageMain();

				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedForever")}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}

		InitializeDoor();
	}

	public virtual void InitializeDoor()
	{

	}

	public override void Interact()
	{
		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_mechanicalLockController.Interact();
			}
			if (_electronicLockController != null && !_electronicLockController.WasUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_electronicLockController.Interact();
			}
			if (_electronicElectricalPanel != null)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null && _electronicElectricalPanel == null)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| _electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true))
			{
				PerformDoorInteraction();

				if (_isDoorDouble == true)
				{
					_doorSibling.PerformDoorInteraction();
				}
			}
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	public override void InteractCutscene()
	{
		Interact();
	}

	public virtual void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedKey")}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
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
				(_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null)
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())))
			{
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedForever")}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}
	}

	protected virtual void UnlockDoor()
	{
		WasOpenableUnlocked = true;

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	protected virtual void PerformDoorInteraction()
	{
		_isAdditionalInteractionHintActive = false;

		if (_changeScene == null)
		{
			if (_currentAnimation != null)
			{
				StopCoroutine(_currentAnimation);
			}

			if (!IsObjectOpened)
			{
				Debug.Log($"Opened {InteractionObjectNameUI}");
				_isObjectOpened = true;
				_currentAnimation = StartCoroutine(OpenDoor());
			}
			else
			{
				Debug.Log($"Closed {InteractionObjectNameUI}");
				_isObjectOpened = false;
				_currentAnimation = StartCoroutine(CloseDoor());
			}

			SetUnlockedDoorHintMessageMain();
		}
		else
		{
			StartCoroutine(LoadGameplayScene());
		}
	}

	protected virtual void SetUnlockedDoorHintMessageMain()
	{
		if (IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	private IEnumerator OpenDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _openedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	private IEnumerator CloseDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _closedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null)
		{
			data.OpenableUndestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<OpenableUndestructableObjectData>>();
		}
		if (!data.OpenableUndestructableObjectsData.ContainsKey(currentScene))
		{
			data.OpenableUndestructableObjectsData[currentScene] = new List<OpenableUndestructableObjectData>();
		}

		var targetList = data.OpenableUndestructableObjectsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.OpenableUndestructableObjectIndex == GameplayObjectIndex);

		if (indexInList != -1)
		{
			var existingItem = targetList[indexInList];

			existingItem.IsOpenableUndestructableObjectUnlocked = WasOpenableUnlocked;
			existingItem.IsOpenableUndestructableObjectOpened = _isObjectOpened;
			existingItem.OpenableUndestructableObjectNameSystem = InteractionObjectNameSystem;

			targetList[indexInList] = existingItem;
		}
		else
		{
			targetList.Add(new OpenableUndestructableObjectData
			{
				OpenableUndestructableObjectIndex = GameplayObjectIndex,
				OpenableUndestructableObjectNameSystem = InteractionObjectNameSystem,
				IsOpenableUndestructableObjectUnlocked = WasOpenableUnlocked,
				IsOpenableUndestructableObjectOpened = _isObjectOpened
			});
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null || !data.OpenableUndestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.OpenableUndestructableObjectIndex == GameplayObjectIndex);

		if (savedState.Equals(default(OpenableUndestructableObjectData))) yield break;

		WasOpenableUnlocked = savedState.IsOpenableUndestructableObjectUnlocked;
		_isObjectOpened = savedState.IsOpenableUndestructableObjectOpened;

		yield return null;
	}
}