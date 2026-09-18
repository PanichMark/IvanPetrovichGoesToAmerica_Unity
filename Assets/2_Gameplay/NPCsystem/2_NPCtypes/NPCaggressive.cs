public class NPCaggressive : NPCabstract
{
	public override bool IsInteractionHintMessageFailActive => false;

	override public void Interact()
	{
		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead)
		{
			_pickable.Interact();
			return;
		}
	}
}