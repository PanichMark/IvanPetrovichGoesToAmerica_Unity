using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionStep : MissionStepAbstract
{
	[TextArea(3, 10)]
	public string stepDescription = "Описание шага для игрока";
	public List<MissionStepConditionAbstract> conditions = new List<MissionStepConditionAbstract>();

	public override void OnStepCompleted()
	{
		if (AreAllConditionsMet())
		{
			// Находит активный менеджер в сцене и сообщает о завершении шага.
			FindObjectOfType<MissionsManager>().CompleteCurrentStep();
		}
	}

	private bool AreAllConditionsMet()
	{
		return conditions.All(cond => cond.IsConditionMet());
	}
}