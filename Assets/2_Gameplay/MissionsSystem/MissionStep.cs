using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : MissionStepAbstract, ICurrentMissionStep
{
	[TextArea(3, 10)]
	public string StepDescription;
	public List<MissionStepConditionAbstract> Sonditions = new List<MissionStepConditionAbstract>();

	// --- ДОБАВЬТЕ ЭТО СВОЙСТВО ---
	// Оно преобразует список конкретных условий в список общих интерфейсов
	public IReadOnlyList<ICurrentMissionCondition> Conditions
	{
		get { return Sonditions.ConvertAll(c => (ICurrentMissionCondition)c); }
	}
	// ---------------------------------


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