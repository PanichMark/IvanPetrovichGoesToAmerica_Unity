using UnityEngine;
using System.Collections;

public class MissionStepConditionTimeCountdownRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private float _countdownDuration;

	private MissionStepConditionTimeCountdown _condition;
	private Coroutine _timerCoroutine;

	private void Start()
	{
		if (_linkedMissionCondition is MissionStepConditionTimeCountdown condition)
		{
			_condition = condition;
			_condition.OnStepConditionTimeTick += HandleTimeUpdate;

			_timerCoroutine = StartCoroutine(CountdownCoroutine(_countdownDuration));
		}
	}

	private IEnumerator CountdownCoroutine(float duration)
	{
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float timeRemaining = duration - elapsed;
			_condition.UpdateTimer(timeRemaining);
			yield return null;
			elapsed += Time.deltaTime;
		}

		_condition.UpdateTimer(0f);
		_condition.CompleteCondition();
	}

	private void HandleTimeUpdate(float timeRemaining)
	{
		Debug.Log($"Осталось времени: {timeRemaining:F1}");
	}

	private void OnDestroy()
	{
		if (_condition != null)
		{
			_condition.OnStepConditionTimeTick -= HandleTimeUpdate;
		}
		if (_timerCoroutine != null)
		{
			StopCoroutine(_timerCoroutine);
		}
	}
}