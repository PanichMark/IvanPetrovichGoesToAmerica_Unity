using UnityEngine;
using System.Collections;

public class WeaponRangedBergmannBayard : WeaponRangedAbstract
{
	public override string WeaponName => "BergmannBayard";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo9mm;
	public override float WeaponDamage => 20f;
	public override bool IsWeaponAuto => true;

	private PlayerCameraController _playerCameraController;

	protected override void InitializeWeaponRanged()
	{
		_weaponAutoAttackSpeedRate = 0.1f;
		_playerCameraController =  ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");
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

	public override void StartAutoAttacking()
	{
		if (_isWeaponAutoAttacking || PlayerMagazineAmmoCurrent <= 0) return;
		_isWeaponAutoAttacking = true;
		if (_weaponAutoAttackCourutine == null)
		{
			_weaponAutoAttackCourutine = StartCoroutine(AutoAttackCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponAutoAttacking = false;
		if (_weaponAutoAttackCourutine != null)
		{
			StopCoroutine(_weaponAutoAttackCourutine);
			_weaponAutoAttackCourutine = null;
		}
	}

	public override IEnumerator AutoAttackCourutine()
	{
		while (true)
		{
			if (!_isWeaponAutoAttacking)
			{
				break;
			}

			_playerCameraController.ApplyRecoil();
			ShootPlayerWeapon(WeaponDamage);

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);

			if (PlayerMagazineAmmoCurrent <= 0)
			{
				_isWeaponAutoAttacking = false;
				break;
			}
		}
		_weaponAutoAttackCourutine = null;
	}
}