using UnityEngine;

[CreateAssetMenu(fileName = "IsFirstGameLaunch", menuName = "BootstrapConfigs/IsFirstGameLaunch", order = 1)]
public class BootstrapConfigIsFirstGameLaunch : ScriptableObject
{
	public bool IsFirstGameLaunch = false;
}