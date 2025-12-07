using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaManager : MonoBehaviour, IDataPersistence
{
	public static PlayerManaManager Instance { get; private set; } // Статическое поле экземпляра

	public Slider ManaBarSlider;
	public Button ManaReplenishtemButton;
	public TextMeshProUGUI ManaReplenishItemNumber;
	public int MaxPlayerMana { get; private set; } = 100;
	public int CurrentPlayerMana { get; private set; }

	public int MaxManaReplenishItemsNumber { get; private set; } = 9;

	public int CurrentManaReplenishItemsNumber { get; private set; }

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

	void Start()
	{
		ManaReplenishtemButton.onClick.AddListener(() => UseManaReplenishItem());

		ManaBarSlider.maxValue = MaxPlayerMana;

	}

	void Update()
	{
		ManaBarSlider.value = CurrentPlayerMana;

		ManaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();

		if (MenuManager.IsPauseMenuOpened)
		{
			ManaBarSlider.gameObject.SetActive(false);
		}
		else
		{
			ManaBarSlider.gameObject.SetActive(true);
		}
	}

	private void UseManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber > 0)
		{
			if (CurrentPlayerMana < MaxPlayerMana)
			{
				Debug.Log("Used ManaReplenish Item");
				CurrentManaReplenishItemsNumber--;

				CurrentPlayerMana += 34;
			}
			else Debug.Log("Mana is already Full");
		}
		else Debug.Log("0 ManaReplenish Items");

	}
	public void AddManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber < 9)
		{
			Debug.Log("Added ManaReplenish Item");
			CurrentManaReplenishItemsNumber++;
		}
		else Debug.Log("Max ManaReplenish Items");

	}

	public void SaveData(ref GameData data)
	{
		data.PlayerMana = CurrentPlayerMana;
		data.ManaReplenishItems = CurrentManaReplenishItemsNumber;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerMana = data.PlayerMana;
		CurrentManaReplenishItemsNumber = data.ManaReplenishItems;
	}
}
