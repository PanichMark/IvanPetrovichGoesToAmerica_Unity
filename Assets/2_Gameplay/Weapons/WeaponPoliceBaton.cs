using UnityEngine;
using System.Collections;

public class WeaponPoliceBaton : WeaponMeleeAbstract
{
	private IInputDevice inputDevice;
	private PlayerMovementController playerMovementController;
	private PlayerWeaponController weaponController;

	private GameObject ChokeNPCtext;

	private bool isAbleToChoke = false;
	private bool npcDetected = false;
	private bool isItRightHand;

	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Baton icon");

	protected override void SetUpMeleeWeapon()
	{
		CapsuleHeight = 1.8f;
		CapsuleRadius = 0.3f;
		ForwardOffset = 0.5f;
		AttackDelay = 0.5f;

		ChokeNPCtext = ServiceLocator.Resolve<GameObject>("ChokeNPCtext");
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		if (weaponController.rightHandWeaponComponent is WeaponPoliceBaton)
		{
			isItRightHand = true;
		}
		if (weaponController.leftHandWeaponComponent is WeaponPoliceBaton)
		{
			isItRightHand = false;	
		}

		inputDevice = ServiceLocator.Resolve<IInputDevice>("inputDevice");
	}
	private void Update()
	{
		Vector3 playerPosition = AttackPoint.transform.position;
		Vector3 playerForward = AttackPoint.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * ForwardOffset;
		Vector3 endPoint = startPoint + AttackPoint.transform.up * CapsuleHeight;

		Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, CapsuleRadius);

		bool newDetection = false;
		foreach (var hit in hitColliders)
		{
			if (hit.gameObject == AttackPoint) continue;
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

		ChokeNPCtext.SetActive(isAbleToChoke);
	}

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
			if ((isItRightHand && inputDevice.GetKeyRightHandWeaponAttackReleased()) ||
				(!isItRightHand && inputDevice.GetKeyLeftHandWeaponAttackReleased()))
			{
				Debug.Log("Failed to choke!!!");
				currentChokeCoroutine = null;
				yield break; 
			}

			elapsed += Time.deltaTime;
			yield return null; 
		}

		Debug.Log("Choke SUCCESS!!!");
		currentChokeCoroutine = null;
	}

	public override void WeaponAttack()
	{
		if (isAbleToChoke)
		{
			PerformChokeAttack();
			return; 
		}

		base.WeaponAttack();
	}
}