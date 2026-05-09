using UnityEngine;
using System.Collections;

public class InteractionObjectSafeSection : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private int safeSectionSlotNumber;
	[SerializeField][Range(0, 9)] private int correctSectionPosition;

	public int CorrectSectionPosition => correctSectionPosition;
	public bool IsSectionPositionCorrect { get; private set; }
	public int currentSectionPosition { get; private set; }

	private float sectionRotationSpeed = 0.15f;
	private Coroutine sectionCoroutine;

	public string InteractionHintAction { get; protected set; }
	public string InteractionObjectNameUI => safeSectionSlotNumber.ToString();
	public virtual string InteractionHintMessageMain => $"Rotate section #{InteractionObjectNameUI}";
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionObjectNameSystem => null;

	public void Interact()
	{
		if (sectionCoroutine == null)
		{
			sectionCoroutine = StartCoroutine(RotateSmoothly(sectionRotationSpeed));
		}
	}

	IEnumerator RotateSmoothly(float duration)
	{
		currentSectionPosition = currentSectionPosition < 9 ? currentSectionPosition + 1 : 0;

		Quaternion rotateFrom = transform.localRotation;
		Quaternion rotateTo = transform.localRotation * Quaternion.Euler(0, 36, 0);

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			transform.localRotation = Quaternion.Slerp(rotateFrom, rotateTo, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Debug.Log($"Section #{InteractionObjectNameUI} new position is {currentSectionPosition}");

		IsSectionPositionCorrect = currentSectionPosition == correctSectionPosition;

		if (IsSectionPositionCorrect)
			Debug.Log($"Section #{InteractionObjectNameUI} CORRECT");

		transform.localRotation = rotateTo;
		sectionCoroutine = null;
	}

	public void SetSectionPositionToCorrect()
	{
		IsSectionPositionCorrect = true;
	}
}