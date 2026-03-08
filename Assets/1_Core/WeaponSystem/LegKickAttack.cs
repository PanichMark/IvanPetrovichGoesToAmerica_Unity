using UnityEngine;
using System.Collections;

public class LegKickAttack : MonoBehaviour
{
	private IInputDevice inputDevice;
	private PlayerMovementController playerMovementController;
	//private InteractionController interactionController;
	private GameObject cachedPlayer;
	public bool IsPlayerLegKicking { get; private set; }

	// Высота и радиус капсулы
	private float CapsuleHeight;     // Высота капсулы (примерное расстояние вдоль оси Y)
	private float CapsuleRadius;   // Радиус капсулы
	private float ForwardOffset;    // Смещение вперёд от центра игрока

	public float WeaponDamage { get; private set; } = 50;

	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice, GameObject cachedPlayer, PlayerMovementController playerMovementController)
	{
		this.inputDevice = inputDevice;
		this.cachedPlayer = cachedPlayer;
		this.playerMovementController = playerMovementController;
		//this.interactionController = interactionController;


		CapsuleHeight = 1.8f;      // Высота капсулы (примерное расстояние вдоль оси Y)
		CapsuleRadius = 0.3f;      // Радиус капсулы
		ForwardOffset = 0.5f;      // Смещение вперёд от центра игрока
		IsPlayerLegKicking = false;

		Debug.Log("LegKickAttack Initialized");
	}

	/*
	private void OnDrawGizmos()
	{
		
		// Нижняя и верхняя точки капсулы
		Vector3 startPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset;
		Vector3 endPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset + cachedPlayer.transform.up * CapsuleHeight;

		// Рисуем верхнюю и нижнюю полусферу
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(startPoint, CapsuleRadius);
		Gizmos.DrawWireSphere(endPoint, CapsuleRadius);

		// Центральный соединяющий куб
		Vector3 cylinderCenter = (startPoint + endPoint) / 2;
		Quaternion rotation = Quaternion.LookRotation(Vector3.right, transform.forward);
		Gizmos.DrawWireCube(cylinderCenter, new Vector3(CapsuleRadius * 2, CapsuleHeight, CapsuleRadius * 2));
	}
	*/
	
	void Update()
	{
		

		if (inputDevice.GetKeyLegKick() && !IsPlayerLegKicking && (playerMovementController.CurrentPlayerMovementStateType == "PlayerIdle" || playerMovementController.CurrentPlayerMovementStateType == "PlayerWalking"
			|| playerMovementController.CurrentPlayerMovementStateType == "PlayerRunning" || playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" ||
			playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking"))
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

		if (playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingIdle" || playerMovementController.CurrentPlayerMovementStateType == "PlayerCrouchingWalking")
		{
			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerCrouchingIdle);
		}
		else
		{
			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerIdle);
		}

	

		StartCoroutine(playerMovementController.DisablePlayerMovementDuringLegKickAttack());
		StartCoroutine(DisableLegKickAttackActivation());

		// Нижняя и верхняя точки капсулы
		Vector3 startPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset;
		Vector3 endPoint = cachedPlayer.transform.position + cachedPlayer.transform.forward * ForwardOffset + cachedPlayer.transform.up * CapsuleHeight;

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

	
	
	}
	
	IEnumerator DisableLegKickAttackActivation()
	{
		IsPlayerLegKicking = true;
		yield return new WaitForSeconds(0.95f);
		IsPlayerLegKicking = false;
	}

	IEnumerator DelayLegKickAttackDamage(IDamageable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime); // Ждем нужную задержку

		// Наносим урон после окончания ожидания
		target.TakeDamage(damageAmount);
	}
}

