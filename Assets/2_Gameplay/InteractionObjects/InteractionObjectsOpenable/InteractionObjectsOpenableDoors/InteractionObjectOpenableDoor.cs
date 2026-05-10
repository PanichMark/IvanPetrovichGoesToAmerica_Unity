using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract
{
	protected bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => _isAdditionalInteractionHintActive;

	private LocalizationManager _localizationManager;
	[SerializeField] private InteractionObjectLockMechanical _mechanicalLockController;
	[SerializeField] private InteractionObjectLockElectronic _electronicLockController;

	private float _doorOpeningSpeed = 200f;
	private Coroutine _currentAnimation;

	private Quaternion _openedRotation;
	private Quaternion _closedRotation;
	[SerializeField] private int _doorOpenAngle;

	public override string InteractionHintMessageAdditional => $"{InteractionObjectNameUI} is locked!";

	void Start()
	{
		IsDoorOpened = false;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintAction = _localizationManager.GetLocalizedString("OpenDoor");

		Vector3 openedEulerAngles = new Vector3(0, 0, _doorOpenAngle);
		_openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		_closedRotation = Quaternion.Euler(closedEulerAngles);

		_localizationManager.OnLanguageChangeEvent += ChangeLanguage;

		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_mechanicalLockController == null || _mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
		}
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintAction = _localizationManager.GetLocalizedString("OpenDoor");

		if (_mechanicalLockController != null && !_mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = _mechanicalLockController.InteractionHintMessageMain;
			_mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (_mechanicalLockController == null || _mechanicalLockController.WasUnlocked)
		{
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
		}
	}

	private void UnlockDoor()
	{
		InteractionHintAction = _localizationManager.GetLocalizedString("OpenDoor");
		_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
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
			InteractionHintAction = _localizationManager.GetLocalizedString("CloseDoor");
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			_currentAnimation = StartCoroutine(OpenDoor());
		}
		else
		{
			Debug.Log($"Closed {InteractionObjectNameUI}");
			IsDoorOpened = false;
			InteractionHintAction = _localizationManager.GetLocalizedString("OpenDoor");
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			_currentAnimation = StartCoroutine(CloseDoor());
		}
	}

	private void Update() { }

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