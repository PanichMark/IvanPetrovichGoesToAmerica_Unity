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
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesEnum currentScene)) return;

		// Инициализируем словарь или список для текущей сцены, если их нет
		if (data.PickableObjectsByScene == null)
		{
			data.PickableObjectsByScene = new Dictionary<GameScenesEnum, List<PickableObjectData>>();
		}
		if (!data.PickableObjectsByScene.ContainsKey(currentScene))
		{
			data.PickableObjectsByScene[currentScene] = new List<PickableObjectData>();
		}

		var targetList = data.PickableObjectsByScene[currentScene];

		int indexInList = targetList.FindIndex(item => item.PickableObjectIndex == PickableObjectIndex);

		var updatedItem = new PickableObjectData
		{
			PickableObjectIndex = PickableObjectIndex,
			PickableObjectNameSystem = InteractionObjectNameSystem,
			WasPickableObjectPickedUp = IsObjectPickedUp
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}
	}

	public override void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesEnum currentScene)) return;

		if (data.PickableObjectsByScene == null || !data.PickableObjectsByScene.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == PickableObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) return;

		IsObjectPickedUp = savedState.WasPickableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			gameObject.SetActive(false);
		}
	}
}