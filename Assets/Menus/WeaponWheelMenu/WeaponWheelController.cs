using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponWheelController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private WeaponController weaponController;
	private PlayerBehaviour playerBehaviour;
	private MenuManager menuManager;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, WeaponController weaponController)
	{
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour;
		this.weaponController = weaponController;
		this.menuManager = menuManager;
		Debug.Log("WeaponWheelController Initialized");
	}

	public Canvas WeaponWheelMenuCanvas;

	public TextMeshProUGUI WeaponText;

	public bool IsWeaponLeftHand { get; private set; }
	public bool IsWeaponWheelActive { get; private set; }

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	public TextMeshProUGUI WeaponWheelName;


	void Update()
	{
		bool currentRightHandPressed = inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = inputDevice.GetKeyLeftHandWeaponWheel();
		
		// Обновляем состояние, только если изменилось нажатие кнопки
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
			//OnWeaponChangedHandler("right");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = false;
			ShowWeaponName();

			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}

		// Обработка левой руки
		else if (leftHandPressed)
		{
			EnableWeaponWheelMenuCanvas("left");
			//OnWeaponChangedHandler("left");
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
