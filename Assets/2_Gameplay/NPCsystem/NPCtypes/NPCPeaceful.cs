using UnityEngine;

public class NPCPeaceful : NPCAbstract
{
	// Приватная ссылка на контроллер состояний NPC
	
	// Получаем ссылку на контроллер состояний NPC в методе Start
	

	override public void Interact()
	{
		// Проверяем, что NPC находится именно в состоянии Default
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" && _npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

	
		if (_NPCDialogueController.RussianDialogueFile != null && _NPCDialogueController.EnglishDialogueFile != null)
		{
			// Если есть диалоги для текущего языка, начинаем диалог
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