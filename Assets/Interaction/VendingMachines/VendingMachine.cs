using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
	[SerializeField] private GameObject goodsForSaleModel;
	[SerializeField] private int goodsPrice;
	[SerializeField] private string goodsName;
	[SerializeField] private string vendingMachineName;
	private string interactionHint;
	public virtual string InteractionObjectNameUI => vendingMachineName;
	public virtual string InteractionHint => interactionHint;
	

	public string InteractionObjectNameSystem => null;



	private void Start()
	{
		if (PlayerMoneyManager.Instance.PlayerMoney < goodsPrice)
		{
			interactionHint = "Недостаточно денег для покупки";
		}
		else interactionHint = $"Купить {goodsName} в {InteractionObjectNameUI}?";

	}
	public void Interact()
	{
		if (PlayerMoneyManager.Instance.PlayerMoney >= goodsPrice)
		{
			Vector3 spawnPosition = transform.position + new Vector3(-1f, 0.5f, 0f); // Сместили объект вверх на единицу

			Debug.Log($"Вы купили {goodsName} в {InteractionObjectNameUI}");
			Instantiate(goodsForSaleModel, spawnPosition, Quaternion.identity);
			PlayerMoneyManager.Instance.DeductMoney(-goodsPrice);
			//interactionHint = "bruh!";
		}
		else Debug.Log("Not enought Money");
	}
}
	