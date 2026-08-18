using UnityEngine;

[CreateAssetMenu(fileName = "PickableData", menuName = "InteractionObjects/Pickable/PickableData")]
public class InteractionObjectPickableData : ScriptableObject
{
	public InteractionObjectsPickableTypes PickableType;
	public Vector3 Position;
    public Quaternion Rotation;
}
