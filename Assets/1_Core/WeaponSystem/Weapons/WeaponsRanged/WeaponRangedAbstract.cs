using System.Collections;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	protected PlayerResourcesAmmoManager _playerResourcesAmmoManager;

	protected GameObject _shootPoint;
	protected PlayerCameraStateMachineController _playerCameraStateMachineController;
	public abstract AmmoTypes PlayerWeaponAmmoType { get; }
	protected GameObject _VFXshottEffect;
	protected Transform _VFXspawnPoint;
	protected GameObject _vfxInstance;
	public int PlayerMagazineAmmoCurrent { get; set; }
	public int PlayerMagazineAmmoMax { get; protected set; }
	
	public int PlayerAmmoTotalCurrent => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoCurrent;
	public int PlayerAmmoTotalMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoMax;
	protected BulletHoleManager _bulletHoleManager;
	protected PlayerCameraController _playerCameraController;

	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon)
		{
			_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");
			_shootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
			_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");
			_bulletHoleManager = ServiceLocator.Resolve<BulletHoleManager>("BulletHoleManager");

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

		//Debug.Log(_vfxInstance.transform.position);

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

		// Проверяем, есть ли вообще коллайдер и трансформ у объекта
		if ((hitInfo.collider.CompareTag("Untagged") || hitInfo.collider.CompareTag("Interactable")) && hitInfo.transform.gameObject.layer != 9 && hitInfo.transform.gameObject.layer != 11)
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

		ApplyWeaponRangedRecoil();
	}

	public void ReloadPlayerWeapon()
	{
		if (PlayerMagazineAmmoCurrent >= PlayerMagazineAmmoMax)
		{
			Debug.Log("Magazine is already full");
			return;
		}

		int reserve = PlayerAmmoTotalCurrent;

		if (reserve <= 0)
		{
			Debug.Log("Not enough Ammo to reload");
			return;
		}

		int ammoToAdd = Mathf.Min(reserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);

		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];
		data.TotalAmmoCurrent -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;

		PlayerMagazineAmmoCurrent += ammoToAdd;


		if (System.Enum.TryParse(WeaponName, out WeaponsRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.TotalAmmoCurrent);

			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		Debug.Log("Reloaded");
	}

	public void SetPlayerMagazineProperties(int maxAmmo, int currentAmmo)
	{
		PlayerMagazineAmmoMax = maxAmmo;
		PlayerMagazineAmmoCurrent = currentAmmo;
	}

	public void Reload()
	{
		if (_isThisPlayerWeapon)
		{
			ReloadPlayerWeapon();
		}
	}

	public void ShootNPCweapon()
	{
		// TODO
	}

	public void ReloadNPCweapon()
	{
		// TODO
	}

	protected abstract void InitializeWeaponRanged();
	protected abstract void ApplyWeaponRangedRecoil();
}