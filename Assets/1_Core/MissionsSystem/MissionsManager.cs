using UnityEngine;

public class MissionsManager : MonoBehaviour
{
	public MissionAbstract ActiveMission;
	private int _currentStepIndex = 0;

	public delegate void InteractionEventHandler(GameObject interactedObject);
	public static event InteractionEventHandler OnAnyObjectInteracted;

	public delegate void DestructionEventHandler(GameObject destroyedObject, bool wasLethal);
	public static event DestructionEventHandler OnAnyObjectDestroyed;

	public void CheckAndCompleteCurrentStep()
	{
		if (ActiveMission == null) return;
		if (_currentStepIndex >= ActiveMission.Steps.Length) return;

		ActiveMission.Steps[_currentStepIndex].OnStepCompleted();
	}

	public void CompleteCurrentStep()
	{
		_currentStepIndex++;
		if (_currentStepIndex < ActiveMission.Steps.Length)
		{
			// Здесь можно добавить логику для показа уведомления о новом шаге
		}
		else
		{
			EndMission();
		}
	}

	private void EndMission()
	{
		Debug.Log("Миссия завершена!");
	}
}