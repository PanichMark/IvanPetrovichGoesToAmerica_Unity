using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableNonThrowable : InteractionObjectPickableAbstract
{
	public static InteractionObjectPickableNonThrowable CreateWithName(GameObject obj, string interactionItemNameSystem)
	{
		var component = obj.GetComponent<InteractionObjectPickableNonThrowable>();
		if (component == null)
		{
			component = obj.AddComponent<InteractionObjectPickableNonThrowable>();
		}
		component._interactionObjectNameSystem = interactionItemNameSystem;

		return component;
	}

	public override void SaveData(ref GameData data)
	{
		List<PickableObjectData> targetList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			targetList = data.PickableObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Church))
		{
			targetList = data.PickableObjects_Scene_1_Church;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			targetList = data.PickableObjects_Scene_1_Street;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_RevenueHouse))
		{
			targetList = data.PickableObjects_Scene_1_RevenueHouse;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_InnerYard))
		{
			targetList = data.PickableObjects_Scene_1_InnerYard;
		}

		if (targetList != null)
		{
			int indexInList = targetList.FindIndex(item => item.PickableObjectIndex == PickableObjectIndex);

			if (indexInList != -1)
			{
				PickableObjectData updatedItem = new PickableObjectData
				{
					PickableObjectIndex = PickableObjectIndex,
					PickableObjectNameSystem = InteractionObjectNameSystem,
					WasPickableObjectPickedUp = IsObjectPickedUp
				};

				targetList[indexInList] = updatedItem;
			}
			else
			{
				targetList.Add(new PickableObjectData
				{
					PickableObjectIndex = PickableObjectIndex,
					PickableObjectNameSystem = InteractionObjectNameSystem,
					WasPickableObjectPickedUp = IsObjectPickedUp
				});
			}
		}
	}

	public override void LoadData(GameData data)
	{
		
	}
}