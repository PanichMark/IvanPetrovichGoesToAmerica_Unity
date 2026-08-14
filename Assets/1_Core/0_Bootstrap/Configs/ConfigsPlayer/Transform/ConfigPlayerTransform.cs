using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayerTransform", menuName = "Configs/Player/Transform")]
public class ConfigPlayerTransform : ScriptableObject
{
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}