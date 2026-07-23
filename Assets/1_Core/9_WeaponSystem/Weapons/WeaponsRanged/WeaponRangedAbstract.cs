using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	protected PlayerResourcesAmmoManager _playerResourcesAmmoManager;

	protected GameObject _shootPoint;
	protected PlayerCameraStateMachineController _playerCameraStateMachineController;
	protected Coroutine _currentWeaponPlayerShootRoutine;

	protected abstract float _weaponRange { get; }

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
			if (_playerWeaponAnimationController.IsReloading)
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
		if (_isWeaponPlayerAutoAttacking || PlayerMagazineAmmoCurrent <= 0)
			return;

		_isWeaponPlayerAutoAttacking = true;

		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponPlayerAutoAttacking = false;

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
			if (!_isWeaponPlayerAutoAttacking)
			{
				break; 
			}

			Coroutine shootingCoroutine = StartCoroutine(ShootWeaponPlayer(WeaponDamage));

			ApplyWeaponRecoil();

			yield return (shootingCoroutine);

			if (PlayerMagazineAmmoCurrent <= 0)
			{
				_isWeaponPlayerAutoAttacking = false;
				break;
			}
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected abstract IEnumerator OnSpecificShootMechanics();

	protected IEnumerator ShootWeaponPlayer(float weaponDamage)
	{
		Debug.Log($"{WeaponName} Shoot");

		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		if (WeaponName != WeaponNames.Shotgun)
		{
			RaycastHit[] hits = Physics.RaycastAll(_shootPoint.transform.position, _shootPoint.transform.forward, _weaponRange);
			System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

			if (hits.Length > 0)
			{
				SpawnBulletHoleDecal(hits);
				ProcessDamage(hits, weaponDamage, 3);
			}
		}

		SpawnMuzzleVFX();
		PlayerMagazineAmmoCurrent--;
		StartCoroutine(OnSpecificShootMechanics());

		ApplyWeaponRecoil();
		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		_currentWeaponPlayerShootRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponPalmAttackAnimation(this));
		yield return _currentWeaponPlayerShootRoutine;
		_currentWeaponPlayerShootRoutine = null;
	}

	protected void ProcessDamage(RaycastHit[] hits, float weaponDamage, float headshotMultiplier)
	{
		HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

		foreach (var hit in hits)
		{
			if (((1 << hit.collider.gameObject.layer) & _playerWeaponController.LayersToDamage) != 0)
			{
				IDamageable damageable = null;
				Transform checkTarget = hit.transform;

				while (checkTarget != null)
				{
					damageable = checkTarget.GetComponent<IDamageable>();
					if (damageable != null)
					{
						if (!damagedTargets.Contains(damageable))
						{
							damagedTargets.Add(damageable);
							float finalDamage = weaponDamage;

							if (((1 << hit.collider.gameObject.layer) & (_playerWeaponController.LayersHeads)) != 0)
							{
								finalDamage *= headshotMultiplier;
							}

							Debug.Log($"{WeaponName} Damaged {damageable} by {finalDamage}");
							damageable.TakeDamage(finalDamage);
						}
						break;
					}

					if (checkTarget.gameObject.layer == _playerWeaponController.LayerNPC && checkTarget != hit.transform)
					{
						break;
					}

					checkTarget = checkTarget.parent;
				}
			}
		}
	}

	protected void SpawnBulletHoleDecal(RaycastHit[] allHits)
	{
		if (allHits.Length == 0)
		{
			return;
		}

		Debug.Log($"[BaseWeapon] SpawnDecal called. Total hits received: {allHits.Length}");

		foreach (var hit in allHits)
		{
			string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
			Debug.Log($"  -> Hit Object: {hit.collider.name}, Layer: {layerName} ({hit.collider.gameObject.layer})");
		}

		RaycastHit targetHit = new RaycastHit();
		bool foundTarget = false;

		foreach (var hit in allHits)
		{
			int layerMaskCheck = (1 << hit.collider.gameObject.layer) & _playerWeaponController.LayersToDamage;

			if (layerMaskCheck != 0)
			{
				targetHit = hit;
				foundTarget = true;
				break;
			}
		}

		if (!foundTarget)
		{
			targetHit = allHits[0];
		}

		if (targetHit.collider.gameObject.layer != 9 && targetHit.collider.gameObject.layer != 11 && targetHit.collider.gameObject.layer != 16)
		{
			bool isBloodTarget = ((1 << targetHit.collider.gameObject.layer) & (_playerWeaponController.LayersOrganisms)) != 0;
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, targetHit.normal);

			Debug.Log($"[BaseWeapon] Spawning decal on: {targetHit.collider.name} at {targetHit.point}. Blood: {isBloodTarget}");

			_bulletHoleManager.SpawnDecal(targetHit.point, rot, isBloodTarget, targetHit.transform);
		}
		else
		{
			Debug.Log("[BaseWeapon] Skipped decal spawn because layer was UI/Ignore/Aura");
		}
	}

	protected void SpawnMuzzleVFX()
	{
		_vfxInstance = Instantiate(_VFXshottEffect, _VFXspawnPoint.position, _VFXspawnPoint.rotation, _VFXspawnPoint.transform);

		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			_vfxInstance.layer = LayerMask.NameToLayer("FirstPerson");
		}

		Destroy(_vfxInstance, 0.05f);
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
			if (_playerWeaponAnimationController.IsReloading)
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