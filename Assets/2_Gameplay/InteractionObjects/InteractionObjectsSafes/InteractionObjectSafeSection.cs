using UnityEngine;
using System.Collections;

public class InteractionObjectSafeSection : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private int _safeSectionSlotNumber;
	[SerializeField][Range(0, 9)] private int _correctSectionPosition;

	public int CorrectSectionPosition => _correctSectionPosition;
	public bool IsSectionPositionCorrect { get; private set; }
	public int currentSectionPosition { get; private set; }

	private float _sectionRotationSpeed = 0.15f;
	private Coroutine _sectionCoroutine;

	public string InteractionHintMessageAction { get; protected set; }
	public string InteractionObjectNameUI => _safeSectionSlotNumber.ToString();
	public virtual string InteractionHintMessageMain => $"Rotate section #{InteractionObjectNameUI}";
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionObjectNameSystem => null;

	public void Interact()
	{
		if (_sectionCoroutine == null)
		{
			_sectionCoroutine = StartCoroutine(RotateSmoothly(_sectionRotationSpeed));
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

		IsSectionPositionCorrect = currentSectionPosition == _correctSectionPosition;

		if (IsSectionPositionCorrect)
			Debug.Log($"Section #{InteractionObjectNameUI} CORRECT");

		transform.localRotation = rotateTo;
		_sectionCoroutine = null;
	}

	public void SetSectionPositionToCorrect()
	{
		IsSectionPositionCorrect = true;
	}
}