using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelMenuButton : MonoBehaviour
{
	private PlayerWeaponController weaponController;
	private WeaponWheelMenuController weaponWheelController;
	private GameObject WeaponPrefab;
	private string WeaponName;
	private Button _button;

	private Color originalNormalColor;

	private GameObject currentWeapon;

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
		_button = button; 

		originalNormalColor = _button.colors.normalColor;

		this.weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheel;

		this.weaponController.OnWeaponChanged += OnWeaponChange;
	}

	private void OnOpenWeaponWheel(string activeHand)
	{
		if (activeHand == "left")
		{
			previousWeapon = weaponController.LeftHandWeapon;
		}
		else
		{
			previousWeapon = weaponController.RightHandWeapon;
		}
		HandleOnWeaponChanged(activeHand);
	}

	private void OnWeaponChange(string activeHand)
	{
		HandleOnWeaponChanged(activeHand);
		previousWeapon = currentWeapon;
	}

	private void HandleOnWeaponChanged(string activeHand)
	{
		if (activeHand == "left")
		{
			currentWeapon = weaponController.LeftHandWeapon;
		}
		else
		{
			currentWeapon = weaponController.RightHandWeapon;
		}
		if (currentWeapon != previousWeapon)
		{
			UpdateButtonColor(currentWeapon);
		}
		else
		{
			UpdateButtonColor(previousWeapon);
		}
	}

	private void UpdateButtonColor(GameObject activeWeapon)
	{
		if (activeWeapon == null)
		{
			ChangeButtonColor(originalNormalColor);
			return;
		}

		WeaponAbstract activeWeaponComponent = activeWeapon.GetComponent<WeaponAbstract>();

		WeaponAbstract buttonWeaponComponent = WeaponPrefab.GetComponent<WeaponAbstract>();

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

	private void OnDestroy()
	{
		weaponController.OnWeaponChanged -= OnWeaponChange;
		this.weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
	}

	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}
}