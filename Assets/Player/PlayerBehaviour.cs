using UnityEngine;

public class PlayerBehaviour: MonoBehaviour
{
	private IInputDevice inputDevice;

	// Конструктор принимает зависимость


	
	//WeaponController weaponController;
	//InteractionController interactionController;

	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; }

	
	void Start()
	{
		
		//weaponController = GetComponent<WeaponController>();
		//interactionController = GetComponent<InteractionController>();
		
	}

	void Update()
	{
		if (inputDevice.GetKeyLeftHandWeaponAttack())
		{
			ArmPlayer();
		}


		if (inputDevice.GetKeyHideWeapons())
		{
			//if (IsPlayerArmed && (weaponController.RightHandWeapon != null || weaponController.LeftHandWeapon != null))
			//{
				DisarmPlayer();
			//}
		}

		//Debug.Log("was armed: " + WasPlayerArmed);
		//Debug.Log("is " +IsPlayerArmed);
		
	}
	

	public void ArmPlayer()
	{
		//if (!IsPlayerArmed && interactionController.CurrentPickableObject == null)
		//{
			IsPlayerArmed = true;
			WasPlayerArmed = false;
		/*
			if (weaponController.RightHandWeapon != null)
			{
				weaponController.ShowWeapon("right");
			}

			if (weaponController.LeftHandWeapon != null)
			{
				weaponController.ShowWeapon("left");
			}


			Debug.Log("PlayerArmed");
		}
		*/
		Debug.Log("PlayerArmed");
	}

	public void DisarmPlayer()
	{
		//if (IsPlayerArmed || interactionController.CurrentPickableObject != null)
		//{
			IsPlayerArmed = false;

			
			
			WasPlayerArmed = true;



		/*

			if (weaponController.RightHandWeapon != null)
			{
				weaponController.HideWeapon("right");
			}

			if (weaponController.LeftHandWeapon != null)
			{
				weaponController.HideWeapon("left");
			}

			Debug.Log("PlayerDisarmed");
		}
		else WasPlayerArmed = false;
		*/
		Debug.Log("PlayerDisarmed");
	}


	public void Initialize(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
		Debug.Log("PlayerBehaviour Initialized");
	}
}

