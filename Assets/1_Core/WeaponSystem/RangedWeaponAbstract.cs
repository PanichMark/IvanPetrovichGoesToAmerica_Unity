using TMPro;
using UnityEngine;

public class RangedWeaponAbstract : WeaponAbstract
{
	public TMP_Text PlayerAmmoText;
	private GameObject playerCamera;
	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	public int PlayerAmmoTotalMax { get; private set; }
	public int PlayerAmmoTotalCurrent { get; private set; }
	public int PlayerAmmoMagazineMax { get; private set; }
	public int PlayerAmmoMagazineCurrent { get; private set; }
	public int PlayerAmmoReserve { get; private set; }

	private void Update()
	{
		Debug.Log(PlayerAmmoMagazineCurrent);
	}

	private void Start()
	{
		playerCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		playerResourcesAmmoManager = ServiceLocator.Resolve<PlayerResourcesAmmoManager>("playerResourcesAmmoManager");

		PlayerAmmoTotalMax = 40;
		PlayerAmmoTotalCurrent = 10;
		PlayerAmmoMagazineMax = 5;
		PlayerAmmoMagazineCurrent = 5;
		

		PlayerAmmoReserve = PlayerAmmoTotalCurrent - PlayerAmmoMagazineCurrent;
	}

	public override void WeaponAttack()
	{
		Debug.Log("Revolver Attack");
		Shoot(WeaponDamage);
		//PlayerAmmoManager.Instance.Shoot(WeaponDamage);
	}

	public void Shoot(float weaponDamage)
	{
		if (PlayerAmmoMagazineCurrent > 0)
		{
			// Посылаем луч от положения камеры в направлении её обзора
			RaycastHit hitInfo;
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 100f))
			{
				// Проверяем, попал ли луч в объект с интерфейсом IDamageable
				IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
				if (damageable != null)
				{
					damageable.TakeDamage(weaponDamage); // Вызываем метод TakeDamage у объекта
				}

			}
			Debug.Log("RevolverAttack");
			PlayerAmmoMagazineCurrent--;
			PlayerAmmoTotalCurrent--;
			Debug.Log($"Magazine ammo remaining: {PlayerAmmoMagazineCurrent}");
		}
		else
		{
			Debug.Log("Not enought Ammo");
		}

	}



	public void AddAmmo(int ammoNumber)
	{
		// Проверяем, достигли ли мы максимального общего количества патронов
		if (PlayerAmmoTotalCurrent >= PlayerAmmoTotalMax)
		{
			Debug.Log("Нельзя добавить патроны: достигнут максимум.");
			return;
		}

		// Вычисляем фактическое количество патронов, которое можно добавить
		int actualAdded = Mathf.Min(ammoNumber, PlayerAmmoTotalMax - PlayerAmmoTotalCurrent);

		// Добавляем патроны к общему запасу
		PlayerAmmoTotalCurrent += actualAdded;

		// Обновляем резервные патроны
		PlayerAmmoReserve = PlayerAmmoTotalCurrent - PlayerAmmoMagazineCurrent;
	}

	// Метод для перезарядки магазина
	public void Reload()
	{
		// Высчитываем, сколько патронов можем добавить в магазин
		int ammoToAdd = Mathf.Min(PlayerAmmoReserve, PlayerAmmoMagazineMax - PlayerAmmoMagazineCurrent);

		// Если магазин уже полон или нет патронов в резерве, не выполняем операцию


		if (PlayerAmmoMagazineCurrent == 5)
		{
			Debug.Log("Magazine is alreafy full");
			return;
		}
		else if (PlayerAmmoReserve == 0)
		{
			Debug.Log("Not enough Ammo to reload");
			return;
		}
		else
		{
			Debug.Log("Reloaded");
			// Переносим патроны из резерва в магазин
			PlayerAmmoMagazineCurrent += ammoToAdd;
			PlayerAmmoReserve -= ammoToAdd;
		}
	}


}
