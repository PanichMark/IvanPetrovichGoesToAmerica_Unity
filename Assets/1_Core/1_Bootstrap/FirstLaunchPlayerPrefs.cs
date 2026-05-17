using UnityEngine;

public  class FirstLaunchPlayerPrefs
{
	private const string KEY_IS_FIRST_LAUNCH = "IsGameLaunchedForTheFirstTime";

	public bool IsFirstLaunch
	{
		get => PlayerPrefs.GetInt(KEY_IS_FIRST_LAUNCH, 1) == 1;
		set => PlayerPrefs.SetInt(KEY_IS_FIRST_LAUNCH, value ? 1 : 0);
	}

	public  void ResetFirstLaunchFlag()
	{
		IsFirstLaunch = false;
	}
}