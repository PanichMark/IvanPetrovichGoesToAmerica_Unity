using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDrawer : InteractionObjectOpenableAbstract
{
	protected LocalizationManager localizationManager;

	[SerializeField] protected float OpeningSpeed = 3f;

	protected Coroutine currentAnimation;

	protected Vector3 openedPosition;
	protected Vector3 closedPosition;
	[SerializeField] protected float openLengthForward;

	public void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);

		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";

		closedPosition = transform.localPosition;
		openedPosition = transform.localPosition + new Vector3(0, 0, openLengthForward);

		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
		IsDoorOpened = false;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);

		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
		if (currentAnimation != null)
		{
			StopCoroutine(currentAnimation);
		}

		if (!IsDoorOpened)
		{
			InteractionHintAction = localizationManager.GetLocalizedString("CloseDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(OpenDrawer());
		}
		else
		{
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(CloseDrawer());
		}
	}

	IEnumerator OpenDrawer()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

		while (Mathf.Abs(transform.localPosition.z - openedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, openedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseDrawer()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsDoorOpened = false;

		while (Mathf.Abs(transform.localPosition.z - closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, closedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}