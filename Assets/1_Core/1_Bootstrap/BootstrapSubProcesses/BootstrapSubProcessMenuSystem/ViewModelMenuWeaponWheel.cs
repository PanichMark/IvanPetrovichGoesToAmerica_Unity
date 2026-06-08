using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuWeaponWheel
{
	public GameObject GameObjectWeaponWheelSegment;

	public TextMeshProUGUI TextWeaponWheelWeaponName;
	public Image ImageWeaponWheelWeaponIcon;
	public GameObject TextWeaponAmmoMagazineNumber;
	public GameObject TextWeaponAmmoReserveNumber;
	public GameObject TextWeaponAmmoSeparator;

	public TextMeshProUGUI TextWeaponWheelHandType;

	public Button ButtonUseHealingItem;
	public TextMeshProUGUI TextHealingItemNumber;

	public Button ButtonUseManaReplenishItem;
	public TextMeshProUGUI TextManaReplenishItemNumber;

	public ViewModelMenuWeaponWheel(Bootstrap bootstrap, GameObject canvas)
	{
		GameObjectWeaponWheelSegment = Resources.Load<GameObject>("WeaponSystem/WeaponWheel/WeaponWheelSegment");

		TextWeaponWheelWeaponName = canvas.transform.Find("TextWeaponWheelWeapon").GetComponent<TextMeshProUGUI>();
		ImageWeaponWheelWeaponIcon = canvas.transform.Find("ImageWeaponWheelWeapon").GetComponent<Image>();
		TextWeaponAmmoMagazineNumber = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoMagazineNumber");
		TextWeaponAmmoReserveNumber = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoReserveNumber");
		TextWeaponAmmoSeparator = bootstrap.FindDeepGameObject(canvas, "WeaponAmmoSeparator");

		TextWeaponWheelHandType = canvas.transform.Find("TextWeaponWheelHandType").GetComponent<TextMeshProUGUI>();

		ButtonUseHealingItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseHealingItem").GetComponent<Button>();
		TextHealingItemNumber = bootstrap.FindDeepGameObject(canvas, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();

		ButtonUseManaReplenishItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseManaReplenishItem").GetComponent<Button>();
		TextManaReplenishItemNumber = bootstrap.FindDeepGameObject(canvas, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();
	}
}