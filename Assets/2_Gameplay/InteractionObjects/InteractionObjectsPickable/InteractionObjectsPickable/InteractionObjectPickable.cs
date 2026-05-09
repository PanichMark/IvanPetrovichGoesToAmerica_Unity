using UnityEngine;

public class InteractionObjectPickable : InteractionObjectPickableAbstract
{
	public static InteractionObjectPickable CreateWithName(GameObject obj, string interactionItemNameSystem)
	{
		var component = obj.AddComponent<InteractionObjectPickable>();
		component.interactionObjectNameSystem = interactionItemNameSystem;
		return component;
	}
}