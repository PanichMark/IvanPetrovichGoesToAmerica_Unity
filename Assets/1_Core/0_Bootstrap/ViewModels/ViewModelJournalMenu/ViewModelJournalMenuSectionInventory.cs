using System.Runtime.InteropServices;
using UnityEngine;

public class ViewModelJournalMenuSectionInventory
{
	public GameObject TextPlayerMoneyRubles;
	public GameObject TextHealingItems;
	public GameObject TextManaReplenishItems;

	public GameObject[] InventoryRows;
	public GameObject[] ButtonsInventorySlots;
	public GameObject[] ImagesInventorySlotItems;

	public GameObject Scrollbar;
	public GameObject ScrollbarHandle;

	public GameObject TextCurrentInventoryItemName;
	public GameObject ImageCurrentInventoryItem;
	public GameObject TextCurrentInventoryItemDescription;

	public ViewModelJournalMenuSectionInventory(Bootstrap bootstrap, GameObject canvas)
	{
		InventoryRows = new GameObject[bootstrap.GameData.NumberOfInventoryRows];
		ButtonsInventorySlots = new GameObject[bootstrap.GameData.NumberOfInventorySlots];
		ImagesInventorySlotItems = new GameObject[bootstrap.GameData.NumberOfInventorySlots];

		TextPlayerMoneyRubles = bootstrap.FindDeepGameObject(canvas, "TextPlayerMoneyRubles");
		TextHealingItems = bootstrap.FindDeepGameObject(canvas, "TextHealingItems");
		TextManaReplenishItems = bootstrap.FindDeepGameObject(canvas, "TextManaReplenishItems");

		Scrollbar = bootstrap.FindDeepGameObject(canvas, "Scrollbar");
		ScrollbarHandle = bootstrap.FindDeepGameObject(canvas, "ScrollbarHandle");

		TextCurrentInventoryItemName = bootstrap.FindDeepGameObject(canvas, "TextCurrentInventoryItemName");
		ImageCurrentInventoryItem = bootstrap.FindDeepGameObject(canvas, "ImageCurrentInventoryItem");
		TextCurrentInventoryItemDescription = bootstrap.FindDeepGameObject(canvas, "TextCurrentInventoryItemDescription");

		for (int i = 0; i < bootstrap.GameData.NumberOfInventoryRows; i++)
		{
			string inventoryRow = "InventoryRow" + (i + 1);
			InventoryRows[i] = bootstrap.FindDeepGameObject(canvas, inventoryRow);
		}

		for (int i = 0; i < bootstrap.GameData.NumberOfInventorySlots; i++)
		{
			
			string inventoryItemSlot = "InventoryItemSlot" + (i + 1);
			string inventoryItemSlotImage = "InventoryItemSlotImage" + (i + 1);

			
			ButtonsInventorySlots[i] = bootstrap.FindDeepGameObject(canvas, inventoryItemSlot);
			ImagesInventorySlotItems[i] = bootstrap.FindDeepGameObject(canvas, inventoryItemSlotImage);
		}
	}
}
