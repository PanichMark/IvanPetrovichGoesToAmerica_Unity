// Файл: MissionStepConditionAbstract.cs (в сборке Core)
using UnityEngine;

public abstract class MissionStepConditionAbstract : ScriptableObject
{
	// Ссылка на объект, которому принадлежит это условие (например, на Дверь)
	// Мы не будем перетаскивать её вручную.
	public GameObject OwnerObject { get; private set; }

	// Метод для регистрации владельца. Его будет вызывать скрипт на объекте.
	public void RegisterOwner(GameObject owner)
	{
		OwnerObject = owner;
	}

	// Метод, который вызывается, когда условие выполнено.
	// Он найдет MissionsManager и сообщит ему о завершении.
	protected void NotifyMissionManager()
	{
		var manager = FindObjectOfType<MissionsManager>();
		if (manager != null)
		{
			manager.CheckAndCompleteCurrentStep();
		}
		else
		{
			Debug.LogError("MissionsManager не найден в сцене!");
		}
	}

	public abstract bool IsConditionMet();
}