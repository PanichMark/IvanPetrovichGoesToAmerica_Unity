using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaController : MonoBehaviour, IJsonSaveLoad
{
	private Slider _sliderComponentManaBar;
	private GameObject _sliderManaBarFillArea;
	private Button _buttonManaReplenishtem;
	private TextMeshProUGUI _manaReplenishItemNumber;
	public float MaxPlayerMana { get; private set; } = 100f;
	public float CurrentPlayerMana { get; private set; }
	private Coroutine _autoRefillRoutine;

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
		_autoRefillRoutine = StartCoroutine(AutoRefillMana());

		Debug.Log("PlayerResourcesManaManager Initialized");
	}

	public void ConfigApplyPlayerMana(int setMana)
	{
		CurrentPlayerMana = setMana;

		_sliderComponentManaBar.value = CurrentPlayerMana * 0.24f;
	}

	public void ConfigApplyPlayerManaReplenishItems(int setManaReplenishItems)
	{
		CurrentManaReplenishItemsNumber = setManaReplenishItems;

		_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();
	}

	private void UseManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber > 0)
		{
			if (CurrentPlayerMana < MaxPlayerMana)
			{
				CurrentManaReplenishItemsNumber--;

				if (CurrentPlayerMana >= MaxPlayerMana)
				{
					CurrentPlayerMana = MaxPlayerMana;
				}

				ReplenishMana(_manaItemEffect);

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

		if (CurrentPlayerMana >= MaxPlayerMana)
		{
			CurrentPlayerMana = MaxPlayerMana;
		}

		_sliderComponentManaBar.value = CurrentPlayerMana * 0.24f;

		//ShowSliderManaBarFillArea();

		if (CurrentPlayerMana >= 5f && _autoRefillRoutine != null)
		{
			StopCoroutine(_autoRefillRoutine);
			_autoRefillRoutine = null;
		}

		Debug.Log($"replenished: {Mana} mana");
	}

	public void UseMana(int ManaCost)
	{
		CurrentPlayerMana -= ManaCost;

		_sliderComponentManaBar.value = CurrentPlayerMana * 0.24f;

		if (CurrentPlayerMana <= 0)
		{
			//HideSliderManaBarFillArea();
		}

		if (CurrentPlayerMana < 5f && _autoRefillRoutine == null)
		{
			_autoRefillRoutine = StartCoroutine(AutoRefillMana());
		}

		Debug.Log($"used: {ManaCost} mana");
	}

	private void ShowSliderManaBarFillArea()
	{
		_sliderManaBarFillArea.SetActive(true);
	}

	private void HideSliderManaBarFillArea()
	{
		_sliderManaBarFillArea.SetActive(false);
	}

	public void ShowButtonUseManaReplenishItem()
	{
		_buttonManaReplenishtem.gameObject.SetActive(true);
	}

	public void HideButtonUseManaReplenishItem()
	{
		_buttonManaReplenishtem.gameObject.SetActive(false);
	}

	private IEnumerator AutoRefillMana()
	{
		yield return new WaitForSeconds(1f);

		while (CurrentPlayerMana < 5f)
		{
			CurrentPlayerMana = Mathf.Min(CurrentPlayerMana + 1f, 5f);
			_sliderComponentManaBar.value = CurrentPlayerMana * 0.24f;
			yield return new WaitForSeconds(1f);
		}

		_autoRefillRoutine = null;
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.PlayerResources.PlayerMana = CurrentPlayerMana;
		data.PlayerResources.PlayerManaReplenishItemsNumber = CurrentManaReplenishItemsNumber;
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		CurrentPlayerMana = data.PlayerResources.PlayerMana;
		CurrentManaReplenishItemsNumber = data.PlayerResources.PlayerManaReplenishItemsNumber;

		_sliderComponentManaBar.value = CurrentPlayerMana * 0.24f;

		_manaReplenishItemNumber.text = CurrentManaReplenishItemsNumber.ToString();

		yield return null;
	}
}