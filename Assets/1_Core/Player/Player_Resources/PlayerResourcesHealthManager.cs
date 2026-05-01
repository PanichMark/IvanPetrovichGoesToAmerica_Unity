using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourcesHealthManager : MonoBehaviour, ISaveLoad
{
	public void Initialize(GameController gameController, Slider HealthBarSlider, Button HealingItemButton, TextMeshProUGUI HealingItemNumber)
	{
		this.HealthBarSlider = HealthBarSlider;
		this.HealingItemButton = HealingItemButton;
		this.HealingItemNumber = HealingItemNumber;
		this.gameController = gameController;
		this.HealingItemButton.onClick.AddListener(() => UseHealingItem());

		this.HealthBarSlider.maxValue = MaxPlayerHealth;
		Debug.Log("PlayerResourcesHealth Initialized");
	}
	private GameController gameController;
	private Slider HealthBarSlider;
    private Button HealingItemButton;
    private TextMeshProUGUI HealingItemNumber;
    public int MaxPlayerHealth { get; private set; } = 100;
    public int CurrentPlayerHealth { get; private set; }

    public int MaxHealingItemsNumber { get; private set; } = 9;

	public int CurrentHealingItemsNumber { get; private set; }

	private bool isPlayerDead;



    void Update()
    {
        HealthBarSlider.value = CurrentPlayerHealth;

        HealingItemNumber.text = CurrentHealingItemsNumber.ToString();


		//ReceiveDamage(1);
		//if (MenuManager.IsPauseMenuOpened)
		//{
		//	HealthBarSlider.gameObject.SetActive(false);
		//}
		//else
		//{
			//HealthBarSlider.gameObject.SetActive(true);
		//}
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

	public void ReceiveDamage(int Damage)
	{
		CurrentPlayerHealth -= Damage;

		if (CurrentPlayerHealth <= 0)
		{
			CurrentPlayerHealth = 0;
			isPlayerDead = true;
			gameController.PlayerIsDead();
		}
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


