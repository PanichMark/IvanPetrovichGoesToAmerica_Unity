using UnityEngine;
using System.Collections; // Обязательно для использования IEnumerator и WaitForSeconds

public class InteractionObjectVendingMachineChooseAmmoType : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionObjectVendingMachineAmmo vendingMachine;
	[SerializeField] private string Name = "Выбор патронов";
	[SerializeField] private float rotationDuration = 1f; // Длительность анимации
	[SerializeField] private Vector3 rotationAxis = Vector3.right; // Ось поворота (X)
	[SerializeField] private float rotationAngle = 90f; // Угол поворота

	public string InteractionObjectNameSystem => Name;
	public string InteractionObjectNameUI => Name;
	public string InteractionHintMessageMain => "Сменить тип патронов";
	public string InteractionHintAction => "Сменить";
	public string InteractionHintMessageAdditional => "Подождите!";

	// Флаг блокировки, чтобы предотвратить повторный вызов Interact()
	private bool isBusy = false;

	public bool IsInteractionHintMessageAdditionalActive => isBusy;

	public void Interact()
	{
		// Если анимация уже идет, игнорируем нажатие
		if (isBusy)
			return;

		// Запускаем корутину и блокируем взаимодействие
		
		vendingMachine.SetCurrentAmmoType(vendingMachine.currentAmmoIndex + 1);
		StartCoroutine(RotateAndChangeAmmo());
		isBusy = true;
	}

	private IEnumerator RotateAndChangeAmmo()
	{
		
		// Сохраняем начальное вращение объекта
		Quaternion startRotation = transform.rotation;
		// Вычисляем конечное вращение (поворот на 90 градусов вокруг оси X)
		Quaternion endRotation = startRotation * Quaternion.Euler(rotationAxis * rotationAngle);

		float elapsedTime = 0f;

		// Плавно поворачиваем объект в течение rotationDuration секунд
		while (elapsedTime < rotationDuration)
		{
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationDuration);
			elapsedTime += Time.deltaTime;
			yield return null; // Ждем следующего кадра
		}

		// Убеждаемся, что объект точно в конечной позиции (исправляем плавающие ошибки)
		transform.rotation = endRotation;


		Debug.Log($"Выбран тип патронов: {vendingMachine.ammoTypes[vendingMachine.currentAmmoIndex].ammoName}");

		// Разблокируем возможность нового взаимодействия
		isBusy = false;
	}
}