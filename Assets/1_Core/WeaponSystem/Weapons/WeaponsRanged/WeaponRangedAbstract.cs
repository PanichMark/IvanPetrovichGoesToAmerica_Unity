using UnityEngine;
using System.Collections;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _shootPoint;

	public int PlayerAmmoTotalMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoCurrent;

	// --- ДОБАВИТЬ ЭТИ ПОЛЯ ---
	// В классе WeaponRangedAbstract

	// Поля (оставляем как есть)
	private bool _isFiring;
	private Coroutine _fireCoroutine;
	protected float _fireRate = 0.1f;

	// ... (другие методы)

	// Метод StartFiring (оставляем как есть)
	public void StartFiring()
	{
		if (_isFiring || PlayerMagazineAmmoCurrent <= 0) return;
		_isFiring = true;
		if (_fireCoroutine == null)
		{
			_fireCoroutine = StartCoroutine(FireAuto());
		}
	}

	public override void StopWeaponAutoAttack()
	{
		_isFiring = false;
		if (_fireCoroutine != null)
		{
			StopCoroutine(_fireCoroutine);
			_fireCoroutine = null;
		}
	}

	// --- ИЗМЕНЕННЫЙ МЕТОД FireAuto ---
	private IEnumerator FireAuto()
	{
		// Цикл работает бесконечно, но мы проверяем флаг _isFiring ДО выстрела.
		// Это гарантирует, что как только StopFiring() будет вызван, текущая итерация
		// завершится, и новая не начнется.
		while (true)
		{
			// Проверяем флаг ДО выстрела
			if (!_isFiring)
			{
				break; // Выходим из цикла
			}

			ShootPlayerWeapon(WeaponDamage);

			yield return new WaitForSeconds(_fireRate);

			// Дополнительная проверка на случай, если патроны кончились во время ожидания
			if (PlayerMagazineAmmoCurrent <= 0)
			{
				_isFiring = false;
				break;
			}
		}
		_fireCoroutine = null;
	}

	public AmmoTypes PlayerWeaponAmmoType { get; protected set; }

	public int PlayerMagazineAmmoMax { get; protected set; }
	public int PlayerMagazineAmmoCurrent { get; set; }

	private void Start()
	{
		if (_isThisPlayerWeapon)
		{
			_shootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");

			InitializeWeaponRanged();
		}
	}

	protected abstract void InitializeWeaponRanged();

	// --- ЗАМЕНИТЬ МЕТОД WeaponAttack ---
	// Заменить весь ваш текущий метод WeaponAttack на этот:
	public override void WeaponAttack()
	{
		// Проверка, что это оружие игрока и есть патроны
		if (_isThisPlayerWeapon && PlayerMagazineAmmoCurrent > 0)
		{
			// --- НОВАЯ ЛОГИКА ---
			// Если это НЕ одиночная атака (т.е. автоматическое оружие)
			if (!IsSingleAttack)
			{
				// Запускаем или продолжаем автоматический огонь
				StartFiring();
			}
			// Если это одиночная атака (револьвер, дробовик)
			else
			{
				// Просто делаем один выстрел
				ShootPlayerWeapon(WeaponDamage);
			}
		}
		else if (_isThisPlayerWeapon)
		{
			Debug.Log($"Not enough Ammo {WeaponName}");
		}
	}



	private void ShootPlayerWeapon(float weaponDamage)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hitInfo, 100f))
		{
			IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponDamage);
			}
		}

		PlayerMagazineAmmoCurrent--;
		Debug.Log($"Shoot {WeaponName}");

		if (System.Enum.TryParse(WeaponName, out WeaponRangedEnum parsedWeaponType))
		{
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}
	}

	public void ShootNPCweapon()
	{
		// TODO
	}

	public void Reload()
	{
		if (_isThisPlayerWeapon)
		{
			ReloadPlayerWeapon();
		}
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

		// НОВЫЕ ВЫЗОВЫ:
		if (System.Enum.TryParse(this.WeaponName, out WeaponRangedEnum parsedWeaponType))
		{
			// Вызываем метод для обновления данных о резерве
			_playerResourcesAmmoManager.NotifyReserveAmmoChanged(PlayerWeaponAmmoType, data.TotalAmmoCurrent);

			// Вызываем метод для обновления данных о магазине
			_playerResourcesAmmoManager.NotifyMagazineAmmoChanged(parsedWeaponType, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		}

		Debug.Log("Reloaded");
	}

	public void ReloadNPCweapon()
	{
		// TODO
	}

	public void SetPlayerWeaponAmmoType(AmmoTypes type)
	{
		PlayerWeaponAmmoType = type;
	}

	public void SetPlayerMagazineProperties(int maxAmmo, int currentAmmo)
	{
		PlayerMagazineAmmoMax = maxAmmo;
		PlayerMagazineAmmoCurrent = currentAmmo;
	}
}