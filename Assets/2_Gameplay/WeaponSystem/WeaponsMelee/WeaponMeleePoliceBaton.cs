using UnityEngine;
using System.Collections;

public class WeaponMeleePoliceBaton : WeaponMeleeAbstract
{
	public override WeaponNames WeaponName => WeaponNames.Baton;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override float WeaponDamage => 45f;
	public override bool IsWeaponAuto => false;
	public override float WeaponAttackSpeedRate => 1.560f;

	public override float MeleeAttackDelay => 0.840f;

	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerWeaponController _weaponController;

	private Coroutine currentChokeCoroutine = null;

	private GameObject _chokeNPCtext;

	private bool _isAbleToChoke = false;
	private bool _npcDetected = false;
	private bool _isItRightHand;

	protected override void InitializeWeaponMelee()
	{
		_inputDevice = ServiceLocator.Resolve<IInputDevice>("InputDevice");
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>("PlayerMovementStateMachineController");
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		_chokeNPCtext = ServiceLocator.Resolve<GameObject>("TextChokeNPC");

		if (_weaponController.RightHandWeaponComponent is WeaponMeleePoliceBaton)
		{
			_isItRightHand = true;
		}
		if (_weaponController.LeftHandWeaponComponent is WeaponMeleePoliceBaton)
		{
			_isItRightHand = false;	
		}

		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
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

	private void Update()
	{
		if (!_isWeaponInitialized)
			return;

		Vector3 playerPosition = _attackPoint.transform.position;
		Vector3 playerForward = _attackPoint.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, _capsuleRadius);

		bool newDetection = false;
		foreach (var hit in hitColliders)
		{
			if (hit.gameObject == _attackPoint) continue;
			if (hit.GetComponent<NPCAbstract>() != null)
			{
				newDetection = true;
				break;
			}
		}
		_npcDetected = newDetection;

		bool isCrouching = _playerMovementStateMachineController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingIdle") ||
						   _playerMovementStateMachineController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingWalking");

		_isAbleToChoke = _npcDetected && isCrouching;

		_chokeNPCtext.SetActive(_isAbleToChoke);
	}

	private void PerformChokeAttack()
	{
		if (currentChokeCoroutine != null)
		{
			StopCoroutine(currentChokeCoroutine);
		}

		currentChokeCoroutine = StartCoroutine(ChokeCoroutine());
	}

	private IEnumerator ChokeCoroutine()
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
}