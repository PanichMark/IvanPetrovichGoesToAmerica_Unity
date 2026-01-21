using UnityEngine;

public class InteractionObjectPickable : InteractionObjectPickableAbstract
{
	// Статический метод фабрики для инициализации объекта с нужным именем
	public static InteractionObjectPickable CreateWithName(GameObject obj, string interactionItemNameUI)
	{
		var component = obj.AddComponent<InteractionObjectPickable>();
		component._interactionItemNameUI = interactionItemNameUI;
		return component;
	}
}


