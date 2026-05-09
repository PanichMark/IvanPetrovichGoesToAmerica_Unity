using TMPro;
using UnityEngine;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] protected GameObject goodsForSaleModel;
	[SerializeField] protected int goodsPrice;
	[SerializeField] protected string goodsName;
	[SerializeField] private string vendingMachineName;
	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	private bool isAdditionalInteractionHintActive;

	public virtual string InteractionHintMessageMain => $"Купить {goodsName} в {InteractionObjectNameUI} за {goodsPrice} рублей?";
	public virtual string InteractionHintMessageAdditional => "Недостаточно денег!";
	public string InteractionHintAction { get; protected set; }
	public string InteractionObjectNameSystem => vendingMachineName;
	public virtual bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;

	public virtual string InteractionObjectNameUI => vendingMachineName;

	protected void Start()
	{
		playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
	}

	public void Interact()
	{
		if (playerResourcesMoneyManager.PlayerMoney >= goodsPrice)
		{
			Vector3 spawnPosition = transform.position + new Vector3(-1f, 0.5f, 0f);

			Debug.Log($"You bought {goodsName} from {InteractionObjectNameUI}");
			Instantiate(goodsForSaleModel, spawnPosition, Quaternion.identity);
			playerResourcesMoneyManager.DeductMoney(-goodsPrice);

			isAdditionalInteractionHintActive = false;
		}
		else
		{
			Debug.Log("Not enough money");
			isAdditionalInteractionHintActive = true;
		}
	}
}