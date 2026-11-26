using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOutcome", menuName = "Missions/StepConditions/DialogueOutcome")]
public class MissionStepConditionDialogueOutcome : MissionStepConditionAbstract
{
	// Вызывается из Registration, когда проверка внутри MonoBehaviour прошла успешно
	public void CompleteCondition()
	{
		if (!_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			//Debug.Log("[Условие] Диалоговое условие выполнено.");
		}
	}

	public override void ResetStepCondition()
	{
		_isConditionMet = false;
	}
}