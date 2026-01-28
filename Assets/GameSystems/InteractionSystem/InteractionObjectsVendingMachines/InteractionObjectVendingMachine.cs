using TMPro;
using UnityEngine;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractUsedItem, IInteractable
{
	[SerializeField] private GameObject goodsForSaleModel;
	[SerializeField] private int goodsPrice;
	[SerializeField] private string goodsName;
	[SerializeField] private string vendingMachineName;
	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	//[SerializeField] private string additionalInteractionHint;
	private bool isAdditionalInteractionHintActive;
	public virtual string InteractionObjectNameUI => vendingMachineName;
	public virtual string MainInteractionHint => $"Купить {goodsName} в {InteractionObjectNameUI} за {goodsPrice} рублей?";
	public virtual string AdditionalInteractionHint => "Недостаточно денег!";

	public string InteractionObjectNameSystem => null;
	public virtual bool IsAdditionalInteractionHintActive => isAdditionalInteractionHintActive;

	

	private void Start()
	{
		playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");

	}

	
	public void Interact()
	{
		
		if (playerResourcesMoneyManager.PlayerMoney >= goodsPrice)
		{
			Vector3 spawnPosition = transform.position + new Vector3(-1f, 0.5f, 0f); // Сместили объект вверх на единицу

			Debug.Log($"Вы купили {goodsName} в {InteractionObjectNameUI}");
			Instantiate(goodsForSaleModel, spawnPosition, Quaternion.identity);
			playerResourcesMoneyManager.DeductMoney(-goodsPrice);

			isAdditionalInteractionHintActive = false;

		}
		else
		{
			Debug.Log("Not enought Money");

			isAdditionalInteractionHintActive = true;
		}
		
	}
}
	

