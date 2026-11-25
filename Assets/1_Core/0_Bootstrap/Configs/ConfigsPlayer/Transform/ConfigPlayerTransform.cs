using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayerTransform", menuName = "Bootstrap/PlayerConfigs/Transform")]
public class ConfigPlayerTransform : ScriptableObject
{
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}