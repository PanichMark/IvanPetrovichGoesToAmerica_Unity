using UnityEngine;

public class NPCmovementAnimationController : MonoBehaviour
{
	private NPCmovementController _NPCmovementController;

	public void Initialize(NPCmovementController NPCmovementController)
	{
		_NPCmovementController = NPCmovementController;
	}
}
