using System.Collections;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	protected PlayerResourcesAmmoManager _playerResourcesAmmoManager;

	protected GameObject _shootPoint;
	protected PlayerCameraStateMachineController _playerCameraStateMachineController;
	protected Coroutine _currentWeaponPlayerShootRoutine;

	protected abstract float _waitForAmmoRefill { get; }
	public abstract AmmoTypes PlayerWeaponAmmoType { get; }
	protected bool _isWeaponPlayerShooting;
	public abstract bool IsReloadingAnimationSingle { get; }
	protected GameObject _VFXshottEffect;
	protected Transform _VFXspawnPoint;
	protected GameObject _vfxInstance;
	protected bool _isNPCreloading;
	public int PlayerMagazineAmmoCurrent { get; set; }

	public int PlayerMagazineAmmoMax { get; protected set; }
	
	public int PlayerAmmoReserve => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoReserve;
	public int PlayerAmmoMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoMax;
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
			
			_playerCameraStateMachineController.OnCameraStateChanged += ChangeVFXspawnPoint;
			InitializeWeaponRanged();
		}

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform.Find("VFX");
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform.Find("VFX");
		}

		_bulletHoleManager = ServiceLocator.Resolve<BulletHoleManager>("BulletHoleManager");
	}

	private void OnDestroy()
	{
		if (_playerCameraStateMachineController != null)
		{
			_playerCameraStateMachineController.OnCameraStateChanged -= ChangeVFXspawnPoint;
		}
	}

	private void ChangeVFXspawnPoint()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_VFXspawnPoint = FirstPersonWeaponModelInstance.transform.Find("VFX");
		}
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			_VFXspawnPoint = ThirdPersonWeaponModelInstance.transform.Find("VFX");
		}
	}

	public override void WeaponAttack()
	{
		if (_isThisPlayerWeapon)
		{
			if (_playerWeaponAnimationController.IsPlayerReloading)
			{
				Debug.Log("Can't shoot during reload");
				return;
			}
			if (PlayerMagazineAmmoCurrent == 0)
			{
				Debug.Log("Magazine empty!");
				return;
			}

			if (IsWeaponAuto)
			{
				StartAutoAttackingWeaponPlayer();
			}
			else
			{
				StartCoroutine(ShootWeaponPlayer(WeaponDamage));
			}
		}
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (_isWeaponPlayerAutoShooting || PlayerMagazineAmmoCurrent <= 0)
			return;

		_isWeaponPlayerAutoShooting = true;

		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponPlayerAutoShooting = false;

		if (_currentWeaponPlayerAutoAttackCourutine != null)
		{
			StopCoroutine(_currentWeaponPlayerAutoAttackCourutine);
			_currentWeaponPlayerAutoAttackCourutine = null;
		}
	}

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (true)
		{
			if (!_isWeaponPlayerAutoShooting)
			{
				break; 
			}

			Coroutine shootingCoroutine = StartCoroutine(ShootWeaponPlayer(WeaponDamage));

			ApplyWeaponRecoil();

			yield return (shootingCoroutine);

			if (PlayerMagazineAmmoCurrent <= 0)
			{
				_isWeaponPlayerAutoShooting = false;
				break;
			}
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator ShootWeaponPlayer(float weaponDamage)
	{
		_currentWeaponPlayerShootRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponShootAnimation(this));

		RaycastHit hitInfo;
		IDamageable damageable = null;

		_vfxInstance = Instantiate(
			_VFXshottEffect,
			_VFXspawnPoint.position,
			_VFXspawnPoint.rotation,
			_VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
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

		if (System.Enum.TryParse(WeaponName.ToString(), out WeaponNames parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		ApplyWeaponRecoil();

		yield return _currentWeaponPlayerShootRoutine;

		_currentWeaponPlayerShootRoutine = null;
	}

	public virtual IEnumerator ReloadWeaponPlayer(bool isSecondAnimation)
	{
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		var data = _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType];

		Coroutine animRoutine = StartCoroutine(_playerWeaponAnimationController.PrepareForReloadingWeapon(this, IsReloadingAnimationSingle, isSecondAnimation));
		yield return new WaitForSeconds(_waitForAmmoRefill);

		data.AmmoReserve -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType] = data;
		PlayerMagazineAmmoCurrent += ammoToAdd;

		if (System.Enum.TryParse(WeaponName.ToString(), out WeaponNames parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.AmmoReserve);
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		yield return animRoutine;

		Debug.Log("Reloaded");
		yield return null;
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
			if (_currentWeaponPlayerShootRoutine != null)
			{
				Debug.Log("Can't reload during shooting");
				return;
			}
			if (_playerWeaponAnimationController.IsPlayerReloading)
			{
				Debug.Log("Already reloading");
				return;
			}
			if (PlayerMagazineAmmoCurrent >= PlayerMagazineAmmoMax)
			{
				Debug.Log("Magazine is already full");
				return;
			}
			if (PlayerAmmoReserve <= 0)
			{
				Debug.Log("Not enough Ammo to reload");
				return;
			}

			StartCoroutine(ReloadWeaponPlayer(false));
		}
		else
		{
			//StartCoroutine(ReloadWeaponNPC()); // npc reload
		}
	}

	public void ShootWeaponNPC()
	{
	}

	public IEnumerator ReloadWeaponNPC()
	{
		_isNPCreloading = true;

		yield return null;
	}

	protected abstract void InitializeWeaponRanged();

	protected abstract void ApplyWeaponRecoil();
}