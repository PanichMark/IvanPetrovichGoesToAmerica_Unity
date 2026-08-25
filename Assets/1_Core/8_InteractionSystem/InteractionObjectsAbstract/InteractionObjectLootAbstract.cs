using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLootAbstract : GameplayObjectJsonSaveLoad, IInteractable, IGainedItem
{
	public event IInteractable.InteractableObjectHandler OnInteract;
	[SerializeField] protected string _interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;

	public GameObject GameObjectPlayer { get; protected set; }
	protected LocalizationManager _localizationManager;
	public Collider LootObjectCollider { get; protected set; }
	public string InteractionHintMessageAction { get; protected set; }

	public bool WasLootItemCollected { get; protected set; }


	public TextMeshProUGUI NameGainedItem => null;

	public virtual Sprite IconGainedItem => LootObjectIcon;
	public virtual string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameSystem)}";

	public virtual Sprite LootObjectIcon { get; protected set; }

	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Max")} {InteractionObjectNameUI}!";
	public virtual bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		LootObjectCollider = GetComponent<Collider>();
		GameObjectPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
		InitializeLootObject();
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public virtual void Interact()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			Destroy(rb);
		}

		LootObjectCollider.enabled = false;
		gameObject.tag = "Untagged";
		StartCoroutine(MoveTowardsPlayer());
		OnInteract?.Invoke();
	}

	public virtual void InteractCutscene()
	{
		gameObject.SetActive(false);
	}

	public virtual void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
	}

	protected virtual void InitializeLootObject()
	{

	}

	private IEnumerator MoveTowardsPlayer()
	{
		float currentSpeed = 3.5f; 
		float speedIncrease = 5;

		while (true)
		{
			Vector3 targetPosition = GameObjectPlayer.transform.position + Vector3.up * 1f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				gameObject.SetActive(false);
				break;
			}

			currentSpeed += speedIncrease * Time.deltaTime;
			yield return null;
		}

		WasLootItemCollected = true;

		OnAfterLooted();
	}

	protected virtual void OnAfterLooted()
	{

	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.LootObjectsData == null)
		{
			data.LootObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<LootObjectData>>();
		}
		if (!data.LootObjectsData.ContainsKey(currentScene))
		{
			data.LootObjectsData[currentScene] = new List<LootObjectData>();
		}

		var targetList = data.LootObjectsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.LootObjectIndex == GameplayObjectIndex);

		var updatedItem = new LootObjectData
		{
			LootObjectIndex = GameplayObjectIndex,
			LootObjectNameSystem = InteractionObjectNameSystem,
			IsLootObjectCollected = WasLootItemCollected
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

		if (data.LootObjectsData == null || !data.LootObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			LootObjectData savedState = sourceList.Find(item => item.LootObjectIndex == GameplayObjectIndex);

			//Debug.Log($"{savedState.LootObjectNameSystem} {gameObject.name}");
			//Debug.Log(savedState.IsLootObjectCollected);

			if (savedState.LootObjectIndex != 0 && savedState.IsLootObjectCollected)
			{
				WasLootItemCollected = true;
				gameObject.SetActive(false);
			}
		}

		yield return null;
	}
}