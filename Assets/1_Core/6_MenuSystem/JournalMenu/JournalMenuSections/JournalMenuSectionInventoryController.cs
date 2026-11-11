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

	private ViewModelJournalMenuSectionInventory _viewModelJournalMenuSectionInventory;
	private Bootstrap _bootstrap;
	public void Initialize(
		Bootstrap bootstrap,
		ViewModelJournalMenuSectionInventory viewModelJournalMenuSectionInventory)
	{
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
	}
}