using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
	private GameObject wheelSegmentPrefab;           // Префаб сегмента
	private Transform centerPoint;                   // Центр круга
	private GameObject WeaponWheelMenuCanvas;            // Canvas меню выбора оружия
	public TextMeshProUGUI WeaponText { get; private set; }               // Текущий выбор оружия
	public TextMeshProUGUI WeaponWheelName { get; private set; }       // Название меню (левая/правая рука)

	private List<GameObject> wheelSegments = new List<GameObject>();
	private bool IsWeaponWheelActive = false;
	private bool IsWeaponLeftHand = false;

	private IInputDevice inputDevice;
	private WeaponController weaponController;
	private PlayerBehaviour playerBehaviour;
	private MenuManager menuManager;

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	public float radius = 100f;

	public event System.Action<int> OnSegmentSelected;

	void Start()
	{
		// createWheel(); // Вызывается позже при активации меню
	}

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, WeaponController weaponController,
		GameObject wheelSegmentPrefab, Transform centerPoint, GameObject WeaponWheelMenuCanvas, TextMeshProUGUI WeaponText, TextMeshProUGUI WeaponWheelName)
	{
		this.inputDevice = inputDevice;
		this.playerBehaviour = playerBehaviour;
		this.weaponController = weaponController;
		this.menuManager = menuManager;
		this.wheelSegmentPrefab = wheelSegmentPrefab;
		this.centerPoint = centerPoint;
		this.WeaponWheelMenuCanvas = WeaponWheelMenuCanvas;
		this.WeaponText = WeaponText;
		this.WeaponWheelName = WeaponWheelName;
		Debug.Log("WeaponWheelController Initialized");

		weaponController.OnWeaponUnlocked += OnWeaponUnlocked;
	}

	private void OnWeaponUnlocked(GameObject weaponPrefab)
	{
		RecreateWheel();
	}

	void Update()
	{
		bool currentRightHandPressed = inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = inputDevice.GetKeyLeftHandWeaponWheel();

		if ((currentRightHandPressed != previousRightHandPressed || currentLeftHandPressed != previousLeftHandPressed) && weaponController.hasAnyWeapon)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		previousRightHandPressed = currentRightHandPressed;
		previousLeftHandPressed = currentLeftHandPressed;
	}

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			EnableWeaponWheelMenuCanvas("right");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = false;
			ShowWeaponName();
			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}
		else if (leftHandPressed)
		{
			EnableWeaponWheelMenuCanvas("left");
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = true;
			ShowWeaponName();
			WeaponWheelName.text = "ЛЕВАЯ РУКА";
		}
		else
		{
			DisableWeaponWheelMenuCanvas(!IsWeaponLeftHand);
			IsWeaponWheelActive = false;
		}
	}

	void CreateWheel()
	{
		List<GameObject> activeWeapons = weaponController.CollectActiveWeapons();

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
			GameObject segmentInstance = Instantiate(wheelSegmentPrefab);
			segmentInstance.transform.SetParent(centerPoint.parent);
			segmentInstance.name = $"Segment {i + 1}";

			Button button = segmentInstance.GetComponent<Button>();
			button.onClick.AddListener(() => OnSegmentSelected?.Invoke(i));

			GameObject iconObject = new GameObject("Icon");
			iconObject.transform.SetParent(button.transform, false);
			iconObject.transform.localPosition = Vector3.zero;
			iconObject.transform.localScale = Vector3.one;

			Image iconImage = iconObject.AddComponent<Image>();
			WeaponClass weaponComponent = activeWeapons[i].GetComponent<WeaponClass>();
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
			iconRectTransform.sizeDelta = new Vector2(100, 100);
			iconRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			iconRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			iconRectTransform.pivot = new Vector2(0.5f, 0.5f);

			float adjustedAngle = i * angleStep + 90f;
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, radius);
			segmentInstance.transform.position = centerPoint.position + positionOnCircle;

			WeaponWheelButton buttonScript = segmentInstance.GetComponent<WeaponWheelButton>();
			if (buttonScript != null)
			{
				buttonScript.Initialize(weaponController, this, activeWeapons[i], weaponComponent);
			}

			wheelSegments.Add(segmentInstance);
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
		foreach (var seg in wheelSegments)
			Destroy(seg.gameObject);

		wheelSegments.Clear();
		CreateWheel();
	}

	private void EnableWeaponWheelMenuCanvas(string handType)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(true);
		menuManager.OpenWeaponWheelMenu(handType);
		RecreateWheel();
	}

	private void DisableWeaponWheelMenuCanvas(bool isItRightWeaponWheelMenuCanvas)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(false);
		if (!menuManager.IsPauseMenuOpened)
		{
			menuManager.CloseWeaponWheelMenu(isItRightWeaponWheelMenuCanvas);
		}
	}

	public void ShowWeaponName()
	{
		if (IsWeaponLeftHand)
		{
			if (weaponController.LeftHandWeapon != null)
			{
				WeaponText.text = weaponController.leftHandWeaponComponent.WeaponNameUI;
			}
			else
			{
				WeaponText.text = "";
			}
		}
		else if (IsWeaponLeftHand == false)
		{
			if (weaponController.RightHandWeapon != null)
			{
				WeaponText.text = weaponController.rightHandWeaponComponent.WeaponNameUI;
			}
			else
			{
				WeaponText.text = "";
			}
		}
	}
}