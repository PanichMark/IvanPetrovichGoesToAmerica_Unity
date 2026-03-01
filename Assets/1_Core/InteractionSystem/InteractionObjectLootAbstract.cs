using UnityEngine;
using System;

using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public abstract class InteractionObjectLootAbstract : MonoBehaviour, IInteractable, IInteractGainedItem, ISaveLoad
{
	
	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;
	protected LocalizationManager localizationManager;
	public Collider Collider { get; protected set; }

	public virtual string InteractionObjectNameUI {  get; protected set; }

	public Sprite LootObjectImage;

	public virtual string InteractionHintMessageMain => $"{InteractionHintAction} {InteractionObjectNameUI}";
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	private void Start()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		Collider = GetComponent<Collider>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("Player");

		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Loot");
		ThisMethodSetsActionName();
	}


	protected virtual void ThisMethodSetsActionName()
	{

	}
	public string InteractionHintAction { get; protected set; }

	public virtual bool WasLootItemCollected { get; protected set; }


	// Поле для внутреннего индекса и хранения типа предмета
	public int LootItemIndex { get; protected set; }

	public TextMeshProUGUI GainedItemtext => null;

	public virtual Sprite GainedItemImage => LootObjectImage;

	internal void AssignLootItemIndex(int index)
	{
		LootItemIndex = index;
	}



	public virtual void Interact()
	{
			if (CachedPlayer != null)
			{
				Collider.enabled = false;
				gameObject.tag = "Untagged";

				StartCoroutine(MoveTowardsTarget());
			}
			else
			{
				Debug.Log("Player not found!");
			}
	}
	public GameObject CachedPlayer { get; protected set; }
	IEnumerator MoveTowardsTarget()
	{
		while (true)
		{
			// Рассчитываем новую целевую позицию каждый кадр
			Vector3 targetPosition = CachedPlayer.transform.position + Vector3.up * 1f;

			// Перемещаем объект к новой позиции
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			// Выход из цикла, если объект вплотную приблизился к игроку
			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				Destroy(gameObject);
				break;
			}

			yield return null;
		}
	}

	public void SaveData(ref GameData data)
	{


		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			data.LootObjects_Scene_0_Test[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootObjects_Scene_0_Test[LootItemIndex].LootItemName = InteractionObjectNameSystem;

			if (WasLootItemCollected == true)
			{
				data.LootObjects_Scene_0_Test[LootItemIndex].WasLootItemCollected = true;
			}
			else data.LootObjects_Scene_0_Test[LootItemIndex].WasLootItemCollected = false;

		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
		{
			data.LootObjects_Scene_1_StreetMain[LootItemIndex].LootItemIndex = LootItemIndex;
			data.LootObjects_Scene_1_StreetMain[LootItemIndex].LootItemName = InteractionObjectNameSystem;

			if (WasLootItemCollected == true)
			{
				data.LootObjects_Scene_1_StreetMain[LootItemIndex].WasLootItemCollected = true;
			}
			else data.LootObjects_Scene_1_StreetMain[LootItemIndex].WasLootItemCollected = false;

		}
		

	}


	public void LoadData(GameData data)
	{

		//Debug.Log("This is: " + LootItemIndex + " " + interactionObjectNameSystem);
		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
		
			if (data.LootObjects_Scene_0_Test[LootItemIndex].WasLootItemCollected == true)
			{
				
				WasLootItemCollected = true;
				Destroy(gameObject);
				//Debug.Log("DESTROY: " + LootItemIndex);
			}
			//Debug.Log("Its #: " + data.LootObjects_Scene_0_Test[LootItemIndex]);
		}

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_StreetMain))
		{
			if (data.LootObjects_Scene_1_StreetMain[LootItemIndex].WasLootItemCollected == true)
			{
				WasLootItemCollected = true;
				Destroy(gameObject);
			}
		}
		
	}
}

