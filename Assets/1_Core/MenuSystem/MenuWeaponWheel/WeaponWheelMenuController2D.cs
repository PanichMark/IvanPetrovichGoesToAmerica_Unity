using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelMenuController2D : MonoBehaviour, IWeaponWheelMenuController
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _weaponWheelSegment;                    
	private GameObject _weaponWheelMenuCanvas;
	private Bootstrap _bootstrap;
	private GameObject _textWeaponAmmoMagazineNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoMagazineNumber;

	private GameObject _textWeaponAmmoReserveNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoReserveNumber;
	private GameObject _textWeaponAmmoSeparator;
	private TextMeshProUGUI _textComponentWeaponAmmoSeparator;
	private GameObject _weaponWheelRadius;
	private GameObject _weaponIconImage;
	public TextMeshProUGUI WeaponText { get; private set; }            
	public TextMeshProUGUI WeaponWheelName { get; private set; }       

	private List<GameObject> _wheelSegments = new List<GameObject>();
	private bool _isWeaponLeftHand = false;
	private WeaponRangedAbstract _weaponRangedAbstractRight;
	private WeaponRangedAbstract _weaponRangedAbstractLeft;
	public delegate void WeaponWheelMenuHandler(WeaponHandsEnum activeHand);
	public event WeaponWheelMenuHandler OnOpenWeaponWheelMenu;

	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private PlayerWeaponController _weaponController;
	private PlayerBehaviourController _playerBehaviour;
	private MenuManager _menuManager;

	private string _weaponWheelHandRight;
	private string _weaponWheelHandLeft;

	private bool _isHoveringOverButton;
	private bool _previousRightHandPressed = false;
	private bool _previousLeftHandPressed = false;
	private float _radius = 130;

	public event System.Action<int> OnSegmentSelected;

	public Image WeaponIcon {  get; private set; }

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		GameObject weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel,
		GameObject PlayerCamera)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_playerBehaviour = playerBehaviour;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_weaponController = weaponController;
		_menuManager = menuManager;
		_weaponWheelSegment = viewModelMenuWeaponWheel.GameObjectWeaponWheelSegment;
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = viewModelMenuWeaponWheel.TextWeaponWheelWeaponName;
		WeaponWheelName = viewModelMenuWeaponWheel.TextWeaponWheelHandType;
		WeaponIcon = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon.GetComponent<Image>();
		_weaponIconImage = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon;
		_weaponWheelRadius = viewModelMenuWeaponWheel.WeaponWheelRadius;
		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;
		_textComponentWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator.GetComponent<TextMeshProUGUI>();
		_weaponWheelRadius.SetActive(true);
		_weaponIconImage.SetActive(true);
		_weaponWheelMenuCanvas.gameObject.SetActive(false);
		RecreateWheel();
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";
		_weaponController.OnAnyWeaponUnlocked += OnWeaponUnlocked;

		Debug.Log("WeaponWheelMenuController");
	}

	public void HideWeaponAmmo()
	{
		//Debug.Log("SHOW WEAPON AMMO");


		_textWeaponAmmoMagazineNumber.SetActive(false);
		_textWeaponAmmoReserveNumber.SetActive(false);
		_textWeaponAmmoSeparator.SetActive(false);
	}

	public void ShowWeaponAmmo(WeaponRangedAbstract weaponComponent)
	{
		_textWeaponAmmoMagazineNumber.SetActive(true);
		_textWeaponAmmoReserveNumber.SetActive(true);
		_textWeaponAmmoSeparator.SetActive(true);
		
		WeaponsRangedEnum newKey = (WeaponsRangedEnum)System.Enum.Parse(typeof(WeaponsRangedEnum), weaponComponent.WeaponName);

		if (_playerResourcesAmmoManager.WeaponsRangedDictionary.TryGetValue(newKey, out var newData))
		{
			_textComponentWeaponAmmoMagazineNumber.text = newData.MagazineAmmoCurrent.ToString();
		}

		if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(weaponComponent.PlayerWeaponAmmoType, out var ammoData))
		{
			_textComponentWeaponAmmoReserveNumber.text = ammoData.TotalAmmoCurrent.ToString();
		}
	}

	public void ShowWeaponAmmo()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				if (_weaponController.LeftHandWeaponComponent is WeaponRangedAbstract)
				{
					_textWeaponAmmoMagazineNumber.SetActive(true);
					_textWeaponAmmoReserveNumber.SetActive(true);
					_textWeaponAmmoSeparator.SetActive(true);

					_weaponRangedAbstractLeft = _weaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
					_textComponentWeaponAmmoMagazineNumber.text = _weaponRangedAbstractLeft.PlayerMagazineAmmoCurrent.ToString();
					_textComponentWeaponAmmoReserveNumber.text = _weaponRangedAbstractLeft.PlayerAmmoTotalCurrent.ToString();
				}
				else
				{
					_textWeaponAmmoMagazineNumber.SetActive(false);
					_textWeaponAmmoReserveNumber.SetActive(false);
					_textWeaponAmmoSeparator.SetActive(false);
				}
			}
			else
			{
				_textWeaponAmmoMagazineNumber.SetActive(false);
				_textWeaponAmmoReserveNumber.SetActive(false);
				_textWeaponAmmoSeparator.SetActive(false);
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				if (_weaponController.RightHandWeaponComponent is WeaponRangedAbstract)
				{
					_textWeaponAmmoMagazineNumber.SetActive(true);
					_textWeaponAmmoReserveNumber.SetActive(true);
					_textWeaponAmmoSeparator.SetActive(true);

					_weaponRangedAbstractRight = _weaponController.RightHandWeapon.GetComponent<WeaponRangedAbstract>();
					_textComponentWeaponAmmoMagazineNumber.text = _weaponRangedAbstractRight.PlayerMagazineAmmoCurrent.ToString();
					_textComponentWeaponAmmoReserveNumber.text = _weaponRangedAbstractRight.PlayerAmmoTotalCurrent.ToString();
				}
				else
				{
					_textWeaponAmmoMagazineNumber.SetActive(false);
					_textWeaponAmmoReserveNumber.SetActive(false);
					_textWeaponAmmoSeparator.SetActive(false);
				}
			}
			else
			{
				_textWeaponAmmoMagazineNumber.SetActive(false);
				_textWeaponAmmoReserveNumber.SetActive(false);
				_textWeaponAmmoSeparator.SetActive(false);
			}
		}
	}

	public void OnWeaponUnlocked(GameObject weaponPrefab)
	{
		RecreateWheel();
	}
	
	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;
		bool currentRightHandPressed = _inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = _inputDevice.GetKeyLeftHandWeaponWheel();

		if ((currentRightHandPressed != _previousRightHandPressed || currentLeftHandPressed != _previousLeftHandPressed) && _weaponController.HasAnyWeapon)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		_previousRightHandPressed = currentRightHandPressed;
		_previousLeftHandPressed = currentLeftHandPressed;
	}

	public void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandRight);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = false;
			ShowWeaponName();
			ShowWeaponIcon();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandRight;
		}
		else if (leftHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandLeft);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = true;
			ShowWeaponName();
			ShowWeaponIcon();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandLeft;
		}
		else
		{
			HideWeaponWheelMenuCanvas();
		}
	}

	public void CreateWheel()
	{
		List<GameObject> activeWeapons = _weaponController.CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return;

		activeWeapons.Sort((a, b) =>
		{
			int indexA = _weaponController.ExtractWeaponIndex(a.name);
			int indexB = _weaponController.ExtractWeaponIndex(b.name);
			return indexA.CompareTo(indexB);
		});

		float angleStep = 360f / activeWeapons.Count;

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			GameObject segmentInstance = Instantiate(_weaponWheelSegment);
			segmentInstance.name = $"Segment {i + 1}";

			segmentInstance.transform.SetParent(_weaponWheelMenuCanvas.transform, false);

			Button button = segmentInstance.GetComponent<Button>();
			button.onClick.AddListener(() => OnSegmentSelected?.Invoke(i));

			RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
			buttonRectTransform.sizeDelta = new Vector2(50, 50);

			GameObject iconObject = new GameObject("Icon");
			iconObject.transform.SetParent(button.transform, false);
			iconObject.transform.localPosition = Vector3.zero;
			iconObject.transform.localScale = Vector3.one;

			Image iconImage = iconObject.AddComponent<Image>();
			WeaponAbstract weaponComponent = activeWeapons[i].GetComponent<WeaponAbstract>();
			if (weaponComponent != null)
			{
				iconImage.sprite = weaponComponent.WeaponIcon;
			}
			else
			{
				Debug.LogError($"Отсутствует компонент WeaponClass у объекта {activeWeapons[i]}");
			}

			iconImage.type = Image.Type.Simple;
			iconImage.fillMethod = Image.FillMethod.Horizontal;
			iconImage.fillAmount = 1f;

			RectTransform iconRectTransform = iconObject.GetComponent<RectTransform>();
			iconRectTransform.sizeDelta = new Vector2(50, 50);
			iconRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			iconRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			iconRectTransform.pivot = new Vector2(0.5f, 0.5f);

			float adjustedAngle = i * angleStep + 90f;
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, _radius);
			segmentInstance.transform.localPosition = positionOnCircle;

			WeaponWheelMenuButton2D buttonScript = segmentInstance.GetComponent<WeaponWheelMenuButton2D>();
			if (buttonScript != null)
			{
				buttonScript.Initialize(_localizationManager, _weaponController, this, activeWeapons[i], weaponComponent);
			}

			_wheelSegments.Add(segmentInstance);
		}
	}

	private Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}

	public void RecreateWheel()
	{
		foreach (var seg in _wheelSegments)
			Destroy(seg.gameObject);

		_wheelSegments.Clear();
		CreateWheel();
	}

	private void ShowWeaponWheelMenuCanvas()
	{
		_weaponWheelMenuCanvas.gameObject.SetActive(true);
		_menuManager.OpenWeaponWheelMenu();
	}

	private void HideWeaponWheelMenuCanvas()
	{
		_weaponWheelMenuCanvas.gameObject.SetActive(false);
		if (!_menuManager.IsPauseMenuOpened)
		{
			_menuManager.CloseWeaponWheelMenu();
		}
	}

	public void ShowWeaponIcon()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				WeaponIcon.gameObject.SetActive(true);
				WeaponIcon.sprite = _weaponController.LeftHandWeaponComponent.WeaponIcon;
			}
			else
			{
				if (!_isHoveringOverButton)
				{
					WeaponIcon.gameObject.SetActive(false);
				}
				WeaponIcon.sprite = null;
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				WeaponIcon.gameObject.SetActive(true);
				WeaponIcon.sprite = _weaponController.RightHandWeaponComponent.WeaponIcon;
			}
			else
			{
				if (!_isHoveringOverButton)
				{
					WeaponIcon.gameObject.SetActive(false);
				}
				WeaponIcon.sprite = null;
			}
		}
	}

	public void ShowWeaponName()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.LeftHandWeaponComponent.WeaponNameSystem);
			}
			else
			{
				WeaponText.text = "";
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.RightHandWeaponComponent.WeaponNameSystem);
			}
			else
			{
				WeaponText.text = "";
			}
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";

		ShowWeaponName();
	}

	private void OnDestroy()
	{
		foreach (GameObject segment in _wheelSegments)
		{
			Destroy(segment);
		}

		_wheelSegments.Clear();
	}
}