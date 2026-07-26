using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsData
{
	//Is game launched for the first time
	private const string WAS_INITIAL_LANGUAGE_CHOSEN = "WasInitialLanguageChosen";
	private const string WERE_TERMS_AND_CONDITIONS_SIGNED = "WereTermsAndConditionsSigned";

	public bool WasInitialLanguageChosen
	{
		get => PlayerPrefs.GetInt(WAS_INITIAL_LANGUAGE_CHOSEN, 0) == 1;
	}

	public void SetChooseInitialLanguage()
	{
		PlayerPrefs.SetInt(WAS_INITIAL_LANGUAGE_CHOSEN, 1);
		PlayerPrefs.Save();
	}

	//SettingsSectionGeneral

	//ScreenResolution
	//WindowType
	public int FPSlimit { get; set; }
	//HUDtype
	public float CameraFOV { get; set; }
	public int ScreenBrightness { get; set; }

	public string WeaponWheelType { get; set; }

	//SettingsSectionControls
	public float MouseSensitivityX { get; set; }
	public float MouseSensitivityY { get; set; }
	public Dictionary<string, KeyCode> KeyBindings { get; set; } = new Dictionary<string, KeyCode>();


	//SettingsSectionGraphics
	//NONE for Demo version!!!!

	//Audio
	public int VolumeGeneral { get; set; }
	public int VolumeEnvironment { get; set; }
	public int VolumeEffects { get; set; }
	public int VolumeVoices { get; set; }
	public int VolumeMusicAmbience { get; set; }
	public int VolumeMusicIngame { get; set; }
	public string CurrentLanguage { get; set; }
}