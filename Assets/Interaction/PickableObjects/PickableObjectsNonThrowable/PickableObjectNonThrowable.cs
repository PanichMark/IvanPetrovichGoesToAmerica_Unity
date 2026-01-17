using UnityEngine;

public class PickableObjectNonThrowable : PickableObjectAbstract
{
	// Статический метод фабрики для инициализации объекта с нужным именем
	public static PickableObjectNonThrowable CreateWithName(GameObject obj, string interactionItemNameUI)
	{
		var component = obj.AddComponent<PickableObjectNonThrowable>();
		component._interactionItemNameUI = interactionItemNameUI;
		return component;
	}
}


