using UnityEngine;
using System.Collections;

public class WeaponPoliceBaton : WeaponMeleeAbstract
{
	private IInputDevice _inputDevice;
	private PlayerMovementController _playerMovementController;
	private PlayerWeaponController _weaponController;

	private GameObject _chokeNPCtext;

	private bool _isAbleToChoke = false;
	private bool _npcDetected = false;
	private bool _isItRightHand;

	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponPoliceBatonIcon");

	protected override void SetUpMeleeWeapon()
	{
		CapsuleHeight = 1.8f;
		CapsuleRadius = 0.3f;
		ForwardOffset = 0.5f;
		AttackDelay = 0.5f;

		_chokeNPCtext = ServiceLocator.Resolve<GameObject>("TextChokeNPC");
		_playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		if (_weaponController.rightHandWeaponComponent is WeaponPoliceBaton)
		{
			_isItRightHand = true;
		}
		if (_weaponController.leftHandWeaponComponent is WeaponPoliceBaton)
		{
			_isItRightHand = false;	
		}

		_inputDevice = ServiceLocator.Resolve<IInputDevice>("InputDevice");
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
		_npcDetected = newDetection;

		bool isCrouching = _playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingIdle") ||
						   _playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingWalking");

		_isAbleToChoke = _npcDetected && isCrouching;

		_chokeNPCtext.SetActive(_isAbleToChoke);
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
			if ((_isItRightHand && _inputDevice.GetKeyRightHandWeaponAttackReleased()) ||
				(!_isItRightHand && _inputDevice.GetKeyLeftHandWeaponAttackReleased()))
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
		if (_isAbleToChoke)
		{
			PerformChokeAttack();
			return; 
		}

		base.WeaponAttack();
	}
}