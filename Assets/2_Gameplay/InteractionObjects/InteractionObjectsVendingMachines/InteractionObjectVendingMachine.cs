using TMPro;
using UnityEngine;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] protected GameObject _goodsForSaleModel;
	[SerializeField] protected int _goodsPrice;
	[SerializeField] protected string _goodsName;
	[SerializeField] private string _vendingMachineName;
	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;
	private bool _isAdditionalInteractionHintActive;

	public virtual string InteractionHintMessageMain => $"Купить {_goodsName} в {InteractionObjectNameUI} за {_goodsPrice} рублей?";
	public virtual string InteractionHintMessageAdditional => "Недостаточно денег!";
	public string InteractionHintAction { get; protected set; }
	public string InteractionObjectNameSystem => _vendingMachineName;
	public virtual bool IsInteractionHintMessageAdditionalActive => _isAdditionalInteractionHintActive;

	public virtual string InteractionObjectNameUI => _vendingMachineName;

	protected void Start()
	{
		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
	}

	public void Interact()
	{
		if (_playerResourcesMoneyManager.PlayerMoney >= _goodsPrice)
		{
			Vector3 spawnPosition = transform.position + new Vector3(-1f, 0.5f, 0f);

			Debug.Log($"You bought {_goodsName} from {InteractionObjectNameUI}");
			Instantiate(_goodsForSaleModel, spawnPosition, Quaternion.identity);
			_playerResourcesMoneyManager.DeductMoney(-_goodsPrice);

			_isAdditionalInteractionHintActive = false;
		}
		else
		{
			Debug.Log("Not enough money");
			_isAdditionalInteractionHintActive = true;
		}
	}
}