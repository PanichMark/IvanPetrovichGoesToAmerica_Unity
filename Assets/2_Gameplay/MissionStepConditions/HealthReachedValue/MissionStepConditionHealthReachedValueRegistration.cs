using UnityEngine;
using System.Collections.Generic;

public class MissionStepConditionHealthReachedValueRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private bool _isPlayerValue;
	[SerializeField] private List<GameObject> _targetNPCs = new List<GameObject>();

	[SerializeField] private float _targetHealthValue;

	private PlayerHealthController _playerHealthController;
	private Dictionary<GameObject, NPChealthController> _npcHealthControllers = new Dictionary<GameObject, NPChealthController>();

	private void Start()
	{
		if (_linkedMissionCondition is not MissionStepConditionHealthReachedValue healthCondition) return;

		if (_isPlayerValue)
		{
			_playerHealthController = ServiceLocator.Resolve<PlayerHealthController>();
			if (_playerHealthController != null)
			{
				_playerHealthController.OnHealthChanged += HandleHealthChanged;
			}
		}
		else
		{
			foreach (var npc in _targetNPCs)
			{
				if (npc != null)
				{
					var controller = npc.GetComponent<NPChealthController>();
					if (controller != null)
					{
						_npcHealthControllers[npc] = controller;
						controller.OnHealthChanged += HandleHealthChanged;
					}
				}
			}
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
		if (_linkedMissionCondition is not MissionStepConditionHealthReachedValue healthCondition) return;

		if (_isPlayerValue && _playerHealthController != null)
		{
			_playerHealthController.OnHealthChanged -= HandleHealthChanged;
		}
		else
		{
			foreach (var kvp in _npcHealthControllers)
			{
				kvp.Value.OnHealthChanged -= HandleHealthChanged;
			}
		}
	}
}