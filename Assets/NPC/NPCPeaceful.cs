using UnityEngine;

public class NPCPeaceful : NPCAbstract
{
    override public void Interact()
    {
        Debug.Log($"{NPC_name} чтото говорит");
    }
}
