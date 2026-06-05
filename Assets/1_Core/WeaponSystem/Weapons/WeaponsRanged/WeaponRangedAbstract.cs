using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _shootPoint;

	public int PlayerAmmoTotalMax => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => _playerResourcesAmmoManager.AmmoDictionary[PlayerWeaponAmmoType].TotalAmmoCurrent;

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

	public override void WeaponAttack()
	{
		if (_isThisPlayerWeapon)
		{
			if (PlayerMagazineAmmoCurrent > 0)
			{
				ShootPlayerWeapon(WeaponDamage);
			}
			else
			{
				Debug.Log($"Not enough Ammo {WeaponName}");
			}
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

		if (System.Enum.TryParse(this.WeaponName, out WeaponRangedEnum parsedWeaponType))
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