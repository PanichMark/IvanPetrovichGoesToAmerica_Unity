using UnityEngine;
using System.Collections;

public class WeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override string WeaponName => "TeslaShock";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 5;
	public override int ManaCost => 0;
	public override bool IsWeaponAuto => true;
	private GameObject _VFXteslaShock;
	private Transform _VFXspawnPoint;
	private GameObject _vfxInstance;

	public float AttackRange => 2f;

	protected override void InitializeWeaponEugenic()
	{
		_weaponAutoAttackSpeedRate = 0.33f;
		_VFXteslaShock = Resources.Load<GameObject>($"VFXs/VFX_EugenicTeslaShock/3Dmodel_VFX_TeslaShock");

		//Debug.Log(_isThisWeaponRight);
		//Debug.Log(_firstPersonRightHandWeaponSlotTransform);

		if (_weaponHandType == WeaponHandsEnum.HandRight.ToString())
		{
			_VFXspawnPoint = _firstPersonRightHandWeaponSlotTransform;
			//Debug.Log(_firstPersonRightHandWeaponSlotTransform);
		}
		else
		{
			_VFXspawnPoint = _firstPersonLeftHandWeaponSlotTransform;
			//Debug.Log(_firstPersonLeftHandWeaponSlotTransform);
		}
	}

	protected override void AutoEugenicAttack()
	{
		//Debug.Log("--- Отладка координат спавна VFX ---");
		//Debug.Log($"X: {_VFXspawnPoint.position.x}");
		//Debug.Log($"Y: {_VFXspawnPoint.position.y}");
		//Debug.Log($"Z: {_VFXspawnPoint.position.z}");
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
	}

	private IEnumerator ShowVFX()
	{
		_vfxInstance = Instantiate(
			_VFXteslaShock,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation * Quaternion.Euler(90, 0, 0),
			_VFXspawnPoint);

		_vfxInstance.transform.localScale = Vector3.one;

		while (true)
		{ 
			float endTime = Time.time + _weaponAutoAttackSpeedRate;

			yield return new WaitUntil(() => Time.time >= endTime);

			if (!_isWeaponAutoAttacking)
			{
				TurnEugenicVFXOff();
				yield break;
			}
		}
	}

	public override void TurnEugenicVFXOff()
	{
		Debug.Log("TURN OFF!");
		Destroy(_vfxInstance);
	}
}