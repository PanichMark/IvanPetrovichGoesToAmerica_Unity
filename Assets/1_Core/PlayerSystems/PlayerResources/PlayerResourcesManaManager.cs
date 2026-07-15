using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourcesManaManager : MonoBehaviour, ISaveLoad
{
	private Slider _sliderComponentManaBar;
	private GameObject _sliderManaBarFillArea;
	private Button _buttonManaReplenishtem;
	private TextMeshProUGUI _manaReplenishItemNumber;
	public int MaxPlayerMana { get; private set; } = 100;
	public int CurrentPlayerMana { get; private set; }

	private int _manaItemEffect = 34;
	public int MaxManaReplenishItemsNumber { get; private set; } = 9;

	public int CurrentManaReplenishItemsNumber { get; private set; }

	public void Initialize(ViewModelHUDHealthAndMana viewModelHUDHealthAndMana, ViewModelMenuWeaponWheel viewModelMenuWeaponWheel)
	{
		_sliderComponentManaBar = viewModelHUDHealthAndMana.SliderManaBar.GetComponent<Slider>();
		_buttonManaReplenishtem = viewModelMenuWeaponWheel.ButtonUseManaReplenishItem.GetComponent<Button>();
		_manaReplenishItemNumber = viewModelMenuWeaponWheel.TextManaReplenishItemNumber.GetComponent<TextMeshProUGUI>();
		_sliderManaBarFillArea = viewModelHUDHealthAndMana.SliderManaBarFillArea;
		_buttonManaReplenishtem.onClick.AddListener(() => UseManaReplenishItem());

		_sliderComponentManaBar.maxValue = MaxPlayerMana;


		Debug.Log("PlayerResourcesManaManager Initialized");
	}

	private void UseManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber > 0)
		{
			if (CurrentPlayerMana < MaxPlayerMana)
			{
				CurrentManaReplenishItemsNumber--;

				ReplenishMana(_manaItemEffect);

				if (CurrentPlayerMana >= MaxPlayerMana)
				{
					CurrentPlayerMana = MaxPlayerMana;
				}

				_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();

				Debug.Log("Used ManaReplenish Item");
			}
			else Debug.Log("Mana is already Full");
		}
		else Debug.Log("0 ManaReplenish Items");
	}

	public void AddManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber < 9)
		{
			CurrentManaReplenishItemsNumber++;

			_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();

			Debug.Log("Added ManaReplenish Item");
		}
		else Debug.Log("Max ManaReplenish Items");
	}

	public void ReplenishMana(int Mana)
	{
		CurrentPlayerMana += Mana;

		_sliderComponentManaBar.value = CurrentPlayerMana;

		ShowSliderManaBarFillArea();

		Debug.Log($"replenished: {Mana} mana");
	}

	public void UseMana(int ManaCost)
	{
		CurrentPlayerMana -= ManaCost;

		_sliderComponentManaBar.value = CurrentPlayerMana;

		if (CurrentPlayerMana <= 0)
		{
			HideSliderManaBarFillArea();
		}

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

		_sliderComponentManaBar.value = CurrentPlayerMana;

		_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();
	}

	private void ShowSliderManaBarFillArea()
	{
		_sliderManaBarFillArea.SetActive(true);
	}

	private void HideSliderManaBarFillArea()
	{
		_sliderManaBarFillArea.SetActive(false);
	}
}