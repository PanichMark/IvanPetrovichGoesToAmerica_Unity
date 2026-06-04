using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IGainedItem, ISaveLoad
{
	public event IInteractable.InteractableObjectHandler OnInteract;
	[SerializeField] protected string _interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;

	public GameObject GameObjectPlayer { get; protected set; }
	protected LocalizationManager _localizationManager;
	public Collider LootObjectCollider { get; protected set; }
	public string InteractionHintMessageAction { get; protected set; }

	public virtual bool WasLootItemCollected { get; protected set; }

	public int LootObjectIndex { get; protected set; }

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
		SetUpLootObjectReferences();
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public virtual void Interact()
	{
		LootObjectCollider.enabled = false;
		gameObject.tag = "Untagged";
		StartCoroutine(MoveTowardsPlayer());
		OnInteract?.Invoke();
	}

	public virtual void InteractCutscene()
	{
		Destroy(gameObject);
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Loot");
	}

	protected virtual void SetUpLootObjectReferences()
	{

	}

	internal void AssignLootObjectsIndex(int index)
	{
		LootObjectIndex = index;
	}

	IEnumerator MoveTowardsPlayer()
	{
		float currentSpeed = 3.5f; 
		float speedIncrease = 5;

		while (true)
		{
			Vector3 targetPosition = GameObjectPlayer.transform.position + Vector3.up * 1f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				Destroy(gameObject);
				break;
			}

			currentSpeed += speedIncrease * Time.deltaTime;
			yield return null;
		}
	}

	public void SaveData(ref GameData data)
	{
		List<LootObjectData> targetList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			targetList = data.LootObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			targetList = data.LootObjects_Scene_1_StreetMain;
		}

		if (targetList != null)
		{
			// 1. Ищем индекс нашего объекта в списке
			int indexInList = targetList.FindIndex(item => item.LootObjectIndex == this.LootObjectIndex);

			if (indexInList != -1)
			{
				// 2. Если нашли, создаем НОВУЮ структуру с обновленными данными
				LootObjectData updatedItem = new LootObjectData
				{
					LootObjectIndex = this.LootObjectIndex,
					LootObjectNameSystem = this.InteractionObjectNameSystem,
					WasLootObjectCollected = this.WasLootItemCollected // Здесь должно быть true
				};

				// 3. Заменяем СТАРЫЙ элемент в списке на НОВЫЙ
				targetList[indexInList] = updatedItem;
			}
			else
			{
				// Если не нашли - просто добавляем новый (логика не меняется)
				targetList.Add(new LootObjectData
				{
					LootObjectIndex = this.LootObjectIndex,
					LootObjectNameSystem = this.InteractionObjectNameSystem,
					WasLootObjectCollected = this.WasLootItemCollected
				});
			}
		}
	}

	public void LoadData(GameData data)
	{
		// Определяем, из какого списка мы будем загружать
		List<LootObjectData> sourceList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			sourceList = data.LootObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			sourceList = data.LootObjects_Scene_1_StreetMain;
		}

		// Если сцена определена и список существует и не пуст
		if (sourceList != null && sourceList.Count > 0)
		{
			// Ищем данные для нашего объекта
			LootObjectData savedState = sourceList.Find(item => item.LootObjectIndex == this.LootObjectIndex);

			// --- ИСПРАВЛЕННАЯ ПРОВЕРКА ---
			// Проверяем, является ли индекс найденного элемента ненулевым.
			// Если он 0, значит, ничего не нашлось.
			if (savedState.LootObjectIndex != 0 && savedState.WasLootObjectCollected)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
	}
}