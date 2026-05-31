using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuWeaponWheel : IViewModel
{
	public Button ButtonUseHealingItem;
	public TextMeshProUGUI TextHealingItemNumber;
	public Button ButtonUseManaReplenishItem;
	public TextMeshProUGUI TextManaReplenishItemNumber;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonUseHealingItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseHealingItem").GetComponent<Button>();
		TextHealingItemNumber = bootstrap.FindDeepGameObject(canvas, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();
		ButtonUseManaReplenishItem = bootstrap.FindDeepGameObject(canvas, "ButtonUseManaReplenishItem").GetComponent<Button>();
		TextManaReplenishItemNumber = bootstrap.FindDeepGameObject(canvas, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();
	}
}
