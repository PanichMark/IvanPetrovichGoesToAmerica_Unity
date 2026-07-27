using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsData
{
	//Bootstrap Prerequisites
	private string _bootstrapArePrerequisitesMet = PlayerPrefsGameInitializationEnum.BootstrapArePrerequisitesMet.ToString();

	public bool BootstrapArePrerequisitesMet
	{
		get => PlayerPrefs.GetInt(_bootstrapArePrerequisitesMet, 0) == 1;
	}

	public void ChooseInitialLanguage()
	{
		PlayerPrefs.SetInt(_bootstrapArePrerequisitesMet, 1);
		PlayerPrefs.Save();
	}

	//Settings General
	public string ScreenResolution { get; set; }
	public string WindowType { get; set; }
	public int FPSlimit { get; set; }
	public string HUDtype { get; set; }
	public string WeaponWheelType { get; set; }
	public float CameraFOV { get; set; }
	public float ScreenBrightness { get; set; }

	//Not available in Demo
	//public string GameDifficulty { get; set; }
	public bool ShowIngameTutorials { get; set; }
	public bool ShowBlood { get; set; }

	//Settings Controls
	public Dictionary<InputControlsEnum, KeyCode> KeyBindings { get; set; } = new Dictionary<InputControlsEnum, KeyCode>();
	public float MouseSensitivityX { get; set; }
	public float MouseSensitivityY { get; set; }

	//Not available in Demo
	//Settings Graphics

	//Settings Audio
	public string Language { get; set; }
	public int VolumeGeneral { get; set; }
	public int VolumeEnvironment { get; set; }
	public int VolumeEffects { get; set; }
	public int VolumeVoices { get; set; }
	public int VolumeMusicAmbience { get; set; }
	public int VolumeMusicIngame { get; set; }
}

public enum PlayerPrefsGameInitializationEnum
{
	BootstrapArePrerequisitesMet
}