using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsData
{
	//Is game launched for the first time
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

	//SettingsSectionGeneral

	//ScreenResolution
	//WindowType
	//FPSlimit
	public float CameraFOV { get; set; } = 60;
	public int ScreenBrightness { get; set; }
	//HUDtype


	//SettingsSectionControls
	public float MouseSensitivityX { get; set; } = 1;
	public float MouseSensitivityY { get; set; } = 1;
	public Dictionary<string, KeyCode> KeyBindings { get; set; } = new Dictionary<string, KeyCode>();


	//SettingsSectionGraphics
	//NONE for Demo version!!!!

	//Audio
	public int VolumeGeneral {  get; set; }
	public int VolumeEnvironment { get; set; }
	public int VolumeEffects { get; set; }
	public int VolumeVoices { get; set; }
	public int VolumeMusicAmbience { get; set; }
	public int VolumeMusicIngame { get; set; }
	public string CurrentLanguage { get; set; }
}