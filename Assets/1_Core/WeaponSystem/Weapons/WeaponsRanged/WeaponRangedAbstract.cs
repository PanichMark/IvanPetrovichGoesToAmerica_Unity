using TMPro;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _shootPoint;

	public int PlayerAmmoTotalMax => _playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => _playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoCurrent;

	public AmmoTypes WeaponAmmoType { get; protected set; }

	public int PlayerMagazineAmmoMax { get; protected set; }
	public int PlayerMagazineAmmoCurrent { get; set; }

	private void Start()
	{
		if (_isThisPlayerWeapon)
		{
			_shootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
		}
	}

	public override void WeaponAttack()
	{
		if (PlayerMagazineAmmoCurrent > 0)
		{
			Shoot(WeaponDamage);

			// Просто вызываем Action, если на него кто-то подписан (HUD)
			_playerResourcesAmmoManager.OnMagazineAmmoChanged?.Invoke(WeaponAmmoType, PlayerMagazineAmmoCurrent);
		}
		else
		{
			Debug.Log("Not enough Ammo");
		}
	}

	private void Shoot(float weaponDamage)
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
	}
	// Метод для установки типа патронов (нужен для загрузки данных)
	public void SetWeaponAmmoType(AmmoTypes type)
	{
		WeaponAmmoType = type;
	}

	// Метод для установки параметров магазина
	public void SetMagazineProperties(int maxAmmo, int currentAmmo)
	{
		PlayerMagazineAmmoMax = maxAmmo;
		PlayerMagazineAmmoCurrent = currentAmmo;
	}
	public void Reload()
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

		// Правильный способ изменить структуру в словаре
		var data = _playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType];
		data.TotalAmmoCurrent -= ammoToAdd;
		_playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType] = data;

		PlayerMagazineAmmoCurrent += ammoToAdd;

		// Вызываем Actions для обновления HUD
		_playerResourcesAmmoManager.OnReserveAmmoChanged?.Invoke(WeaponAmmoType, data.TotalAmmoCurrent);
		_playerResourcesAmmoManager.OnMagazineAmmoChanged?.Invoke(WeaponAmmoType, PlayerMagazineAmmoCurrent);

		Debug.Log("Reloaded");
	}
}