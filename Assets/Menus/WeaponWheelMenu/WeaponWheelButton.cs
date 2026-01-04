using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponWheelButton : MonoBehaviour
{
	 private WeaponController weaponController;
	 private WeaponWheelController weaponWheelController;
	 private GameObject WeaponPrefab;
	//[SerializeField] private string WeaponWheelButtonName;
	 private string WeaponName;

	private void Start()
	{
		//var button = GetComponent<Button>();
		//button.onClick.AddListener(() => weaponController.SelectWeapon(WeaponPrefab));
		//button.onClick.AddListener(() => Debug.Log("Bruh!"));
	}


	public void Initialize(WeaponController weaponController, WeaponWheelController weaponWheelController)
	{
		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;	


	}

	public void HoverEnter()
    {
		weaponWheelController.WeaponText.text = "BRUH";
		//weaponWheelController.WeaponText.text = WeaponName;
	}



	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	}

	
}

