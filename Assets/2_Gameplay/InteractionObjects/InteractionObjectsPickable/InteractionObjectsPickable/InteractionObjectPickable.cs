using UnityEngine;

public class InteractionObjectPickable : InteractionObjectPickableAbstract
{
	// Статический метод фабрики для инициализации объекта с нужным именем
	public static InteractionObjectPickable CreateWithName(GameObject obj, string interactionItemNameSystem)
	{
		var component = obj.AddComponent<InteractionObjectPickable>();
		component.interactionObjectNameSystem = interactionItemNameSystem;
		return component;
	}
}