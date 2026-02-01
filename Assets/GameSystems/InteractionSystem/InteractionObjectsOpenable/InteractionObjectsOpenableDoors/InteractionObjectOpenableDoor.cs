using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract
{
	//public override string InteractionItemName => "Дверь";

	private bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;

	[SerializeField]
	private InteractionObjectLock lockController;   // Поле для добавления контроллера замка
											 //private bool isDoorUnlocked;
											 //private bool WasDoorKeyFound;

	private float doorOpeningSpeed = 200f; // Скорость открытия-закрытия

	private Coroutine currentAnimation;     // Переменная для хранения активной корутины

	private Quaternion openedRotation;       // Угловое положение открытой двери
	private Quaternion closedRotation;     // Угловое положение закрытой двери
	[SerializeField] private int doorOpenAngle;
	
	public override string InteractionHintMessageAdditional => $"{InteractionObjectNameUI} заперта!";

	void Start()
	{
		// Настройка состояний вращения
		Vector3 openedEulerAngles = new Vector3(0, 0, doorOpenAngle);
		openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		closedRotation = Quaternion.Euler(closedEulerAngles);

		IsDoorOpened = false;

		if (lockController != null && lockController.WasUnlocked == false)
		{
			interactionHintMessageMain = lockController.InteractionHintMessageMain;
			lockController.OnUnlockLock += UnlockDoor;
		}

		
		if (lockController == null || lockController.WasUnlocked == true)
		{
			interactionHintMessageMain = !IsDoorOpened ? $"Открыть {InteractionObjectNameUI}" : $"Закрыть {InteractionObjectNameUI}";
		}

		
	}

	private void UnlockDoor()
	{
		interactionHintMessageMain = !IsDoorOpened ? $"Открыть {InteractionObjectNameUI}" : $"Закрыть {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
		if (lockController != null && lockController.WasUnlocked == false)
		{
			// Если замок установлен, сначала пытаемся открыть замок
			Debug.Log("Запускается попытка взлома замка...");
			lockController.Interact();

			
		}

		if (lockController == null || lockController.WasUnlocked == true)
		{
			// Если замка нет, сразу открываем дверь
			PerformDoorInteraction();
		}
	}
	private void PerformDoorInteraction()
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

