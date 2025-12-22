using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelsButton : MonoBehaviour
{
	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;


	public void Initialize(WeaponController weaponController, WeaponWheelController weaponWheelController)
	{
		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;
		Debug.Log($"{WeaponWheelButtonName}Button Initialized");


	}


	public string WeaponWheelButtonName;
    public string AvailableWeaponName;
 





	public void HoverEnter()
    {
		weaponWheelController.WeaponText.text = AvailableWeaponName;
	}



	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	}

	
}

