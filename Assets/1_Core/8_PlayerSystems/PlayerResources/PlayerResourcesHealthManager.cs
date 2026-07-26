using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerResourcesHealthManager : MonoBehaviour, IDamageable, ISaveLoad
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private Slider _sliderHealthBar;
	private Button _buttonHealingItem;
	private TextMeshProUGUI _healingItemNumber;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	public float MaxPlayerHealth { get; private set; } = 100;
	public float CurrentPlayerHealth { get; private set; }
	private int _healingItemEffect = 34;
	public int MaxHealingItemsNumber { get; private set; } = 9;

	public int CurrentHealingItemsNumber { get; private set; }

	public bool IsObjectDestroyed => false;

	private bool _isFalling;
	private float _fallStartTime;
	private const float MinFallDurationForDamage = 1.0f;
	private const float DamagePerSecondOverThreshold = 0.2f;

	public float CurrentHealth => CurrentPlayerHealth;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		ViewModelHUDHealthAndMana viewModelHUDHealthAndMana,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel)
	{
		_bootstrap = bootstrap;
		_sliderHealthBar = viewModelHUDHealthAndMana.SliderHealthBar.GetComponent<Slider>();
		_buttonHealingItem = viewModelMenuWeaponWheel.ButtonUseHealingItem.GetComponent<Button>();
		_healingItemNumber = viewModelMenuWeaponWheel.TextHealingItemNumber.GetComponent<TextMeshProUGUI>();
		_gameController = gameController;
		_buttonHealingItem.onClick.AddListener(() => UseHealingItem());

		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementStateMachineController.OnChangeMovementState += FallDamage;
		_sliderHealthBar.maxValue = MaxPlayerHealth;

		Debug.Log("PlayerResourcesHealthManager Initialized");
	}

	void Update()
    {
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (Input.GetKeyDown(KeyCode.T) && SceneManager.GetSceneAt(1).name != GameScenesEnum.Scene_0_MainMenu.ToString())
		{
			TakeDamage(99999);
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

				//Debug.Log(CurrentPlayerHealth);
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

	private void FallDamage(PlayerMovementStateTypes playerMovementStateType)
	{
		if (playerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
		{
			if (!_isFalling)
			{
				_isFalling = true;
				_fallStartTime = Time.time;
			}
		}
		else if (_isFalling)
		{
			CalculateAndApplyFallDamage();
			_isFalling = false;
		}
	}

	private void CalculateAndApplyFallDamage()
	{
		float realGameplayDuration = (Time.time - _fallStartTime) * Time.timeScale;

		if (realGameplayDuration > MinFallDurationForDamage)
		{
			float excessDuration = realGameplayDuration - MinFallDurationForDamage;

			// Целая часть секунд сверху порога, которая влияет на множитель
			int fullSecondsOverThreshold = Mathf.FloorToInt(excessDuration);

			// Базовый урон за первую лишнюю секунду
			float baseDamage = MaxPlayerHealth * DamagePerSecondOverThreshold;

			// Экспоненциальный рост: 10% * (2 ^ количество полных лишних секунд)
			float damageToTake = baseDamage * Mathf.Pow(2f, fullSecondsOverThreshold);

			TakeDamage(damageToTake);
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