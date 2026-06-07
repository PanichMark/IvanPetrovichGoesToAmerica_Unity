using UnityEngine;

public class ViewModelHUDAmmo
{
	public GameObject TextRightWeaponAmmoMagazineNumber;
	public GameObject TextRightWeaponAmmoReserveNumber;
	public GameObject RightWeaponAmmoSeparator;
	public GameObject TextLeftWeaponAmmoMagazineNumber;
	public GameObject TextLeftWeaponAmmoReserveNumber;
	public GameObject LeftWeaponAmmoSeparator;
	public GameObject TextChokeNPC;

	public ViewModelHUDAmmo(Bootstrap bootstrap, GameObject canvas)
	{
		TextRightWeaponAmmoMagazineNumber = canvas.transform.Find("TextRightWeaponAmmoMagazineNumber").gameObject;
		TextRightWeaponAmmoReserveNumber = canvas.transform.Find("TextRightWeaponAmmoReserveNumber").gameObject;
		RightWeaponAmmoSeparator = canvas.transform.Find("RightWeaponAmmoSeparator").gameObject;
		TextLeftWeaponAmmoMagazineNumber = canvas.transform.Find("TextLeftWeaponAmmoMagazineNumber").gameObject;
		TextLeftWeaponAmmoReserveNumber = canvas.transform.Find("TextLeftWeaponAmmoReserveNumber").gameObject;
		LeftWeaponAmmoSeparator = canvas.transform.Find("LeftWeaponAmmoSeparator").gameObject;
		TextChokeNPC = canvas.transform.Find("TextChokeNPC").gameObject;
	}
}
