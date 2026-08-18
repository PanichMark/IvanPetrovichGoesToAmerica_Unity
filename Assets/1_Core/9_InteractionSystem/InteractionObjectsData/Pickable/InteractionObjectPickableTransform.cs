using UnityEngine;

[CreateAssetMenu(fileName = "InteractionObjectPickableTransform", menuName = "InteractionObjects/Pickable/Transform")]
public class InteractionObjectPickableTransform : ScriptableObject
{
    public Vector3 Position;
    public Quaternion Rotation;
}
