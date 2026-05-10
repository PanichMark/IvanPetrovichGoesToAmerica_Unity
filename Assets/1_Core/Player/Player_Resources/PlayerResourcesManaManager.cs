using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourcesManaManager : MonoBehaviour, ISaveLoad
{
	private Slider _sliderManaBar;
	private Button _buttonManaReplenishtem;
	private TextMeshProUGUI _manaReplenishItemNumber;
	public int MaxPlayerMana { get; private set; } = 100;
	public int CurrentPlayerMana { get; private set; }

	public int MaxManaReplenishItemsNumber { get; private set; } = 9;

	public int CurrentManaReplenishItemsNumber { get; private set; }

	public void Initialize(Slider ManaBarSlider, Button ManaReplenishtemButton, TextMeshProUGUI ManaReplenishItemNumber)
	{
		_sliderManaBar = ManaBarSlider;
		_buttonManaReplenishtem = ManaReplenishtemButton;	
		_manaReplenishItemNumber = ManaReplenishItemNumber;
		_buttonManaReplenishtem.onClick.AddListener(() => UseManaReplenishItem());

		_sliderManaBar.maxValue = MaxPlayerMana;
		Debug.Log("PlayerResourcesMana Initialized");
	}

	void Update()
	{
		_sliderManaBar.value = CurrentPlayerMana;

		_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();
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

	public void UseMana(int ManaCost)
	{
		CurrentPlayerMana -= ManaCost;
		Debug.Log($"used: {ManaCost} mana");
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