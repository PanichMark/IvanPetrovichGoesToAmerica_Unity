using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	protected PlayerWeaponAmmoController _playerResourcesAmmoManager;

	public Transform WeaponRangedShootPoint {  get; protected set; }
	protected PlayerCameraStateMachineController _playerCameraStateMachineController;
	protected Coroutine _currentWeaponPlayerShootRoutine;

	public abstract float WeaponRange { get; }
	protected HUDweaponsController _HUDweaponsController;
	protected abstract float _waitForAmmoRefill { get; }
	public abstract AmmoTypes PlayerWeaponAmmoType { get; }

	public abstract bool LeavesBulletHole {  get; }
	protected bool _isWeaponPlayerShooting;
	public abstract bool IsReloadingAnimationSingle { get; }
	protected GameObject _VFXmuzzleFlashEffect1stPerson;
	protected GameObject _VFXmuzzleFlashEffect3rdPerson;
	protected bool _isNPCreloading;
	public int PlayerMagazineAmmoCurrent { get; set; }

	public int PlayerMagazineAmmoMax { get; protected set; }
	
	public int PlayerAmmoReserve => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoReserve;
	public int PlayerAmmoMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].AmmoMax;

	public int NPCmagazineAmmoCurrent { get; protected set; }
	public int NPCmagazineAmmoMax { get; protected set; }

	protected ObjectPoolWeaponController _bulletHoleManager;
	protected PlayerCameraController _playerCameraController;

	public override void InitializeWeaponPlayer()
	{
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>();
		WeaponRangedShootPoint = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera).transform;
		_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerWeaponAmmoController>();
		_playerCameraController = ServiceLocator.Resolve<PlayerCameraController>();
		_VFXmuzzleFlashEffect1stPerson = FirstPersonWeaponModelInstance.transform.Find("VFX")?.gameObject;
		_HUDweaponsController = ServiceLocator.Resolve<HUDweaponsController>();
		

		_VFXmuzzleFlashEffect3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("VFX")?.gameObject;
		
		_bulletHoleManager = ServiceLocator.Resolve<ObjectPoolWeaponController>();

		InitializeWeaponRanged();
	}

	public override void InitializeWeaponNPC(Transform NPCweaponRangedShootPoint)
	{
		//Debug.Log("INITIALIZE!!!");
		WeaponRangedShootPoint = NPCweaponRangedShootPoint;
		_bulletHoleManager = ServiceLocator.Resolve<ObjectPoolWeaponController>();
		//Debug.Log(_bulletHoleManager);
		_VFXmuzzleFlashEffect3rdPerson = gameObject.transform.Find("VFX")?.gameObject;
	}

	public override void WeaponPlayerAttack()
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
			_isAttacking = true;
			StartAutoAttackingWeaponPlayer();
		}
		else
		{
			_isAttacking = true;
			StartCoroutine(ShootRangedWeaponPlayer(_weaponDamage));
		}
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (IsWeaponPlayerAutoAttacking || PlayerMagazineAmmoCurrent <= 0)
		{
			return;
		}

		IsWeaponPlayerAutoAttacking = true;

		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttackingWeaponPlayer()
	{
		IsWeaponPlayerAutoAttacking = false;

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

			//Debug.Log("RANGED AUYO!!!!");
			if (!IsWeaponPlayerAutoAttacking)
			{
				break; 
			}

			Coroutine shootingCoroutine = StartCoroutine(ShootRangedWeaponPlayer(_weaponDamage));

			ApplyWeaponRecoil();

			yield return (shootingCoroutine);

			if (PlayerMagazineAmmoCurrent <= 0)
			{
				IsWeaponPlayerAutoAttacking = false;
				break;
			}
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator OnSpecificShootMechanics()
	{
		yield return null;
	}

	public override void WeaponNPCattack()
	{
		if (NPCmagazineAmmoCurrent <= 0)
		{
			ShootRangedWeaponNPC(_weaponDamage);
		}
		else
		{
			ReloadWeaponNPC();
		}
	}

	protected void ShootRangedWeaponNPC(float weaponDamage)
	{
		//_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		ShootRaycasts(weaponDamage, false);

		NPCmagazineAmmoCurrent--;
	}

	protected IEnumerator ShootRangedWeaponPlayer(float weaponDamage)
	{
		Debug.Log($"{WeaponName} Shoot");

		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		_HUDweaponsController.AnimateWeaponCrosshairOnShoot(this);

		ShootRaycasts(weaponDamage, true);



		StartCoroutine(ShowMuzzleVFX());

		PlayerMagazineAmmoCurrent--;
		StartCoroutine(OnSpecificShootMechanics());

		ApplyWeaponRecoil();
		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		_currentWeaponPlayerShootRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponPalmAttackAnimation(this));
		yield return _currentWeaponPlayerShootRoutine;
		_currentWeaponPlayerShootRoutine = null;
	}

	private void ShootRaycasts(float weaponDamage, bool isPlayerRayCast)
	{
		//Debug.Log(weaponDamage);

		if (WeaponName != PlayerWeaponNames.Shotgun)
		{
			RaycastHit[] hits = null; 

			if (isPlayerRayCast)
			{
				hits = Physics.RaycastAll(WeaponRangedShootPoint.transform.position, WeaponRangedShootPoint.transform.forward, WeaponRange);
				
				Debug.DrawRay(WeaponRangedShootPoint.transform.position, WeaponRangedShootPoint.transform.forward * WeaponRange, Color.red, 2f);
			}
			else
			{
				hits = Physics.RaycastAll(WeaponRangedShootPoint.transform.position, -WeaponRangedShootPoint.transform.right, WeaponRange);

				Debug.DrawRay(WeaponRangedShootPoint.transform.position, -WeaponRangedShootPoint.transform.right * WeaponRange, Color.red, 2f);
			}

			System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

			if (hits.Length > 0)
			{
				// Создаем временный список для фильтрации
				List<RaycastHit> filteredHits = new List<RaycastHit>();

				foreach (var hit in hits)
				{
					// Если на объекте нет NavMeshAgent — добавляем его в список попаданий
					if (hit.collider.GetComponent<NavMeshAgent>() == null)
					{
						filteredHits.Add(hit);
					}
				}

				// Если после пропуска агентов остались валидные цели
				if (filteredHits.Count > 0)
				{
					SpawnBulletHoleDecal(filteredHits.ToArray());
					ProcessDamage(filteredHits.ToArray(), weaponDamage, 3);
				}
			}
		}
	}

	protected void ProcessDamage(RaycastHit[] hits, float weaponDamage, float headshotMultiplier)
	{
		HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

		foreach (var hit in hits)
		{
			if (((1 << hit.collider.gameObject.layer) & _layersToDamage) != 0)
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

							if (((1 << hit.collider.gameObject.layer) & (_layersHeads)) != 0)
							{
								finalDamage *= headshotMultiplier;
							}

							//Debug.Log($"{WeaponName} Damaged {damageable} by {finalDamage}");
							damageable.TakeDamage(finalDamage);
						}
						break;
					}

					if (checkTarget.gameObject.layer == _layerNPC && checkTarget != hit.transform)
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

		//Debug.Log($"[BaseWeapon] SpawnDecal called. Total hits received: {allHits.Length}");

		foreach (var hit in allHits)
		{
			string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
			//Debug.Log($"  -> Hit Object: {hit.collider.name}, Layer: {layerName} ({hit.collider.gameObject.layer})");
		}

		RaycastHit targetHit = new RaycastHit();
		bool foundTarget = false;

		foreach (var hit in allHits)
		{
			int layerMaskCheck = (1 << hit.collider.gameObject.layer) & _layersToDamage;

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
			bool isBloodTarget = ((1 << targetHit.collider.gameObject.layer) & (_layersOrganisms)) != 0;
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, targetHit.normal);

			//Debug.Log($"[BaseWeapon] Spawning decal on: {targetHit.collider.name} at {targetHit.point}. Blood: {isBloodTarget}");

			if (LeavesBulletHole)
			{
				_bulletHoleManager.SpawnDecal(targetHit.point, rot, isBloodTarget, targetHit.transform);
			}
		}
		else
		{
			//Debug.Log("[BaseWeapon] Skipped decal spawn because layer was UI/Ignore/Aura");
		}
	}

	protected IEnumerator ShowMuzzleVFX()
	{
		if (_VFXmuzzleFlashEffect1stPerson != null)
		{
			_VFXmuzzleFlashEffect1stPerson.SetActive(true);
		}

		if (_VFXmuzzleFlashEffect3rdPerson != null)
		{
			_VFXmuzzleFlashEffect3rdPerson.SetActive(true);
		}

		yield return new WaitForSeconds(0.05f);

		if (_VFXmuzzleFlashEffect1stPerson != null)
		{
			_VFXmuzzleFlashEffect1stPerson.SetActive(false);
		}

		if (_VFXmuzzleFlashEffect3rdPerson != null)
		{
			_VFXmuzzleFlashEffect3rdPerson.SetActive(false);
		}
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

		if (System.Enum.TryParse(WeaponName.ToString(), out PlayerWeaponNames parsedWeaponType))
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


	public void ReloadWeaponNPC()
	{
		_isNPCreloading = true;
	}

	protected abstract void InitializeWeaponRanged();

	protected virtual void ApplyWeaponRecoil()
	{

	}
}