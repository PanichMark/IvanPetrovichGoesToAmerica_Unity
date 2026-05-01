using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableVentCover : InteractionObjectOpenableDrawer
{
	[SerializeField] private float openLengthUp; // Добавляем новое поле для движения вверх

	// Переопределяем Start, чтобы учесть новое направление
	public void Start()
	{
		base.Start();

		// Начальное положение (закрытое)
		closedPosition = transform.localPosition;

		// Сначала двигаем вперёд, потом вверх
		openedPosition = transform.localPosition + new Vector3(0, 0, openLengthForward);
		openedPosition += new Vector3(0, openLengthUp, 0);
	}

	// Переопреде
	// ляем ChangeLanguage, если нужно (можно оставить как есть, если нет доп. строк)
	/*
	public override void ChangeLanguage()
	{
		base.ChangeLanguage();
	}
	*/

	// Переопределяем Interact для новой логики открытия/закрытия
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

	// Новая корутина для открытия крышки вентиляции: сначала вперёд, потом вверх
	IEnumerator OpenVentCover()
	{
		Debug.Log($"Was opened {InteractionObjectNameUI}");
		IsDoorOpened = true;

		Vector3 intermediatePos = transform.localPosition + new Vector3(0, 0, openLengthForward);

		// Двигаем вперёд
		while (Mathf.Abs(transform.localPosition.z - intermediatePos.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, intermediatePos, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		// Двигаем вверх
		while (Mathf.Abs(transform.localPosition.y - openedPosition.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, openedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}

	// Новая корутина для закрытия: сначала вниз, потом назад
	IEnumerator CloseVentCover()
	{
		Debug.Log($"Was closed {InteractionObjectNameUI}");
		IsDoorOpened = false;

		Vector3 intermediatePos = transform.localPosition - new Vector3(0, openLengthUp, 0);

		// Двигаем вниз
		while (Mathf.Abs(transform.localPosition.y - intermediatePos.y) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, intermediatePos, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		// Двигаем назад
		while (Mathf.Abs(transform.localPosition.z - closedPosition.z) > 0.001f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, closedPosition, Time.deltaTime * OpeningSpeed);
			yield return null;
		}

		currentAnimation = null;
	}
}