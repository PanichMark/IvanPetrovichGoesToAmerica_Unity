using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponWheelMenuController3D : MonoBehaviour, IWeaponWheelMenuController
{
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private GameObject _weaponWheelMenuCanvas;
	private Bootstrap _bootstrap;
	private GameObject _playerCamera;
	private int _CurrentShowWeaponIndex;
	private GameObject _weaponToSelect;
	private WeaponAbstract _weaponToSelectComponent;
	private GameObject _textWeaponAmmoMagazineNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoMagazineNumber;
	private GameObject _textWeaponAmmoReserveNumber;
	private TextMeshProUGUI _textComponentWeaponAmmoReserveNumber;
	private bool _isRotating = false;
	private GameObject _textWeaponAmmoSeparator;
	private Quaternion _targetRotation; 
	private float _rotationSpeed = 40f;  
	private GameObject _weaponModelsContainer;
	private List<GameObject> _weaponModels3D = new List<GameObject>();
	public TextMeshProUGUI WeaponText { get; private set; }
	public TextMeshProUGUI WeaponWheelName { get; private set; }
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
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = viewModelMenuWeaponWheel.TextWeaponWheelWeaponName.GetComponent<TextMeshProUGUI>();
		_weaponWheelRadius = viewModelMenuWeaponWheel.WeaponWheelRadius;
		WeaponWheelName = viewModelMenuWeaponWheel.TextWeaponWheelHandType.GetComponent<TextMeshProUGUI>();
		_weaponIconImage = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon;

		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetComponent<TextMeshProUGUI>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;

		_weaponWheelRadius.SetActive(false);
		_weaponIconImage.SetActive(false);
		RecreateWheel();
		_weaponWheelMenuCanvas.gameObject.SetActive(false);
		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponController.OnAnyWeaponUnlocked += OnWeaponUnlocked;

		Debug.Log("WeaponWheelMenuController3D Initialized");
	}

	public void HideWeaponAmmo()
	{
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

		if (_weaponModelsContainer != null && _weaponModelsContainer.activeSelf && !_isRotating)
		{
			float scrollInput = Input.GetAxis("Mouse ScrollWheel");

			if (scrollInput != 0 && _weaponModels3D.Count > 0)
			{
				_isRotating = true;

				Quaternion[] worldRotations = new Quaternion[_weaponModelsContainer.transform.childCount];
				int i = 0;
				foreach (Transform weaponModel in _weaponModelsContainer.transform)
				{
					worldRotations[i] = weaponModel.rotation;
					i++;
				}

				float angleForOneStep = 360f / _weaponModels3D.Count;
				float direction = Mathf.Sign(scrollInput);

				Quaternion startRotation = _weaponModelsContainer.transform.rotation;
				_targetRotation = startRotation * Quaternion.Euler(0, angleForOneStep * direction, 0);

				StartCoroutine(RotateWeaponModels(worldRotations));
			}
		}
	}

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
			yield return null; 
		}

		_weaponModelsContainer.transform.rotation = _targetRotation;

		List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

		if (weaponsList.Count > 0)
		{
			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			float anglePerSegment = 360f / weaponsList.Count;
			float totalRotatedAngleY = -_weaponModelsContainer.transform.localRotation.eulerAngles.y + 180;
			if (totalRotatedAngleY < 0) totalRotatedAngleY += 360;

			_CurrentShowWeaponIndex = Mathf.RoundToInt(totalRotatedAngleY / anglePerSegment);
			_CurrentShowWeaponIndex %= weaponsList.Count;

			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();
			if (_weaponToSelectComponent is WeaponRangedAbstract)
			{
				ShowWeaponAmmo(_weaponToSelectComponent as WeaponRangedAbstract);
			}
			else
			{
				HideWeaponAmmo();
			}
		}

		ShowWeaponName();

		_isRotating = false;
	}

	public void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			_isWeaponLeftHand = false;
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandRight);
			ShowWeaponWheelMenuCanvas();
		
			ShowWeaponName();
			ShowWeaponPrefabs();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandRight;
		}
		else if (leftHandPressed)
		{
			_isWeaponLeftHand = true;
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandsEnum.HandLeft);
			ShowWeaponWheelMenuCanvas();
		
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

	private void SetWheelLayerToIgnorePostProcessing()
	{
		if (_weaponModelsContainer == null) return; 

		int ignorePostLayer = LayerMask.NameToLayer("IgnorePostProcessing");
		_weaponModelsContainer.layer = ignorePostLayer;

		foreach (Transform child in _weaponModelsContainer.GetComponentsInChildren<Transform>(true))
		{
			child.gameObject.layer = ignorePostLayer;
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

		float containerSpawnDistance = 2.0f;

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

			SetWheelLayerToIgnorePostProcessing();

			float angleStep = 360f / activeWeapons.Count;
			float angle = i * angleStep;

			Vector3 localPosition = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * angle) * _radius,
				0,
				Mathf.Cos(Mathf.Deg2Rad * angle) * _radius
			);

			modelInstance.transform.SetParent(_weaponModelsContainer.transform, true);
			modelInstance.transform.localPosition = localPosition;
			modelInstance.transform.localRotation = Quaternion.identity;

			_weaponModels3D.Add(modelInstance);

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}
		}

		_weaponModelsContainer.transform.rotation = _playerCamera.transform.rotation * Quaternion.Euler(30, 180, 0);

		HideWeaponPrefabs();
	}

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
		List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

		if (weaponsList.Count > 0)
		{
			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			float anglePerSegment = 360f / weaponsList.Count;
			float totalRotatedAngleY = -_weaponModelsContainer.transform.localRotation.eulerAngles.y + 180;
			if (totalRotatedAngleY < 0) totalRotatedAngleY += 360;

			_CurrentShowWeaponIndex = Mathf.RoundToInt(totalRotatedAngleY / anglePerSegment);
			_CurrentShowWeaponIndex %= weaponsList.Count;
			RotateToEquippedWeapon();

			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();
			if (_weaponToSelectComponent is WeaponRangedAbstract)
			{
				ShowWeaponAmmo(_weaponToSelectComponent as WeaponRangedAbstract);
			}
			else
			{
				HideWeaponAmmo();
			}
		}
	}

	private void RotateToEquippedWeapon()
	{

		if (_isWeaponLeftHand && _weaponController.LeftHandWeapon != null)
		{
			List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.LeftHandWeaponComponent;

			int targetIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++)
			{
				if (weaponsList[i].GetComponent<WeaponAbstract>().WeaponNameSystem == activeWeapon.WeaponNameSystem)
				{
					targetIndex = i;
					break;
				}
			}

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo(); 
			}

		}

		if (_isWeaponLeftHand && _weaponController.LeftHandWeapon == null)
		{
			List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});
			WeaponAbstract activeWeapon = _weaponController.LeftHandWeaponComponent;

			int targetIndex = 0;

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo();
			}
		}
		if (!_isWeaponLeftHand && _weaponController.RightHandWeapon != null)
		{
			List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.RightHandWeaponComponent;

			int targetIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++)
			{
				if (weaponsList[i].GetComponent<WeaponAbstract>().WeaponNameSystem == activeWeapon.WeaponNameSystem)
				{
					targetIndex = i;
					break;
				}
			}

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon);
			}
			else
			{
				HideWeaponAmmo(); 
			}
		}

		if (!_isWeaponLeftHand && _weaponController.RightHandWeapon == null)
		{
			List<GameObject> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.RightHandWeaponComponent;

			int targetIndex = 0;
	
			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetComponent<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo(); 
			}
		}
	}

	private void HideWeaponWheelMenuCanvas()
	{
		if (_weaponController.IsAbleToUseRightWeapon || (_weaponController.IsLeftHand && _weaponController.IsAbleToUseLeftWeapon))
		{
			_weaponController.SelectWeapon(_weaponToSelect);
		}

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
			if (_weaponController.LeftHandWeapon != null && _weaponController.LeftHandWeapon == _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.LeftHandWeaponComponent.WeaponNameSystem);
			}
			else if (_weaponController.LeftHandWeapon != _weaponToSelect) 
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponToSelectComponent.WeaponNameSystem);
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null && _weaponController.RightHandWeapon == _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.RightHandWeaponComponent.WeaponNameSystem);
			}
			else if(_weaponController.RightHandWeapon != _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponToSelectComponent.WeaponNameSystem);
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
		Destroy(_weaponModelsContainer);
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_weaponController.OnAnyWeaponUnlocked -= OnWeaponUnlocked;
	}
}