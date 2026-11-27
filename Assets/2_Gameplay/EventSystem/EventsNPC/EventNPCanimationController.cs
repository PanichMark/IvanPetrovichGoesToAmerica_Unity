using UnityEngine;

public class EventNPCanimationController : MonoBehaviour
{
	private EventController _NPCeventController;

	public void Initialize(EventController NPCeventController)
	{
		_NPCeventController = NPCeventController;
	}
}
