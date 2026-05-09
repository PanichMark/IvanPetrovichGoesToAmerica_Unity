using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract
{
	protected bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;

	private LocalizationManager localizationManager;
	[SerializeField] private InteractionObjectLockMechanical mechanicalLockController;
	[SerializeField] private InteractionObjectLockElectronic electronicLockController;

	private float doorOpeningSpeed = 200f;
	private Coroutine currentAnimation;

	private Quaternion openedRotation;
	private Quaternion closedRotation;
	[SerializeField] private int doorOpenAngle;

	public override string InteractionHintMessageAdditional => $"{InteractionObjectNameUI} is locked!";

	void Start()
	{
		IsDoorOpened = false;
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");

		Vector3 openedEulerAngles = new Vector3(0, 0, doorOpenAngle);
		openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		closedRotation = Quaternion.Euler(closedEulerAngles);

		localizationManager.OnLanguageChangeEvent += ChangeLanguage;

		if (mechanicalLockController != null && !mechanicalLockController.WasUnlocked)
		{
			interactionHintMessageMain = mechanicalLockController.InteractionHintMessageMain;
			mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (mechanicalLockController == null || mechanicalLockController.WasUnlocked)
		{
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
		}
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");

		if (mechanicalLockController != null && !mechanicalLockController.WasUnlocked)
		{
			interactionHintMessageMain = mechanicalLockController.InteractionHintMessageMain;
			mechanicalLockController.OnUnlockLock += UnlockDoor;
		}

		if (mechanicalLockController == null || mechanicalLockController.WasUnlocked)
		{
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
		}
	}

	private void UnlockDoor()
	{
		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
		if (mechanicalLockController != null && !mechanicalLockController.WasUnlocked)
		{
			Debug.Log("Attempting to unlock the lock...");
			mechanicalLockController.Interact();
		}

		if (electronicLockController != null && !electronicLockController.WasUnlocked)
		{
			Debug.Log("Attempting to unlock the lock...");
			electronicLockController.Interact();
		}

		if ((mechanicalLockController == null && electronicLockController == null) ||
			mechanicalLockController?.WasUnlocked == true ||
			electronicLockController?.WasUnlocked == true)
		{
			PerformDoorInteraction();
		}
	}

	protected virtual void PerformDoorInteraction()
	{
		isAdditionalInteractionHintActive = false;

		if (currentAnimation != null)
		{
			StopCoroutine(currentAnimation);
		}

		if (!IsDoorOpened)
		{
			Debug.Log($"Opened {InteractionObjectNameUI}");
			IsDoorOpened = true;
			InteractionHintAction = localizationManager.GetLocalizedString("CloseDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(OpenDoor());
		}
		else
		{
			Debug.Log($"Closed {InteractionObjectNameUI}");
			IsDoorOpened = false;
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(CloseDoor());
		}
	}

	private void Update() { }

	IEnumerator OpenDoor()
	{
		while (Quaternion.Angle(transform.localRotation, openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, openedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseDoor()
	{
		while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, closedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}