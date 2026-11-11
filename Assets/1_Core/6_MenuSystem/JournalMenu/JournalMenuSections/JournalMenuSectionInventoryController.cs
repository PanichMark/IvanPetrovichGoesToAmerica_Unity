using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalMenuSectionInventoryController : MonoBehaviour
{
	public GameObject _textPlayerMoneyRubles;
	public TextMeshProUGUI _textComponentPlayerMoneyRubles;

	public GameObject _textHealingItems;
	public TextMeshProUGUI _textComponentHealingItems;

	public GameObject _textManaReplenishItems;
	public TextMeshProUGUI _textComponentManaReplenishItems;

	public GameObject[] _buttonsInventorySlots;
	public Button[] _buttonComponentsInventorySlots;
	public GameObject[] _imagesInventorySlotItems;
	public Image[] _imageComponentsInventorySlotItems;

	public GameObject _scrollbar;
	public Scrollbar _scrollbarComponent;

	public GameObject _scrollbarHandle;
	public RectTransform _scrollbarComponentHandle;

	public GameObject _textCurrentInventoryItemName;
	public TextMeshProUGUI _textComponentCurrentInventoryItemName;

	public GameObject _imageCurrentInventoryItem;
	public Image _imageComponentCurrentInventoryItem;

	public GameObject _textCurrentInventoryItemDescription;
	public TextMeshProUGUI _textComponentCurrentInventoryItemDescription;
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

		for (int i = 0; i < bootstrap.GameData.NumberOfInventorySlots; i++)
		{
			_buttonsInventorySlots[i] = _viewModelJournalMenuSectionInventory.ButtonsInventorySlots[i];
			_buttonComponentsInventorySlots[i] = _buttonsInventorySlots[i].GetComponent<Button>();
			_imagesInventorySlotItems[i] = _viewModelJournalMenuSectionInventory.ImagesInventorySlotItems[i];
			_imageComponentsInventorySlotItems[i] = _imagesInventorySlotItems[i].GetComponent<Image>();
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