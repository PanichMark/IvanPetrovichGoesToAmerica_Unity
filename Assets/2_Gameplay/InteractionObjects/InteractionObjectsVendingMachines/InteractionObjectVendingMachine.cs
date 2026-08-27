using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectVendingMachine : GameplayObjectJsonSaveLoad, IInteractable, IElectroShockable
{
	public delegate void OutOfServiceHandler();
	public event OutOfServiceHandler OnWentOutOfService;

	public delegate void InteractionDelegate();
	private List<GameObject> _spawnedGoods = new List<GameObject>();
	[Header("Object Info")]
	[SerializeField] private string _vendingMachineName;
	[SerializeField] private PlayerMoneyTypes _moneyType;



	[Header("Goods Info")]
	[SerializeField] protected GameObject _goodsForSale;
	[SerializeField] protected int _goodsPrice;
	protected string _goodsName;
	protected float _vendingMachineElectroHealth;
	public bool IsOutOfService { get; protected set; }
	private InteractionObjectLootAbstract _goodsComponent;

	private string _moneyForUI;
	public event IInteractable.InteractableObjectHandler OnInteract;
	private PlayerMoneyController _playerResourcesMoneyManager;
	private bool _isAdditionalInteractionHintActive;
	private LocalizationManager _localizationManager;


	private string _interactionHintMessageFail;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {_goodsName} {InteractionObjectNameUI} {_goodsPrice} {_moneyForUI}?";
	public virtual string InteractionHintMessageFail => _interactionHintMessageFail;
	public string InteractionHintMessageAction => _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Purchase");
	public string InteractionObjectNameSystem => _vendingMachineName;
	public virtual bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	public virtual string InteractionObjectNameUI => _localizationManager.GetLocalizedString(InteractionObjectNameSystem);

	protected void Start()
	{
		SetpUpVendingMachine();

		_goodsComponent = _goodsForSale.GetComponent<InteractionObjectLootAbstract>();

		_playerResourcesMoneyManager = ServiceLocator.Resolve<PlayerMoneyController>();
_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString($"Money_{_moneyType}");

		_localizationManager.OnLanguageChanged += ChangeLangauge;
	}

	public virtual void SetpUpVendingMachine()
	{

	}

	public void ChangeLangauge(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_goodsName = _localizationManager.GetLocalizedString(_goodsComponent.InteractionObjectNameSystem);
		_moneyForUI = _localizationManager.GetLocalizedString($"Money_{_moneyType}");
	}
	protected void InvokeOnWentOutOfService()
	{
		OnWentOutOfService?.Invoke();
	}

	public void Interact()
	{
		if (!IsOutOfService)
		{
			if (_playerResourcesMoneyManager.PlayerMoney >= _goodsPrice)
			{
				_spawnedGoods.RemoveAll(item => item == null || !item.activeInHierarchy);

				if (_spawnedGoods.Count >= 5)
				{
					//Debug.Log(_spawnedGoods.Count);
					Debug.Log("Нельзя купить больше");
					_isAdditionalInteractionHintActive = true;
					_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_PickUpBoughtGoods")}!";
					return;
				}

				SpawnGoods();

				Debug.Log($"Вы купили {_goodsName} из {InteractionObjectNameUI}");

				_playerResourcesMoneyManager.DeductMoney(-_goodsPrice);
				_isAdditionalInteractionHintActive = false;

				//Debug.Log(_spawnedGoods.Count);
			}
			else
			{
				Debug.Log("Недостаточно денег");

				_isAdditionalInteractionHintActive = true;
				_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Money")}!";
			}
		}
		else
		{
			Debug.Log("Out of service");
			_isAdditionalInteractionHintActive = true;
			_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_OutOfService")}!";
		}
	}

	protected void SpawnGoods()
	{
		Vector3 spawnPosition = transform.localPosition + transform.TransformDirection(new Vector3(0, 0.5f, 1));
		float yRotation = transform.eulerAngles.y;
		Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0);

		GameObject instantiatedObject = Instantiate(_goodsForSale, spawnPosition, spawnRotation);

		InteractionObjectLootAbstract spawnedGoodComponent = instantiatedObject.GetComponent<InteractionObjectLootAbstract>();
		spawnedGoodComponent.SetLootObjectAsVendingMachineGood();

		SceneManager.MoveGameObjectToScene(instantiatedObject, SceneManager.GetSceneAt(1));

		Rigidbody rb = instantiatedObject.AddComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.useGravity = true;


		_spawnedGoods.Add(instantiatedObject);
		
	}

	public void InteractCutscene()
	{
		Interact();
	}

	public void Electrify(float damage)
	{
		if (!IsOutOfService)
		{
			_vendingMachineElectroHealth -= damage;

			if (_vendingMachineElectroHealth <= 0)
			{
				IsOutOfService = true;

				OnWentOutOfService?.Invoke();

				StartCoroutine(BreakDownAndSpawnGoods());
			}
		}
	}

	private IEnumerator BreakDownAndSpawnGoods()
	{
		for (int i = 0; i < 5; i++)
		{
			SpawnGoods();
			yield return new WaitForSeconds(0.15f);
		}
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.VendingMachinesData == null)
		{
			data.VendingMachinesData = new Dictionary<GameScenesGameplayDataEnum, List<VendingMachineData>>();
		}
		if (!data.VendingMachinesData.ContainsKey(currentScene))
		{
			data.VendingMachinesData[currentScene] = new List<VendingMachineData>();
		}

		var targetList = data.VendingMachinesData[currentScene];
		int indexInList = targetList.FindIndex(item => item.VendingMachineIndex == GameplayObjectIndex);

		var updatedItem = new VendingMachineData
		{
			VendingMachineIndex = GameplayObjectIndex,
			VendingMachineNameSystem = _vendingMachineName,
			VendingMachineHealth = _vendingMachineElectroHealth,
			VendingMachineSpawnedGoods = _spawnedGoods.Count
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.VendingMachinesData == null || !data.VendingMachinesData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			VendingMachineData savedState = sourceList.Find(item => item.VendingMachineIndex == GameplayObjectIndex);

			if (savedState.VendingMachineIndex != 0)
			{
				_vendingMachineElectroHealth = savedState.VendingMachineHealth;

				if (savedState.VendingMachineSpawnedGoods > 0)
				{
					for (int i = 0; i < savedState.VendingMachineSpawnedGoods; i++)
					{
						SpawnGoods();
					}
				}

				if (_vendingMachineElectroHealth <= 0)
				{
					IsOutOfService = true;
				}
			}
		}

		yield return null;
	}
}