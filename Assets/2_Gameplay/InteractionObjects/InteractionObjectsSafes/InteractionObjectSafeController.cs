using UnityEngine;
using System.Collections;

public class InteractionObjectSafeController : MonoBehaviour, IInteractable
{
	public virtual string InteractionObjectNameSystem => null;
	public string InteractionHintAction { get; protected set; }
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => "Open safe";
	public virtual string InteractionHintMessageAdditional => "Wrong combination!";
	public virtual bool IsInteractionHintMessageAdditionalActive => _isAdditionalInteractionHintActive;

	private bool _isAdditionalInteractionHintActive;
	private bool _isSafeOpened;
	private bool _isInStartMethod;

	public GameObject SafeDoor;
	private Transform _safeDoorTransform;

	public GameObject SafeRotatorySection1;
	public GameObject SafeRotatorySection2;
	public GameObject SafeRotatorySection3;
	private InteractionObjectSafeSection _section1;
	private InteractionObjectSafeSection _section2;
	private InteractionObjectSafeSection _section3;

	private float _safeDoorOpeningSpeed = 100f;
	private Quaternion _safeDoorOpenedRotation;

	void Start()
	{
		_isInStartMethod = true;

		_safeDoorTransform = SafeDoor.GetComponent<Transform>();

		_section1 = SafeRotatorySection1.GetComponent<InteractionObjectSafeSection>();
		_section2 = SafeRotatorySection2.GetComponent<InteractionObjectSafeSection>();
		_section3 = SafeRotatorySection3.GetComponent<InteractionObjectSafeSection>();

		Vector3 openedEulerAngles = new Vector3(0, -90, 0);
		_safeDoorOpenedRotation = Quaternion.Euler(openedEulerAngles);

		if (_isSafeOpened)
		{
			_safeDoorTransform.localRotation = _safeDoorOpenedRotation;
			_section1.SetSectionPositionToCorrect();
			_section2.SetSectionPositionToCorrect();
			_section3.SetSectionPositionToCorrect();
		}

		CheckRotatorySectionCorrection();

		_isInStartMethod = false;
	}

	public void Interact()
	{
		if (!_isSafeOpened)
		{
			CheckRotatorySectionCorrection();
		}
	}

	IEnumerator OpenSafeDoor()
	{
		gameObject.tag = "Untagged";

		SafeRotatorySection1.tag = "Untagged";
		SafeRotatorySection2.tag = "Untagged";
		SafeRotatorySection3.tag = "Untagged";

		while (Quaternion.Angle(_safeDoorTransform.localRotation, _safeDoorOpenedRotation) > 0.1f)
		{
			_safeDoorTransform.localRotation = Quaternion.RotateTowards(
				_safeDoorTransform.localRotation,
				_safeDoorOpenedRotation,
				Time.deltaTime * _safeDoorOpeningSpeed);
			yield return null;
		}
	}

	private void CheckRotatorySectionCorrection()
	{
		if (_section1.currentSectionPosition == _section1.CorrectSectionPosition)
			_section1.SetSectionPositionToCorrect();
		if (_section2.currentSectionPosition == _section2.CorrectSectionPosition)
			_section2.SetSectionPositionToCorrect();
		if (_section3.currentSectionPosition == _section3.CorrectSectionPosition)
			_section3.SetSectionPositionToCorrect();

		if (_section1.IsSectionPositionCorrect && _section2.IsSectionPositionCorrect && _section3.IsSectionPositionCorrect)
		{
			if (!_isInStartMethod)
				Debug.Log("SAFE CORRECT");

			_isAdditionalInteractionHintActive = false;
			_isSafeOpened = true;

			StartCoroutine(OpenSafeDoor());
		}
		else
		{
			if (!_isInStartMethod)
			{
				Debug.Log("SAFE FAILED");
				_isAdditionalInteractionHintActive = true;
			}
		}

		if (_isSafeOpened)
		{
			float yAngle = _section1.CorrectSectionPosition != 0 ? 360f / 10 * _section1.CorrectSectionPosition : 0f;
			Vector3 openedEulerAngles = new Vector3(0, yAngle, 0);
			var sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section1.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = _section2.CorrectSectionPosition != 0 ? 360f / 10 * _section2.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section2.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = _section3.CorrectSectionPosition != 0 ? 360f / 10 * _section3.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			_section3.transform.localRotation = sectionCorrectPositionRotation;
		}
	}
}