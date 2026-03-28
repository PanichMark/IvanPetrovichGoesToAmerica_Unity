using TMPro;
using UnityEngine;

public abstract class RangedWeaponAbstract : WeaponAbstract
{
	private TMP_Text PlayerAmmoText;
	protected AmmoTypes WeaponAmmoType;

	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private GameObject playerCamera;

	// Свойства для общего запаса (Total) - получаем из менеджера
	public int PlayerAmmoTotalMax => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].Max;
	public int PlayerAmmoTotalCurrent => playerResourcesAmmoManager.AmmoDictionary[WeaponAmmoType].Current;

	// Свойства магазина (Magazine) - локальные для оружия
	protected int PlayerAmmoMagazineMax;
	protected int PlayerAmmoMagazineCurrent;

	private void Start()
	{
		playerCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("playerResourcesAmmoManager");

		InitializeWeapon();

		//Debug.Log(PlayerAmmoMagazineMax);
		//Debug.Log(PlayerAmmoMagazineCurrent);
	}

	private void Update()
	{

		
	}

	protected abstract void InitializeWeapon();
	// Этот метод вызывается извне (например, по нажатию кнопки)
	public override void WeaponAttack()
	{
		// Проверяем, есть ли патроны в магазине перед выстрелом
		if (PlayerAmmoMagazineCurrent > 0)
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
		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, 100f))
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
		PlayerAmmoMagazineCurrent--;

		// 2. Уменьшаем общий запас через менеджер
		//Debug.Log(playerResourcesAmmoManager);
		playerResourcesAmmoManager.ModifyAmmo(WeaponAmmoType, -1);
	}

	public void Reload()
	{
		// Проверки на возможность перезарядки
		if (PlayerAmmoMagazineCurrent >= PlayerAmmoMagazineMax)
		{
			Debug.Log("Magazine is already full");
			return;
		}

		// Вычисляем, сколько патронов осталось в общем запасе
		int reserve = PlayerAmmoTotalCurrent - PlayerAmmoMagazineCurrent;

		if (reserve <= 0)
		{
			Debug.Log("Not enough Ammo to reload");
			return;
		}

		// Сколько патронов можем взять из резерва для полной зарядки
		int ammoToAdd = Mathf.Min(reserve, PlayerAmmoMagazineMax - PlayerAmmoMagazineCurrent);

		Debug.Log("Reloaded");

		// Обновляем магазин
		PlayerAmmoMagazineCurrent += ammoToAdd;

		// Уменьшаем общий запас патронов на то количество, которое мы зарядили в магазин
		playerResourcesAmmoManager.ModifyAmmo(WeaponAmmoType, -ammoToAdd);
	}
}