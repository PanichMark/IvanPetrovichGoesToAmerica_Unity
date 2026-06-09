using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectVendingMachine : MonoBehaviour, IInteractable, IElectroShockable
{
	public delegate void InteractionDelegate();
	private List<GameObject> _spawnedGoods = new List<GameObject>();
	[SerializeField] private string _vendingMachineName;
	[SerializeField] protected GameObject _goodsForSale;
	[SerializeField] protected int _goodsPrice;
	protected string _goodsName;
	private InteractionObjectLootAbstract _goodsComponent;
	[SerializeField] private string _moneyType;
	private string _moneyForUI;
	public event IInteractable.InteractableObjectHandler OnInteract;
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
		SetpUpVendingMachine();

		_goodsComponent = _goodsForSale.GetComponent<InteractionObjectLootAbstract>();

		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerResourcesMoneyManager>("PlayerResourcesMoneyManager");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString(_moneyType);

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	public virtual void SetpUpVendingMachine()
	{

	}

	public void ChangeLangauge(LocalizationManager localizationManager)
	{
		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString(_moneyType);
	}

	public void Interact()
	{
		if (_playerResourcesMoneyManager.PlayerMoney >= _goodsPrice)
		{
			_spawnedGoods.RemoveAll(item => item == null || !item.activeInHierarchy);

			if (_spawnedGoods.Count >= 10)
			{
				//Debug.Log(_spawnedGoods.Count);
				Debug.Log("Нельзя купить больше");
				return;
			}

			Vector3 spawnPosition = transform.localPosition + transform.TransformDirection(new Vector3(0, 0.5f, 1));
			float yRotation = transform.eulerAngles.y;
			Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0);

			Debug.Log($"Вы купили {_goodsName} из {InteractionObjectNameUI}");

			GameObject instantiatedObject = Instantiate(_goodsForSale, spawnPosition, spawnRotation);
			SceneManager.MoveGameObjectToScene(instantiatedObject, SceneManager.GetSceneAt(1));
			
			Rigidbody rb = instantiatedObject.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.useGravity = true;

			_spawnedGoods.Add(instantiatedObject);

			_playerResourcesMoneyManager.DeductMoney(-_goodsPrice);
			_isAdditionalInteractionHintActive = false;

			//Debug.Log(_spawnedGoods.Count);
		}
		else
		{
			Debug.Log("Недостаточно денег");
			_isAdditionalInteractionHintActive = true;
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	public void Electrify()
	{
		//throw new System.NotImplementedException();
	}
}