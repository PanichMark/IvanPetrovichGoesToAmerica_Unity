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

	public int PlayerMagazineAmmoMax { get; protected set; }
	public int PlayerMagazineAmmoCurrent { get; protected set; }

	public int NPCmagazineAmmoMax { get; protected set; }
	public int NPCmagazineAmmoCurrent { get; protected set; }

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
		if (PlayerMagazineAmmoCurrent > 0)
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

		if (_isThisPlayerWeapon == true)
		{
			_playerResourcesAmmoManager.RemoveAmmoFromMagazine(WeaponAmmoType, PlayerMagazineAmmoCurrent);
			PlayerMagazineAmmoCurrent--;
		}

		
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

		Debug.Log("Reloaded");

		PlayerMagazineAmmoCurrent += ammoToAdd;

		if (_isThisPlayerWeapon == true)
		{
			_playerResourcesAmmoManager.RemoveAmmoFromReserve(WeaponAmmoType, ammoToAdd);
			_playerResourcesAmmoManager.AddAmmoToMagazine(WeaponAmmoType, PlayerMagazineAmmoCurrent);
		}
	}
}