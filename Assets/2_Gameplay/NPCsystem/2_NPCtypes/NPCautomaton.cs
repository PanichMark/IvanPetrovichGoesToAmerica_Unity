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
		StopAllCoroutines();
		_NPCphrasesController.TemporaryShowPhrases();
	}
}