using System.Collections;
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
		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);
		_currentWeaponPlayerShootRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponShootAnimation(this));

		SpawnMuzzleVFX();

		// --- ГЛАВНЫЙ RAYCAST: собираем ВСЕ объекты на пути ---
		RaycastHit[] hits = Physics.RaycastAll(_shootPoint.transform.position, _shootPoint.transform.forward, _weaponRange);
		System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance)); // Сортируем от лица

		if (hits.Length > 0)
		{
			// Передаем ВЕСЬ массив попаданий в методы обработки
			SpawnBulletHoleDecal(hits);
			ProcessDamage(hits, weaponDamage);

		}

		PlayerMagazineAmmoCurrent--;

		Debug.Log($"Shoot {WeaponName}");
		_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);

		ApplyWeaponRecoil();

		yield return _currentWeaponPlayerShootRoutine;
		_currentWeaponPlayerShootRoutine = null;
	}

	protected virtual void ProcessDamage(RaycastHit[] hits, float weaponDamage)
	{
		int layerNPC = LayerMask.NameToLayer("NPC");
		IDamageable damageable = null;

		// Выносим слои головы в переменные для читаемости (и добавляем роботизированный хитбокс)
		int layerOrganismHead = LayerMask.NameToLayer("HitboxHead_Organism");
		int layerRobotHead = LayerMask.NameToLayer("HitboxHead_Robot");

		foreach (var hit in hits)
		{
			bool isInDamageMask = ((1 << hit.transform.gameObject.layer) & _playerWeaponController.LayersToDamage) != 0;

			if (isInDamageMask)
			{
				Transform targetTransform = hit.transform;

				while (targetTransform != null)
				{
					damageable = targetTransform.GetComponent<IDamageable>();
					if (damageable != null)
					{
						float finalDamage = weaponDamage;

						// Проверяем слой ТОГО ОБЪЕКТА, где найден IDamageable
						if (hit.transform.gameObject.layer == layerOrganismHead ||
							hit.transform.gameObject.layer == layerRobotHead)
						{
							finalDamage *= 3f; // Умножаем урон на 3
							Debug.Log($"Критическое попадание в голову ({hit.transform.name})! Урон x3: {finalDamage}");
						}
						else
						{
							Debug.Log($"Попадание в тело/конечность: {hit.transform.name}, Слой: {hit.transform.gameObject.layer}. Базовый урон: {finalDamage}");
						}

						damageable.TakeDamage(finalDamage);
						return;
					}

					// Останавливаемся, если дошли до корня модели персонажа-NPC
					if (targetTransform.gameObject.layer == layerNPC)
					{
						break;
					}
					targetTransform = targetTransform.parent;
				}
			}
		}
	}

	protected virtual void SpawnBulletHoleDecal(RaycastHit[] allHits)
	{
		int layerOrganismBody = LayerMask.NameToLayer("HitboxBody_Organism");
		int layerOrganismHead = LayerMask.NameToLayer("HitboxHead_Organism");
		int layerNPC = LayerMask.NameToLayer("NPC");

		if (allHits.Length == 0) return;

		RaycastHit targetHit = new RaycastHit();
		bool foundTarget = false;

		// Проходим по ВСЕМ объектам, в которые попала пуля
		foreach (var hit in allHits)
		{
			// Ищем ПЕРВЫЙ объект, чей слой входит в маску повреждений
			if (((1 << hit.transform.gameObject.layer) & _playerWeaponController.LayersToDamage) != 0)
			{
				targetHit = hit;
				foundTarget = true;
				break; // Нашли нужный хитбокс - выходим
			}
		}

		// Если ни один слой не подошел под маску LayersToDamage (попали просто в стену или пол), 
		// то берем самый первый физический объект
		if (!foundTarget)
		{
			targetHit = allHits[0];
		}

		// Проверка слоев UI/Эфиров перед спавном
		if (targetHit.transform.gameObject.layer != 9 && targetHit.transform.gameObject.layer != 11 && targetHit.transform.gameObject.layer != 16)
		{
			bool isBloodTarget = targetHit.transform.gameObject.layer == layerOrganismBody || targetHit.transform.gameObject.layer == layerOrganismHead;
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, targetHit.normal);
			_bulletHoleManager.SpawnDecal(targetHit.point, rot, isBloodTarget, targetHit.transform);
		}
	}

	// Метод SpawnMuzzleVFX остается без изменений
	protected virtual void SpawnMuzzleVFX()
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