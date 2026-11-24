using UnityEngine;
using System.Collections;

public class MissionStepConditionTimeCountdownRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private float _countdownDuration;
	[SerializeField] private bool _showCountDown;
	private int _stepToStartCountDown;
	private MissionStepConditionTimeCountdown _condition;
	private Coroutine _timerCoroutine;
	private MissionsManager _missionsManager;
	private MenuManager _menuManager;

	private void Start()
	{
		if (_linkedMissionCondition is MissionStepConditionTimeCountdown condition)
		{

			_menuManager = ServiceLocator.Resolve<MenuManager>();
			_condition = condition;
			//_condition.OnStepConditionTimeTick += HandleTimeUpdate;

	
				_missionsManager = ServiceLocator.Resolve<MissionsManager>();
				FindStepIndexToStarCountDown();
				_missionsManager.OnCurrentStepChanged += StartCountdownCorotune;
			
			

			
		}
	}

	private void FindStepIndexToStarCountDown()
	{
		for (int i = 0; i < _missionsManager.ActiveMission.MissionSteps.Length; i++)
		{
			//Debug.Log(i);
			if (_missionsManager.GameMissions.MissionsInOrder[0].MissionSteps[i] == _linkedMissionCondition.MissionStepAbstract)
			{
				_stepToStartCountDown = i;
				break;

			}
		}
	}

	private void StartCountdownCorotune()
	{
		if (_missionsManager.CurrentStepIndex == _stepToStartCountDown)
		{
			StartCoroutine(CountdownCoroutine(_countdownDuration));
		}
	}

	private IEnumerator CountdownCoroutine(float duration)
	{
		Debug.Log("STARTED COUNTDOWN!");
		float elapsed = 0f;
		while (elapsed < duration)
		{
			if (!_menuManager.IsAnyMenuOpened)
			{
				float timeRemaining = duration - elapsed;

				_condition.UpdateTimer(timeRemaining, _showCountDown);

				elapsed += Time.deltaTime;
			}
			
		
			yield return null;
			
		}

		_condition.UpdateTimer(0f, _showCountDown);
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