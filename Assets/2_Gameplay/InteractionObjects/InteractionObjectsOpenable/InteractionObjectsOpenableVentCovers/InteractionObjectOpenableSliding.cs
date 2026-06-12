using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableSliding : InteractionObjectOpenableDrawer
{
	[SerializeField] private float _openLengthUp;
	private Vector3 _intermediatePos;
	[SerializeField] protected InteractionObjectElectricalPanel _electronicElectricalPanel;
	private bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;
	public override string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;
	public override void SetUpOpenableSliding()
	{
		_closedPosition = transform.localPosition;
		_openedPosition = transform.localPosition + new Vector3(0, 0, _openLengthForward);
		_openedPosition += new Vector3(0, _openLengthUp, 0);
		_intermediatePos = transform.localPosition + new Vector3(0, 0, _openLengthForward);

		_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_LockedElectricalPanel")}!";
	}

	public override void Interact()
	{
		if (_currentAnimation != null)
		{
			StopCoroutine(_currentAnimation);
		}

		if ((_electronicElectricalPanel != null && _electronicElectricalPanel.IsOutOfService == true)
			|| (_electronicElectricalPanel == null))
		{
			if (!IsObjectOpened)
			{
				InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Close");
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
				_currentAnimation = StartCoroutine(OpenVentCover());
			}
			else
			{
				InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Open");
				_interactionHintMessageMain = $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
				_currentAnimation = StartCoroutine(CloseVentCover());
			}

			_isAdditionalInteractionHintActive = false;	
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	IEnumerator OpenVentCover()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsObjectOpened = true;

		while (Mathf.Abs(transform.localPosition.z - _intermediatePos.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePos, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		while (Mathf.Abs(transform.localPosition.y - _openedPosition.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}

	IEnumerator CloseVentCover()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsObjectOpened = false;

		while (Mathf.Abs(transform.localPosition.y - _intermediatePos.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _intermediatePos, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		while (Mathf.Abs(transform.localPosition.z - _closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closedPosition, Time.deltaTime * _openingSpeed);
			yield return null;
		}

		_currentAnimation = null;
	}
}