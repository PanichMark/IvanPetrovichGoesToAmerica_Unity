using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObjectSafeController : MonoBehaviour, IInteractable
{
	public virtual string InteractionObjectNameSystem => null;
	public string InteractionHintMessageAction { get; protected set; }
	public string InteractionObjectNameUI => null;
	public string InteractionHintMessageMain => "Open safe";
	public virtual string InteractionHintMessageFail => "Wrong combination!";
	public virtual bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	private bool _isAdditionalInteractionHintActive;
	private bool _isSafeOpened;
	private bool _isInStartMethod;

	private	GameObject _safeDoor;
	private Transform _safeDoorTransform;

	private GameObject _safeRotatorySection1;
	private GameObject _safeRotatorySection2;
	private GameObject _safeRotatorySection3;
	private InteractionObjectSafeSection _section1;
	private InteractionObjectSafeSection _section2;
	private InteractionObjectSafeSection _section3;

	private float _safeDoorOpeningSpeed = 100f;
	private Quaternion _safeDoorOpenedRotation;

	void Start()
	{
		_isInStartMethod = true;


		_safeDoor = transform.parent.gameObject;
		_safeDoorTransform = _safeDoor.GetComponent<Transform>();

		_safeRotatorySection1 = transform.parent.Find("SafeSection1").gameObject;
		_safeRotatorySection2 = transform.parent.Find("SafeSection2").gameObject;
		_safeRotatorySection3 = transform.parent.Find("SafeSection3").gameObject;

		_section1 = _safeRotatorySection1.GetComponent<InteractionObjectSafeSection>();
		_section2 = _safeRotatorySection2.GetComponent<InteractionObjectSafeSection>();
		_section3 = _safeRotatorySection3.GetComponent<InteractionObjectSafeSection>();

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

		_safeRotatorySection1.tag = "Untagged";
		_safeRotatorySection2.tag = "Untagged";
		_safeRotatorySection3.tag = "Untagged";

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