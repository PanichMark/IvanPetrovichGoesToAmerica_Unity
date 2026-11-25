using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerMoneyController : MonoBehaviour, IJsonSaveLoad
{
    public int PlayerMoneyRubles { get; private set; }
	public int PlayerMoneyDollars { get; private set; }
	public int PlayerMoneyGoldmark { get; private set; }
	public int PlayerMoneyWeaponLicense { get; private set; }

	private JournalMenuSectionInventoryController _journalMenuSectionInventoryController;

	public void Initialize(
		JournalMenuSectionInventoryController journalMenuSectionInventoryController)
	{
		_journalMenuSectionInventoryController = journalMenuSectionInventoryController;

		UpdateMoneyDisplay();
		Debug.Log("PlayerResourcesMoneyManager Initialized");
	}

	public void ConfigApplyPlayerMoney(int setMoney)
	{
		PlayerMoneyRubles = setMoney;
		//PlayerMoneyDollars = setMoney;
		//PlayerMoneyGoldmark = setMoney;
		//PlayerMoneyWeaponLicense = setMoney;

		UpdateMoneyDisplay();
	}

	public void AddMoney(int moneyAmmount)
    {
        if (moneyAmmount < 0)
        {
            Debug.Log("Can't add negative Money!");
        }
        else
        {
            PlayerMoneyRubles += moneyAmmount;
			UpdateMoneyDisplay(); 
		}
    }

	public void DeductMoney(int moneyAmmount)
	{
		if (moneyAmmount > 0)
		{
			Debug.Log("Can't deduct positive Money!");
		}
		else if (moneyAmmount < -PlayerMoneyRubles)
		{
			Debug.Log("Not enought Money!");
		}
		else
		{
			PlayerMoneyRubles += moneyAmmount;
			UpdateMoneyDisplay(); 
		}
	}

	private void UpdateMoneyDisplay()
	{
		_journalMenuSectionInventoryController.UpdateInventoryMoneyRubles(PlayerMoneyRubles);
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.PlayerResources.PlayerMoneyRubles = PlayerMoneyRubles;
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		PlayerMoneyRubles = data.PlayerResources.PlayerMoneyRubles;
		UpdateMoneyDisplay();

		yield return null;
	}
}