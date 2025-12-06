using UnityEngine;
using System.Collections;

public class SafeRotatorySection : MonoBehaviour, IInteractable
{
	[SerializeField] private int safeSectionSlotNumber;
	[SerializeField] [Range(0, 9)] private int correctSectionPosition;
	public int CorrectSectionPosition => correctSectionPosition; // Только чтение через публичное свойство
	public bool IsSectionPositionCorrect { get; private set; }
	public int currentSectionPosition { get; private set; }

	private float sectionRotationSpeed = 0.15f;
	private Coroutine sectionCoroutine; // Переменная для хранения текущей корутины

	public string InteractionObjectNameUI => safeSectionSlotNumber.ToString();
	public virtual string MainInteractionHint => $"Повернуть ячейку #{InteractionObjectNameUI}";
	public virtual string AdditionalInteractionHint => null;

	public virtual bool IsAdditionalInteractionHintActive => false;

	public string InteractionObjectNameSystem => null;




	public void Interact()
	{
		// Проверяем, есть ли уже запущенная корутина
		if (sectionCoroutine == null)
		{
			// Можно либо остановить текущую корутину и начать новую, либо проигнорировать вызов
			sectionCoroutine = StartCoroutine(RotateSmoothly(sectionRotationSpeed));
		}
	}

	IEnumerator RotateSmoothly(float duration)
	{
		if (currentSectionPosition < 9)
		{
			currentSectionPosition++;
		}
		else
		{
			currentSectionPosition -= 9;
		}
		
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

		if (currentSectionPosition == correctSectionPosition)
		{
			IsSectionPositionCorrect = true;
			Debug.Log($"Section #{InteractionObjectNameUI} CORRECT");
		}
		else
		{
			IsSectionPositionCorrect = false;
		}

		transform.localRotation = rotateTo; // Гарантированно устанавливаем финальное положение
		sectionCoroutine = null; // Освобождаем переменную после завершения корутины
	}

	public void SetSectionPositionToCorrect()
	{
		IsSectionPositionCorrect = true;
	}
}
