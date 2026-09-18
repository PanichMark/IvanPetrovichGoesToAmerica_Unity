using UnityEngine;

public class ViewModelHUDWeapons
{
	public GameObject HUDammo;
	public GameObject TextRightWeaponAmmoMagazineNumber;
	public GameObject TextRightWeaponAmmoReserveNumber;
	public GameObject RightWeaponAmmoBox;
	public GameObject TextLeftWeaponAmmoMagazineNumber;
	public GameObject TextLeftWeaponAmmoReserveNumber;
	public GameObject LeftWeaponAmmoBox;

	public GameObject HUDcrosshiars;

	public GameObject CrosshairRevolver;

	public GameObject CrosshairAutoPistol;
	public GameObject[] ListCrosshairPartsAutoPistol = new GameObject[4];

	public GameObject CrosshairShotgun;
	public GameObject[] ListCrosshairPartsShotgun = new GameObject[4];

	public GameObject CrosshairTranquilizer;

	public ViewModelHUDWeapons(Bootstrap bootstrap, GameObject canvas)
	{
		HUDammo = bootstrap.FindDeepGameObject(canvas, "HUDammo");
		TextRightWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoMagazineNumber");
		TextRightWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextRightWeaponAmmoReserveNumber");
		RightWeaponAmmoBox = bootstrap.FindDeepGameObject(canvas, "RightWeaponAmmoBox");
		TextLeftWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoMagazineNumber");
		TextLeftWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "TextLeftWeaponAmmoReserveNumber");
		LeftWeaponAmmoBox = bootstrap.FindDeepGameObject(canvas, "LeftWeaponAmmoBox");

		HUDcrosshiars = bootstrap.FindDeepGameObject(canvas, "HUDcrosshairs");

		CrosshairRevolver = bootstrap.FindDeepGameObject(HUDcrosshiars, "CrosshairRevolver");

		CrosshairAutoPistol = bootstrap.FindDeepGameObject(HUDcrosshiars, "CrosshairAutoPistol");
		for (int i = 0; i < ListCrosshairPartsAutoPistol.Length; i++)
		{
			ListCrosshairPartsAutoPistol[i] = bootstrap.FindDeepGameObject(CrosshairAutoPistol, $"CrosshairAutoPistolPart{i + 1}");
		}

		CrosshairShotgun = bootstrap.FindDeepGameObject(HUDcrosshiars, "CrosshairShotgun");
		for (int i = 0; i < ListCrosshairPartsShotgun.Length; i++)
		{
			ListCrosshairPartsShotgun[i] = bootstrap.FindDeepGameObject(CrosshairShotgun, $"CrosshairShotgunPart{i + 1}");
		}

		CrosshairTranquilizer = bootstrap.FindDeepGameObject(HUDcrosshiars, "CrosshairTranquilizer");
	}
}