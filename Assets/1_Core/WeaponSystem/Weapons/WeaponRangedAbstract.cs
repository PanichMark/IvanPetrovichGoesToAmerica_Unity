using TMPro;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private TMP_Text _playerAmmoText;

	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _shootPoint;

	public int PlayerAmmoTotalMax => _playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => _playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoCurrent;

	public AmmoTypes WeaponAmmoType { get; protected set; }
	public int MagazineAmmoCurrent { get; protected set; }
	public int MagazineAmmoMax { get; protected set; }

	private void Start()
	{
		if (_isThisPlayerWeapon == true)
		{
			_shootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			_playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
		}

		InitializeWeaponRanged();
	}

	protected abstract void InitializeWeaponRanged();
	
	public override void WeaponAttack()
	{
		if (MagazineAmmoCurrent > 0)
		{
			Shoot(WeaponDamage);
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
		Debug.Log($"{WeaponAmmoType} Attack");

		MagazineAmmoCurrent--;

		if (_isThisPlayerWeapon == true)
		{
			_playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
		}
	}

	public void Reload()
	{
		if (MagazineAmmoCurrent >= MagazineAmmoMax)
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

		int ammoToAdd = Mathf.Min(reserve, MagazineAmmoMax - MagazineAmmoCurrent);

		Debug.Log("Reloaded");

		MagazineAmmoCurrent += ammoToAdd;

		if (_isThisPlayerWeapon == true)
		{
			_playerResourcesAmmoManager.ModifyReserveAmmo(WeaponAmmoType, -ammoToAdd);
			_playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
		}
	}
}