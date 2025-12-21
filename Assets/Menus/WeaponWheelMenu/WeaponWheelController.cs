using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private WeaponController weaponController;
	private PlayerBehaviour playerBehaviour;
	private MenuManager menuManager;

	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, WeaponController weaponController)
	{
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour;
		this.weaponController = weaponController;
		this.menuManager = menuManager;
		Debug.Log("WeaponWheelController Initialized");

		// Подписываемся на событие смены оружия
		weaponController.OnWeaponChanged += OnWeaponChangedHandler; // Подписка на событие
	}

	public Canvas WeaponWheelMenuCanvas; 

	public Button PoliceBatonButton;
	public Button HarmonicaRevolverButton;
	public Button PlungerCrossbowButton;
	public Button EugenicGenieButton;
	public bool IsWeaponLeftHand { get; private set; }
	public bool IsWeaponWheelActive { get; private set; }

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	public TextMeshProUGUI WeaponWheelName;
	//public WeaponWheelsButtons weaponWheelbuttonscript;
	

	void Start()
	{
		//playerInputsList = GetComponent<InputManager>();
		//weaponController = GetComponent<WeaponController>();

		PoliceBatonButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponPoliceBaton)));
		HarmonicaRevolverButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponHarmonicaRevolver)));
		PlungerCrossbowButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponPlungerCrossbow)));
		EugenicGenieButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponEugenicGenie)));

		//weaponController.SelectWeapon(typeof(WeaponPoliceBaton));
	}

	void Update()
	{
		

		
		bool currentRightHandPressed = inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = inputDevice.GetKeyLeftHandWeaponWheel();
		
		// Обновляем состояние, только если изменилось нажатие кнопки
		if (currentRightHandPressed != previousRightHandPressed || currentLeftHandPressed != previousLeftHandPressed)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		previousRightHandPressed = currentRightHandPressed;
		previousLeftHandPressed = currentLeftHandPressed;


		

		if (PoliceBatonButton != null)
		{
			if (weaponController.IsPoliceBatonWeaponUnlocked)
			{
				PoliceBatonButton.gameObject.SetActive(true);
			}
			else PoliceBatonButton.gameObject.SetActive(false);
		}

		if (HarmonicaRevolverButton != null)
		{
			if (weaponController.IsHarmoniceRevolverWeaponUnlocked)
			{
				HarmonicaRevolverButton.gameObject.SetActive(true);
			}
			else HarmonicaRevolverButton.gameObject.SetActive(false);
		}

		if (PlungerCrossbowButton != null)
		{
			if (weaponController.IsPlungerCrossbowWeaponUnlocked)
			{
				PlungerCrossbowButton.gameObject.SetActive(true);
			}
			else PlungerCrossbowButton.gameObject.SetActive(false);
		}

		if (EugenicGenieButton != null)
		{
			if (weaponController.IsEugenicGenieWeaponUnlocked)
			{
				EugenicGenieButton.gameObject.SetActive(true);
			}
			else EugenicGenieButton.gameObject.SetActive(false);
		}
	}

	
	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		// Обработка правой руки
		if (rightHandPressed)
		{
			EnableWeaponWheelMenuCanvas("right");
			OnWeaponChangedHandler("right");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = false;
			//playerBehaviour.ArmPlayer();
			//ChangeWheaponWheelButtonColor("right");
			//weaponWheelbuttonscript.HoverExit();
			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}

		// Обработка левой руки
		else if (leftHandPressed)
		{
			EnableWeaponWheelMenuCanvas("left");
			OnWeaponChangedHandler("left");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = true;
			//playerBehaviour.ArmPlayer();
			//ChangeWheaponWheelButtonColor("left");
			//weaponWheelbuttonscript.HoverExit();
			WeaponWheelName.text = "ЛЕВАЯ РУКА";
		}

		// Деактивация, если ничего не нажато
		else 
		{
			DisableWeaponWheelMenuCanvas(!IsWeaponLeftHand);
			IsWeaponWheelActive = false;
		}
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

	public void ChangeWeaponWheelButtonColorToActive(Button buttonType)
	{
		// Преобразуем HEX в значение цвета
		string hexCode = "#FFEE00"; // добавляем альфа-канал FF (полностью непрозрачный)

		Color newColor;
		if (!ColorUtility.TryParseHtmlString(hexCode, out newColor))
			Debug.LogError("Ошибка конвертации HEX цвета");

		// Меняем цвета всех состояний кнопки
		ColorBlock colors = buttonType.colors;
		colors.normalColor = newColor;
		colors.highlightedColor = newColor;
		colors.selectedColor = newColor;
		colors.pressedColor = newColor;
		colors.disabledColor = newColor;
		buttonType.colors = colors;
	}

	public void ChangeWeaponWheelButtonColorToDefault(Button buttonType)
	{
		// Определяем HEX-коды для двух цветов
		string highlightHexCode = "#D18A24FF"; // Основной цвет для Highlight/Press/Select
		string normalHexCode = "#5B4328FF";    // Отдельный цвет для Normal состояния

		// Создаем объекты Color для обоих цветов
		Color highlightColor;
		Color normalColor;

		// Проверяем оба HEX-кода
		if (!ColorUtility.TryParseHtmlString(highlightHexCode, out highlightColor))
		{
			Debug.LogError("Ошибка конвертации первого HEX цвета");
		}
		if (!ColorUtility.TryParseHtmlString(normalHexCode, out normalColor))
		{
			Debug.LogError("Ошибка конвертации второго HEX цвета");
		}

		// Читаем текущие настройки цветов кнопки
		ColorBlock colors = buttonType.colors;

		// Применяем новые цвета для конкретных состояний
		colors.normalColor = normalColor;       // Только для обычного состояния
		colors.highlightedColor = highlightColor; // Для выделенного состояния
		colors.pressedColor = highlightColor;   // Для нажатого состояния
		colors.selectedColor = highlightColor;  // Для выбранного состояния

		// Оставляем disabledColor без изменений

		// Назначаем обновленные цвета кнопке
		buttonType.colors = colors;
	}


	private void OnWeaponChangedHandler(string handType)
	{
		if (handType == "right")
		{
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "PoliceBaton")
			{
				ChangeWeaponWheelButtonColorToActive(PoliceBatonButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "HarmonicaRevolver")
			{
				ChangeWeaponWheelButtonColorToActive(HarmonicaRevolverButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "PlungerCrossbow")
			{
				ChangeWeaponWheelButtonColorToActive(PlungerCrossbowButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "EugenicGenie")
			{
				ChangeWeaponWheelButtonColorToActive(EugenicGenieButton);
			}

			if (weaponController.RightHandWeapon?.WeaponNameSystem != "PoliceBaton")
			{
				ChangeWeaponWheelButtonColorToDefault(PoliceBatonButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "HarmonicaRevolver")
			{
				ChangeWeaponWheelButtonColorToDefault(HarmonicaRevolverButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "PlungerCrossbow")
			{
				ChangeWeaponWheelButtonColorToDefault(PlungerCrossbowButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "EugenicGenie")
			{
				ChangeWeaponWheelButtonColorToDefault(EugenicGenieButton);
			}
		}

		else if (handType == "left")
		{
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "PoliceBaton")
			{
				ChangeWeaponWheelButtonColorToActive(PoliceBatonButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "HarmonicaRevolver")
			{
				ChangeWeaponWheelButtonColorToActive(HarmonicaRevolverButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "PlungerCrossbow")
			{
				ChangeWeaponWheelButtonColorToActive(PlungerCrossbowButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "EugenicGenie")
			{
				ChangeWeaponWheelButtonColorToActive(EugenicGenieButton);
			}

			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "PoliceBaton")
			{
				ChangeWeaponWheelButtonColorToDefault(PoliceBatonButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "HarmonicaRevolver")
			{
				ChangeWeaponWheelButtonColorToDefault(HarmonicaRevolverButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "PlungerCrossbow")
			{
				ChangeWeaponWheelButtonColorToDefault(PlungerCrossbowButton); ;
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "EugenicGenie")
			{
				ChangeWeaponWheelButtonColorToDefault(EugenicGenieButton);
			}
		}
	}

}
