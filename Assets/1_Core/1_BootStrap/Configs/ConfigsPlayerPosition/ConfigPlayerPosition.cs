using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayerPosition", menuName = "Scriptable Objects/Configs/ConfigPlayerPosition")]
public class ConfigPlayerPosition : ScriptableObject
{
	// Это поле будет отображаться в инспекторе
	public Vector3 playerPosition;
}