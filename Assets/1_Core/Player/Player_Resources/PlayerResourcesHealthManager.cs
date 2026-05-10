using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourcesHealthManager : MonoBehaviour, ISaveLoad
{
	private PlayerBehaviour _playerBehaviour;

	private GameController _gameController;
	private Slider _sliderHealthBar;
	private Button _buttonHealingItem;
	private TextMeshProUGUI _healingItemNumber;
	public int MaxPlayerHealth { get; private set; } = 100;
	public int CurrentPlayerHealth { get; private set; }

	public int MaxHealingItemsNumber { get; private set; } = 9;

	public int CurrentHealingItemsNumber { get; private set; }

	public void Initialize(GameController gameController, PlayerBehaviour playerBehaviour, Slider HealthBarSlider, Button HealingItemButton, TextMeshProUGUI HealingItemNumber)
	{
		_sliderHealthBar = HealthBarSlider;
		_buttonHealingItem = HealingItemButton;
		_healingItemNumber = HealingItemNumber;
		_gameController = gameController;
		_playerBehaviour = playerBehaviour;
		_buttonHealingItem.onClick.AddListener(() => UseHealingItem());

		_sliderHealthBar.maxValue = MaxPlayerHealth;
		Debug.Log("PlayerResourcesHealth Initialized");
	}


	void Update()
    {
        _sliderHealthBar.value = CurrentPlayerHealth;

        _healingItemNumber.text = CurrentHealingItemsNumber.ToString();

		if (Input.GetKeyDown(KeyCode.T))
		{
			ReceiveDamage(900);
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

	public void ReceiveDamage(int Damage)
	{
		CurrentPlayerHealth -= Damage;

		if (CurrentPlayerHealth <= 0)
		{
			CurrentPlayerHealth = 0;
			_gameController.PlayerHasDied();
			_playerBehaviour.DisarmPlayer();
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