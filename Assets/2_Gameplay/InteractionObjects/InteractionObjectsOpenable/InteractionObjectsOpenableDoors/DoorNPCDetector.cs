using UnityEngine;

public class DoorNPCDetector : MonoBehaviour
{
	private InteractionObjectOpenableDoorUndestructable _doorController;
	private Collider _collider;

	public void Initialize(InteractionObjectOpenableDoorUndestructable doorController)
	{
		_doorController = doorController;
		_collider = GetComponent<Collider>();
		_collider.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other);

		if (other.TryGetComponent<NPClivingBeing>(out _))
		{
			_doorController.NPCtryOpeningDoor();
		}
	}
}