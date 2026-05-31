using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuWeaponWheel
{
	public GameObject GameObjectWeaponWheelSegment;
	public TextMeshProUGUI TextWeaponWheelWeaponName;
	public Image ImageWeaponWheelWeaponIcon;
	public Button ButtonUseHealingItem;
	public TextMeshProUGUI TextWeaponWheelHandType;
	public TextMeshProUGUI TextHealingItemNumber;
	public Button ButtonUseManaReplenishItem;
	public TextMeshProUGUI TextManaReplenishItemNumber;

	public ViewModelMenuWeaponWheel(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonUseHealingItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseHealingItem").GetComponent<Button>();
		TextHealingItemNumber = bootstrap.FindDeepGameObject(canvas, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();
		ButtonUseManaReplenishItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseManaReplenishItem").GetComponent<Button>();
		TextManaReplenishItemNumber = bootstrap.FindDeepGameObject(canvas, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();

		GameObjectWeaponWheelSegment = Resources.Load<GameObject>("WeaponWheel/WeaponWheelSegment");

		TextWeaponWheelWeaponName = canvas.transform.Find("TextWeaponWheelWeapon").GetComponent<TextMeshProUGUI>();
		ImageWeaponWheelWeaponIcon = canvas.transform.Find("ImageWeaponWheelWeapon").GetComponent<Image>();
		TextWeaponWheelHandType = canvas.transform.Find("TextWeaponWheelHandType").GetComponent<TextMeshProUGUI>();
	}
}
