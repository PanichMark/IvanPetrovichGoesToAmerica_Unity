using UnityEngine;
using System.Collections;

public class MissionStepConditionTimeCountdownRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private float _countdownDuration;

	private MissionStepConditionTimeCountdown _condition;
	private Coroutine _timerCoroutine;
	private MissionsManager _missionsManager;

	private void Start()
	{
		if (_linkedMissionCondition is MissionStepConditionTimeCountdown condition)
		{
			_missionsManager = ServiceLocator.Resolve<MissionsManager>();

			_condition = condition;
			//_condition.OnStepConditionTimeTick += HandleTimeUpdate;

			_missionsManager.OnCurrentStepChanged += StartCountdownCorotune;

			
		}
	}

	

	private void StartCountdownCorotune()
	{
		for (int i = 0; i < _missionsManager.ActiveMission.MissionSteps.Length; i++)
		{
			//Debug.Log(i);
			if (_missionsManager.GameMissions.MissionsInOrder[0].MissionSteps[i] == _linkedMissionCondition.MissionStepAbstract)
			{
				Debug.Log("COUNTDOWN IS THIS");
				if (_timerCoroutine != null)
				{
					_timerCoroutine = StartCoroutine(CountdownCoroutine(_countdownDuration));
				}
			}
			else
			{
				if (_timerCoroutine != null)
				{
					StopCoroutine(_timerCoroutine);
				}
			}
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

	private void OnDestroy()
	{
		if (_timerCoroutine != null)
		{
			StopCoroutine(_timerCoroutine);
		}

		_missionsManager.OnCurrentStepChanged -= StartCountdownCorotune;
	}
}