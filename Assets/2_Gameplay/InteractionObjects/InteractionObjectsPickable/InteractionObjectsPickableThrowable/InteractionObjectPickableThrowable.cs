using UnityEngine;
using System.Collections;

public class InteractionObjectPickableThrowable : InteractionObjectPickableAbstract, IThrowable, IDamageable
{
	private bool _wasObjectDestroyed;

	private bool _canObjectBeDestroyedOnImpact;
	public bool WasObjectDestroyed => _wasObjectDestroyed;
	public float ObjectThrowPower => 10f;

	private GameObject firstPersonRightHandWeaponSlotGameObject;

	[SerializeField, Min(0)] private float _health;

	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			if (_health <= 0)
			{
				DestroyTrowableObject(); // Вызываем метод уничтожения, если здоровье стало <= 0
			}
		}
	}

	private void Start()
	{
		firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("firstPersonRightHandWeaponSlotGameObject");
		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("Player");
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	public override void PickUpObject()
	{
		if (!IsObjectPickedUp)
		{
			if (firstPersonRightHandWeaponSlotGameObject != null)
			{
				Debug.Log($"Picked up {InteractionObjectNameSystem}");

				gameObject.tag = "Untagged";
				Collider.enabled = false;
				RigidBody.isKinematic = true;

				// Начинаем плавное перемещение
				StartCoroutine(MoveTowardsRightHand());

				// Другие настройки остаются такими же
				transform.parent = firstPersonRightHandWeaponSlotGameObject.transform;
				transform.rotation = Quaternion.Euler(0, firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
				IsObjectPickedUp = true;
			}
			else
			{
				Debug.Log("Player not found!");
			}
		}
	}








	private void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
			RigidBody.isKinematic = true;

			_wasObjectDestroyed = true;
			Destroy(gameObject);
			Debug.Log($"{InteractionObjectNameSystem} was destroyed on impact!");
		}
	}
	
	public void ThrowObject()
	{
		Debug.Log($"Throwed {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		_canObjectBeDestroyedOnImpact = true;
		// Отцепляем объект от игрока
		transform.parent = null;

		////////////////
		// FIX!!!!!!!!!!!!!!!!!!!!!!!
		Vector3 throwDirection = CachedPlayer.transform.forward - Camera.main.transform.up * Mathf.Tan(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);
		throwDirection.Normalize();

		RigidBody.AddForce(throwDirection * ObjectThrowPower, ForceMode.Impulse);
		//FIX!!!!!!!!!!!
		/////////////
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {Health - amount}");
		Health -= amount; // Уменьшаем здоровье на указанное количество единиц
	}

	public void DestroyTrowableObject()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");
		_wasObjectDestroyed = true; // Устанавливаем флаг, что объект разрушен
		Destroy(gameObject); // Уничтожаем объект
	}

	private IEnumerator MoveTowardsRightHand()
	{
		while (true)
		{
			// Рассчитываем новую целевую позицию каждый кадр
			Vector3 targetPosition = firstPersonRightHandWeaponSlotGameObject.transform.position;

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
		transform.position = firstPersonRightHandWeaponSlotGameObject.transform.position;
	}
}