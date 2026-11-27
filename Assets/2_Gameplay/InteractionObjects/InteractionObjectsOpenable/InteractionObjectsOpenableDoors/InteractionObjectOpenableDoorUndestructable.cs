using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class InteractionObjectOpenableDoorUndestructable : InteractionObjectOpenableAbstract
{
	protected bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	[SerializeField] protected int _doorOpenAngle = 90;
	protected string _interactionHintMessageMain;
	private KeyItemsManager _keysManager;
	[SerializeField] protected float _doorOpeningSpeed = 200f;
	[SerializeField] protected InteractionObjectKeyItemData _interactionObjectKeyData;
	[SerializeField] protected InteractionObjectLockMechanical _lockMechanicalController;
	[SerializeField] protected InteractionObjectLockElectronic _lockElectronicController;
	[SerializeField] protected InteractionObjectElectricalPanel _electricalPanel;
	[SerializeField] private InteractionObjectChangeSceneData _changeScene;
	[SerializeField] protected bool _isLockedForever;
	[SerializeField] protected InteractionObjectOpenableDoorUndestructable _doorSibling;
	[SerializeField] protected bool _canNPCOpenDoor = true;
	[SerializeField] protected bool _shouldNPCcloseDoorBehind = true;
	protected DoorNPCDetector _npcDetector;
	protected NavMeshObstacle _navMeshObstacle;
	public override string InteractionHintMessageMain => _interactionHintMessageMain;
	private GameScenesManager _gameSceneManager;
	protected bool _isDoorDouble;
	protected Quaternion _openedRotation;
	protected Quaternion _closedRotation;
	private PlayerMovementController _playerMovementController;
	private string _interactionHintMessageFail;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;
	public bool DidNPCopenedThisDoor {  get; private set; }
	public bool CanNPCopenThis {  get; private set; }

	protected override void InitializeGameplayObjectJsonSaveLoad()
	{
		if (_changeScene == null && _isLockedForever == false)
		{
			_npcDetector = transform.parent.GetComponentInChildren<DoorNPCDetector>();
			//Debug.Log(gameObject.name);
			_npcDetector.Initialize(this);
			CanNPCopenThis = true;
		}


		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>();
		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>();
		_keysManager = ServiceLocator.Resolve<KeyItemsManager>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		Vector3 openedEulerAngles = new Vector3(0, _doorOpenAngle, 0);
		_openedRotation = Quaternion.Euler(openedEulerAngles);
		_navMeshObstacle = GetComponent<NavMeshObstacle>();
		if (_doorSibling == null)
		{
			_isDoorDouble = false;
		}
		else
		{
			_isDoorDouble = true;	
		}

		Vector3 closedEulerAngles = transform.localRotation.eulerAngles;
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (_isObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Close", gameObject.name);

			transform.localRotation = _openedRotation;
		}

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null && !IsOpenableUnlocked)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_LockedByKey", gameObject.name)}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if (_lockMechanicalController != null && !IsOpenableUnlocked)
			{
				_interactionHintMessageMain = _lockMechanicalController.InteractionHintMessageMain;
				_lockMechanicalController.OnUnlockLock += UnlockDoor;
			}
			if (_lockElectronicController != null && !IsOpenableUnlocked)
			{
				_interactionHintMessageMain = _lockElectronicController.InteractionHintMessageMain;
				_lockElectronicController.OnUnlockLock += UnlockDoor;
			}
			if (_electricalPanel != null && !IsOpenableUnlocked)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_LockedByElectricalPanel", gameObject.name)}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
				_electricalPanel.OnWentOutOfService += UnlockDoor;
			}
			if ((_interactionObjectKeyData == null && _lockMechanicalController == null && _lockElectronicController == null && _electricalPanel == null)
				|| (_interactionObjectKeyData != null && (_keysManager.CollectedKeyItems.Contains(_interactionObjectKeyData.KeyItemID.ToString()) || IsOpenableUnlocked))
				|| (_lockMechanicalController != null && IsOpenableUnlocked)
				|| (_lockElectronicController != null && IsOpenableUnlocked)
				|| _electricalPanel != null && IsOpenableUnlocked)
			{
				SetUnlockedDoorHintMessageMain();
				IsOpenableUnlocked = true;
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_LockedForever", gameObject.name)}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}

		InitializeDoor();
	}

	private IEnumerator LoadGameplayScene()
	{
		gameObject.transform.SetParent(null);
		DontDestroyOnLoad(gameObject);

		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene(_changeScene.SceneToLoad));

		_playerMovementController.SetPlayerPosition(_changeScene.PlayerPosition);
		_playerMovementController.SetPlayerRotationY(_changeScene.PlayerRotationY);

		Destroy(gameObject);
	}

	public virtual void InitializeDoor()
	{

	}

	public override void Interact()
	{
		base.Interact();
		//OnInteract?.Invoke();

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null && !IsOpenableUnlocked)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if (_lockMechanicalController != null && !IsOpenableUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_lockMechanicalController.Interact();
			}
			if (_lockElectronicController != null && !IsOpenableUnlocked)
			{
				Debug.Log("Attempting to unlock the lock...");
				_lockElectronicController.Interact();
			}
			if (_electricalPanel != null && !IsOpenableUnlocked)
			{
				_isAdditionalInteractionHintActive = true;
			}
			if ((_interactionObjectKeyData == null && _lockMechanicalController == null && _lockElectronicController == null && _electricalPanel == null)
				|| (_interactionObjectKeyData != null && (_keysManager.CollectedKeyItems.Contains(_interactionObjectKeyData.KeyItemID.ToString()) || IsOpenableUnlocked))
				|| (_lockMechanicalController != null && IsOpenableUnlocked)
				|| (_lockElectronicController != null && IsOpenableUnlocked)
				|| _electricalPanel != null && IsOpenableUnlocked)
			{
				PerformOpenableInteraction();

				if (_isDoorDouble == true)
				{
					_doorSibling.PerformOpenableInteraction();
				}
			}
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	public override void InteractAfterDialogueOrCutscene()
	{
		Interact();
	}

	private void OnDestroy()
	{
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}

	public virtual void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Open", gameObject.name);

		if (!_isLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_LockedByKey", gameObject.name)}!";
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
			if (_lockMechanicalController != null && !_lockMechanicalController.WasUnlocked)
			{
				_interactionHintMessageMain = _lockMechanicalController.InteractionHintMessageMain;
				_lockMechanicalController.OnUnlockLock += UnlockDoor;
			}
			if (_lockElectronicController != null && !_lockElectronicController.WasUnlocked)
			{
				_interactionHintMessageMain = _lockElectronicController.InteractionHintMessageMain;
				_lockElectronicController.OnUnlockLock += UnlockDoor;
			}
			if (IsOpenableUnlocked ||
				(_interactionObjectKeyData == null && _lockMechanicalController == null && _lockElectronicController == null)
				|| (_lockMechanicalController != null && _lockMechanicalController.WasUnlocked)
				|| (_lockElectronicController != null && _lockElectronicController.WasUnlocked)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeyItems.Contains(_interactionObjectKeyData.KeyItemID.ToString())))
			{
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			}
		}
		else
		{
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Fail_LockedForever", gameObject.name)}!";
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		}
	}

	protected virtual void UnlockDoor()
	{
		if (IsOpenableUnlocked == false)
		{

			IsOpenableUnlocked = true;

			InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Open", gameObject.name);
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

			if (_isDoorDouble == true)
			{
				_doorSibling.UnlockDoor();
			}
		}
	}
	//public event IInteractable.InteractableObjectHandler OnInteract;

	public void NPCtryOpeningDoor()
	{
		if (IsOpenableUnlocked && !IsObjectOpened && _canNPCOpenDoor)
		{
			DidNPCopenedThisDoor = true;
			Debug.Log($"NPC Opened {InteractionObjectNameUI}");
			_isObjectOpened = true;
			_currentAnimation = StartCoroutine(OpenDoor());
		}
	}

	public void NPCtryClosingDoor()
	{
		if (IsObjectOpened && _canNPCOpenDoor && _shouldNPCcloseDoorBehind)
		{
			if (DidNPCopenedThisDoor)
			{
				Debug.Log($"NPC Closed {InteractionObjectNameUI}");
				_isObjectOpened = false;
				_currentAnimation = StartCoroutine(CloseDoor());
			}
			
			DidNPCopenedThisDoor = false;
		}
	}

	protected override void PerformOpenableInteraction()
	{
		base.PerformOpenableInteraction();

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
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Close", gameObject.name);
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_Open", gameObject.name);
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	public virtual void SetDoorToOpenedPosition()
	{
		//Quaternion startRotation = transform.localRotation;
		//transform.localRotation = startRotation * _openedRotation;
		if (_changeScene == null && _isLockedForever == false)
		{
			_navMeshObstacle.enabled = false;
		}
	}

	public virtual void SetDoorToClosedPosition()
	{
		//Quaternion startRotation = transform.localRotation;
		//transform.localRotation = startRotation * _closedRotation;
		if (_changeScene == null && _isLockedForever == false)
		{
			_navMeshObstacle.enabled = true;
		}
	}

	protected virtual IEnumerator OpenDoor()
	{
		Quaternion startRotation = _closedRotation;
		Quaternion targetRotation = _closedRotation * _openedRotation;

		while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		transform.localRotation = targetRotation;
		SetDoorToOpenedPosition();
		_currentAnimation = null;
	}

	protected virtual IEnumerator CloseDoor()
	{
		Quaternion startRotation = _closedRotation;
		Quaternion targetRotation = _closedRotation;

		while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		transform.localRotation = targetRotation;
		SetDoorToClosedPosition();
		_currentAnimation = null;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null)
		{
			data.OpenableUndestructableObjectsData = new Dictionary<GameScenesGameplayEnum, List<OpenableUndestructableObjectData>>();
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

			existingItem.IsOpenableUndestructableObjectUnlocked = IsOpenableUnlocked;
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
				IsOpenableUndestructableObjectUnlocked = IsOpenableUnlocked,
				IsOpenableUndestructableObjectOpened = _isObjectOpened
			});
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.OpenableUndestructableObjectsData == null || !data.OpenableUndestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.OpenableUndestructableObjectIndex == GameplayObjectIndex);

		if (savedState.Equals(default(OpenableUndestructableObjectData))) yield break;

		IsOpenableUnlocked = savedState.IsOpenableUndestructableObjectUnlocked;
		_isObjectOpened = savedState.IsOpenableUndestructableObjectOpened;

		if (IsOpenableUnlocked == true)
		{
			UnlockDoor();

			if (_isDoorDouble)
			{
				_doorSibling.UnlockDoor();
			}
		}

		if (IsObjectOpened == true)
		{
			SetDoorToOpenedPosition();

			if (_isDoorDouble)
			{
				_doorSibling.SetDoorToOpenedPosition();
			}
		}
		else
		{
			SetDoorToClosedPosition();

			if (_isDoorDouble)
			{
				_doorSibling.SetDoorToClosedPosition();
			}
		}

			yield return null;
	}
}