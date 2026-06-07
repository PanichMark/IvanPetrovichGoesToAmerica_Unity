using UnityEngine;

[CreateAssetMenu(fileName = "IsFirstGameLaunch", menuName = "BootstrapConfigs/IsFirstGameLaunch", order = 1)]
public class ConfigBootstrapFirstGameLaunch : ScriptableObject
{
	public bool IsFirstGameLaunch = false;
}