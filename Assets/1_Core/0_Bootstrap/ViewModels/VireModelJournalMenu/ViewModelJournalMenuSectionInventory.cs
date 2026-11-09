using UnityEngine;

public class ViewModelJournalMenuSectionInventory
{
	public GameObject TextPlayerMoney;
	public GameObject TextCurrentPlayerMoneyDisplay;

	public ViewModelJournalMenuSectionInventory(Bootstrap bootstrap, GameObject canvas)
	{
		TextPlayerMoney = bootstrap.FindDeepGameObject(canvas, "TextPlayerMoney");
		TextCurrentPlayerMoneyDisplay = bootstrap.FindDeepGameObject(canvas, "TextCurrentPlayerMoneyDisplay");
	}
}
