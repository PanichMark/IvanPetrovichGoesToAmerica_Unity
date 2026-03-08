using UnityEngine;

public class NPCNeutral : NPCAbstract
{
	override public void Interact()
	{
		// Проверяем, что NPC находится именно в состоянии Default
		if (_npcStateMachineController?.CurrentNPCState != "Default")
			return;

		StopAllCoroutines(); // Останавливаем предыдущие показы, если были запущены
		StartCoroutine(ShowAndHidePhrase()); // Начинаем процедуру показа и сокрытия фразы
	}
}
