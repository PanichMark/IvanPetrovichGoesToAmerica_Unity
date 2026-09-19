using UnityEngine;

public class NPCautomaton : NPCabstract
{
	public override void Interact()
	{
		StopAllCoroutines();
		_NPCphrasesController.TemporaryShowPhrases();
	}
}
