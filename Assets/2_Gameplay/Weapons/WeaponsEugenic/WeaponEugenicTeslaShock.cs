using UnityEngine;

public class WWeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public float AttackRange => 2f; // Радиус зоны поражения - 2 метра

	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "TeslaShock";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponWheel/WeaponWheel_WeaponIcons/Weapon{WeaponType}{WeaponName}Icon");
	public override bool IsWeaponAuto => true;

	public override float WeaponDamage => 50;
	public override int ManaCost => 5;

	protected override void InitializeWeaponEugenic()
	{
		// Здесь задаем скорость именно для этого оружия
		_weaponAutoAttackSpeedRate = 0.1f; // Атаковать каждые 0.3 секунды
	}

	// Этот метод теперь НЕ ОБЯЗАТЕЛЕН для данного класса!
	// Но мы можем его реализовать, если захотим дать TeslaShock
	// возможность стрелять и одиночными выстрелами по ЛКМ.
	// Если он останется пустым, ничего страшного не произойдет.
	protected override void PerformSingleEugenicAttack()
	{
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
		}
	}
}