using TMPro;
using UnityEngine;

public class PlayerAmmoManager : MonoBehaviour
{
	public static PlayerAmmoManager Instance { get; private set; } // Статическое поле экземпляра\

	public TMP_Text PlayerAmmoText;

	public int PlayerAmmoTotalMax { get; private set; } = 40;
	public int PlayerAmmoTotalCurrent { get; private set; } = 10;
	public int PlayerAmmoMagazineMax { get; private set; } = 5;
	public int PlayerAmmoMagazineCurrent { get; private set; } = 5;
	public int PlayerAmmoReserve { get; private set; }

	private void Awake()
	{
		// Паттерн Singleton: предотвращаем создание второго экземпляра
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject); // Сохраняется при смене уровней
		}
		else
		{
			Destroy(gameObject); // Уничтожаем лишние экземпляры
		}
	}
	private void Start()
	{
		PlayerAmmoReserve = PlayerAmmoTotalCurrent - PlayerAmmoMagazineCurrent;
	}


	private void Update()
	{
		//Debug.Log("Total " + PlayerAmmoTotalCurrent);
		//Debug.Log("Magazine " + PlayerAmmoMagazineCurrent);
		//Debug.Log("Reserve " + PlayerAmmoReserve);

		if (InputManager.Instance.GetKeyReload())
		{
			Reload();
		}

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
		else if (PlayerAmmoMagazineCurrent == 0)
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

