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

	// Список сегментов
	private List<GameObject> wheelSegments = new List<GameObject>();
	private bool IsWeaponWheelActive = false;
	private bool IsWeaponLeftHand = false;

	private IInputDevice inputDevice;
	private WeaponController weaponController;
	private PlayerBehaviour playerBehaviour;
	private MenuManager menuManager;

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	// Радиус окружности кнопок
	public float radius = 100f;

	// Делегат для выбора сегмента
	public event System.Action<int> OnSegmentSelected;

	void Start()
	{
		//CreateWheel(); // Создаём колесо оружия
	}

	// Инициализация контроллера
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, WeaponController weaponController)
	{
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour;
		this.weaponController = weaponController;
		this.menuManager = menuManager;
		Debug.Log("WeaponWheelController Initialized");

		// Подписываемся на событие разблокировки оружия
		weaponController.OnWeaponUnlocked += OnWeaponUnlocked;
	}

	// Обработчик события разблокировки оружия
	private void OnWeaponUnlocked(GameObject weaponPrefab)
	{
		// Пересоздаем колесо оружия
		RecreateWheel();
		//Debug.Log("Пересоздали кольцо");
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



	void CreateWheel()
	{
		// Собираем список разблокированных видов оружия
		List<GameObject> activeWeapons = weaponController.CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return; // Нельзя создать колесо без оружия

		// Сортировка оружия по индексу
		activeWeapons.Sort((a, b) =>
		{
			int indexA = int.Parse(a.name.Split('_')[1]);
			int indexB = int.Parse(b.name.Split('_')[1]);
			return indexA.CompareTo(indexB);
		});

		float angleStep = 360f / activeWeapons.Count; // Угловое расстояние между сегментами

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			GameObject segmentInstance = Instantiate(wheelSegmentPrefab);
			var bruh = segmentInstance.GetComponent<WeaponWheelButton>();
			bruh.Initialize(weaponController, this);



			segmentInstance.transform.SetParent(centerPoint.parent); // Родитель — тот же, что и у центра
			segmentInstance.name = $"Segment {i + 1}";

			// Настраиваем иконку и реакцию на нажатие кнопки
			var button = segmentInstance.GetComponent<Button>();
			button.onClick.AddListener(() => OnSegmentSelected?.Invoke(i)); // Отправляем индекс кнопки

			// Создаем дочерний объект для иконки
			GameObject iconObject = new GameObject("Icon");
			iconObject.transform.SetParent(button.transform, false);
			iconObject.transform.localPosition = Vector3.zero;
			iconObject.transform.localScale = Vector3.one;

			// Добавляем компонент Image к иконке
			Image iconImage = iconObject.AddComponent<Image>();
			iconImage.sprite = activeWeapons[i].GetComponent<SpriteRenderer>().sprite; // Загрузка иконки оружия

			// Вычисляем позицию кнопки на окружности
			float adjustedAngle = i * angleStep + 90f; // Учтем сдвиг на -90 градусов
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, radius);
			segmentInstance.transform.position = centerPoint.position + positionOnCircle;

			wheelSegments.Add(segmentInstance); // Запоминаем новый сегмент
		}
	}



	// Функция для вычисления позиции на окружности
	private Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}

	// Метод рекреации колеса
	public void RecreateWheel()
	{
		foreach (var seg in wheelSegments)
			Destroy(seg.gameObject); // Удаляем предыдущие сегменты

		wheelSegments.Clear(); // Чистка списка

		CreateWheel(); // Формирование нового колеса
		//Debug.Log("Пересоздали кольцо");
	}

	private void EnableWeaponWheelMenuCanvas(string handType)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(true); // Показываем Canvas
		menuManager.OpenWeaponWheelMenu(handType);

		// Принудительно обновляем компоновку Canvas
		//LayoutRebuilder.ForceRebuildLayoutImmediate(WeaponWheelMenuCanvas.GetComponent<RectTransform>());

		// Пересоздаем колесо оружия при первом открытии
		RecreateWheel();
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