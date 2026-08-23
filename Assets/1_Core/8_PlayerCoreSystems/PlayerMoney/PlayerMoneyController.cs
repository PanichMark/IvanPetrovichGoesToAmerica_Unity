using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerMoneyController : MonoBehaviour, ISaveLoad
{
	private TMP_Text _playerComponentMoneyText;

    public int PlayerMoney { get; private set; }
	
	public void Initialize(GameObject playerMoneyText)
	{
		_playerComponentMoneyText = playerMoneyText.GetComponent<TMP_Text>();
		
		UpdateMoneyDisplay();
		Debug.Log("PlayerResourcesMoneyManager Initialized");
	}

	public void AddMoney(int moneyAmmount)
    {
        if (moneyAmmount < 0)
        {
            Debug.Log("Can't add negative Money!");
        }
        else
        {
            PlayerMoney += moneyAmmount;
			UpdateMoneyDisplay(); 
		}
    }

	public void DeductMoney(int moneyAmmount)
	{
		if (moneyAmmount > 0)
		{
			Debug.Log("Can't deduct positive Money!");
		}
		else if (moneyAmmount < -PlayerMoney)
		{
			Debug.Log("Not enought Money!");
		}
		else
		{
			PlayerMoney += moneyAmmount;
			UpdateMoneyDisplay(); 
		}
	}
	private void UpdateMoneyDisplay()
	{
		_playerComponentMoneyText.text = PlayerMoney.ToString();
	}

	public IEnumerator SaveData(GameData data)
	{
		data.PlayerResources.PlayerMoney = PlayerMoney;
		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		PlayerMoney = data.PlayerResources.PlayerMoney;
		UpdateMoneyDisplay();

		yield return null;
	}
}