using UnityEngine;
using System.Collections;

public class LegKickAttack : MonoBehaviour
{
	private IInputDevice inputDevice;

	// Конструктор принимает зависимость
	public LegKickAttack(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
	}

	PlayerMovementController playerMovementController;
	InteractionController interactionController;

	public bool IsPlayerLegKicking { get; private set; }

	// Высота и радиус капсулы
	float CapsuleHeight;     // Высота капсулы (примерное расстояние вдоль оси Y)
	float CapsuleRadius;   // Радиус капсулы
	float ForwardOffset;    // Смещение вперёд от центра игрока

	public float WeaponDamage { get; private set; } = 50;
		
	void Start()
	{
		playerMovementController = GetComponent<PlayerMovementController>();
		interactionController = GetComponent<InteractionController>();

		CapsuleHeight = 1.8f;      // Высота капсулы (примерное расстояние вдоль оси Y)
		CapsuleRadius = 0.3f;      // Радиус капсулы
		ForwardOffset = 0.5f;      // Смещение вперёд от центра игрока
	}

	private void OnDrawGizmos()
	{
		
		// Нижняя и верхняя точки капсулы
		Vector3 startPoint = transform.position + transform.forward * ForwardOffset;
		Vector3 endPoint = transform.position + transform.forward * ForwardOffset + transform.up * CapsuleHeight;

		// Рисуем верхнюю и нижнюю полусферу
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(startPoint, CapsuleRadius);
		Gizmos.DrawWireSphere(endPoint, CapsuleRadius);

		// Центральный соединяющий куб
		Vector3 cylinderCenter = (startPoint + endPoint) / 2;
		Quaternion rotation = Quaternion.LookRotation(Vector3.right, transform.forward);
		Gizmos.DrawWireCube(cylinderCenter, new Vector3(CapsuleRadius * 2, CapsuleHeight, CapsuleRadius * 2));
	}

	void Update()
	{
		if (inputDevice.GetKeyLegKick() && !IsPlayerLegKicking && interactionController.CurrentPickableObject == null)
		{ 
			LegKick();
		}

		if (!playerMovementController.IsPlayerCrouching)
		{
			CapsuleHeight = 1.8f;
		}
		else CapsuleHeight = 1;

		//Debug.Log(IsPlayerLegKicking);
	}

	public void LegKick()
	{
		Debug.Log("LegKick attack");
		
		StartCoroutine(playerMovementController.DisablePlayerMovementDuringLegKickAttack());
		StartCoroutine(DisableLegKickAttackActivation());

		// Нижняя и верхняя точки капсулы
		Vector3 startPoint = transform.position + transform.forward * ForwardOffset;
		Vector3 endPoint = transform.position + transform.forward * ForwardOffset + transform.up * CapsuleHeight;

		// Физически проверяем объекты, захваченные капсулой
		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject.CompareTag("Player"))
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayLegKickAttackDamage(damageable, 0.5f, WeaponDamage));
			}
		}

		if (playerMovementController.IsPlayerCrouching == false && (playerMovementController.CurrentPlayerMovementStateType != "PlayerSliding" || playerMovementController.CurrentPlayerMovementStateType != "PlayerLedgeClimbing"))
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		else if (playerMovementController.IsPlayerCrouching == true && (playerMovementController.CurrentPlayerMovementStateType != "PlayerSliding" || playerMovementController.CurrentPlayerMovementStateType != "PlayerLedgeClimbing"))
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
	}

	IEnumerator DisableLegKickAttackActivation()
	{
		IsPlayerLegKicking = true;
		yield return new WaitForSeconds(1f);
		IsPlayerLegKicking = false;
	}

	IEnumerator DelayLegKickAttackDamage(IDamageable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime); // Ждем нужную задержку

		// Наносим урон после окончания ожидания
		target.TakeDamage(damageAmount);
	}
}
