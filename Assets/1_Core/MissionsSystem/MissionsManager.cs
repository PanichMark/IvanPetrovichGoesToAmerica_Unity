using UnityEngine;

public class MissionsManager : MonoBehaviour
{
	public MissionAbstract activeMission;
	private int currentStepIndex = 0;

	public delegate void InteractionEventHandler(GameObject interactedObject);
	public static event InteractionEventHandler OnAnyObjectInteracted;

	public delegate void DestructionEventHandler(GameObject destroyedObject, bool wasLethal);
	public static event DestructionEventHandler OnAnyObjectDestroyed;

	public void CheckAndCompleteCurrentStep()
	{
		if (activeMission == null) return;
		if (currentStepIndex >= activeMission.steps.Length) return;

		activeMission.steps[currentStepIndex].OnStepCompleted();
	}

	public void CompleteCurrentStep()
	{
		currentStepIndex++;
		if (currentStepIndex < activeMission.steps.Length)
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