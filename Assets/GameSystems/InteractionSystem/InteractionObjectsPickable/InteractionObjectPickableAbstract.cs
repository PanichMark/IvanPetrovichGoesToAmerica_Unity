using UnityEngine;
using System.Collections;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable
{
	private LocalizationManager localizationManager;
	public Collider Collider { get; protected set; }
	public Rigidbody RigidBody { get; protected set; }
	public GameObject CachedPlayer {  get; protected set; }

	[SerializeField] protected string interactionItemNameSystem;
	public virtual string InteractionObjectNameSystem => interactionItemNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }
	public string MainInteractionHintMessage => $"{MainInteractionHintAction} {InteractionObjectNameUI}?";

	private string MainInteractionHintAction = "HUDInteraction_HintAction_Pickable";

	public virtual string AdditionalInteractionHintMessage => null;
	public virtual bool IsAdditionalInteractionHintMessageActive => false;

	public bool IsObjectPickedUp { get; protected set; }

	void Start()
	{
		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("Player");
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionItemNameSystem);
		MainInteractionHintAction = localizationManager.GetLocalizedString(MainInteractionHintAction);
	}

	public void Interact()
	{
		PickUpObject();
	}

	public void PickUpObject()
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
				StartCoroutine(MoveTowardsTarget());

				// Другие настройки остаются такими же
				transform.parent = CachedPlayer.transform;
				//transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
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
	}

	IEnumerator MoveTowardsTarget()
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