using TMPro;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private TMP_Text PlayerAmmoText;

	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private GameObject ShootPoint;

	public int PlayerAmmoTotalMax => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoCurrent;

	public AmmoTypes WeaponAmmoType { get; protected set; }
	public int MagazineAmmoCurrent { get; protected set; }
	public int MagazineAmmoMax { get; protected set; }

	private void Start()
	{
		if (IsThisPlayerWeapon == true)
		{
			ShootPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");
			playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("PlayerResourcesAmmoManager");
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
		if (Physics.Raycast(ShootPoint.transform.position, ShootPoint.transform.forward, out hitInfo, 100f))
		{
			IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponDamage);
			}
		}
		Debug.Log($"{WeaponAmmoType} Attack");

		MagazineAmmoCurrent--;

		if (IsThisPlayerWeapon == true)
		{
			playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
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

		if (IsThisPlayerWeapon == true)
		{
			playerResourcesAmmoManager.ModifyReserveAmmo(WeaponAmmoType, -ammoToAdd);
			playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
		}
	}
}