using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoor : InteractionObjectOpenableAbstract
{
	//public override string InteractionItemName => "Дверь";

	protected bool isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;
	
	private LocalizationManager localizationManager;
	[SerializeField] private InteractionObjectLock lockController;   // Поле для добавления контроллера замка
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
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		IsDoorOpened = false;
		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		// Настройка состояний вращения
		Vector3 openedEulerAngles = new Vector3(0, 0, doorOpenAngle);
		openedRotation = Quaternion.Euler(openedEulerAngles);

		Vector3 closedEulerAngles = new Vector3(0, 0, 0);
		closedRotation = Quaternion.Euler(closedEulerAngles);
	
		

		if (lockController != null && lockController.WasUnlocked == false)
		{
			interactionHintMessageMain = lockController.InteractionHintMessageMain;
			lockController.OnUnlockLock += UnlockDoor;
		}

		
		if (lockController == null || lockController.WasUnlocked == true)
		{
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}"; 
		}

		
	}

	private void UnlockDoor()
	{
		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}

	public override void Interact()
	{
		if (lockController != null && lockController.WasUnlocked == false)
		{
			// Если замок установлен, сначала пытаемся открыть замок
			Debug.Log("Запускается попытка взлома замка...");
			lockController.Interact();
			//Debug.Log("LMAO!");

		}

		if (lockController == null || lockController.WasUnlocked == true)
		{
			// Если замка нет, сразу открываем дверь
			PerformDoorInteraction();
			//Debug.Log("BRUH!");
		}
		
	}
	protected virtual void PerformDoorInteraction()
	{
		isAdditionalInteractionHintActive = false;

		// Останавливаем ранее запущенную корутину, если она существует
		if (currentAnimation != null)
		{
			StopCoroutine(currentAnimation);
		}

		if (!IsDoorOpened)
		{
			Debug.Log($"Was opened {InteractionObjectNameUI}");
			IsDoorOpened = true;
			InteractionHintAction = localizationManager.GetLocalizedString("CloseDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(OpenDoor()); // Начинаем новую корутину
		}
		else
		{
			Debug.Log($"Was closed {InteractionObjectNameUI}");
			IsDoorOpened = false;
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(CloseDoor()); // Начинаем новую корутину
		}
	}
	private void Update()
	{
		//Debug.Log(currentAnimation);
	}

	IEnumerator OpenDoor()
	{
	
		
		while (Quaternion.Angle(transform.localRotation, openedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, openedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseDoor()
	{
	
	
		while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.1f)
		{
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, closedRotation, Time.deltaTime * doorOpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}

