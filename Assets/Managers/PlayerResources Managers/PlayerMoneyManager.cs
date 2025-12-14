using UnityEngine;
using TMPro;

public class PlayerMoneyManager : MonoBehaviour, IDataPersistence
{
	public static PlayerMoneyManager Instance { get; private set; } // Статическое поле экземпляра
	//public InputManager playerInputsList;
	public TMP_Text PlayerMoneyText;

    public int PlayerMoney { get; private set; }

	private void Awake()
	{
		// Паттерн Singleton: предотвращаем создание второго экземпляра
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject); // Сохраняется при смене уровней
		}
		else
		{
			Destroy(gameObject); // Уничтожаем лишние экземпляры
		}
	}


	private void Start()
	{
		//playerInputsList = GetComponent<InputManager>();
		UpdateMoneyDisplay(); // После изменения сразу обновить интерфейс
	}

	private void Update()
	{
		//Debug.Log(PlayerMoney);
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
		if (PlayerMoneyText != null)
		{
			PlayerMoneyText.text = PlayerMoney.ToString(); // Форматируем текст для вывода суммы
		}
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

