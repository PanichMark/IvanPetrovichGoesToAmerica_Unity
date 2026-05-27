using TMPro;
using UnityEngine;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private string _vendingMachineName;
	[SerializeField] protected GameObject _goodsForSale;
	[SerializeField] protected int _goodsPrice;
	protected string _goodsName;
	private InteractionObjectLootAbstract _goodsComponent;
	[SerializeField] private string _moneyType;
	private string _moneyForUI;
	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;
	private bool _isAdditionalInteractionHintActive;
	private LocalizationManager _localizationManager;
	
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {_goodsName} {InteractionObjectNameUI} {_goodsPrice} {_moneyForUI}?";
	public virtual string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Money")}!";
	public string InteractionHintMessageAction => _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Purchase");
	public string InteractionObjectNameSystem => _vendingMachineName;
	public virtual bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	public virtual string InteractionObjectNameUI => _localizationManager.GetLocalizedString(InteractionObjectNameSystem);

	protected void Start()
	{
		_goodsComponent = _goodsForSale.GetComponent<InteractionObjectLootAbstract>();

		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString(_moneyType);

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	public void ChangeLangauge()
	{
		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString(_moneyType);
	}

	public void Interact()
	{
		if (_playerResourcesMoneyManager.PlayerMoney >= _goodsPrice)
		{
			Vector3 spawnPosition = transform.localPosition + transform.TransformDirection(new Vector3(0, 0.5f, 1));

			// Получаем текущий поворот по Y
			float yRotation = transform.eulerAngles.y;

			// Создаём кватернион только с поворотом по Y
			Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0);

			Debug.Log($"You bought {_goodsName} from {InteractionObjectNameUI}");
			Instantiate(_goodsForSale, spawnPosition, spawnRotation);
			_playerResourcesMoneyManager.DeductMoney(-_goodsPrice);

			_isAdditionalInteractionHintActive = false;
		}
		else
		{
			Debug.Log("Not enough money");
			_isAdditionalInteractionHintActive = true;
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}
}