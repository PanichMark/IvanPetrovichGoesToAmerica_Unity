using UnityEngine;

public class NPCNeutral : NPCAbstract
{
	override public void Interact()
	{
		// Проверяем, что NPC находится именно в состоянии Default
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" && _npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

	
		if (_NPCDialogueController.RussianDialogueFile != null && _NPCDialogueController.EnglishDialogueFile != null)
		{
			// Если есть диалоги для текущего языка, начинаем диалог
			_npcStateMachineController.RotateTowardsPlayer();
			_NPCDialogueController.Interact();
		}
		else
		{
			// Обычное поведение
			StopAllCoroutines();
			StartCoroutine(ShowAndHidePhrase());
		}
	}
}
