using UnityEngine;

[CreateAssetMenu(fileName = "DoorScenePlayerTransform", menuName = "InteractionObjects/ChangeScene/PlayerTransform")]
public class InteractionObjectChangeScenePlayerTransform : ScriptableObject
{
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}
