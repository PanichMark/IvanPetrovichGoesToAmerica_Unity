using UnityEngine;
using System.Collections;

public class SafeController : MonoBehaviour, IInteractable
{
	private bool isAdditionalInteractionHintActive;
	public virtual string InteractionObjectNameSystem => null;

	public GameObject SafeDoor;
	private Transform safeDoorTransform;

	public GameObject SafeRotatorySection1;
	public GameObject SafeRotatorySection2;
	public GameObject SafeRotatorySection3;
	private SafeRotatorySection section1;
	private SafeRotatorySection section2;
	private SafeRotatorySection section3;

	private bool wasSafeOpened;

	private float safeDoorOpeningSpeed = 100f;
	private Quaternion safeDoorOpenedRotation;

	private bool isInStartMethod;

	public string InteractionObjectNameUI => null;
	public string MainInteractionHint => "Открыть сейф";
	public virtual string AdditionalInteractionHint => "Неправильная комбинация!";
	public virtual bool IsAdditionalInteractionHintActive => isAdditionalInteractionHintActive;


	
	void Start()
	{


		isInStartMethod = true;

		safeDoorTransform = SafeDoor.GetComponent<Transform>();

		section1 = SafeRotatorySection1.GetComponent<SafeRotatorySection>();
		section2 = SafeRotatorySection2.GetComponent<SafeRotatorySection>();
		section3 = SafeRotatorySection3.GetComponent<SafeRotatorySection>();

		Vector3 openedEulerAngles = new Vector3(0, -90, 0);
		safeDoorOpenedRotation = Quaternion.Euler(openedEulerAngles);

		if (wasSafeOpened == true)
		{
			safeDoorTransform.transform.localRotation = safeDoorOpenedRotation;
			section1.SetSectionPositionToCorrect();
			section2.SetSectionPositionToCorrect();
			section3.SetSectionPositionToCorrect();
		}


		CheckRotatorySectionCorrection();

		

		isInStartMethod = false;
	}

	public void Interact()
	{
		if (wasSafeOpened == false)
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

		while (Quaternion.Angle(safeDoorTransform.transform.localRotation, safeDoorOpenedRotation) > 0.1f)
		{
			safeDoorTransform.transform.localRotation = Quaternion.RotateTowards(safeDoorTransform.transform.localRotation,
				safeDoorOpenedRotation, Time.deltaTime * safeDoorOpeningSpeed);
			yield return null;
		}
	}

	private void CheckRotatorySectionCorrection()
	{
		if (section1.currentSectionPosition == section1.CorrectSectionPosition)
		{
			section1.SetSectionPositionToCorrect();
		}
		if (section2.currentSectionPosition == section2.CorrectSectionPosition)
		{
			section2.SetSectionPositionToCorrect();
		}
		if (section3.currentSectionPosition == section3.CorrectSectionPosition)
		{
			section3.SetSectionPositionToCorrect();
		}

		if(section1.IsSectionPositionCorrect && section2.IsSectionPositionCorrect && section3.IsSectionPositionCorrect)
		{
			if (isInStartMethod == false)
			{
				Debug.Log("SAFE CORRECT");
			}

			isAdditionalInteractionHintActive = false;
			wasSafeOpened = true;

			StartCoroutine(OpenSafeDoor());
		}
		else
		{
			if (isInStartMethod == false)
			{
				Debug.Log("SAFE FAILED");
				isAdditionalInteractionHintActive = true;

			}
		}

		if (wasSafeOpened == true)
		{
			float yAngle = section1.CorrectSectionPosition != 0 ? 360f / 10 * section1.CorrectSectionPosition : 0f;
			Vector3 openedEulerAngles = new Vector3(0, yAngle, 0);
			var sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			section1.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = section2.CorrectSectionPosition != 0 ? 360f / 10 * section2.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			section2.transform.localRotation = sectionCorrectPositionRotation;

			yAngle = section3.CorrectSectionPosition != 0 ? 360f / 10 * section3.CorrectSectionPosition : 0f;
			openedEulerAngles = new Vector3(0, yAngle, 0);
			sectionCorrectPositionRotation = Quaternion.Euler(openedEulerAngles);
			section3.transform.localRotation = sectionCorrectPositionRotation;
		}
	}
}
