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

		StopAllCoroutines(); // Останавливаем предыдущие показы, если были запущены
		StartCoroutine(ShowAndHidePhrase()); // Начинаем процедуру показа и сокрытия фразы
	}
}