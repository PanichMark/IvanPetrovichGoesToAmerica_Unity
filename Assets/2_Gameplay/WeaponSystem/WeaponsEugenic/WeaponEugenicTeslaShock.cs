using UnityEngine;
using System.Collections;

public class WWeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override string WeaponName => "TeslaShock";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 5;
	public override int ManaCost => 2;
	public override bool IsWeaponAuto => true;
	private GameObject _VFXteslaShock;
	private Transform _VFXspawnPoint;

	public float AttackRange => 2f;

	protected override void InitializeWeaponEugenic()
	{
		_weaponAutoAttackSpeedRate = 0.2f;
		_VFXteslaShock = Resources.Load<GameObject>($"VFXs/VFX_EugenicTeslaShock/3Dmodel_VFX_TeslaShock");

		//Debug.Log(_isThisWeaponRight);
		//Debug.Log(_firstPersonRightHandWeaponSlotTransform);

		if (_isThisWeaponRight)
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
		Debug.Log("--- Отладка координат спавна VFX ---");
		Debug.Log($"X: {_VFXspawnPoint.position.x}");
		Debug.Log($"Y: {_VFXspawnPoint.position.y}");
		Debug.Log($"Z: {_VFXspawnPoint.position.z}");

		StartCoroutine(ShowVFX());

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
		GameObject vfxInstance = Instantiate(
	 _VFXteslaShock,
	 _VFXspawnPoint.position,
	 _VFXspawnPoint.rotation,
	 _VFXspawnPoint // <-- Указываем родителя прямо здесь
 );
		//Debug.Log(_VFXspawnPoint.transform);
		vfxInstance.transform.SetParent(_VFXspawnPoint, true);
		yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);
		//Destroy(vfxInstance);
	}
}