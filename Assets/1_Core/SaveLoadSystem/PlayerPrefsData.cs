using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsData
{
	public float FOV { get; set; }

	public string Language { get; set; }

	public Dictionary<string, KeyCode> KeyBindings { get; set; } = new Dictionary<string, KeyCode>();

	private const string KEY_IS_FIRST_LAUNCH = "IsGameLaunchedForTheFirstTime";

	public bool IsFirstLaunch
	{
		get => PlayerPrefs.GetInt(KEY_IS_FIRST_LAUNCH, 1) == 1;
		set => PlayerPrefs.SetInt(KEY_IS_FIRST_LAUNCH, value ? 1 : 0);
	}

	public void SetNotFirstLaunch()
	{
		PlayerPrefs.SetInt(KEY_IS_FIRST_LAUNCH, 0);
		PlayerPrefs.Save();
	}
}