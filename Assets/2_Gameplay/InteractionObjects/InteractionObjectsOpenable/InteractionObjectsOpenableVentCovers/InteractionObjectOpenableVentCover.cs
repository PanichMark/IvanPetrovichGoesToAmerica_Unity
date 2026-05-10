using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableVentCover : InteractionObjectOpenableDrawer
{
	[SerializeField] private float _openLengthUp;
	private Vector3 _intermediatePos;

	public void Start()
	{
		base.Start();

		_closedPosition = transform.localPosition;
		_openedPosition = transform.localPosition + new Vector3(0, 0, _openLengthForward);
		_openedPosition += new Vector3(0, _openLengthUp, 0);
		_intermediatePos = transform.localPosition + new Vector3(0, 0, _openLengthForward);
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
			_currentAnimation = StartCoroutine(OpenVentCover());
		}
		else
		{
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			_interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			_currentAnimation = StartCoroutine(CloseVentCover());
		}
	}

	IEnumerator OpenVentCover()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

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
		IsDoorOpened = false;

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