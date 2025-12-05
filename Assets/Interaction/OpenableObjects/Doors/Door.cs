using UnityEngine;
using System.Collections;

public class Door : OpenableObjectAbstract
{
	//public override string InteractionItemName => "Дверь";

	private bool isAdditionalInteractionHintActive;
	public override bool IsAdditionalInteractionHintActive => isAdditionalInteractionHintActive;

	[SerializeField] private bool isDoorKeyLocked;
	//private bool isDoorUnlocked;
	//private bool WasDoorKeyFound;

	private float doorOpeningSpeed = 200f; // Скорость открытия-закрытия

	private Coroutine currentAnimation;     // Переменная для хранения активной корутины

	private Quaternion openedRotation;       // Угловое положение открытой двери
	private Quaternion closedRotation;     // Угловое положение закрытой двери
	[SerializeField] private int doorOpenAngle;
	
	public override string AdditionalInteractionHint => $"{InteractionObjectNameUI} заперта!";

	void Start()
	{
		// Настройка состояний вращения
		Vector3 openedEulerAngles = new Vector3(0, 0, doorOpenAngle);
		openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		closedRotation = Quaternion.Euler(closedEulerAngles);

		IsDoorOpened = false;
	}

	public override void Interact()
	{
		if (!isDoorKeyLocked)
		{
			isAdditionalInteractionHintActive = false;

			// Останавливаем ранее запущенную корутину, если она существует
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
			}

			if (!IsDoorOpened)
			{
				currentAnimation = StartCoroutine(OpenDoor()); // Начинаем новую корутину
			}
			else
			{
				currentAnimation = StartCoroutine(CloseDoor()); // Начинаем новую корутину
			}
		}
		else isAdditionalInteractionHintActive = true;

	}

	private void Update()
	{
		//Debug.Log(currentAnimation);
	}

	IEnumerator OpenDoor()
	{
		Debug.Log($"Была открыта {InteractionObjectNameUI}");
		IsDoorOpened = true;

		while (Quaternion.Angle(transform.localRotation, openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, openedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseDoor()
	{
		Debug.Log($"Была закрыта {InteractionObjectNameUI}");
		IsDoorOpened = false;

		while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, closedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}