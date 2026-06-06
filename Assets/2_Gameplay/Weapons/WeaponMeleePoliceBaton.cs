using UnityEngine;
using System.Collections;

public class WeaponMeleePoliceBaton : WeaponMeleeAbstract
{
	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerWeaponController _weaponController;

	private GameObject _chokeNPCtext;

	private bool _isAbleToChoke = false;
	private bool _npcDetected = false;
	private bool _isItRightHand;
	public override bool IsWeaponAuto => false;

	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "PoliceBaton";

	public override string WeaponType => WeaponTypes.Melee.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponMeleePoliceBatonIcon");

	protected override void InitializeWeaponMelee()
	{
		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
		_attackDelay = 0.5f;

		_chokeNPCtext = ServiceLocator.Resolve<GameObject>("TextChokeNPC");
		_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>("PlayerMovementStateMachineController");
		_weaponController = ServiceLocator.Resolve<PlayerWeaponController>("WeaponController");

		if (_weaponController.RightHandWeaponComponent is WeaponMeleePoliceBaton)
		{
			_isItRightHand = true;
		}
		if (_weaponController.LeftHandWeaponComponent is WeaponMeleePoliceBaton)
		{
			_isItRightHand = false;	
		}

		_inputDevice = ServiceLocator.Resolve<IInputDevice>("InputDevice");
	}
	private void Update()
	{
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