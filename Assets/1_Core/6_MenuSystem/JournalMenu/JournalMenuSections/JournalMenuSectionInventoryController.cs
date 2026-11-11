using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JournalMenuSectionInventoryController : MonoBehaviour
{
	private GameObject _textPlayerMoneyRubles;
	private TextMeshProUGUI _textComponentPlayerMoneyRubles;

	private GameObject _textHealingItems;
	private TextMeshProUGUI _textComponentHealingItems;

	private GameObject _textManaReplenishItems;
	private TextMeshProUGUI _textComponentManaReplenishItems;

	private GameObject[] _inventoryRows;
	private GameObject[] _buttonsInventorySlots;
	private Button[] _buttonComponentsInventorySlots;
	private GameObject[] _imagesInventorySlotItems;
	private Image[] _imageComponentsInventorySlotItems;

	private GameObject _scrollbar;
	private Scrollbar _scrollbarComponent;

	private GameObject _scrollbarHandle;
	private RectTransform _scrollbarComponentHandle;

	private GameObject _textCurrentInventoryItemName;
	private TextMeshProUGUI _textComponentCurrentInventoryItemName;

	private GameObject _imageCurrentInventoryItem;
	private Image _imageComponentCurrentInventoryItem;

	private GameObject _textCurrentInventoryItemDescription;
	private TextMeshProUGUI _textComponentCurrentInventoryItemDescription;
	private float _scrollbarHandleSize = 0.11f;
	private ViewModelJournalMenuSectionInventory _viewModelJournalMenuSectionInventory;
	private Bootstrap _bootstrap;

	private string _textMoneyRublesName;
	private string _textHealingItemName;
	private string _textManaReplenishItemName;

	private int _rubles;
	private int _healingItems;
	private int _manaReplenishItems;

	private LocalizationManager _localizationManager;
	private List<string> _currentKeyItemIDs;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		ViewModelJournalMenuSectionInventory viewModelJournalMenuSectionInventory)
	{
		_localizationManager = localizationManager;
		_viewModelJournalMenuSectionInventory = viewModelJournalMenuSectionInventory;
		_bootstrap = bootstrap;

		_textPlayerMoneyRubles = _viewModelJournalMenuSectionInventory.TextPlayerMoneyRubles;
		_textComponentPlayerMoneyRubles = _textPlayerMoneyRubles.GetComponent<TextMeshProUGUI>();

		_textHealingItems = _viewModelJournalMenuSectionInventory.TextHealingItems;
		_textComponentHealingItems = _textHealingItems.GetComponent<TextMeshProUGUI>();

		_textManaReplenishItems = _viewModelJournalMenuSectionInventory.TextManaReplenishItems;
		_textComponentManaReplenishItems = _textManaReplenishItems.GetComponent<TextMeshProUGUI>();

		_buttonsInventorySlots = new GameObject[_bootstrap.GameData.NumberOfSafeFileSlots];
		_buttonComponentsInventorySlots = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];

		_imagesInventorySlotItems = new GameObject[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imageComponentsInventorySlotItems = new Image[_bootstrap.GameData.NumberOfSafeFileSlots];
		_inventoryRows = new GameObject[_bootstrap.GameData.NumberOfInventoryRows];

		for (int i = 0; i < bootstrap.GameData.NumberOfInventoryRows; i++)
		{
			_inventoryRows[i] = _viewModelJournalMenuSectionInventory.InventoryRows[i];
		}

		for (int i = 0; i < bootstrap.GameData.NumberOfInventorySlots; i++)
		{
			_buttonsInventorySlots[i] = _viewModelJournalMenuSectionInventory.ButtonsInventorySlots[i];
			_buttonComponentsInventorySlots[i] = _buttonsInventorySlots[i].GetComponent<Button>();
			_imagesInventorySlotItems[i] = _viewModelJournalMenuSectionInventory.ImagesInventorySlotItems[i];
			_imageComponentsInventorySlotItems[i] = _imagesInventorySlotItems[i].GetComponent<Image>();

			int closureIndex = i;
			EventTrigger trigger = _buttonsInventorySlots[i].GetComponent<EventTrigger>();
			if (trigger == null) trigger = _buttonsInventorySlots[i].AddComponent<EventTrigger>();

			EventTrigger.Entry entryEnter = new EventTrigger.Entry();
			entryEnter.eventID = EventTriggerType.PointerEnter;
			entryEnter.callback.AddListener((data) => { OnInventorySlotHoverEnter(closureIndex); });
			trigger.triggers.Add(entryEnter);
		}

		_scrollbar = _viewModelJournalMenuSectionInventory.Scrollbar;
		_scrollbarComponent = _scrollbar.GetComponent<Scrollbar>();

		_scrollbarHandle = _viewModelJournalMenuSectionInventory.ScrollbarHandle;
		_scrollbarComponentHandle = _scrollbarHandle.GetComponent<RectTransform>();

		_textCurrentInventoryItemName = _viewModelJournalMenuSectionInventory.TextCurrentInventoryItemName;
		_textComponentCurrentInventoryItemName = _textCurrentInventoryItemName.GetComponent<TextMeshProUGUI>();

		_imageCurrentInventoryItem = _viewModelJournalMenuSectionInventory.ImageCurrentInventoryItem;
		_imageComponentCurrentInventoryItem = _imageCurrentInventoryItem.GetComponent<Image>();

		_textCurrentInventoryItemDescription = _viewModelJournalMenuSectionInventory.TextCurrentInventoryItemDescription;
		_textComponentCurrentInventoryItemDescription = _textCurrentInventoryItemDescription.GetComponent<TextMeshProUGUI>();

		Canvas.willRenderCanvases += EnforceFixedHandleSize;

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
	}

	public void UpdateInventoryMoneyRubles(int newNumber)
	{
		_rubles = newNumber;
		_textComponentPlayerMoneyRubles.text = $": {_rubles.ToString()} {_textMoneyRublesName}";
	}

	public void UpdateInventoryHealingItems(int newNumber)
	{
		_healingItems = newNumber;
		_textComponentHealingItems.text = $": {_healingItems.ToString()} {_textHealingItemName}";
	}

	public void UpdateInventoryManaReplenishItems(int newNumber)
	{
		_manaReplenishItems = newNumber;
		_textComponentManaReplenishItems.text = $": {_manaReplenishItems.ToString()} {_textManaReplenishItemName}";
	}

	private void OnInventorySlotHoverEnter(int slotIndex)
	{
		if (slotIndex >= _currentKeyItemIDs.Count) return;

		string keyItemID = _currentKeyItemIDs[slotIndex];
		InteractionObjectKeyItemData itemData = _bootstrap.GameData.KeyItemsList.KeyItemsInOrder
			.FirstOrDefault(x => x.KeyItemID.ToString() == keyItemID);

		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			_textComponentCurrentInventoryItemDescription.text = itemData.KeyItemDescription_RU.text;
		}
		if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
		{
			_textComponentCurrentInventoryItemDescription.text = itemData.KeyItemDescription_EN.text;
		}
		_imageCurrentInventoryItem.SetActive(true);
		_imageComponentCurrentInventoryItem.sprite = itemData.KeyItemSprite;
		_textComponentCurrentInventoryItemName.text = _localizationManager.GetLocalizedString(itemData.KeyItemName);
	}

	public void ReceiveKeyItemsData(List<string> keyItemIDs)
	{
		_textComponentCurrentInventoryItemName.text = string.Empty;
		_textComponentCurrentInventoryItemDescription.text = string.Empty;
		_imageCurrentInventoryItem.SetActive(false);

		_currentKeyItemIDs = keyItemIDs;

		if (keyItemIDs.Count <= 12)
		{
			_scrollbarHandle.SetActive(false);
		}
		else
		{
			_scrollbarHandle.SetActive(true);
		}

		int fullRowsCount = (keyItemIDs.Count + 3) / 4;
		Debug.Log("Количество полных строк: " + fullRowsCount);

		for (int i = 0; i < _inventoryRows.Length; i++)
		{
			_inventoryRows[i].SetActive(i < fullRowsCount);
		}

		for (int i = 0; i < _buttonsInventorySlots.Length; i++)
		{
			_buttonsInventorySlots[i].SetActive(false);
			_imageComponentsInventorySlotItems[i].sprite = null;
		}

		for (int i = 0; i < keyItemIDs.Count && i < _buttonsInventorySlots.Length; i++)
		{
			_buttonsInventorySlots[i].SetActive(true);

			InteractionObjectKeyItemData itemData = _bootstrap.GameData.KeyItemsList.KeyItemsInOrder
				.FirstOrDefault(x => x.KeyItemID.ToString() == keyItemIDs[i]);

			if (itemData != null)
			{
				_imageComponentsInventorySlotItems[i].sprite = itemData.KeyItemSprite;
			}
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textMoneyRublesName = _localizationManager.GetLocalizedString("Money_Ruble");
		_textHealingItemName = _localizationManager.GetLocalizedString("HealingItem");
		_textManaReplenishItemName = _localizationManager.GetLocalizedString("ManaReplenishItem");

		UpdateInventoryMoneyRubles(_rubles);
		UpdateInventoryHealingItems(_healingItems);
		UpdateInventoryManaReplenishItems(_manaReplenishItems);
	}
}