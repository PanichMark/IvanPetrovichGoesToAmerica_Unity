using UnityEngine;

public class WeaponEugenicGenieBreath : WeaponEugenicAbstract
{
	public override string WeaponName => "GenieBreath";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 100;
	public override int ManaCost =>	20;
	public override bool IsWeaponAuto => false;

	private float _eugenicAttackRange = 5f;
	private float _eugenicGenieBreathKnockbackForce = 10f;

	protected override void InitializeWeaponEugenic()
	{

	}

	protected override void SingleEugenicAttack()
	{
		if (_isThisPlayerWeapon == true)
		{
			_playerResourcesManaManager.UseMana(ManaCost);

			Vector3 attackOrigin = _eugenicAttackDirection.transform.position + _eugenicAttackDirection.transform.forward * 1.5f;

			Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, _eugenicAttackRange);

			foreach (Collider hit in hitColliders)
			{
				IDamageable damageable = hit.GetComponent<IDamageable>();
				if (damageable != null)
				{
					damageable.TakeDamage(WeaponDamage);
					Debug.Log($"Нанесено {WeaponDamage} урона объекту: {hit.name}");
				}

				IBreakable breakable = hit.GetComponent<IBreakable>();
				if (breakable != null)
				{
					breakable.TakeDamage(WeaponDamage);
					Debug.Log($"Нанесено {WeaponDamage} урона объекту: {hit.name}");
				}
			}

			foreach (Collider hit in hitColliders)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb != null && !rb.isKinematic)
				{
					Vector3 knockbackDirection = _eugenicSourcePoint.transform.forward.normalized;

					rb.AddForce(knockbackDirection * _eugenicGenieBreathKnockbackForce, ForceMode.Impulse);
					Debug.Log($"Отброшен Rigidbody: {hit.name}");
				}
			}
		}
	}

	public override void TurnEugenicVFXOff()
	{
		throw new System.NotImplementedException();
	}
}