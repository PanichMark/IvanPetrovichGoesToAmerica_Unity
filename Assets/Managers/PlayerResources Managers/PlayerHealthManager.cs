using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour, IDataPersistence
{
	public static PlayerHealthManager Instance { get; private set; } // Статическое поле экземпляра

	public Slider HealthBarSlider;
    public Button HealingItemButton;
    public TextMeshProUGUI HealingItemNumber;
    public int MaxPlayerHealth { get; private set; } = 100;
    public int CurrentPlayerHealth { get; private set; }

    public int MaxHealingItemsNumber { get; private set; } = 9;

	public int CurrentHealingItemsNumber { get; private set; }

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
        HealingItemButton.onClick.AddListener(() => UseHealingItem());

        HealthBarSlider.maxValue = MaxPlayerHealth;

    }

    void Update()
    {
        HealthBarSlider.value = CurrentPlayerHealth;

        HealingItemNumber.text = CurrentHealingItemsNumber.ToString();

		if (MenuManager.IsPauseMenuOpened)
		{
			HealthBarSlider.gameObject.SetActive(false);
		}
		else
		{
			HealthBarSlider.gameObject.SetActive(true);
		}
	}

    private void UseHealingItem()
    {
        if (CurrentHealingItemsNumber > 0)
        {
            if (CurrentPlayerHealth < MaxPlayerHealth)
            {
            Debug.Log("Used Healing Item");
            CurrentHealingItemsNumber--;

                CurrentPlayerHealth += 34;
            }
            else Debug.Log("Health is already Full");
		}
		else Debug.Log("0 Healing Items");

	}
    public void AddHealingItem()
    {
		if (CurrentHealingItemsNumber < 9)
        {
			Debug.Log("Added Healing Item");
			CurrentHealingItemsNumber++;
		}
        else Debug.Log("Max Healing Items");

	}

	public void SaveData(ref GameData data)
	{
		data.PlayerHealth = CurrentPlayerHealth;
		data.HealingItems = CurrentHealingItemsNumber;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerHealth = data.PlayerHealth;
		CurrentHealingItemsNumber = data.HealingItems;
	}
}
