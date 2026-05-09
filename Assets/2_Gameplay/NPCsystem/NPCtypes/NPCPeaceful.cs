using UnityEngine;

public class NPCPeaceful : NPCAbstract
{
	// Ссылка на контроллер состояний NPC.

	// Получаем ссылку на контроллер состояний NPC в методе Start.

	public override void Interact()
	{
		// Проверяем, что NPC находится в подходящем состоянии.
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" &&
			_npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

		// Если есть диалоги для текущего языка, начинаем диалог.
		if (_NPCdialogueController.RussianDialogueFile != null &&
			_NPCdialogueController.EnglishDialogueFile != null)
		{
			_npcStateMachineController.RotateTowardsPlayer();
			_NPCdialogueController.Interact();
		}
		else
		{
			// Обычное поведение.
			StopAllCoroutines();
			StartCoroutine(ShowAndHidePhrase());
		}
	}
}