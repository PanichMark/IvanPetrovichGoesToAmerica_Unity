using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : MissionStepAbstract, IMissionStep
{
	[TextArea(3, 10)]
	public string StepDescription;
	[SerializeField] private bool showMissionMarker;


	// --- ДОБАВЬТЕ ЭТО СВОЙСТВО ---
	// Оно преобразует список конкретных условий в список общих интерфейсов


	public bool ShowMissionMarker => showMissionMarker;

	// ---------------------------------

	/*
	public override void OnStepCompleted()
	{
		if (AreAllConditionsMet())
		{
			// Находит активный менеджер в сцене и сообщает о завершении шага.
			FindObjectOfType<MissionsManager>().CompleteCurrentStep(false);
		}
	}
	*/
	//private bool AreAllConditionsMet()
	//{
		//return StepConditions.All(cond => cond.IsConditionMet());
	//}
}