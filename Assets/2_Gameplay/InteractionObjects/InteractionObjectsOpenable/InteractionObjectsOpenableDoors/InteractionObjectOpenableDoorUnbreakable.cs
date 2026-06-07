using System.Collections;
using UnityEngine;

public class InteractionObjectOpenableDoorUnbreakable : InteractionObjectOpenableAbstract
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
	[SerializeField] protected bool _IsLockedForever;


	public override string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	private Coroutine _currentAnimation;

	private Quaternion _openedRotation;
	private Quaternion _closedRotation;

	private string _interactionHintMessageFail;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;

	void Start()
	{
		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		Vector3 openedEulerAngles = new Vector3(0, _doorOpenAngle, 0);
		_openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (!_IsLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_KeyLocked")}!";
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
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null)
				|| (_mechanicalLockController != null && _mechanicalLockController.WasUnlocked)
				|| (_electronicLockController != null && _electronicLockController.WasUnlocked)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())))
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
	}

	public override void Interact()
	{
		if (!_IsLockedForever)
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
			if ((_interactionObjectKeyData == null && _mechanicalLockController == null && _electronicLockController == null)
				|| (_mechanicalLockController?.WasUnlocked == true)
				|| (_electronicLockController?.WasUnlocked == true)
				|| (_interactionObjectKeyData != null && _keysManager.CollectedKeys.Contains(_interactionObjectKeyData.keyID.ToString())))
			{
				PerformDoorInteraction();
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

		if (!_IsLockedForever)
		{
			if (_interactionObjectKeyData != null)
			{
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_KeyLocked")}!";
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

		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

		if (!IsObjectOpened)
		{
			Debug.Log($"Opened {InteractionObjectNameUI}");
			IsObjectOpened = true;
			_currentAnimation = StartCoroutine(OpenDoor());
		}
		else
		{
			Debug.Log($"Closed {InteractionObjectNameUI}");
			IsObjectOpened = false;
			_currentAnimation = StartCoroutine(CloseDoor());
		}

		SetUnlockedDoorHintMessageMain();
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

	IEnumerator OpenDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _openedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	IEnumerator CloseDoor()
	{
		while (Quaternion.Angle(transform.localRotation, _closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _closedRotation, Time.deltaTime * _doorOpeningSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}
}