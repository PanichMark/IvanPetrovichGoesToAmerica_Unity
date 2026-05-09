using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableVentCover : InteractionObjectOpenableDrawer
{
	[SerializeField] private float openLengthUp;
	private Vector3 intermediatePos;

	public void Start()
	{
		base.Start();

		closedPosition = transform.localPosition;
		openedPosition = transform.localPosition + new Vector3(0, 0, openLengthForward);
		openedPosition += new Vector3(0, openLengthUp, 0);
		intermediatePos = transform.localPosition + new Vector3(0, 0, openLengthForward);
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
			currentAnimation = StartCoroutine(OpenVentCover());
		}
		else
		{
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(CloseVentCover());
		}
	}

	IEnumerator OpenVentCover()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

		while (Mathf.Abs(transform.localPosition.z - intermediatePos.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, intermediatePos, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		while (Mathf.Abs(transform.localPosition.y - openedPosition.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, openedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseVentCover()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsDoorOpened = false;

		while (Mathf.Abs(transform.localPosition.y - intermediatePos.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, intermediatePos, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		while (Mathf.Abs(transform.localPosition.z - closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, closedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}