using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelsButtons : MonoBehaviour
{
	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;


	public void Initialize(WeaponController weaponController, WeaponWheelController weaponWheelController)
	{
		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;
		Debug.Log($"{WeaponWheelButtonName}Button Initialized");
	}


	public string WeaponWheelButtonName;
    public string AvailableWeaponName;
    public TextMeshProUGUI WeaponText;
    //public Image WeaponSelected;
    //public Sprite WeaponIcon;
    //public Button WeaponButton;

	//public InputManager PlayerInputsList;

    void Start()
    {
		WeaponText.text = "";
        //WeaponButton = GetComponent<Button>();
    }

	

	public void HoverEnter()
    {
		WeaponText.text = AvailableWeaponName;
	}

	

	public void HoverExit()
	{
        if (weaponWheelController.IsWeaponLeftHand)
        {
            WeaponText.text = weaponController.LeftHandWeapon?.WeaponNameUI;
        }
        else if (weaponWheelController.IsWeaponLeftHand == false)
		{
			WeaponText.text = weaponController.RightHandWeapon?.WeaponNameUI;
		}
		
	}
	/*
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
	*/
	/*
	public void ChangeWheaponWheelButtonColor(string handType)
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
	*/

}

