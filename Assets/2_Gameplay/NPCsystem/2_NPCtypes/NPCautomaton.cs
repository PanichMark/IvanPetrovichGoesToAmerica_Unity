using UnityEngine;

public class NPCautomaton : NPCabstract
{
	[SerializeField] private NPCphrasesDataAutomaton _NPCPhrasesDataAutomaton;

	protected override void InitializeNPCabstract()
	{

	}


	public override void Interact()
	{
		StopAllCoroutines();
		_NPCphrasesController.TemporaryShowPhrases();
	}
}
