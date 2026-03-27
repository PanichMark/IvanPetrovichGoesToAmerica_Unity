using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable
{
	protected LocalizationManager localizationManager;

	protected Collider playerCollider; // Ссылка на коллайдер игрока
	protected bool isCollisionIgnored = false; // Флаг для отслеживания игнорирования физики
	protected bool isPlayerInsideTrigger = false; // Флаг: игрок внутри триггера объекта
												  // Слои для Physics.IgnoreLayerCollision
	protected int pickableLayer;
	protected int playerLayer;
	public GameObject CachedPlayer { get; protected set; }
	public Collider Collider { get; protected set; }
	public Rigidbody RigidBody { get; protected set; }

	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }
	public string InteractionHintMessageMain => $"{InteractionHintAction} {InteractionObjectNameUI}?";

	public string InteractionHintAction {  get; protected set; }

	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	public bool IsObjectPickedUp { get; protected set; }


	void Start()
	{
		pickableLayer = LayerMask.NameToLayer("Pickable");
		playerLayer = LayerMask.NameToLayer("Player");
		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("Player");
		playerCollider = CachedPlayer.GetComponent<Collider>(); // Получаем коллайдер игрока

		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
	}

	public void Interact()
	{
		PickUpObject();
	}

	public virtual void PickUpObject()
	{
		if (!IsObjectPickedUp)
		{
			if (CachedPlayer != null)
			{
				Debug.Log($"Picked up {InteractionObjectNameSystem}");
				
				gameObject.tag = "Untagged";
				Collider.enabled = false;
				RigidBody.isKinematic = true;

				// Начинаем плавное перемещение
				StartCoroutine(MoveTowardsInFrontOfPlayer());

				// Другие настройки остаются такими же
				transform.parent = CachedPlayer.transform;
				transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
				IsObjectPickedUp = true;
			}
			else
			{
				Debug.Log("Player not found!");
			}
		}
	}

	public virtual void DropOffObject()
	{
		Debug.Log($"Dropped off {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		// Отцепляем объект от игрока
		transform.parent = null;

		// 1. Игнорируем столкновения между слоем этого объекта (Pickable) и слоем Player
		Physics.IgnoreLayerCollision(gameObject.layer, playerLayer, true);

		// 2. Запускаем корутину, которая подождет 0.1 секунды и вернет столкновения
		StartCoroutine(EnableCollisionAfterDelay(0.25f));

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(1));
	}
	// Корутина: ждет заданное время и включает столкновения обратно
	IEnumerator EnableCollisionAfterDelay(float delay)
	{
		// Ждем 0.1 секунды (или другое указанное время)
		yield return new WaitForSeconds(delay);

		// Проверка нужна на случай, если объект уничтожили раньше, чем сработала корутина
		if (this != null && gameObject != null)
		{
			// Возвращаем возможность столкновений между Pickable и Player
			Physics.IgnoreLayerCollision(gameObject.layer, playerLayer, false);
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == CachedPlayer)
		{
			isPlayerInsideTrigger = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == CachedPlayer)
		{
			isPlayerInsideTrigger = false;
			// Если игрок вышел из триггера — возвращаем столкновения
			if (isCollisionIgnored && playerCollider != null)
			{
				Physics.IgnoreCollision(Collider, playerCollider, false);
				isCollisionIgnored = false;
			}
		}
	}
	IEnumerator MoveTowardsInFrontOfPlayer()
	{
		while (true)
		{
			// Рассчитываем новую целевую позицию каждый кадр
			Vector3 targetPosition = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;

			// Перемещаем объект к новой позиции
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			// Выход из цикла, если объект вплотную приблизился к игроку
			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				break;
			}

			yield return null;
		}

		// Установим последнюю позицию на случай погрешности
		transform.position = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;
	}

	public void SaveData(ref GameData data)
	{
		
	}

	public void LoadData(GameData data)
	{
		
	}
}