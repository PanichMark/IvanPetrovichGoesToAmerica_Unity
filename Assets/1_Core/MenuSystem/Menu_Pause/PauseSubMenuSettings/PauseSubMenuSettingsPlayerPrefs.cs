using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseSubMenuSettingsPlayerPrefs: MonoBehaviour
{
	public string FOV { get; private set; } = "FOV";

	private Bootstrap _bootstrap;
	public string Language { get; private set; } = "Language";
	private IInputDevice _inputDevice;
	public string KeybindingsPrefix  { get; private set; } = "KeyBinding_";
	
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;

	public void Initialize(Bootstrap bootstrap, IInputDevice inputDevice, PauseSubMenuSettingsController pauseSubMenuSettingsController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;

		var defaultBindings = _inputDevice.GetDefaultKeyBindings();
		List<string> actionNames = new List<string>(defaultBindings.Keys);

		_bootstrap.OnLoadSettingsData += () => LoadSettings(actionNames);

		_pauseSubMenuSettingsController.OnSaveSettingsGeneralData += SaveSettingsGeneral;
		_pauseSubMenuSettingsController.OnResetSettingsGeneralData += ResetSettingsGeneral;
		_pauseSubMenuSettingsController.OnSaveSettingsControlsData += SaveSettingsControls;
		_pauseSubMenuSettingsController.OnResetSettingsControlsData += ResetSettingsControls;
		_pauseSubMenuSettingsController.OnSaveSettingsGraphicsData += SaveSettingsGraphics;
		_pauseSubMenuSettingsController.OnResetSettingsGraphicsData += ResetSettingsGraphics;
		_pauseSubMenuSettingsController.OnSaveSettingsAudioData += SaveSettingsAudio;
		_pauseSubMenuSettingsController.OnResetSettingsAudioData += ResetSettingsAudio;

		Debug.Log("SettingsPlayerPrefs Initialized");
	}

	public void SaveSettingsGeneral(PlayerPrefsData data)
	{
		PlayerPrefs.SetFloat(FOV, data.FOV);

		PlayerPrefs.Save(); 
	}

	public void SaveSettingsControls(PlayerPrefsData data)
	{
		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(KeybindingsPrefix + binding.Key, binding.Value.ToString());
		}

		PlayerPrefs.Save();
	}

	public void SaveSettingsGraphics(PlayerPrefsData data)
	{
		PlayerPrefs.Save();
	}

	public void SaveSettingsAudio(PlayerPrefsData data)
	{
		PlayerPrefs.SetString(Language, data.Language);

		PlayerPrefs.Save();
	}

	public void LoadSettings(List<string> actionNamesToLoad)
	{
		var data = new PlayerPrefsData();

		data.FOV = PlayerPrefs.GetFloat(FOV);

		data.Language = PlayerPrefs.GetString(Language);

		if (actionNamesToLoad != null && actionNamesToLoad.Count > 0)
		{
			int loadedBindingsCount = 0;

			foreach (string actionName in actionNamesToLoad)
			{
				string key = KeybindingsPrefix + actionName;
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

		_pauseSubMenuSettingsController.ApplySystemLoadedSettings(data);
	}

	public void ResetSettingsGeneral()
	{
		PlayerPrefs.DeleteKey(FOV);

		Debug.Log("RESET SETTINGS!!");
	}

	public void ResetSettingsControls()
	{
		string allKeysString = PlayerPrefs.GetString("");
		string[] allKeysArray = allKeysString.Split('\0');

		foreach (string key in allKeysArray)
		{
			if (!string.IsNullOrEmpty(key) && key.StartsWith(KeybindingsPrefix))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		PlayerPrefs.Save();

		Debug.Log("RESET SETTINGS!!");
	}

	public void ResetSettingsGraphics()
	{
		PlayerPrefs.Save();

		Debug.Log("RESET SETTINGS!!");
	}

	public void ResetSettingsAudio()
	{
		string currentLanguage = PlayerPrefs.GetString(Language);

		PlayerPrefs.SetString(Language, currentLanguage);

		PlayerPrefs.Save();

		Debug.Log("RESET SETTINGS!!");
	}
}