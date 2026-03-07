using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelMenuButton : MonoBehaviour
{
	private WeaponController weaponController;
	private WeaponWheelMenuController weaponWheelController;
	private GameObject WeaponPrefab;
	private string WeaponName;
	private Sprite WeaponIcon;
	private Button _button;
	// Поле для хранения оригинального цвета кнопки
	private Color originalNormalColor;
	public void Initialize(WeaponController weaponController, WeaponWheelMenuController weaponWheelController, GameObject weaponPrefab, WeaponAbstract weaponComponent)
	{
		var button = GetComponent<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		_button = button; // Сохраняем ссылку на кнопку

		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;
		this.WeaponPrefab = weaponPrefab;
		WeaponName = weaponComponent.WeaponNameUI;
		WeaponIcon = weaponComponent.WeaponIcon;
		// СОХРАНЯЕМ ТЕКУЩИЙ ЦВЕТ НАЧАЛЬНОЙ КНОПКИ
		originalNormalColor = _button.colors.normalColor;
		// Подписываемся на событие открытия меню
		this.weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheelMenu;
	}

	// Этот метод будет вызван при открытии меню
	private void OnOpenWeaponWheelMenu()
	{
		// Определяем активную руку и соответствующее ей оружие
		GameObject activeWeapon = null;
		if (weaponController.isLeftHand)
		{
			activeWeapon = weaponController.LeftHandWeapon;
		}
		else
		{
			activeWeapon = weaponController.RightHandWeapon;
		}

		//Debug.Log("Active Weapon Path: " + (activeWeapon != null ? activeWeapon.name : "NULL"));
		//Debug.Log("Weapon Prefab Path: " + (WeaponPrefab != null ? WeaponPrefab.name : "NULL"));
		// Если активная кнопка соответствует данному оружию, красим её в нужный цвет
		if (activeWeapon == WeaponPrefab)
		{
			
			ColorBlock colors = _button.colors;
			colors.normalColor = new Color(209f / 255f, 138f / 255f, 36f / 255f); // Золотистый цвет
			_button.colors = colors;
		}
		else
		{
			// В противном случае возвращаем кнопку к исходному цвету
			ColorBlock colors = _button.colors;
			colors.normalColor = originalNormalColor; // Используем сохранённый оригинальный цвет
			_button.colors = colors;
		}
	}

	public void HoverEnter()
	{
		weaponWheelController.WeaponText.text = WeaponName;
	}

	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	}

	private void SelectWeapon()
	{
		// Окраска кнопки в красный при выборе оружия
		ColorBlock colors = _button.colors;
		colors.normalColor = new Color(209f / 255f, 138f / 255f, 36f / 255f);
		_button.colors = colors;

		// Логика выбора оружия
		if (weaponController.isAbleToUseRightWeapon)
		{
			weaponController.SelectWeapon(WeaponPrefab);
		}
		else
		{
			if (weaponController.isLeftHand && weaponController.isAbleToUseLeftWeapon)
			{
				weaponController.SelectWeapon(WeaponPrefab);
			}
		}
	}

	// Не забываем отписываться от события при уничтожении объекта
	private void OnDestroy()
	{
		weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheelMenu;
	}
}