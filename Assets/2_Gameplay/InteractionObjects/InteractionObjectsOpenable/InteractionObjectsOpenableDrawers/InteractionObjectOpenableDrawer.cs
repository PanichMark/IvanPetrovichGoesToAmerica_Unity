using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDrawer : InteractionObjectOpenableAbstract
{
	protected LocalizationManager localizationManager;

	[SerializeField] protected float OpeningSpeed = 3f; // Скорость открытия-закрытия ящика

	protected Coroutine currentAnimation;     // Переменная для хранения активной корутины

	protected Vector3 openedPosition;        // Открытое положение ящика
	protected Vector3 closedPosition;        // Закрытое положение ящика
	[SerializeField] protected float openLengthForward;

	public void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);


		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
		// Начальное положение закрылого ящика
		closedPosition = transform.localPosition;

		// Открываем ящик вперёд по оси Z на 0.45 единицы
		openedPosition = transform.localPosition + new Vector3(0, 0, openLengthForward);
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
		IsDoorOpened = false;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);


		InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
		interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
	}


	public override void Interact()
	{
		// Останавливаем ранее запущенную корутину, если она существует
		if (currentAnimation != null)
		{
			StopCoroutine(currentAnimation);
		}

		if (!IsDoorOpened)
		{
			InteractionHintAction = localizationManager.GetLocalizedString("CloseDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(OpenDrawer()); // Начинаем новую корутину
		}
		else
		{
			InteractionHintAction = localizationManager.GetLocalizedString("OpenDoor");
			interactionHintMessageMain = $"{InteractionHintAction} {InteractionObjectNameUI}";
			currentAnimation = StartCoroutine(CloseDrawer()); // Начинаем новую корутину
		}
	}

	IEnumerator OpenDrawer()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

		while (Mathf.Abs(transform.localPosition.z - openedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, openedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	IEnumerator CloseDrawer()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsDoorOpened = false;

		while (Mathf.Abs(transform.localPosition.z - closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, closedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}

