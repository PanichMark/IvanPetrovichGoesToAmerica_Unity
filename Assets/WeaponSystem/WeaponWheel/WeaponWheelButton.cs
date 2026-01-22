using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelButton : MonoBehaviour
{
	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;
	private GameObject WeaponPrefab;
	private string WeaponName;
	private Sprite WeaponIcon;

	private void Start()
	{
		var button = GetComponent<Button>();
		button.onClick.AddListener(() => weaponController.SelectWeapon(WeaponPrefab));
	}

	public void Initialize(WeaponController weaponController, WeaponWheelController weaponWheelController, GameObject weaponPrefab, WeaponAbstract weaponComponent)
	{
		this.weaponController = weaponController;
		this.weaponWheelController = weaponWheelController;
		WeaponPrefab = weaponPrefab;
		WeaponName = weaponComponent.WeaponNameUI;
		WeaponIcon = weaponComponent.WeaponIcon;
	}

	public void HoverEnter()
	{
		weaponWheelController.WeaponText.text = WeaponName;
	}

	public void HoverExit()
	{
		weaponWheelController.ShowWeaponName();
	}
}
