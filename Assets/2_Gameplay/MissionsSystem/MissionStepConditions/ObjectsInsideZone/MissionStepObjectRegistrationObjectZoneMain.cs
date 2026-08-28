using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MissionStepObjectRegistrationObjectZoneMain : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	private void Start()
	{
		var collider = GetComponent<Collider>();
		if (!collider.isTrigger)
		{
			Debug.LogError($"{name}: Коллайдер должен быть типа Trigger!");
			enabled = false;
			return;
		}

		// Говорим условию: "Я — твоя зона, зафиксируй меня как Owner"
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.InitializeWithZone(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.ReportObjectEntered(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.ReportObjectExited(other.gameObject);
		}
	}
}