using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
	public GameObject wheelSegmentPrefab; // Prefab отдельного сегмента (кнопки)
	public Transform centerPoint;         // Центр кольца
	public Canvas WeaponWheelMenuCanvas;  // Canvas меню выбора оружия
	public TextMeshProUGUI WeaponText;    // Текст для отображения текущего оружия
	public TextMeshProUGUI WeaponWheelName;// Текст для заголовка меню (левая/правая рука)

	// Единый словарь для разблокированных видов оружия
	// Ключ: строка "ИмяОружия_Индекс", значение: объект оружия
	private Dictionary<string, GameObject> UnlockedWeapons = new Dictionary<string, GameObject>();

	private List<GameObject> wheelSegments = new List<GameObject>();
	private bool IsWeaponWheelActive = false;
	private bool IsWeaponLeftHand = false;

	private IInputDevice inputDevice;
	private WeaponController weaponController;
	private PlayerBehaviour playerBehaviour;
	private MenuManager menuManager;

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	// Переменная radius, чтобы она была доступна в методах
	public float radius; // Радиус окружности кнопок

	public event System.Action<int> OnSegmentSelected; // Сигнал о выборе сегмента

	void Start()
	{
		CreateWheel(); // Создаём колесо оружия

		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/MeleePoliceBaton_0"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/MeleeSilverRapier_1"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/MeleeFirefighterSaw_2"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedHarmonicaRevolver_3"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedBergmanPistol_4"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedSawedoffShotgun_5"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedPlungerCrossbow_6"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedGrenadeLauncher_7"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/RangedSniperRifle_8"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/EugenicGenie_9"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/EugenicFireball_10"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/EugenicElectroshock_11"));
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWHeelButtons/EugenicMorozkoFrost_12"));


	}

	// Метод добавления разблокированного оружия
	public void AddUnlockedWeapon(GameObject weaponPrefab)
	{
		// Выделяем имя оружия и индекс из имени префаба
		string[] parts = weaponPrefab.name.Split('_');
		if (parts.Length != 2)
		{
			Debug.LogError("Некорректное имя префаба оружия!");
			return;
		}

		string weaponName = parts[0]; // Имя оружия
		int index = int.Parse(parts[1]); // Индекс оружия

		// Создаем ключ в формате "ИмяОружия_Индекс"
		string key = $"{weaponName}_{index}";

		// Добавляем оружие в словарь
		UnlockedWeapons[key] = weaponPrefab;

		RecreateWheel(); // Пересоздаём колесо оружия
	}

	// Обновлённое условие открытия колеса
	void Update()
	{
		bool currentRightHandPressed = inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = inputDevice.GetKeyLeftHandWeaponWheel();

		// Обновляем состояние, только если изменилось нажатие кнопки и есть хотя бы одно оружие
		if ((currentRightHandPressed != previousRightHandPressed || currentLeftHandPressed != previousLeftHandPressed) && weaponController.hasAnyWeapon)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		previousRightHandPressed = currentRightHandPressed;
		previousLeftHandPressed = currentLeftHandPressed;
	}

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		// Обработка правой руки
		if (rightHandPressed)
		{
			EnableWeaponWheelMenuCanvas("right");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = false;
			ShowWeaponName();

			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}

		// Обработка левой руки
		else if (leftHandPressed)
		{
			EnableWeaponWheelMenuCanvas("left");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = true;
			ShowWeaponName();

			WeaponWheelName.text = "ЛЕВАЯ РУКА";
		}

		// Деактивация, если ничего не нажато
		else
		{
			DisableWeaponWheelMenuCanvas(!IsWeaponLeftHand);
			IsWeaponWheelActive = false;
		}
	}



	public void RecreateWheel()
	{
		foreach (var seg in wheelSegments)
			Destroy(seg.gameObject); // Удаляем предыдущие сегменты

		wheelSegments.Clear(); // Чистка списка

		CreateWheel(); // Формирование нового колеса
	}

	void CreateWheel()
	{
		// Собираем список разблокированных видов оружия
		List<GameObject> activeWeapons = CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return; // Нельзя создать колесо без оружия

		float angleStep = 360f / activeWeapons.Count; // Угловое расстояние между сегментами

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			GameObject segmentInstance = Instantiate(wheelSegmentPrefab);
			segmentInstance.transform.SetParent(centerPoint.parent); // Родитель — тот же, что и у центра
			segmentInstance.name = $"Segment {i + 1}";

			// Настраиваем иконку и реакцию на нажатие кнопки
			var button = segmentInstance.GetComponent<Button>();
			button.image.sprite = activeWeapons[i].GetComponent<SpriteRenderer>().sprite; // Загрузка иконки оружия
			button.onClick.AddListener(() => OnSegmentSelected.Invoke(i));

			// Вычисляем позицию кнопки на окружности
			float adjustedAngle = i * angleStep + 90f; // Учтем сдвиг на -90 градусов
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, radius);
			segmentInstance.transform.position = centerPoint.position + positionOnCircle;

			wheelSegments.Add(segmentInstance); // Запоминаем новый сегмент
		}
	}

	// Собираем активный список оружия, исключая пустые позиции
	private List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(UnlockedWeapons.Values);
	}

	// Функция для вычисления позиции на окружности
	private Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}

	// Инициализация контроллера
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, WeaponController weaponController)
	{
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour;
		this.weaponController = weaponController;
		this.menuManager = menuManager;
		Debug.Log("WeaponWheelController Initialized");
	}

	private void EnableWeaponWheelMenuCanvas(string handType)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(true); // Показываем Canvas
		menuManager.OpenWeaponWheelMenu(handType);
	}

	private void DisableWeaponWheelMenuCanvas(bool IsItRightWeaponWheelMenuCanvas)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(false); // Скрываем Canvas
		if (!menuManager.IsPauseMenuOpened)
		{
			menuManager.CloseWeaponWheelMenu(IsItRightWeaponWheelMenuCanvas);
		}
	}

	public void ShowWeaponName()
	{
		if (IsWeaponLeftHand)
		{
			if (weaponController.LeftHandWeapon != null)
			{
				WeaponText.text = weaponController.leftHandWeaponComponent.WeaponNameUI;
			}
			else
			{
				WeaponText.text = "";
			}
		}
		else if (IsWeaponLeftHand == false)
		{
			if (weaponController.RightHandWeapon != null)
			{
				WeaponText.text = weaponController.rightHandWeaponComponent.WeaponNameUI;
			}
			else
			{
				WeaponText.text = "";
			}
		}
	}
}