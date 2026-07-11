using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableSliding : InteractionObjectOpenableAbstract
{
	[SerializeField] protected float _openingSpeed;
	protected Coroutine _currentAnimation;

	[SerializeField] private Vector3 _intermediatePositionOffset;
	[SerializeField] private Vector3 _openedPositionOffset;
	
	private Vector3 _closedPosition;
	private Vector3 _intermediatePosition;
	private Vector3 _openedPosition;

	[SerializeField] protected InteractionObjectElectricalPanel _electronicElectricalPanel;
	private bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;

	protected string _interactionHintMessageMain;
	public override string InteractionHintMessageMain => _interactionHintMessageMain;

	public void Start()
	{
		IsObjectOpened = false;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
		_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedElectricalPanel")}!";

		UpdatePositions();

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void UpdatePositions()
	{
		_closedPosition = transform.localPosition;
		_intermediatePosition = _closedPosition + _intermediatePositionOffset;
		_openedPosition = _intermediatePosition + _openedPositionOffset;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

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

		if ((_electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true) || (_electronicElectricalPanel == null))
		{
			if (!IsObjectOpened)
			{
				InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
				_currentAnimation = StartCoroutine(OpenSequence());
			}
			else
			{
				InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
				_currentAnimation = StartCoroutine(CloseSequence());
			}

			_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
			_isAdditionalInteractionHintActive = false;
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	public override void InteractCutscene()
	{
		if (!IsObjectOpened)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
		}

		_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	}

	private IEnumerator OpenSequence()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsObjectOpened = true;

		if (_intermediatePositionOffset != Vector3.zero)
		{
			while (Vector3.Distance(transform.localPosition, _intermediatePosition) > 0.001f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePosition, Time.deltaTime * _openingSpeed);
				yield return null;
			}
		}

		while (Vector3.Distance(transform.localPosition, _openedPosition) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}
		
		_currentAnimation = null;
	}

	private IEnumerator CloseSequence()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsObjectOpened = false;

		if (_intermediatePositionOffset != Vector3.zero)
		{
			while (Vector3.Distance(transform.localPosition, _intermediatePosition) > 0.001f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePosition, Time.deltaTime * _openingSpeed);
				yield return null;
			}
		}
		
		while (Vector3.Distance(transform.localPosition, _closedPosition) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}
}