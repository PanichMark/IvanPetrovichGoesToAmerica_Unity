using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponWheelMenuButton : MonoBehaviour
{
	private PlayerWeaponController weaponController;
	private WeaponWheelMenuController weaponWheelController;
	private GameObject WeaponPrefab;
	private string WeaponName;
	private Sprite WeaponIcon;
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
		WeaponIcon = weaponComponent.WeaponIcon;

		var button = GetComponent<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		button.onClick.AddListener(() => this.weaponWheelController.ShowWeaponIcon());

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
		weaponWheelController.WeaponIcon.gameObject.SetActive(true);
		weaponWheelController.WeaponText.text = WeaponName;
		weaponWheelController.WeaponIcon.sprite = WeaponIcon;
	}

	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
		weaponWheelController.ShowWeaponIcon();
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
		weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
	}

	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}
}