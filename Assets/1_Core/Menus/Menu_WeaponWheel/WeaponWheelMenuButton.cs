using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WeaponWheelMenuButton : MonoBehaviour
{
	private PlayerWeaponController weaponController;
	private WeaponWheelMenuController weaponWheelController;
	private GameObject WeaponPrefab;
	private string WeaponName;
	private Button _button;

	// Поле для хранения оригинального цвета кнопки
	private Color originalNormalColor;

	// Текущее активное оружие
	private GameObject currentWeapon;

	// Предыдущее активное оружие
	private GameObject previousWeapon;

	public void Initialize(PlayerWeaponController weaponController, WeaponWheelMenuController weaponWheelController, GameObject weaponPrefab, WeaponAbstract weaponComponent)
	{


		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;
		this.WeaponPrefab = weaponPrefab;
		WeaponName = weaponComponent.WeaponNameUI;

		var button = GetComponent<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		button.onClick.AddListener(() => this.weaponWheelController.ShowWeaponIconBig());
		_button = button; // Сохраняем ссылку на кнопку

		// СОХРАНЯЕМ ТЕКУЩИЙ ЦВЕТ НАЧАЛЬНОЙ КНОПКИ
		originalNormalColor = _button.colors.normalColor;

		// Подписываемся на событие изменения активного оружия

		this.weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheel;

		this.weaponController.OnWeaponChanged += OnWeaponChange;
	}

	private void OnOpenWeaponWheel(string activeHand)
	{


		// Определяем новое активное оружие
		if (activeHand == "left")
		{
			previousWeapon = weaponController.LeftHandWeapon;
		}
		else
		{
			previousWeapon = weaponController.RightHandWeapon;
		}
		//Debug.Log(previousWeapon);
		HandleOnWeaponChanged(activeHand);
	}

	private void OnWeaponChange(string activeHand)
	{
		
		//Debug.Log(previousWeapon);
		HandleOnWeaponChanged(activeHand);
		previousWeapon = currentWeapon;
	}

	// Метод обработки события изменения активного оружия
	private void HandleOnWeaponChanged(string activeHand)
	{
		
			
		
		// Определяем новое активное оружие
		if (activeHand == "left")
		{
			currentWeapon = weaponController.LeftHandWeapon;
		}
		else
		{
			currentWeapon = weaponController.RightHandWeapon;
		}

		// Если оружие изменилось, обновляем цвет кнопки
		if (currentWeapon != previousWeapon)
		{
			UpdateButtonColor(currentWeapon);
			//Debug.Log("NEW");
		}
		else
		{
			UpdateButtonColor(previousWeapon);
			//Debug.Log("OLD");
		}

		// Обновляем предыдущее оружие;

		//Debug.Log(currentWeapon);
	
	}

	// Этот метод будет вызван при изменении активного оружия
	// WeaponWheelMenuButton.cs

	private void UpdateButtonColor(GameObject activeWeapon)
	{
		// Проверяем, что активное оружие вообще существует
		if (activeWeapon == null)
		{
			ChangeButtonColor(originalNormalColor);
			return;
		}

		// Получаем компонент WeaponAbstract от активного оружия в руке
		WeaponAbstract activeWeaponComponent = activeWeapon.GetComponent<WeaponAbstract>();

		// Получаем компонент WeaponAbstract от префаба, который представляет эта кнопка
		WeaponAbstract buttonWeaponComponent = WeaponPrefab.GetComponent<WeaponAbstract>();

		// Теперь сравниваем их уникальные системные имена
		if (activeWeaponComponent != null && buttonWeaponComponent != null)
		{
			if (activeWeaponComponent.WeaponNameSystem == buttonWeaponComponent.WeaponNameSystem)
			{
				ChangeButtonColor(new Color(209f / 255f, 138f / 255f, 36f / 255f));
			}
			else
			{
				ChangeButtonColor(originalNormalColor);
			}
		}
	}

	public void HoverEnter()
	{
		weaponWheelController.WeaponText.text = WeaponName;
		//ChangeButtonColor(new Color(209f / 255f, 138f / 255f, 36f / 255f));
	}

	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	
	}

	private void SelectWeapon()
	{
		if (weaponController.isAbleToUseRightWeapon || (weaponController.isLeftHand && weaponController.isAbleToUseLeftWeapon))
		{
			weaponController.SelectWeapon(WeaponPrefab);
		}
	}

	// Не забываем отписаться от события при уничтожении объекта
	private void OnDestroy()
	{
		weaponController.OnWeaponChanged -= OnWeaponChange;
		this.weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
	}

	// Вспомогательная функция для смены цвета
	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}
}