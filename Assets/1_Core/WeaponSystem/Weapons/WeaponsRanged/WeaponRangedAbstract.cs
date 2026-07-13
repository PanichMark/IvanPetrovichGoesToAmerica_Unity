using System.Collections;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	protected PlayerResourcesAmmoManager _playerResourcesAmmoManager;

	protected GameObject _shootPoint;
	protected PlayerCameraStateMachineController _playerCameraStateMachineController;
	public abstract AmmoTypes PlayerWeaponAmmoType { get; }

	public abstract WeaponsRangedEnum RangedWeaponType { get; }

	protected GameObject _VFXshottEffect;
	protected Transform _VFXspawnPoint;
	protected GameObject _vfxInstance;
	public int PlayerMagazineAmmoCurrent { get; set; }
	public int PlayerMagazineAmmoMax { get; protected set; }
	
	public int PlayerAmmoReserve => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoReserve;
	public int PlayerAmmoMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoMax;
	protected BulletHoleManager _bulletHoleManager;
	protected PlayerCameraController _playerCameraController;
	protected bool _isReloading;
	protected WeaponAnimationController _weaponAnimationController;

	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon)
		{
			_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
			_shootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
			_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");
			
			_playerCameraStateMachineController.OnCameraStateChanged += ChangeVFXSpawnPoint;
			InitializeWeaponRanged();
		}

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform.Find("VFX");
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform.Find("VFX");
		}

		_bulletHoleManager = ServiceLocator.Resolve<BulletHoleManager>("BulletHoleManager");
		_weaponAnimationController = ServiceLocator.Resolve<WeaponAnimationController>("WeaponAnimationController");
	}

	private void OnDestroy()
	{
		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnCameraStateChanged -= ChangeVFXSpawnPoint;
		}
	}

	private void ChangeVFXSpawnPoint()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform.Find("VFX");
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson.ToString())
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform.Find("VFX");
		}
	}

	public override void WeaponAttack()
	{
		if (PlayerMagazineAmmoCurrent > 0)
		{
			if (!_isReloading)
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
			else
			{
				Debug.Log($"Can't shoot as Reloading!");
			}
		}
		else if (_isThisPlayerWeapon)
		{
			Debug.Log($"Not enough Ammo for {WeaponName}");
		}
	}

	public override void StartAutoAttacking()
	{
		if (_isWeaponAutoAttacking || PlayerMagazineAmmoCurrent <= 0)
			return;

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

			ShootPlayerWeapon(WeaponDamage);

			ApplyWeaponRangedRecoil();

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);

			if (PlayerMagazineAmmoCurrent <= 0)
			{
				_isWeaponAutoAttacking = false;
				break;
			}
		}

		_weaponAutoAttackCourutine = null;
	}

	protected virtual void ShootPlayerWeapon(float weaponDamage)
	{
		RaycastHit hitInfo;
		IDamageable damageable = null;

		_vfxInstance = Instantiate(
			_VFXshottEffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation,
			_VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson.ToString())
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		Destroy(_vfxInstance, 0.05f);


		if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hitInfo, 100f))
		{
			damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null && hitInfo.transform.gameObject.layer != 9)
			{
				Debug.Log($"Попадание в объект: {hitInfo.transform.name}, Слой: {hitInfo.transform.gameObject.layer}");
				damageable.TakeDamage(weaponDamage);
			}
		}

		if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
		{
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

			_bulletHoleManager.SpawnDecal(hitInfo.point, rot, damageable != null, hitInfo.transform);
		}
		
		PlayerMagazineAmmoCurrent--;

		Debug.Log($"Shoot {WeaponName}");

		if (System.Enum.TryParse(WeaponName, out WeaponsRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		ApplyWeaponRangedRecoil();
	}

	protected virtual IEnumerator ReloadPlayerWeapon()
	{
		if (PlayerMagazineAmmoCurrent >= PlayerMagazineAmmoMax)
		{
			Debug.Log("Magazine is already full");
			yield break;
		}

		if (PlayerAmmoReserve <= 0)
		{
			Debug.Log("Not enough Ammo to reload");
			yield break;
		}

		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);

		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		_weaponAnimationController.PrepareReloadAnimation(RangedWeaponType);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;

		PlayerMagazineAmmoCurrent += ammoToAdd;

		if (System.Enum.TryParse(WeaponName, out WeaponsRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);

			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		_isReloading = false;

		yield return null;

		Debug.Log("Reloaded");
	}

	public void SetPlayerMagazineProperties(int maxAmmo, int currentAmmo)
	{
		PlayerMagazineAmmoMax = maxAmmo;
		PlayerMagazineAmmoCurrent = currentAmmo;
	}



	public void Reload()
	{
		_isReloading = true;

		if (_isThisPlayerWeapon)
		{
			StartCoroutine(ReloadPlayerWeapon());
		}
		else
		{
			StartCoroutine(ReloadNPCweapon());
		}
	}

	public void ShootNPCweapon()
	{
	}

	public IEnumerator ReloadNPCweapon()
	{
		yield return null;
	}

	protected abstract void InitializeWeaponRanged();

	protected abstract void ApplyWeaponRangedRecoil();
}