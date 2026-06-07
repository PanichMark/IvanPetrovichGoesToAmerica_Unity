using UnityEngine;

[CreateAssetMenu(fileName = "DoorScenePlayerTransform", menuName = "InteractionObjects/Openable/DoorScenePlayerTransform")]
public class InteractionObjectOpenableDoorScenePlayerTransform : ScriptableObject
{
	public Vector3 PlayerPosition;
	public int PlayerRotation;
}
