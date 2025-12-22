using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponWheelsButton : MonoBehaviour
{
	[SerializeField] private WeaponController weaponController;
	[SerializeField] private WeaponWheelController weaponWheelController;
	[SerializeField] private GameObject WeaponPrefab;
	[SerializeField] private string WeaponWheelButtonName;
	[SerializeField] private string AvailableWeaponName;

	private void Start()
	{
		var button = GetComponent<Button>();
		button.onClick.AddListener(() => weaponController.SelectWeapon(WeaponPrefab));
		//button.onClick.AddListener(() => Debug.Log("Bruh!"));
	}


	

	public void HoverEnter()
    {
		weaponWheelController.WeaponText.text = AvailableWeaponName;
	}



	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	}

	
}

