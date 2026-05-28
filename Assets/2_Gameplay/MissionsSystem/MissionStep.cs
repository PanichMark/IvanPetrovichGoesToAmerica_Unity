using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : MissionStepAbstract
{
	[TextArea(3, 10)]
	public string StepDescription;
	public List<MissionStepConditionAbstract> Sonditions = new List<MissionStepConditionAbstract>();

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
		return Sonditions.All(cond => cond.IsConditionMet());
	}
}