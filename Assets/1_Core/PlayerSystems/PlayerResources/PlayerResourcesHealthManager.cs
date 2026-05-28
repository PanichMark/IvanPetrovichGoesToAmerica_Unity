using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerResourcesHealthManager : MonoBehaviour, IDamageable, ISaveLoad
{
	private GameController _gameController;
	private Slider _sliderHealthBar;
	private Button _buttonHealingItem;
	private TextMeshProUGUI _healingItemNumber;
	public float MaxPlayerHealth { get; private set; } = 100;
	public float CurrentPlayerHealth { get; private set; }
	private int _healingItemEffect = 34;
	public int MaxHealingItemsNumber { get; private set; } = 9;

	public int CurrentHealingItemsNumber { get; private set; }

	public bool WasObjectDestroyed => false;

	public float CurrentHealth => CurrentPlayerHealth;

	private bool _isInitialized;

	public void Initialize(GameController gameController, Slider HealthBarSlider, Button HealingItemButton, TextMeshProUGUI HealingItemNumber)
	{
		_sliderHealthBar = HealthBarSlider;
		_buttonHealingItem = HealingItemButton;
		_healingItemNumber = HealingItemNumber;
		_gameController = gameController;
		_buttonHealingItem.onClick.AddListener(() => UseHealingItem());

		_sliderHealthBar.maxValue = MaxPlayerHealth;

		_isInitialized = true;

		Debug.Log("PlayerResourcesHealth Initialized");
	}

	void Update()
    {
		if (!_isInitialized)
			return;

		if (Input.GetKeyDown(KeyCode.T) && SceneManager.GetSceneAt(1).name != "Scene_0_MainMenu")
		{
			TakeDamage(900);
		}
	}

    private void UseHealingItem()
    {
        if (CurrentHealingItemsNumber > 0)
        {
            if (CurrentPlayerHealth < MaxPlayerHealth)
            {	
				CurrentHealingItemsNumber--;

                CurrentPlayerHealth += _healingItemEffect;

				if (CurrentPlayerHealth >= MaxPlayerHealth)
				{
					CurrentPlayerHealth = MaxPlayerHealth;
				}

				Debug.Log(CurrentPlayerHealth);
				_sliderHealthBar.value = CurrentPlayerHealth;

				_healingItemNumber.text = CurrentHealingItemsNumber.ToString();

				Debug.Log("Used Healing Item");
			}
            else Debug.Log("Health is already Full");
		}
		else Debug.Log("0 Healing Items");
	}

    public void AddHealingItem()
    {
		if (CurrentHealingItemsNumber < 9)
		{ 
			CurrentHealingItemsNumber++;

			_healingItemNumber.text = CurrentHealingItemsNumber.ToString();

			Debug.Log("Added Healing Item");
		}
        else Debug.Log("Max Healing Items");
	}

	public void ReceiveHealth(float health)
	{
		CurrentPlayerHealth += health;

		if (CurrentPlayerHealth > MaxPlayerHealth)
		{
			CurrentPlayerHealth = MaxPlayerHealth;
		}

		_sliderHealthBar.value = CurrentPlayerHealth;
	}

	public void TakeDamage(float amount)
	{
		CurrentPlayerHealth -= amount;

		_sliderHealthBar.value = CurrentPlayerHealth;

		if (CurrentPlayerHealth <= 0)
		{
			ObjectIsFullyDamaged();
		}
	}

	public void ObjectIsFullyDamaged()
	{
		CurrentPlayerHealth = 0;
		StartCoroutine(_gameController.PlayerHasDied());
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

		_sliderHealthBar.value = CurrentPlayerHealth;

		_healingItemNumber.text = CurrentHealingItemsNumber.ToString();
	}


}