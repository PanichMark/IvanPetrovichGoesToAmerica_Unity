using UnityEngine;
using TMPro;

public class PlayerResourcesMoneyManager : MonoBehaviour, ISaveLoad
{
	
	private TMP_Text playerMoneyText;

    public int PlayerMoney { get; private set; }
	

	public void Initialize(TMP_Text playerMoneyText)
	{
		this.playerMoneyText = playerMoneyText;
		

		UpdateMoneyDisplay();
		Debug.Log("PlayerResourcesMoney Initialized");
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
			UpdateMoneyDisplay(); // После изменения сразу обновить интерфейс
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
			UpdateMoneyDisplay(); // После изменения сразу обновить интерфейс
		}
	}
	private void UpdateMoneyDisplay()
	{

		playerMoneyText.text = PlayerMoney.ToString(); // Форматируем текст для вывода суммы
		
	}

	public void SaveData(ref GameData data)
	{
		data.PlayerMoney = this.PlayerMoney;
	}

	public void LoadData(GameData data)
	{
		this.PlayerMoney = data.PlayerMoney;
		UpdateMoneyDisplay();
	}
}


