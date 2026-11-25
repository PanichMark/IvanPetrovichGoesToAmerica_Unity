using UnityEngine;

public class NPCautomaton : NPCabstract
{
	[SerializeField] private NPCphrasesDataAutomaton _NPCPhrasesDataAutomaton;

	protected override void InitializeNPCabstract()
	{
		_NPCphrasesController.InitializeAutomaton(
			this,
			_NPCPhrasesDataAutomaton);
	}

	public override void Interact()
	{
		CallOnInteract();
		StopAllCoroutines();
		_NPCphrasesController.TemporaryShowPhrases();
	}
}