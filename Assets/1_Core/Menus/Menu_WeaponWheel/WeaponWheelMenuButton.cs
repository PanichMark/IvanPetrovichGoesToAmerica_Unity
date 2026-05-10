using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponWheelMenuButton : MonoBehaviour
{
	private PlayerWeaponController _weaponController;
	private WeaponWheelMenuController _weaponWheelController;
	private GameObject _WeaponPrefab;
	private string _WeaponName;
	private Sprite _WeaponIcon;
	private Button _button;

	private Color _originalNormalColor;

	private GameObject _currentWeapon;

	private GameObject _previousWeapon;

	public void Initialize(PlayerWeaponController weaponController, WeaponWheelMenuController weaponWheelController, GameObject weaponPrefab, WeaponAbstract weaponComponent)
	{
		_weaponController = weaponController;
		_weaponWheelController = weaponWheelController;
		_WeaponPrefab = weaponPrefab;
		_WeaponName = weaponComponent.WeaponNameUI;
		_WeaponIcon = weaponComponent.WeaponIcon;

		var button = GetComponent<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		button.onClick.AddListener(() => this._weaponWheelController.ShowWeaponIcon());

		_button = button; 

		_originalNormalColor = _button.colors.normalColor;

		_weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheel;

		_weaponController.OnWeaponChanged += OnWeaponChange;
	}

	private void OnOpenWeaponWheel(string activeHand)
	{
		if (activeHand == "left")
		{
			_previousWeapon = _weaponController.LeftHandWeapon;
		}
		else
		{
			_previousWeapon = _weaponController.RightHandWeapon;
		}
		HandleOnWeaponChanged(activeHand);
	}

	private void OnWeaponChange(string activeHand)
	{
		HandleOnWeaponChanged(activeHand);
		_previousWeapon = _currentWeapon;
	}

	private void HandleOnWeaponChanged(string activeHand)
	{
		if (activeHand == "left")
		{
			_currentWeapon = _weaponController.LeftHandWeapon;
		}
		else
		{
			_currentWeapon = _weaponController.RightHandWeapon;
		}
		if (_currentWeapon != _previousWeapon)
		{
			UpdateButtonColor(_currentWeapon);
		}
		else
		{
			UpdateButtonColor(_previousWeapon);
		}
	}

	private void UpdateButtonColor(GameObject activeWeapon)
	{
		if (activeWeapon == null)
		{
			ChangeButtonColor(_originalNormalColor);
			return;
		}

		WeaponAbstract activeWeaponComponent = activeWeapon.GetComponent<WeaponAbstract>();

		WeaponAbstract buttonWeaponComponent = _WeaponPrefab.GetComponent<WeaponAbstract>();

		if (activeWeaponComponent != null && buttonWeaponComponent != null)
		{
			if (activeWeaponComponent.WeaponNameSystem == buttonWeaponComponent.WeaponNameSystem)
			{
				ChangeButtonColor(new Color(209f / 255f, 138f / 255f, 36f / 255f));
			}
			else
			{
				ChangeButtonColor(_originalNormalColor);
			}
		}
	}

	public void HoverEnter()
	{
		_weaponWheelController.WeaponIcon.gameObject.SetActive(true);
		_weaponWheelController.WeaponText.text = _WeaponName;
		_weaponWheelController.WeaponIcon.sprite = _WeaponIcon;
	}

	public void HoverExit()
	{
		_weaponWheelController.ShowWeaponName();
		_weaponWheelController.ShowWeaponIcon();
	}

	private void SelectWeapon()
	{
		if (_weaponController.isAbleToUseRightWeapon || (_weaponController.isLeftHand && _weaponController.isAbleToUseLeftWeapon))
		{
			_weaponController.SelectWeapon(_WeaponPrefab);
		}
	}

	private void OnDestroy()
	{
		_weaponController.OnWeaponChanged -= OnWeaponChange;
		_weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
	}

	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}
}