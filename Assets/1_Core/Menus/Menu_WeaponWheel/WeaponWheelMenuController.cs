using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelMenuController : MonoBehaviour
{
	private GameObject _weaponWheelSegment;                    
	private GameObject _weaponWheelMenuCanvas;            
	public TextMeshProUGUI WeaponText { get; private set; }            
	public TextMeshProUGUI WeaponWheelName { get; private set; }       

	private List<GameObject> _wheelSegments = new List<GameObject>();
	private bool _isWeaponWheelActive = false;
	private bool _isWeaponLeftHand = false;

	public delegate void WeaponWheelMenuHandler(string activeHand);
	public event WeaponWheelMenuHandler OnOpenWeaponWheelMenu;

	private IInputDevice _inputDevice;
	private PlayerWeaponController _weaponController;
	private PlayerBehaviour _playerBehaviour;
	private MenuManager _menuManager;

	private bool _isHoveringOverButton;
	private bool _previousRightHandPressed = false;
	private bool _previousLeftHandPressed = false;
	private bool _isInitialized = false;
	private float _radius = 130;

	public event System.Action<int> OnSegmentSelected;

	public Image WeaponIcon {  get; private set; }

	public void Initialize(
		IInputDevice inputDevice,
		MenuManager menuManager,
		PlayerBehaviour playerBehaviour,
		PlayerWeaponController weaponController,
		GameObject weaponWheelMenuCanvas,
		GameObject wheelSegmentPrefab,
		TextMeshProUGUI weaponText,
		Image weaponIconBig,
		TextMeshProUGUI weaponWheelName)
	{
		_inputDevice = inputDevice;
		_playerBehaviour = playerBehaviour;
		_weaponController = weaponController;
		_menuManager = menuManager;
		_weaponWheelSegment = wheelSegmentPrefab;
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = weaponText;
		WeaponWheelName = weaponWheelName;
		WeaponIcon = weaponIconBig;

		_weaponWheelMenuCanvas.gameObject.SetActive(false);
		_isInitialized = true;

		_weaponController.OnWeaponUnlocked += OnWeaponUnlocked;

		Debug.Log("WeaponWheel Initialized");
	}

	private void OnWeaponUnlocked(GameObject weaponPrefab)
	{
		RecreateWheel();
	}
	
	void Update()
	{
		if (!_isInitialized)
			return;
		bool currentRightHandPressed = _inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = _inputDevice.GetKeyLeftHandWeaponWheel();

		if ((currentRightHandPressed != _previousRightHandPressed || currentLeftHandPressed != _previousLeftHandPressed) && _weaponController.hasAnyWeapon)
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
			OnOpenWeaponWheelMenu?.Invoke("right");
			EnableWeaponWheelMenuCanvas();
			_isWeaponWheelActive = true;
			_isWeaponLeftHand = false;
			ShowWeaponName();
			ShowWeaponIcon();
			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}
		else if (leftHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke("left");
			EnableWeaponWheelMenuCanvas();
			_isWeaponWheelActive = true;
			_isWeaponLeftHand = true;
			ShowWeaponName();
			ShowWeaponIcon();
			WeaponWheelName.text = "ЛЕВАЯ РУКА";
		}
		else
		{
			DisableWeaponWheelMenuCanvas();
			_isWeaponWheelActive = false;
		}
	}

	void CreateWheel()
	{
		List<GameObject> activeWeapons = _weaponController.CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return;

		activeWeapons.Sort((a, b) =>
		{
			int indexA = int.Parse(a.name.Split('_')[1]);
			int indexB = int.Parse(b.name.Split('_')[1]);
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

			WeaponWheelMenuButton buttonScript = segmentInstance.GetComponent<WeaponWheelMenuButton>();
			if (buttonScript != null)
			{
				buttonScript.Initialize(_weaponController, this, activeWeapons[i], weaponComponent);
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

	private void EnableWeaponWheelMenuCanvas()
	{
		_weaponWheelMenuCanvas.gameObject.SetActive(true);
		_menuManager.OpenWeaponWheelMenu();
	}

	private void DisableWeaponWheelMenuCanvas()
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
				WeaponIcon.sprite = _weaponController.leftHandWeaponComponent.WeaponIcon;
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
				WeaponIcon.sprite = _weaponController.rightHandWeaponComponent.WeaponIcon;
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
				WeaponText.text = _weaponController.leftHandWeaponComponent.WeaponNameUI;
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
				WeaponText.text = _weaponController.rightHandWeaponComponent.WeaponNameUI;
			}
			else
			{
				WeaponText.text = "";
			}
		}
	}
}