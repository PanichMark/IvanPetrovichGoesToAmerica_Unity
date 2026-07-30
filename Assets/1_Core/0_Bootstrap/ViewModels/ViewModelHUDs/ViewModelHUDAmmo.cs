using UnityEngine;

public class ViewModelHUDAmmo
{
	public GameObject TextRightWeaponAmmoMagazineNumber;
	public GameObject TextRightWeaponAmmoReserveNumber;
	public GameObject RightWeaponAmmoBox;
	public GameObject TextLeftWeaponAmmoMagazineNumber;
	public GameObject TextLeftWeaponAmmoReserveNumber;
	public GameObject LeftWeaponAmmoBox;
	public GameObject TextChokeNPC;
	public GameObject HUDammo;

	public ViewModelHUDAmmo(Bootstrap bootstrap, GameObject canvas)
	{
		TextRightWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoMagazineNumber");
		TextRightWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoReserveNumber");
		RightWeaponAmmoBox = bootstrap.FindDeepGameObject(canvas, "RightWeaponAmmoBox");
		TextLeftWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoMagazineNumber");
		TextLeftWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoReserveNumber");
		LeftWeaponAmmoBox = bootstrap.FindDeepGameObject(canvas, "LeftWeaponAmmoBox");
		TextChokeNPC = bootstrap.FindDeepGameObject(canvas, "TextChokeNPC");

		HUDammo = bootstrap.FindDeepGameObject(canvas, "HUDammo");
	}
}
