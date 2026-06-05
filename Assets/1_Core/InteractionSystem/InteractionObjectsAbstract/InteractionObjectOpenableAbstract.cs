using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable, ISaveLoad
{
	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI => null;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public int OpenableObjectIndex { get; protected set; }
	public bool WasOpenableUnlocked { get; protected set; }
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	public virtual bool IsObjectOpened { get; protected set; }


	public event IInteractable.InteractableObjectHandler OnInteract;
	public void AssignOpenableObjectsIndexes(int index)
	{
		OpenableObjectIndex = index;
	}
	public abstract void Interact();

	public abstract void InteractCutscene();

	public void SaveData(ref GameData data)
	{
		List<OpenableObjectData> targetList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			targetList = data.OpenableObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Church))
		{
			targetList = data.OpenableObjects_Scene_1_Church;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			targetList = data.OpenableObjects_Scene_1_Street;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_RevenueHouse))
		{
			targetList = data.OpenableObjects_Scene_1_RevenueHouse;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_InnerYard))
		{
			targetList = data.OpenableObjects_Scene_1_InnerYard;
		}

		if (targetList != null)
		{
			int indexInList = targetList.FindIndex(item => item.OpenableObjectIndex == OpenableObjectIndex);

			if (indexInList != -1)
			{
				var existingItem = targetList[indexInList];

				existingItem.WasOpenableObjectUnlocked = WasOpenableUnlocked;
				existingItem.WasOpenableObjectOpened = IsObjectOpened;
				existingItem.OpenableObjectNameSystem = InteractionObjectNameSystem;

				targetList[indexInList] = existingItem;
			}
			else
			{
				targetList.Add(new OpenableObjectData
				{
					OpenableObjectIndex = OpenableObjectIndex,
					OpenableObjectNameSystem = InteractionObjectNameSystem,
					WasOpenableObjectUnlocked = WasOpenableUnlocked,
					WasOpenableObjectOpened = IsObjectOpened
				});
			}
		}
	}

	public void LoadData(GameData data)
	{
		//throw new System.NotImplementedException();
	}
}