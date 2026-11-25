using UnityEngine;
using System.Collections.Generic;

public class MissionStepConditionHealthReachedValueRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private bool _isPlayerValue;
	[SerializeField] private GameObject _targetNPC;

	[SerializeField] private float _targetHealthValue;

	private PlayerHealthController _playerHealthController;
	private NPChealthController _NPChealthController;

	private void Start()
	{
		if (_isPlayerValue)
		{
			_playerHealthController = ServiceLocator.Resolve<PlayerHealthController>();
	
			_playerHealthController.OnHealthChanged += HandleHealthChanged;
			
		}
		else
		{


			_NPChealthController = _targetNPC.GetComponent<NPChealthController>();


			_NPChealthController.OnHealthChanged += HandleHealthChanged;
		
			
		}
	}

	private void HandleHealthChanged(float currentHealth)
	{
		if (_linkedMissionCondition is not MissionStepConditionHealthReachedValue healthCondition) return;
		if (healthCondition.IsConditionMet) return;

		if (currentHealth <= _targetHealthValue)
		{
			healthCondition.CompleteCondition();
		}
	}

	private void OnDestroy()
	{

		if (_isPlayerValue)
		{
			_playerHealthController.OnHealthChanged -= HandleHealthChanged;
		}
		else
		{
			_NPChealthController.OnHealthChanged -= HandleHealthChanged;
		}
	}
}