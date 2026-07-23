using UnityEngine;
using System.Collections;

public class WeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override WeaponNames WeaponName => WeaponNames.TeslaShock;
	public override WeaponTypes WeaponType => WeaponTypes.Eugenic;
	public override float WeaponDamage => 5;
	public override int ManaCost => 0;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	public override float WeaponAttackSpeedRate => 0.4f;
	public override bool IsWeaponAuto => true;
	private GameObject _VFXteslaShock;
	private Transform _VFXspawnPoint;
	private GameObject _vfxInstance;
	public float AttackRange => 2f;

	protected override void InitializeWeaponEugenic()
	{
		_VFXteslaShock = Resources.Load<GameObject>($"VFXs/VFX_EugenicTeslaShock/3Dmodel_VFX_TeslaShock");
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");


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
			if (!_isWeaponPlayerAutoAttacking)
			{
				break;
			}

			StartCoroutine(SingleEugenicAttack());

			yield return new WaitForSeconds(0.8f);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				_isWeaponPlayerAutoAttacking = false;
				break;
			}
		}
		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected override IEnumerator SingleEugenicAttack()
	{
		Debug.Log("SINGLE BRUH!");

		_currentWeaponPlayerEugenicAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponPalmAttackAnimation(this));

		if (_vfxInstance == null)
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
				damageable.TakeDamage(WeaponDamage);
				Debug.Log($"[{WeaponName}] Нанесено {WeaponDamage} урона объекту: {hit.name}");
			}

			IElectroShockable electroShokable = hit.GetComponent<IElectroShockable>();
			if (electroShokable != null)
			{
				electroShokable.Electrify(WeaponDamage);
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
		_vfxInstance = Instantiate(
			_VFXteslaShock,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation * Quaternion.Euler(0, 0, 0),
			_VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		_vfxInstance.transform.localScale = Vector3.one;

		while (true)
		{ 
			float endTime = Time.time + WeaponAttackSpeedRate;

			yield return new WaitUntil(() => Time.time >= endTime);

			if (!_isWeaponPlayerAutoAttacking)
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

		if (_vfxInstance != null)
		{
			TurnEugenicVFXOff();
			StartCoroutine(ShowVFX());
		}
	}

	public override void TurnEugenicVFXOff()
	{
		Debug.Log("TURN OFF!");
		Destroy(_vfxInstance);
	}
}