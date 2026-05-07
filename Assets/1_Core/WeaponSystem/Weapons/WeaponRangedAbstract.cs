using TMPro;
using UnityEngine;

public abstract class WeaponRangedAbstract : WeaponAbstract
{
	private TMP_Text PlayerAmmoText;


	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private GameObject ShootPoint;

	// Свойства для общего запаса (Total) - получаем из менеджера
	public int PlayerAmmoTotalMax => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoMax;
	public int PlayerAmmoTotalCurrent => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].TotalAmmoCurrent;

	// Свойства магазина (Magazine) - локальные для оружия
	public AmmoTypes WeaponAmmoType { get; protected set; }
	public int MagazineAmmoCurrent { get; protected set; }
	public int MagazineAmmoMax { get; protected set; }

	private void Start()
	{
		//Debug.Log(IsThisPlayerWeapon);
		if (IsThisPlayerWeapon == true)
		{
			ShootPoint = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
			playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("playerResourcesAmmoManager");
		}


		InitializeWeaponRanged();

		//Debug.Log(PlayerAmmoMagazineMax);
		//Debug.Log(PlayerAmmoMagazineCurrent);
	}



	protected abstract void InitializeWeaponRanged();
	// Этот метод вызывается извне (например, по нажатию кнопки)
	public override void WeaponAttack()
	{
		// Проверяем, есть ли патроны в магазине перед выстрелом
		if (MagazineAmmoCurrent > 0)
		{
			Shoot(WeaponDamage);
		}
		else
		{
			Debug.Log("Not enough Ammo");
		}
	}

	// Логика самого выстрела: рейкаст и уменьшение счетчиков
	private void Shoot(float weaponDamage)
	{
		// --- Логика попадания ---
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

		// --- Логика боеприпасов ---
		// 1. Уменьшаем магазин
		MagazineAmmoCurrent--;

		// 2. Уменьшаем общий запас через менеджер
		//Debug.Log(playerResourcesAmmoManager);
		//playerResourcesAmmoManager.ModifyAmmo(WeaponAmmoType, -1);
		if (IsThisPlayerWeapon == true)
		{
			playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
		}
	}

	public void Reload()
	{
		// Проверки на возможность перезарядки
		if (MagazineAmmoCurrent >= MagazineAmmoMax)
		{
			Debug.Log("Magazine is already full");
			return;
		}

		// Вычисляем, сколько патронов осталось в общем запасе
		int reserve = PlayerAmmoTotalCurrent;

		if (reserve <= 0)
		{
			Debug.Log("Not enough Ammo to reload");
			return;
		}

		// Сколько патронов можем взять из резерва для полной зарядки
		int ammoToAdd = Mathf.Min(reserve, MagazineAmmoMax - MagazineAmmoCurrent);

		Debug.Log("Reloaded");

		// Обновляем магазин
		MagazineAmmoCurrent += ammoToAdd;

		if (IsThisPlayerWeapon == true)
		{
			// Уменьшаем общий запас патронов на то количество, которое мы зарядили в магазин
			playerResourcesAmmoManager.ModifyReserveAmmo(WeaponAmmoType, -ammoToAdd);
			playerResourcesAmmoManager.ModifyMagazineAmmo(WeaponAmmoType, MagazineAmmoCurrent);
		}
	}
}