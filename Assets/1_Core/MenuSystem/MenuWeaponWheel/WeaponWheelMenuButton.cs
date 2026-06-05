using UnityEngine;
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
	LocalizationManager _localizationManager;
	private GameObject _currentWeapon;
	private WeaponAbstract _weaponComponent;
	private GameObject _previousWeapon;

	public void Initialize(LocalizationManager localizationManager, PlayerWeaponController weaponController, WeaponWheelMenuController weaponWheelController, GameObject weaponPrefab, WeaponAbstract weaponComponent)
	{
		_localizationManager = localizationManager;
		_weaponController = weaponController;
		_weaponWheelController = weaponWheelController;
		_WeaponPrefab = weaponPrefab;
		_weaponComponent = weaponComponent;
		_WeaponName = _localizationManager.GetLocalizedString(_weaponComponent.WeaponNameSystem);
		_WeaponIcon = _weaponComponent.WeaponIcon;

		var button = GetComponent<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		button.onClick.AddListener(() => _weaponWheelController.ShowWeaponIcon());

		_button = button; 

		_originalNormalColor = _button.colors.normalColor;

		_weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheel;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponController.OnWeaponChanged += OnWeaponChange;
	}

	private void OnOpenWeaponWheel(WeaponHandsEnum activeHand)
	{
		if (activeHand == WeaponHandsEnum.HandLeft)
		{
			_previousWeapon = _weaponController.LeftHandWeapon;
		}
		else
		{
			_previousWeapon = _weaponController.RightHandWeapon;
		}
		HandleOnWeaponChanged(activeHand);
	}

	private void OnWeaponChange(WeaponHandsEnum activeHand)
	{
		HandleOnWeaponChanged(activeHand);
		_previousWeapon = _currentWeapon;
	}

	private void HandleOnWeaponChanged(WeaponHandsEnum activeHand)
	{
		if (activeHand == WeaponHandsEnum.HandLeft)
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
		if (_weaponController.IsAbleToUseRightWeapon || (_weaponController.IsLeftHand && _weaponController.IsAbleToUseLeftWeapon))
		{
			_weaponController.SelectWeapon(_WeaponPrefab);
		}
	}

	private void OnDestroy()
	{
		_weaponController.OnWeaponChanged -= OnWeaponChange;
		_weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}

	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_WeaponName = _localizationManager.GetLocalizedString(_weaponComponent.WeaponNameSystem);
	}
}