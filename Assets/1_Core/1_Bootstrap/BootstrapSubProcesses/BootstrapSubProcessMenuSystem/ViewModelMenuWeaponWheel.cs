using UnityEngine;

public class ViewModelMenuWeaponWheel
{
	public GameObject GameObjectWeaponWheelSegment;

	public GameObject TextWeaponWheelWeaponName;
	public GameObject ImageWeaponWheelWeaponIcon;
	public GameObject TextWeaponAmmoMagazineNumber;
	public GameObject TextWeaponAmmoReserveNumber;
	public GameObject TextWeaponAmmoSeparator;

	public GameObject WeaponWheelRadius;

	public GameObject TextWeaponWheelHandType;

	public GameObject ButtonUseHealingItem;
	public GameObject TextHealingItemNumber;

	public GameObject ButtonUseManaReplenishItem;
	public GameObject TextManaReplenishItemNumber;

	public ViewModelMenuWeaponWheel(Bootstrap bootstrap, GameObject canvas)
	{
		GameObjectWeaponWheelSegment = Resources.Load<GameObject>("WeaponSystem/WeaponWheel/WeaponWheelSegment");
		
		TextWeaponWheelWeaponName = bootstrap.FindDeepGameObject(canvas, "TextWeaponWheelWeapon");
		ImageWeaponWheelWeaponIcon = bootstrap.FindDeepGameObject(canvas, "ImageWeaponWheelWeapon");
		TextWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoMagazineNumber");
		TextWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoReserveNumber");
		TextWeaponAmmoSeparator = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoSeparator");

		WeaponWheelRadius = bootstrap.FindDeepGameObject(canvas, "Radius");
		
		TextWeaponWheelHandType = bootstrap.FindDeepGameObject(canvas, "TextWeaponWheelHandType");

		ButtonUseHealingItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseHealingItem");
		TextHealingItemNumber = bootstrap.FindDeepGameObject(canvas, "TextHealingItemNumber");

		ButtonUseManaReplenishItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseManaReplenishItem");
		TextManaReplenishItemNumber = bootstrap.FindDeepGameObject(canvas, "TextManaReplenishItemNumber");
	}
}