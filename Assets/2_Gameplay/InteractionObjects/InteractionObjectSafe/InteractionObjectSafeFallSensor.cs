using UnityEngine;

public class InteractionObjectSafeFallSensor : MonoBehaviour
{
	// Ссылка на ваш основной контроллер, который висит на двери
	private InteractionObjectSafeUndestructableController _safeController;
	private GameObject _safeDoor;

	public void Initialize(
		InteractionObjectSafeUndestructableController safeController,
		 GameObject safeDoor)
	{
		_safeController = safeController;
		_safeDoor = safeDoor;
	}
	private void OnCollisionEnter(Collision collision)
	{
		// Используем ReferenceEquals для строгого сравнения ссылок (тот самый объект или нет)
		if (ReferenceEquals(collision.collider.gameObject, _safeDoor))
		{
			return; // Игнорируем столкновение со своей же дверью
		}

		if (!_safeController.IsFalling)
		{
			return;
		}

		//Debug.Log("COLLISION BODY! Объект: " + collision.collider.name);


		_safeController.OnSafeBodyCollided(collision);
	
	}
}