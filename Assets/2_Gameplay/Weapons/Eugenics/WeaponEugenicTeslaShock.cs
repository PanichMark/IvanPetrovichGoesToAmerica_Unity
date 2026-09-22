using UnityEngine;
using System.Collections;

public class WeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.TeslaShock;
	public override WeaponTypes WeaponType => WeaponTypes.Eugenic;
	public override int ManaCost => 5;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	public override float WeaponAttackSpeedRate => 0.4f;
	public override bool IsWeaponAuto => true;
	public float AttackRange => 2f;

	public override float TimeBetweenAbilityToAttack => 0.41f;

	protected override void InitializeWeaponEugenic()
	{
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>();


		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform;
		}

		_playerCameraStateMachineController.OnCameraStateChanged += ChangeVFXSpawnPoint;
	}


	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (true)
		{
			//Debug.Log("scvwergerg");

			if (!IsWeaponPlayerAutoAttacking)
			{
				
				//Debug.Log("STOP!!!");
				break;
			}

			StartCoroutine(SingleEugenicAttack());

			yield return new WaitForSeconds(0.8f);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				IsWeaponPlayerAutoAttacking = false;
				//Debug.Log("STOP AMAN!!!!!!");
				break;
			}
		}
		_currentWeaponPlayerAutoAttackCourutine = null;

		
	}

	protected override IEnumerator SingleEugenicAttack()
	{
	


		//Debug.Log("SINGLE BRUH!");

		_currentWeaponPlayerEugenicAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponPalmAttackAnimation(this));

		if (!_weaponAudioSource.isPlaying)
		{
			_weaponAudioSource.PlayOneShot(_weaponSoundAttack);
		}

		if (_vfxInstanceAttack == null)
		{
			StartCoroutine(ShowVFX());
		}

		_playerResourcesManaManager.UseMana(ManaCost);

		Vector3 attackOrigin = _eugenicAttackDirection.transform.position + _eugenicAttackDirection.transform.forward * 1.5f;

		Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, AttackRange);

		foreach (Collider hit in hitColliders)
		{
			IDamageable damageable = hit.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(_weaponDamage);
				Debug.Log($"[{WeaponName}] Нанесено {_weaponDamage} урона объекту: {hit.name}");
			}

			IElectroShockable electroShokable = hit.GetComponent<IElectroShockable>();
			if (electroShokable != null)
			{
				electroShokable.Electrify(_weaponDamage);
				Debug.Log($"[{WeaponName}] Электроударил {hit.name}");
			}
		}

		yield return _currentWeaponPlayerEugenicAttackRoutine;


		_isAttacking = false;
		

		_currentWeaponPlayerEugenicAttackRoutine = null;
	}

	private void OnDestroy()
	{
		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnCameraStateChanged -= ChangeVFXSpawnPoint;
		}

		if (_playerWeaponController != null)
		{
			//_playerWeaponController.OnAllWeaponsHidden -= TurnEugenicVFXOff;
		}
	}
	
	private IEnumerator ShowVFX()
	{
		_vfxInstanceAttack = Instantiate(
			_VFXeffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation * Quaternion.Euler(12, 45, 0),
			_VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_vfxInstanceAttack.layer = LayerMask.NameToLayer("FirstPerson");
		}

		_vfxInstanceAttack.transform.localScale = Vector3.one;

		while (true)
		{ 
			float endTime = Time.time + WeaponAttackSpeedRate;

			yield return new WaitUntil(() => Time.time >= endTime);

			if (!IsWeaponPlayerAutoAttacking)
			{
				TurnEugenicVFXOff();
				yield break;
			}
		}
	}

	private void ChangeVFXSpawnPoint()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform;
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform;
		}

		if (_vfxInstanceAttack != null)
		{
			TurnEugenicVFXOff();
			StartCoroutine(ShowVFX());
		}
	}

	public override void TurnEugenicVFXOff()
	{
		//Debug.Log("TURN OFF!");
		Destroy(_vfxInstanceAttack);
	}

	protected override void ChangeInspectVFXStransform()
	{
		_vfxInstanceInspectRight.transform.localScale *= 0.3f;
		_vfxInstanceInspectLeft.transform.localScale *= 0.3f;

		_vfxInstanceInspectRight.transform.localRotation *= Quaternion.Euler(0f, -90f, 0f);
		_vfxInstanceInspectLeft.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
	}
}