using UnityEngine;

public class MissionStepConditionDialogueOutcomeRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	// Параметры настройки в инспекторе
	[SerializeField] private int _branchIndexToTrack;
	[SerializeField] private bool _expectedYesOutcome;

	private NPCdialogueController _dialogueController;

	private void Start()
	{
		_dialogueController = GetComponent<NPCdialogueController>();
		if (_dialogueController == null)
		{
			Debug.LogError($"На объекте {gameObject.name} нет NPCdialogueController.");
			return;
		}

		// Регистрируем владельца (самого NPC) в условии
		_linkedMissionCondition.RegisterOwner(gameObject);

		// Подписываемся на общий селектор диалога
		_dialogueController.OnDialogueOptionSelected += HandleDialogueSelection;
	}

	private void OnDestroy()
	{
		if (_dialogueController != null)
		{
			_dialogueController.OnDialogueOptionSelected -= HandleDialogueSelection;
		}
	}

	private void HandleDialogueSelection(int triggeredBranchIndex, bool isYesSelected)
	{
		// Проверяем, что условие еще не выполнено (защита от повторных вызовов)
		if (_linkedMissionCondition.IsConditionMet) return;

		// Сравниваем данные из события с нашими настройками в инспекторе
		if (triggeredBranchIndex == _branchIndexToTrack && isYesSelected == _expectedYesOutcome)
		{
			// Если типы совпадают, вызываем метод завершения
			if (_linkedMissionCondition is MissionStepConditionDialogueOutcome outcomeCondition)
			{
				outcomeCondition.CompleteCondition();
			}
		}
	}
}