using UnityEngine;

public class NPCNeutral : NPCAbstract
{
	override public void Interact()
	{
		Debug.Log($"{NPC_name}у все равно на вас...");
	}
}
