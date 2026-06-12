using UnityEngine;

public class WeaponRangedSawedOffShotgun : WeaponRangedAbstract
{
	public override string WeaponName => "SawedOffShotgun";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo12gauge;
	public override float WeaponDamage => 100f;
	public override bool IsWeaponAuto => false;
	protected override void InitializeWeaponRanged()
	{
		
	}

	public override void WeaponAttack()
	{
		if (PlayerMagazineAmmoCurrent > 0)
		{
			if (IsWeaponAuto)
			{
				StartAutoAttacking();
			}
			else
			{
				ShootPlayerWeapon(WeaponDamage);
			}
		}
		else if (_isThisPlayerWeapon)
		{
			Debug.Log($"Not enough Ammo {WeaponName}");
		}
	}

	protected override void ShootPlayerWeapon(float weaponDamage)
	{
		RaycastHit hitInfo;
		IDamageable damageable = null;

		if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hitInfo, 100f))
		{
			damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponDamage);
			}

			IBreakable breakable = hitInfo.transform.GetComponent<IBreakable>();
			if (breakable != null)
			{
				breakable.TakeDamage(weaponDamage);
			}
		}

		// Проверяем, есть ли вообще коллайдер и трансформ у объекта
		if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 11)
		{
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

			// Добавляем четвертый параметр - Transform родителя
			// Мы всегда хотим, чтобы след был дочерним объектом
			_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
		}

		PlayerMagazineAmmoCurrent--;
		Debug.Log($"Shoot {WeaponName}");

		if (System.Enum.TryParse(WeaponName, out WeaponsRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		WeaponRangedRecoil();
	}

	protected override void WeaponRangedRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(15, 0.05f, 0.5f);
	}
}