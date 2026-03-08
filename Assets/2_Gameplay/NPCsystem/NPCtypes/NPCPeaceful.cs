using UnityEngine;

public class NPCPeaceful : NPCAbstract
{
	// Приватная ссылка на контроллер состояний NPC
	
	// Получаем ссылку на контроллер состояний NPC в методе Start
	

	override public void Interact()
	{
		// Проверяем, что NPC находится именно в состоянии Default
		if (_npcStateMachineController?.CurrentNPCState != "Default")
			return;

		// Выполняем само взаимодействие
		Debug.Log($"{NPC_name} говорит что-то интересное.");
	}
}