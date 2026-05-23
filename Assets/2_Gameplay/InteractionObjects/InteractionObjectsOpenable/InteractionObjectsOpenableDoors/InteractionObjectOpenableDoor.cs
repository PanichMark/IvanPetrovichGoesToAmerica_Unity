using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract
{
	protected bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	[SerializeField] private int _doorOpenAngle = 90;
	private string _interactionHintMessageMain;

	[SerializeField] private float _doorOpeningSpeed = 200f;
	[SerializeField] private InteractionObjectLockMechanical _mechanicalLockController;
	[SerializeField] private InteractionObjectLockElectronic _electronicLockController;
	public override string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	private Coroutine _currentAnimation;

	private Quaternion _openedRotation;
	private Quaternion _closedRotation;
	

	public override string InteractionHintMessageFail => $"{InteractionObjectNameUI} {_localizationManager.GetLocalizedString(_interactionObjectNameSystem)}!";

	void Start()
	{
		IsDoorOpened = false;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		Vector3 openedEulerAngles = new Vector3(0, _doorOpenAngle, 0);
		_openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		

		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_mechanicalLockController == null || _mechanicalLockController.WasUnlocked)
		{
			SetUnlockedDoorHintMessageMain();

			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
		}

		
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");

		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_mechanicalLockController == null || _mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
		}
	}

	private void UnlockDoor()
	{
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
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

		if ((_mechanicalLockController == null && _electronicLockController == null) ||
			_mechanicalLockController?.WasUnlocked == true ||
			_electronicLockController?.WasUnlocked == true)
		{
			PerformDoorInteraction();
		}
	}

	protected virtual void PerformDoorInteraction()
	{
		_isAdditionalInteractionHintActive = false;

		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

	

		if (!IsDoorOpened)
		{
			Debug.Log($"Opened {InteractionObjectNameUI}");
			IsDoorOpened = true;
			_currentAnimation = StartCoroutine(OpenDoor());
		}
		else
		{
			Debug.Log($"Closed {InteractionObjectNameUI}");
			IsDoorOpened = false;
			_currentAnimation = StartCoroutine(CloseDoor());
		}

		SetUnlockedDoorHintMessageMain();
	}

	private void SetUnlockedDoorHintMessageMain()
	{
		if (IsDoorOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
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