using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
	//InputManager playerInputsList;
	public Canvas WeaponWheelMenuCanvas; 

	public Button PoliceBatonButton;
	public Button HarmonicaRevolverButton;
	public Button PlungerCrossbowButton;
	public Button EugenicGenieButton;
	public bool IsWeaponLeftHand { get; private set; }
	public bool IsWeaponWheelActive { get; private set; }

	private bool previousRightHandPressed = false;
	private bool previousLeftHandPressed = false;

	public TextMeshProUGUI WeaponWheelName;
	public WeaponWheelsButtons weaponWheelbuttonscript;
	 WeaponController weaponController;

	void Start()
	{
		//playerInputsList = GetComponent<InputManager>();
		weaponController = GetComponent<WeaponController>();

		PoliceBatonButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponPoliceBaton)));
		HarmonicaRevolverButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponHarmonicaRevolver)));
		PlungerCrossbowButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponPlungerCrossbow)));
		EugenicGenieButton.onClick.AddListener(() => weaponController.SelectWeapon(typeof(WeaponEugenicGenie)));
	}

	void Update()
	{
		bool currentRightHandPressed = InputManager.Instance.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = InputManager.Instance.GetKeyLeftHandWeaponWheel();
		
		// Обновляем состояние, только если изменилось нажатие кнопки
		if (currentRightHandPressed != previousRightHandPressed || currentLeftHandPressed != previousLeftHandPressed)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		previousRightHandPressed = currentRightHandPressed;
		previousLeftHandPressed = currentLeftHandPressed;

		if (PoliceBatonButton != null)
		{
			if (weaponController.IsPoliceBatonWeaponUnlocked)
			{
				PoliceBatonButton.gameObject.SetActive(true);
			}
			else PoliceBatonButton.gameObject.SetActive(false);
		}

		if (HarmonicaRevolverButton != null)
		{
			if (weaponController.IsHarmoniceRevolverWeaponUnlocked)
			{
				HarmonicaRevolverButton.gameObject.SetActive(true);
			}
			else HarmonicaRevolverButton.gameObject.SetActive(false);
		}

		if (PlungerCrossbowButton != null)
		{
			if (weaponController.IsPlungerCrossbowWeaponUnlocked)
			{
				PlungerCrossbowButton.gameObject.SetActive(true);
			}
			else PlungerCrossbowButton.gameObject.SetActive(false);
		}

		if (EugenicGenieButton != null)
		{
			if (weaponController.IsEugenicGenieWeaponUnlocked)
			{
				EugenicGenieButton.gameObject.SetActive(true);
			}
			else EugenicGenieButton.gameObject.SetActive(false);
		}
	}

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		// Обработка правой руки
		if (rightHandPressed && !leftHandPressed && !IsWeaponWheelActive)
		{
			EnableWeaponWheelMenuCanvas("right");
			
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = false;
			//playerBehaviour.ArmPlayer();
			ChangeWheaponWheelButtonColor("right");
			weaponWheelbuttonscript.HoverExit();
			WeaponWheelName.text = "ПРАВАЯ РУКА";
		}

		// Обработка левой руки
		else if (leftHandPressed && !rightHandPressed && !IsWeaponWheelActive)
		{
			EnableWeaponWheelMenuCanvas("left");
			
			IsWeaponWheelActive = true;
			IsWeaponLeftHand = true;
			//playerBehaviour.ArmPlayer();
			ChangeWheaponWheelButtonColor("left");
			weaponWheelbuttonscript.HoverExit();
			WeaponWheelName.text = "ЛЕВАЯ РУКА";
		}

		// Деактивация, если ничего не нажато
		else if (!leftHandPressed && !rightHandPressed)
		{
			DisableWeaponWheelMenuCanvas(!IsWeaponLeftHand);
			IsWeaponWheelActive = false;
		}
	}

	private void EnableWeaponWheelMenuCanvas(string handType)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(true); // Показываем Canvas
		MenuManager.OpenWeaponWheelMenu(handType);
	}

	private void DisableWeaponWheelMenuCanvas(bool IsItRightWeaponWheelMenuCanvas)
	{
		WeaponWheelMenuCanvas.gameObject.SetActive(false); // Скрываем Canvas
		if (!MenuManager.IsPauseMenuOpened)
		{
			MenuManager.CloseWeaponWheelMenu(IsItRightWeaponWheelMenuCanvas);
		}
	}

	public void ChangeWheaponWheelButtonColor(string handType)
	{
		if (handType == "right")
		{
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "PoliceBaton")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(PoliceBatonButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "HarmonicaRevolver")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(HarmonicaRevolverButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "PlungerCrossbow")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(PlungerCrossbowButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem == "EugenicGenie")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(EugenicGenieButton);
			}

			if (weaponController.RightHandWeapon?.WeaponNameSystem != "PoliceBaton")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(PoliceBatonButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "HarmonicaRevolver")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(HarmonicaRevolverButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "PlungerCrossbow")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(PlungerCrossbowButton);
			}
			if (weaponController.RightHandWeapon?.WeaponNameSystem != "EugenicGenie")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(EugenicGenieButton);
			}
		}

		else if (handType == "left")
		{
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "PoliceBaton")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(PoliceBatonButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "HarmonicaRevolver")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(HarmonicaRevolverButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "PlungerCrossbow")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(PlungerCrossbowButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem == "EugenicGenie")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToActive(EugenicGenieButton);
			}

			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "PoliceBaton")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(PoliceBatonButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "HarmonicaRevolver")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(HarmonicaRevolverButton);
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "PlungerCrossbow")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(PlungerCrossbowButton); ;
			}
			if (weaponController.LeftHandWeapon?.WeaponNameSystem != "EugenicGenie")
			{
				weaponWheelbuttonscript.ChangeWeaponWheelButtonColorToDefault(EugenicGenieButton);
			}
		}
	}
}
