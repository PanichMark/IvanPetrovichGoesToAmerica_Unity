using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelMenuController3D : MonoBehaviour
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
	// --- Новые поля для 3D-колеса ---
	private GameObject _weaponModelsContainer; // Родительский объект для всех 3D-моделей оружия
	private List<GameObject> _weaponModels3D = new List<GameObject>(); // Список самих моделей
	private int _selectedIndex3D = 0; // Индекс выбранного оружия в 3D-колесе
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

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		GameObject weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel)
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

		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;
		_textComponentWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator.GetComponent<TextMeshProUGUI>();

		_weaponWheelMenuCanvas.gameObject.SetActive(false);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

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

	private void OnWeaponUnlocked(GameObject weaponPrefab)
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

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandRight);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = false;
			ShowWeaponName();

			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandRight;
		}
		else if (leftHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandLeft);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = true;
			ShowWeaponName();

			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandLeft;
		}
		else
		{
			HideWeaponWheelMenuCanvas();
		}
	}

	void CreateWheel()
	{
		List<GameObject> activeWeapons = _weaponController.CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return;

		activeWeapons.Sort((a, b) => string.Compare(a.name, b.name));

		float angleStep = 360f / activeWeapons.Count;
		float radius = 1.5f;

		// Создаем или очищаем контейнер
		if (_weaponModelsContainer != null)
		{
			// Очистка старых моделей
			foreach (Transform child in _weaponModelsContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}
		else
		{
			// Создание контейнера, если его нет
			_weaponModelsContainer = new GameObject("WeaponModels_Container");
			_weaponModelsContainer.transform.position = Vector3.zero;
			_weaponModelsContainer.transform.SetParent(this.transform);
		}

		_weaponModels3D.Clear();

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			// Создаем экземпляр модели
			GameObject modelInstance = Instantiate(activeWeapons[i], _weaponModelsContainer.transform);

			// Настраиваем позицию, вращение и масштаб
			modelInstance.transform.localRotation = Quaternion.Euler(90, i * angleStep - 90, 0);
			modelInstance.transform.localPosition = new Vector3(0, 0, radius);
			modelInstance.transform.localScale = Vector3.one * 0.5f;

			// --- ИМЕННО ТО, ЧТО ТЫ ПРОСИЛ ---
			// Находим и УДАЛЯЕМ компонент WeaponAbstract с клона
			WeaponAbstract weaponComp = modelInstance.GetComponent<WeaponAbstract>();
			if (weaponComp != null)
			{
				// Эта строка безвозвратно удаляет компонент с объекта
				Destroy(weaponComp);
			}

			_weaponModels3D.Add(modelInstance);
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
}