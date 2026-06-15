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
	public GameObject HUDammo;

	public ViewModelHUDAmmo(Bootstrap bootstrap, GameObject canvas)
	{
		TextRightWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoMagazineNumber");
		TextRightWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoReserveNumber");
		RightWeaponAmmoSeparator = bootstrap.FindDeepGameObject(canvas, "RightWeaponAmmoSeparator");
		TextLeftWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoMagazineNumber");
		TextLeftWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoReserveNumber");
		LeftWeaponAmmoSeparator = bootstrap.FindDeepGameObject(canvas, "LeftWeaponAmmoSeparator");
		TextChokeNPC = bootstrap.FindDeepGameObject(canvas, "TextChokeNPC");

		HUDammo = bootstrap.FindDeepGameObject(canvas, "HUDammo");
	}
}
