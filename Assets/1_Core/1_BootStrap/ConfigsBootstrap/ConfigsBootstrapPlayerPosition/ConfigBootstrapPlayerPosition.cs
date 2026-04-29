using UnityEngine;

[CreateAssetMenu(fileName = "ConfigBootstrapPlayerPosition", menuName = "Scriptable Objects/Configs/ConfigsBootstrap/ConfigBootstrapPlayerPosition")]
public class ConfigBootstrapPlayerPosition : ScriptableObject
{
	// Это поле будет отображаться в инспекторе
	public Vector3 playerPosition;
}