using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDrawer : InteractionObjectOpenableAbstract
{
	protected LocalizationManager localizationManager;

	[SerializeField] protected float _openingSpeed = 3f;

	protected Coroutine _currentAnimation;

	protected Vector3 _openedPosition;
	protected Vector3 _closedPosition;
	[SerializeField] protected float _openLengthForward;

	public void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(_interactionObjectNameSystem);

		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";

		_closedPosition = transform.localPosition;
		_openedPosition = transform.localPosition + new Vector3(0, 0, _openLengthForward);

		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
		IsDoorOpened = false;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(_interactionObjectNameSystem);

		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

		if (!IsDoorOpened)
		{
			InteractionHintAction = localizationManager.GetLocalizedString("CloseDoor");
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			_currentAnimation = StartCoroutine(OpenDrawer());
		}
		else
		{
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			_currentAnimation = StartCoroutine(CloseDrawer());
		}
	}

	IEnumerator OpenDrawer()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

		while (Mathf.Abs(transform.localPosition.z - _openedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	IEnumerator CloseDrawer()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsDoorOpened = false;

		while (Mathf.Abs(transform.localPosition.z - _closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}
}