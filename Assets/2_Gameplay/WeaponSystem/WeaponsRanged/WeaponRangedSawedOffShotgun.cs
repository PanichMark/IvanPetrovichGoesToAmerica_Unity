using UnityEngine;

public class WeaponRangedSawedOffShotgun : WeaponRangedAbstract
{
	public override string WeaponName => "SawedOffShotgun";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";

	public override WeaponsRangedEnum RangedWeaponType => WeaponsRangedEnum.SawedOffShotgun;

	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponSystem/WeaponWheel/Weapon{WeaponType}{WeaponName}Icon");
	public override AmmoTypes PlayerWeaponAmmoType => AmmoTypes.Ammo12gauge;
	public override float WeaponDamage => 100f;
	public override float _waitForAmmoRefill => 1;
	public override bool IsWeaponAuto => false;
	protected override void InitializeWeaponRanged()
	{
		_VFXshottEffect = Resources.Load<GameObject>($"VFXs/VFX_MuzzleFlash");
	}

	protected override void ShootWeaponPlayer(float weaponDamage)
	{
		int pelletCount = 10;
		float spreadAngle = 7f;
		float range = 100f;

		_vfxInstance = Instantiate(
			_VFXshottEffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation,
			_VFXspawnPoint.transform);
		_vfxInstance.transform.localScale = Vector3.one * 2.5f;

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		Destroy(_vfxInstance, 0.05f);

		for (int i = 0; i < pelletCount; i++)
		{
			RaycastHit hitInfo;

			Quaternion randomRotation = Random.rotationUniform;
			Quaternion spreadRotation = Quaternion.Slerp(Quaternion.identity, randomRotation, spreadAngle / 90f);
			Vector3 finalDirection = spreadRotation * _shootPoint.transform.forward;

			Color rayColor = Physics.Raycast(_shootPoint.transform.position, finalDirection, out hitInfo, range) ? Color.red : Color.yellow;
			Debug.DrawRay(_shootPoint.transform.position, finalDirection * range, rayColor, 2f);

			if (hitInfo.collider != null)
			{
				IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
				if (damageable != null && hitInfo.transform.gameObject.layer != 9)
				{
					damageable.TakeDamage(weaponDamage / pelletCount);
				}

				IBreakable breakable = hitInfo.transform.GetComponent<IBreakable>();
				if (breakable != null)
				{
					breakable.TakeDamage(weaponDamage / pelletCount);
				}

				if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
				{
					Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
					_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
				}
			}
		}

		PlayerMagazineAmmoCurrent--;
		Debug.Log($"Shoot {WeaponName}");

		if (System.Enum.TryParse(WeaponName, out WeaponsRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		ApplyWeaponRecoil();
	}

	protected override void ApplyWeaponRecoil()
	{
		_playerCameraController.ApplyWeaponRecoilSingle(15, 0.05f, 0.5f);
	}
}