using UnityEngine;

public class WeaponEugenicGenieBreath : EugenicWeaponAbstract
{
	private float _attackRange = 5f;
	private float _knockbackForce = 10f;
	private int _damageAmount = 100;
	public override string WeaponNameSystem => "EugenicGenie";
	public override string WeaponNameUI => "Дыхание Джинна";

	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponEugenicGenieBreathIcon");

	protected override void InitializeWeaponEugenic()
	{
		ManaCost = 10;
	}

	public override void WeaponAttack()
	{
		if (IsThisPlayerWeapon == true)
		{

			if (playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
			{
				playerResourcesManaManager.UseMana(ManaCost);

				Vector3 attackOrigin = player.transform.position + player.transform.forward * 1.5f;

				Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, _attackRange);

				foreach (Collider hit in hitColliders)
				{
					IDamageable damageable = hit.GetComponent<IDamageable>();
					if (damageable != null)
					{
						damageable.TakeDamage(_damageAmount);
						Debug.Log($"Нанесено {_damageAmount} урона объекту: {hit.name}");
					}
				}

				foreach (Collider hit in hitColliders)
				{
					Rigidbody rb = hit.GetComponent<Rigidbody>();
					if (rb != null && !rb.isKinematic)
					{
						Vector3 knockbackDirection = camera.transform.forward.normalized;

						rb.AddForce(knockbackDirection * _knockbackForce, ForceMode.Impulse);
						Debug.Log($"Отброшен Rigidbody: {hit.name}");
					}
				}
			}
			else
			{
				Debug.Log($"Not enough mana for {WeaponNameSystem} attack");
			}
		}
	}
}