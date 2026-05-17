using UnityEngine;

[CreateAssetMenu(fileName = "ConfigBootstrapFirstGameLaunch", menuName = "Configs/ConfigsBootstrap/ConfigBootstrapFirstGameLaunch", order = 1)]
public class ConfigBootstrapFirstGameLaunch : ScriptableObject
{
	public bool IsFirstGameLaunch = false;
}