using UnityEngine;

[CreateAssetMenu(fileName = "NPCstateChanged", menuName = "Missions/StepConditions/NPCstateChanged")]
public class MissionStepConditionNPCstateChanged : MissionStepConditionAbstract
{
	public void CheckState(NPCstateTypes newState)
	{
		if (_isConditionMet || !IsActive) return;



		// Логика сравнения состояния, если она нужна, или просто вызов
		_isConditionMet = true;
		_missionStepAbstract.OnStepCompleted(GoToNextStep);
		Debug.Log($"[Условие] Миссия пройдена. NPC перешел в состояние {newState}.");
	}

	public override void ResetStepConditionBySystem()
	{
		_isConditionMet = false;
	}

	public override void ResetStepConditionByReturning()
	{
		_isConditionMet = false;
	}
}