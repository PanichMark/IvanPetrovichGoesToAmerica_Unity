using UnityEngine;

public class NPCeventAnimationController : MonoBehaviour
{
	private NPCeventController _NPCeventController;

	public void Initialize(NPCeventController NPCeventController)
	{
		_NPCeventController = NPCeventController;
	}
}
