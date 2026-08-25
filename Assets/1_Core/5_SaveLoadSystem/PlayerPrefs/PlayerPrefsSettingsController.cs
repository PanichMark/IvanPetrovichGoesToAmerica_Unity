using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsSettingsController: MonoBehaviour
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;

	public delegate void PlayerPrefsSettingsHandler(PlayerPrefsData data);
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionGeneralPlayerPrefs;
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionControlsPlayerPrefs;
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionAudioPlayerPrefs;

	//General
	public string ScreenResolution { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.ScreenResolution.ToString();
	public string WindowType { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.WindowType.ToString();
	public string FPSlimit { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.FPSlimit.ToString();
	public string HUDtype { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.HUDtype.ToString();
	public string WeaponWheelType { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.WeaponWheelType.ToString();
	public string CameraFOV { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.CameraFOV.ToString();
	public string ScreenBrightness { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.ScreenBrightness.ToString();

	//Not available in Demo
	//public string GameDifficulty { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.GameDifficulty.ToString();
	public string ShowIngameTutorials { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.ShowIngameTutorials.ToString();
	public string ShowBlood { get; private set; } = PlayerPrefsSettingsSectionGeneralEnum.ShowBlood.ToString();

	//Controls
	public string KeyBindingPrefix { get; private set; } = PlayerPrefsSettingsSectionControlsEnum.KeyBinding_.ToString();
	public string MouseSensitivityX { get; private set; } = PlayerPrefsSettingsSectionControlsEnum.MouseSensitivityX.ToString();
	public string MouseSensitivityY { get; private set; } = PlayerPrefsSettingsSectionControlsEnum.MouseSensitivityY.ToString();

	//Not available in Demo
	//But its empty logic remains to avoid NULL
	//Graphics

	//Audio
	public string Language { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.Language.ToString();
	public string VolumeGeneral { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeGeneral.ToString();
	public string VolumeEnvironment { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeEnvironment.ToString();
	public string VolumeEffects { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeEffects.ToString();
	public string VolumeVoices { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeVoices.ToString();
	public string VolumeMusicAmbience { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeMusicAmbience.ToString();
	public string VolumeMusicIngame { get; private set; } = PlayerPrefsSettingsSectionAudioEnum.VolumeMusicIngame.ToString();

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;

		var defaultBindings = _inputDevice.GetDefaultKeyBindings();
		List<InputControlsEnum> actionNames = new List<InputControlsEnum>(defaultBindings.Keys);

		_bootstrap.OnLoadSettingsData += () => LoadSettings(actionNames);

		Debug.Log("PlayerPrefsSettingsController Initialized");
	}

	public void SaveSettingsGeneral(PlayerPrefsData data)
	{
		PlayerPrefs.SetInt(FPSlimit, data.FPSlimit);
		PlayerPrefs.SetFloat(CameraFOV, data.CameraFOV);
		PlayerPrefs.SetString(WeaponWheelType, data.WeaponWheelType);
		PlayerPrefs.SetInt(ShowIngameTutorials, data.ShowIngameTutorials ? 1 : 0);
		PlayerPrefs.SetInt(ShowBlood, data.ShowBlood ? 1 : 0);

		PlayerPrefs.Save();

		Debug.Log("Saved Settings General");
	}

	public void SaveSettingsControls(PlayerPrefsData data)
	{
		PlayerPrefs.SetFloat(MouseSensitivityX, data.MouseSensitivityX);
		PlayerPrefs.SetFloat(MouseSensitivityY, data.MouseSensitivityY);

		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(KeyBindingPrefix + binding.Key, binding.Value.ToString());
		}

		PlayerPrefs.Save();

		Debug.Log("Saved Settings Controls");
	}

	public void SaveSettingsGraphics(PlayerPrefsData data)
	{
		PlayerPrefs.Save();

		Debug.Log("Saved Settings Graphics");
	}

	public void SaveSettingsAudio(PlayerPrefsData data)
	{
		PlayerPrefs.SetString(Language, data.Language);

		PlayerPrefs.Save();

		Debug.Log("Saved Settings Audio");
	}

	public void LoadSettings(List<InputControlsEnum> actionNamesToLoad)
	{
		var data = new PlayerPrefsData();

		data.FPSlimit = PlayerPrefs.GetInt(FPSlimit, 60);
		data.CameraFOV = PlayerPrefs.GetFloat(CameraFOV, 60);
		data.WeaponWheelType = PlayerPrefs.GetString(WeaponWheelType, "2D");
		data.ShowIngameTutorials = PlayerPrefs.GetInt(ShowIngameTutorials, 1) == 1;
		data.ShowBlood = PlayerPrefs.GetInt(ShowBlood, 1) == 1;

		data.MouseSensitivityX = PlayerPrefs.GetFloat(MouseSensitivityX, 1);
		data.MouseSensitivityY = PlayerPrefs.GetFloat(MouseSensitivityY, 1);

		if (actionNamesToLoad != null && actionNamesToLoad.Count > 0)
		{
			int loadedBindingsCount = 0;

			foreach (InputControlsEnum actionName in actionNamesToLoad)
			{
				string key = KeyBindingPrefix + actionName;
				if (PlayerPrefs.HasKey(key))
				{
					string savedValueStr = PlayerPrefs.GetString(key);
					KeyCode parsedKeyCode;

					if (Enum.TryParse<KeyCode>(savedValueStr, out parsedKeyCode))
					{
						data.KeyBindings[actionName] = parsedKeyCode;
						loadedBindingsCount++;
					}
					else if (savedValueStr.StartsWith("KeyCode."))
					{
						string trimmedValue = savedValueStr.Substring(8);
						if (Enum.TryParse<KeyCode>(trimmedValue, out parsedKeyCode))
						{
							data.KeyBindings[actionName] = parsedKeyCode;
							loadedBindingsCount++;
						}
					}
				}
			}
		}

		data.Language = PlayerPrefs.GetString(Language);
		OnApplySettingsSectionGeneralPlayerPrefs?.Invoke(data);
		OnApplySettingsSectionControlsPlayerPrefs?.Invoke(data);
		OnApplySettingsSectionAudioPlayerPrefs?.Invoke(data);
	}

	public void ResetSettingsGeneral()
	{
		PlayerPrefs.DeleteKey(CameraFOV);
		PlayerPrefs.DeleteKey(ShowIngameTutorials); 
		PlayerPrefs.DeleteKey(ShowBlood);

		PlayerPrefs.Save();

		Debug.Log("Reset Settings General");
	}

	public void ResetSettingsControls()
	{
		PlayerPrefs.DeleteKey(MouseSensitivityX);
		PlayerPrefs.DeleteKey(MouseSensitivityY);

		string allKeysString = PlayerPrefs.GetString("");
		string[] allKeysArray = allKeysString.Split('\0');

		foreach (string key in allKeysArray)
		{
			if (!string.IsNullOrEmpty(key) && key.StartsWith(KeyBindingPrefix))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		PlayerPrefs.Save();

		Debug.Log("Reset Settings Controls");
	}

	public void ResetSettingsGraphics()
	{
		PlayerPrefs.Save();

		Debug.Log("Reset Settings Graphics");
	}

	public void ResetSettingsAudio()
	{
		string currentLanguage = PlayerPrefs.GetString(Language);

		PlayerPrefs.SetString(Language, currentLanguage);

		PlayerPrefs.Save();

		Debug.Log("Reset Settings Audio");
	}
}