using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PauseSubMenuSettingsPlayerPrefs: MonoBehaviour
{
	public string FOV { get; private set; } = "FOV";

	public string Language { get; private set; } = "Language";
	private IInputDevice _inputDevice;
	public string KeybindingsPrefix  { get; private set; } = "KeyBinding_";
	
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;

	public void Initialize(IInputDevice inputDevice, PauseSubMenuSettingsController pauseSubMenuSettingsController)
	{
		_inputDevice = inputDevice;
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;

		var defaultBindings = _inputDevice.GetDefaultKeyBindings();
		List<string> actionNames = new List<string>(defaultBindings.Keys);

		LoadSettings(actionNames);

		_pauseSubMenuSettingsController.OnSaveSettingsData += SaveSettings;
		_pauseSubMenuSettingsController.OnResetSettingsData += ResetSettings;
		
		Debug.Log("SettingsPlayerPrefs Initialized");
	}

	public void SaveSettings(PlayerPrefsData data)
	{
		PlayerPrefs.SetFloat(FOV, data.FOV);

		PlayerPrefs.SetString(Language, data.Language);

		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(KeybindingsPrefix + binding.Key, binding.Value.ToString());
		}

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

		_pauseSubMenuSettingsController.ApplyLoadedSettings(data);
	}

	public void ResetSettings()
	{
		string currentLanguage = PlayerPrefs.GetString(Language);

		PlayerPrefs.DeleteKey(FOV);

		string allKeysString = PlayerPrefs.GetString("");
		string[] allKeysArray = allKeysString.Split('\0');

		foreach (string key in allKeysArray)
		{
			if (!string.IsNullOrEmpty(key) && key.StartsWith(KeybindingsPrefix))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		PlayerPrefs.SetString(Language, currentLanguage);

		PlayerPrefs.Save();

		Debug.Log("RESET SETTINGS!!");
	}
}