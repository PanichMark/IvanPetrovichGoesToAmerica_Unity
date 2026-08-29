using UnityEngine;
using System.Collections;

public class WeaponMeleeBaton : WeaponMeleeAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Baton;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override float WeaponDamage => 45f;
	public override bool IsWeaponAuto => false;
	public override float WeaponAttackSpeedRate => 1.560f;
	[SerializeField] private AudioClip _weaponSoundSwing;
	public override float MeleeAttackDelay => 0.840f;

	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	private IInputDevice _inputDevice;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerWeaponController _weaponController;

	private Coroutine currentChokeCoroutine = null;

	private GameObject _chokeNPCtext;

	private bool _isAbleToChoke = false;
	private bool _npcDetected = false;
	private bool _isItRightHand;

	private ViewModelHUDInteraction _viewModelHUDInteraction;

	protected override void InitializeWeaponMelee()
	{
		_viewModelHUDInteraction = ServiceLocator.Resolve<ViewModelHUDInteraction>();
		_inputDevice = ServiceLocator.Resolve<IInputDevice>();
_playerMovementStateMachineController = ServiceLocator.Resolve<PlayerMovementStateMachineController>();
_weaponController = ServiceLocator.Resolve<PlayerWeaponController>();

_chokeNPCtext = _viewModelHUDInteraction.TextChokeNPC;

		if (_weaponController.RightHandWeaponComponent is WeaponMeleeBaton)
		{
			_isItRightHand = true;
		}
		if (_weaponController.LeftHandWeaponComponent is WeaponMeleeBaton)
		{
			_isItRightHand = false;	
		}

		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
	}

	public override void WeaponAttack()
	{
		
		if (_isAttacking)
		{
			Debug.Log("Already attacking melee");
			return;
		}

		if (_isAbleToChoke)
		{
			_isAttacking = true;
			PerformChokeAttack();
			return;
		}

		_isAttacking = true;
		StartCoroutine(SingleMeleeWeaponAttack());
		
	}

	protected override IEnumerator SingleMeleeWeaponAttack()
	{
		StartCoroutine(DelayAttackSound());

		_currentWeaponPlayerMeleeAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this, true));

		Vector3 startPoint = _attackPoint.transform.position + _attackPoint.transform.forward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _capsuleRadius, _attackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == _attackPoint)
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayMeleeAttackDamageable(damageable, MeleeAttackDelay));
			}

			if (hit.collider.TryGetComponent<IBreakable>(out var breakable))
			{
				StartCoroutine(DelayMeleeAttackBreakable(breakable, MeleeAttackDelay));
			}
		}

		yield return _currentWeaponPlayerMeleeAttackRoutine;

		_isAttacking = false;

		_currentWeaponPlayerMeleeAttackRoutine = null;
	}

	protected override IEnumerator DelayAttackSound()
	{
		Debug.Log("SOUND ON HIT");
		yield return new WaitForSeconds(MeleeAttackDelay - 0.35f);
		_weaponAudioSource.PlayOneShot(_weaponSoundSwing);
		yield return new WaitForSeconds(0.18f);

		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		yield return null;
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
			if (hit.GetComponent<NPCabstract>() != null)
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