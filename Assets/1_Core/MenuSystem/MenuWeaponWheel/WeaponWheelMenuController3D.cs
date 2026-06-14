using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WeaponWheelMenuController3D : MonoBehaviour
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _weaponWheelSegment;
	private GameObject _weaponWheelMenuCanvas;
	private int _pendingSelectionIndex = -1;
	private Bootstrap _bootstrap;
	private GameObject _playerCamera;
	private GameObject _textWeaponAmmoMagazineNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoMagazineNumber;
	private GameObject _textWeaponAmmoReserveNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoReserveNumber;
	// Флаг: идет ли сейчас вращение?
	private bool _isRotating = false;

	// Ссылка на активную корутину, чтобы можно было её остановить
	private Coroutine _activeRotationCoroutine;
	private GameObject _textWeaponAmmoSeparator;
	private TextMeshProUGUI _textComponentWeaponAmmoSeparator;
	// --- Новые поля для 3D-колеса ---
	private Quaternion _targetRotation; // Конечная цель вращения
	private float _rotationSpeed = 40f;   // Скорость вращения (градусов в секунду)
	private GameObject _weaponModelsContainer; // Родительский объект для всех 3D-моделей оружия
	private List<GameObject> _weaponModels3D = new List<GameObject>(); // Список самих моделей
	private int _selectedIndex3D = 0; // Индекс выбранного оружия в 3D-колесе
	public TextMeshProUGUI WeaponText { get; private set; }
	public TextMeshProUGUI WeaponWheelName { get; private set; }
	private float _scrollAccumulator = 0f;
	private GameObject _weaponWheelRadius;

	private List<GameObject> _wheelSegments = new List<GameObject>();
	private bool _isWeaponLeftHand = false;
	private WeaponRangedAbstract _weaponRangedAbstractRight;
	private WeaponRangedAbstract _weaponRangedAbstractLeft;
	public delegate void WeaponWheelMenuHandler(WeaponHandsEnum activeHand);
	public event WeaponWheelMenuHandler OnOpenWeaponWheelMenu;
	private GameObject _weaponIconImage;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private PlayerWeaponController _weaponController;
	private PlayerBehaviourController _playerBehaviour;
	private MenuManager _menuManager;

	private string _weaponWheelHandRight;
	private string _weaponWheelHandLeft;

	private bool _previousRightHandPressed = false;
	private bool _previousLeftHandPressed = false;
	private float _radius = 1f;

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
		_playerCamera = PlayerCamera;
		_weaponWheelSegment = viewModelMenuWeaponWheel.GameObjectWeaponWheelSegment;
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = viewModelMenuWeaponWheel.TextWeaponWheelWeaponName;
		_weaponWheelRadius = viewModelMenuWeaponWheel.WeaponWheelRadius;
		WeaponWheelName = viewModelMenuWeaponWheel.TextWeaponWheelHandType;
		_weaponIconImage = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon;

		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;
		_textComponentWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator.GetComponent<TextMeshProUGUI>();

		_weaponWheelRadius.SetActive(false);
		_weaponIconImage.SetActive(false);

		_weaponWheelMenuCanvas.gameObject.SetActive(false);

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		//_menuManager.OnOpenPauseMenu += SetWheelLayerToDefault;
		//_menuManager.OnClosePauseMenu += SetWheelLayerToIgnorePostProcessing;
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

		// Обработка открытия/закрытия колеса
		if ((currentRightHandPressed != _previousRightHandPressed || currentLeftHandPressed != _previousLeftHandPressed) && _weaponController.HasAnyWeapon)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		_previousRightHandPressed = currentRightHandPressed;
		_previousLeftHandPressed = currentLeftHandPressed;


		// --- НОВАЯ ЛОГИКА ВРАЩЕНИЯ ---
		if (_weaponModelsContainer != null && _weaponModelsContainer.activeSelf && !_isRotating)
		{
			float scrollInput = Input.GetAxis("Mouse ScrollWheel");

			// Проверяем наличие прокрутки и отсутствие текущей анимации
			if (scrollInput != 0 && _weaponModels3D.Count > 0)
			{
				// Блокируем новые нажатия
				_isRotating = true;

				// Сохраняем текущие мировые повороты моделей
				Quaternion[] worldRotations = new Quaternion[_weaponModelsContainer.transform.childCount];
				int i = 0;
				foreach (Transform weaponModel in _weaponModelsContainer.transform)
				{
					worldRotations[i] = weaponModel.rotation;
					i++;
				}

				// Вычисляем шаг и направление
				float angleForOneStep = 360f / _weaponModels3D.Count;
				float direction = Mathf.Sign(scrollInput);

				// Запоминаем текущий поворот контейнера и целевой поворот
				Quaternion startRotation = _weaponModelsContainer.transform.rotation;
				_targetRotation = startRotation * Quaternion.Euler(0, angleForOneStep * direction, 0);

				// Запускаем корутину для плавной анимации
				StartCoroutine(RotateWeaponModels(worldRotations));
			}
		}
	}
	// Корутина для плавного вращения
	private IEnumerator RotateWeaponModels(Quaternion[] savedWorldRotations)
	{
		Quaternion startRotation = _weaponModelsContainer.transform.rotation;
		float elapsedTime = 0f;

		while (elapsedTime < 1f)
		{
			elapsedTime += Time.deltaTime * _rotationSpeed;
			_weaponModelsContainer.transform.rotation = Quaternion.Slerp(startRotation, _targetRotation, elapsedTime);

			int index = 0;
			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = savedWorldRotations[index];
				index++;
			}
			yield return null; // Ждем следующего кадра
		}

		// Гарантируем точное попадание в конечную точку (защита от погрешностей)
		_weaponModelsContainer.transform.rotation = _targetRotation;

		_isRotating = false;
	}
	// --- КОНЕЦ НОВОЙ ЛОГИКИ ---

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandRight);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = false;
			ShowWeaponName();
			ShowWeaponPrefabs();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandRight;
		}
		else if (leftHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandLeft);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = true;
			ShowWeaponName();
			ShowWeaponPrefabs();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandLeft;
		}
		else
		{
			HideWeaponWheelMenuCanvas();
			HideWeaponPrefabs();
		}
	}
	// Устанавливает слой "IgnorePostProcessing" на контейнер оружия и все его содержимое
	private void SetWheelLayerToIgnorePostProcessing()
	{
		if (_weaponModelsContainer == null) return; // Защита от ошибки

		int ignorePostLayer = LayerMask.NameToLayer("IgnorePostProcessing");
		_weaponModelsContainer.layer = ignorePostLayer;

		foreach (Transform child in _weaponModelsContainer.GetComponentsInChildren<Transform>(true))
		{
			child.gameObject.layer = ignorePostLayer;
		}
	}

	// Устанавливает слой "Default" на контейнер оружия и все его содержимое
	private void SetWheelLayerToDefault()
	{
		if (_weaponModelsContainer == null) return; // Защита от ошибки

		int defaultLayer = LayerMask.NameToLayer("Default");
		_weaponModelsContainer.layer = defaultLayer;

		foreach (Transform child in _weaponModelsContainer.GetComponentsInChildren<Transform>(true))
		{
			child.gameObject.layer = defaultLayer;
		}
	}


	void CreateWheel()
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

		float containerSpawnDistance = 2.0f;


		// Создаем или очищаем контейнер
		if (_weaponModelsContainer != null)
		{
			foreach (Transform child in _weaponModelsContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}
		else
		{
			_weaponModelsContainer = new GameObject("WeaponModels_Container");
			_weaponModelsContainer.transform.SetParent(_playerCamera.transform, false);
			_weaponModelsContainer.transform.position = _playerCamera.transform.TransformPoint(new Vector3(0, 0.25f, containerSpawnDistance));
		}

		_weaponModels3D.Clear();

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			GameObject modelInstance = Instantiate(activeWeapons[i], _weaponModelsContainer.transform);

			// --- НОВОЕ: Устанавливаем слой для самого объекта и ВСЕХ его дочерних объектов ---
			SetWheelLayerToIgnorePostProcessing();


			// Угол для равномерного распределения
			float angleStep = 360f / activeWeapons.Count;
			float angle = i * angleStep;

			// Вычисляем позицию на окружности в локальных координатах контейнера
			Vector3 localPosition = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * angle) * _radius,
				0, // Высота (Y)
				Mathf.Cos(Mathf.Deg2Rad * angle) * _radius
			);

			WeaponAbstract weaponComp = modelInstance.GetComponent<WeaponAbstract>();
			if (weaponComp != null)
			{
				//Destroy(weaponComp);
			}


			// Устанавливаем позицию и поворот
			modelInstance.transform.SetParent(_weaponModelsContainer.transform, true); // true - чтобы сохранить world-space позицию
			modelInstance.transform.localPosition = localPosition;
			modelInstance.transform.localRotation = Quaternion.identity; // Сбрасываем поворот модели, чтобы она смотрела "вверх" относительно контейнера

			_weaponModels3D.Add(modelInstance);


			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}
		}

		_weaponModelsContainer.transform.rotation =
			_playerCamera.transform.rotation *
			Quaternion.Euler(30, 180, 0);

		HideWeaponPrefabs();
	}

	// Скрывает все 3D-модели оружия (созданные префабы)
	private void HideWeaponPrefabs()
	{
		foreach (GameObject weaponModel in _weaponModels3D)
		{
			if (weaponModel != null)
			{
				weaponModel.SetActive(false);
			}
		}
	}

	// Показывает все 3D-модели оружия (созданные префабы)
	private void ShowWeaponPrefabs()
	{
		foreach (GameObject weaponModel in _weaponModels3D)
		{
			if (weaponModel != null)
			{
				weaponModel.SetActive(true);
			}
		}
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
		//ShowWeaponPrefabs();
	}



	private void HideWeaponWheelMenuCanvas()
	{
		// 1. Сначала получаем и сортируем список ОДИН РАЗ.
		List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

		if (weaponsList.Count > 0)
		{
			// Сортируем его так же, как это делается в CreateWheel
			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			// 2. Теперь вычисляем нужный индекс на основе угла поворота.
			float anglePerSegment = 360f / weaponsList.Count;
			float totalRotatedAngleY = -_weaponModelsContainer.transform.localRotation.eulerAngles.y + 180;
			if (totalRotatedAngleY < 0) totalRotatedAngleY += 360;

			int indexToSelect = Mathf.RoundToInt(totalRotatedAngleY / anglePerSegment);
			indexToSelect %= weaponsList.Count; // Страховка от выхода за границы

			//Debug.Log($"[Weapon Wheel] Closing wheel. Total Angle: {totalRotatedAngleY}, Index: {indexToSelect}");

			// 3. Выбираем оружие из УЖЕ ОТСОРТИРОВАННОГО списка.
			GameObject weaponToSelect = weaponsList[indexToSelect];
			string weaponNameForLog = weaponToSelect.name;

			//Debug.Log($"[Weapon Wheel] Selecting weapon at sorted index {indexToSelect}: {weaponNameForLog}");
			if (_weaponController.IsAbleToUseRightWeapon || (_weaponController.IsLeftHand && _weaponController.IsAbleToUseLeftWeapon))
			{
				_weaponController.SelectWeapon(weaponToSelect);
			}
			//Debug.Log("[Weapon Wheel] Weapon selection command sent.");
		}

		// Остальной код закрытия меню остается без изменений
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