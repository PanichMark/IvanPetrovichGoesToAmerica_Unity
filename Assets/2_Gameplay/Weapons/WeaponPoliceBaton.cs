using UnityEngine;
using System.Collections;

public class WeaponPoliceBaton : WeaponMeleeAbstract
{
	// Свойства оружия
	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Baton icon");

	private GameObject ChokeNPCtext;
	// Состояние окружения
	private bool npcDetected = false;
	// Ссылка на контроллер движения игрока
	private PlayerMovementController playerMovementController;
	// Новое поле: флаг возможности удушения
	private bool isAbleToChoke = false;
	private WeaponController weaponController;
	private bool isItRightHand;
	private IInputDevice inputDevice;

	protected override void SetUpMeleeWeapon()
	{
		CapsuleHeight = 1.8f;
		CapsuleRadius = 0.3f;
		ForwardOffset = 0.5f;
		AttackDelay = 0.5f;

		ChokeNPCtext = ServiceLocator.Resolve<GameObject>("ChokeNPCtext");
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		weaponController = ServiceLocator.Resolve<WeaponController>("WeaponController");

		if (weaponController.rightHandWeaponComponent is WeaponPoliceBaton)
		{
			//Debug.Log("PoliceBaton is RIGHT");
			isItRightHand = true;
		}
		if (weaponController.leftHandWeaponComponent is WeaponPoliceBaton)
		{
			//Debug.Log("PoliceBaton is LEFT");
			isItRightHand = false;	
		}

		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
	}



	private void Update()
	{
	

		// --- ЛОГИКА ОБНАРУЖЕНИЯ NPC ---
		Vector3 playerPosition = player.transform.position;
		Vector3 playerForward = player.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * ForwardOffset;
		Vector3 endPoint = startPoint + player.transform.up * CapsuleHeight;

		Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, CapsuleRadius);

		bool newDetection = false;
		foreach (var hit in hitColliders)
		{
			if (hit.gameObject == player) continue;
			if (hit.GetComponent<NPCAbstract>() != null)
			{
				newDetection = true;
				break;
			}
		}
		npcDetected = newDetection;



		bool isCrouching = playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingIdle") ||
						   playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingWalking");

		isAbleToChoke = npcDetected && isCrouching;


	
		
		ChokeNPCtext.SetActive(isAbleToChoke); // Показываем текст только если можно душить
	}

	// НОВЫЙ МЕТОД для удушения
	private Coroutine currentChokeCoroutine = null;

	private void PerformChokeAttack()
	{
		if (currentChokeCoroutine != null)
			StopCoroutine(currentChokeCoroutine);

		currentChokeCoroutine = StartCoroutine(ChokeRoutine());
	}

	private IEnumerator ChokeRoutine()
	{
		Debug.Log("START choke!");
		float chokeDuration = 2f;
		float elapsed = 0f;

		while (elapsed < chokeDuration)
		{
			// Проверяем, не отпущена ли кнопка атаки
			if ((isItRightHand && inputDevice.GetKeyRightHandWeaponAttackReleased()) ||
				(!isItRightHand && inputDevice.GetKeyLeftHandWeaponAttackReleased()))
			{
				Debug.Log("Failed to choke!!!");
				currentChokeCoroutine = null;
				yield break; // Прерываем корутину
			}

			elapsed += Time.deltaTime;
			yield return null; // Ждём следующего кадра
		}

		// Если дошли сюда — удушение успешно
		Debug.Log("Choke SUCCESS!!!");
		currentChokeCoroutine = null;
	}

	// ПЕРЕОПРЕДЕЛЯЕМ базовый метод атаки
	public override void WeaponAttack()
	{
		// Если условия для удушения выполнены - используем спец. атаку
		if (isAbleToChoke)
		{
			PerformChokeAttack();
			return; // Выходим, чтобы не вызывать базовую атаку дубинкой
		}

		// Если условия не выполнены, вызываем обычную атаку из базового класса
		base.WeaponAttack();
	}
}