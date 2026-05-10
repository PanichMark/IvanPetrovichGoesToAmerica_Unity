using UnityEngine;

public class HintMessageController : MonoBehaviour
{
	[SerializeField] private GameObject noteObject;
	private Collider triggerZone;

	private InteractionObjectNote interactionNote;
	private GameObject playerCollider;

	private void Awake()
	{
		// Получаем компонент заметки
		interactionNote = noteObject.GetComponent<InteractionObjectNote>();

		// Получаем ссылку на игрока ОДИН РАЗ при старте
		playerCollider = ServiceLocator.Resolve<GameObject>("PlayerColliderGameObject");

		// Настраиваем зону триггера
	

		triggerZone = GetComponent<Collider>();

		triggerZone.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		// Срабатывает ТОЛЬКО для игрока, полученного из ServiceLocator
		if (other.gameObject == playerCollider)
		{
			// Вызываем метод взаимодействия с заметкой
			interactionNote.Interact();

			// Отключаем триггер, чтобы событие не повторялось
			triggerZone.enabled = false;
		}
	}
}