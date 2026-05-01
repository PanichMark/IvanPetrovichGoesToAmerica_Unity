using TMPro;
using UnityEngine;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractable
{
	[SerializeField] protected GameObject goodsForSaleModel;
	[SerializeField] protected int goodsPrice;
	[SerializeField] protected string goodsName;
	[SerializeField] private string vendingMachineName;
	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	//[SerializeField] private string additionalInteractionHint;
	private bool isAdditionalInteractionHintActive;
	public virtual string InteractionHintMessageMain => $"Купить {goodsName} в {InteractionObjectNameUI} за {goodsPrice} рублей?";
	public virtual string InteractionHintMessageAdditional => "Недостаточно денег!";
	public string InteractionHintAction { get; protected set; }
	public string InteractionObjectNameSystem => vendingMachineName;
	public virtual bool IsInteractionHintMessageAdditionalActive => isAdditionalInteractionHintActive;

	private LocalizationManager localizationManager;
	public virtual string InteractionObjectNameUI => vendingMachineName;
		/*
	{
		get
		{
			localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
			string localizedString = localizationManager.GetLocalizedString(vendingMachineName);

			if (localizedString.StartsWith("Unknown key"))
			{
				Debug.LogError($"Key '{vendingMachineName}' not found");
				return vendingMachineName; // Возврат оригинала, если нет перевода
			}
			return localizedString;
		}
	}
		*/
	protected void Start()
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
	

