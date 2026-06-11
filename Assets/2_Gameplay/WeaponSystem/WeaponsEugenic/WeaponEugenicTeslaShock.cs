using UnityEngine;

public class WWeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override string WeaponName => "TeslaShock";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 5;
	public override int ManaCost => 2;
	public override bool IsWeaponAuto => true;

	public float AttackRange => 2f; 

	protected override void InitializeWeaponEugenic()
	{
		_weaponAutoAttackSpeedRate = 0.2f; 
	}

	protected override void AutoEugenicAttack()
	{
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
}