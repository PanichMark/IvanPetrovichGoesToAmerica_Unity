using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDrawer : InteractionObjectOpenableAbstract
{
	[SerializeField] protected float _openingSpeed = 3f;

	protected Coroutine _currentAnimation;
	public override string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(base.InteractionObjectNameSystem)}";
	protected Vector3 _openedPosition;
	protected Vector3 _closedPosition;
	[SerializeField] protected float _openLengthForward;
	protected string _interactionHintMessageMain;
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	public void Start()
	{
		IsObjectOpened = false;


		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

		_closedPosition = transform.localPosition;
		_openedPosition = transform.localPosition + new Vector3(0, 0, _openLengthForward);

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		IsObjectOpened = false;

	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	public override void Interact()
	{
		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			_currentAnimation = StartCoroutine(OpenDrawer());
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			_currentAnimation = StartCoroutine(CloseDrawer());
		}
	}

	IEnumerator OpenDrawer()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsObjectOpened = true;

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
		IsObjectOpened = false;

		while (Mathf.Abs(transform.localPosition.z - _closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}
}