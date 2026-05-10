using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseSubMenuSettingsPlayerPrefs: MonoBehaviour
{
	public string KEY_FOV { get; private set; } = "FOV";
	public string PREFIX_KEYBINDING  { get; private set; } = "KeyBinding_";

	public void SaveSettings(SettingsData data)
	{
		PlayerPrefs.SetFloat(KEY_FOV, data.FOV);

		PlayerPrefs.Save(); 

		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(PREFIX_KEYBINDING + binding.Key, binding.Value.ToString());
		}
		PlayerPrefs.Save(); 
	}

	public SettingsData LoadSettings(List<string> actionNamesToLoad)
	{
		var data = new SettingsData();

		if (PlayerPrefs.HasKey(KEY_FOV))
		{
			data.FOV = PlayerPrefs.GetFloat(KEY_FOV);
		}

		if (actionNamesToLoad != null && actionNamesToLoad.Count > 0)
		{
			int loadedBindingsCount = 0;

			foreach (string actionName in actionNamesToLoad)
			{
				string key = PREFIX_KEYBINDING + actionName;
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

		return data;
	}
	public void ResetSettings()
	{
		PlayerPrefs.DeleteKey(KEY_FOV);

		string allKeysString = PlayerPrefs.GetString("");
		string[] allKeysArray = allKeysString.Split('\0');

		foreach (string key in allKeysArray)
		{
			if (!string.IsNullOrEmpty(key) && key.StartsWith(PREFIX_KEYBINDING))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		PlayerPrefs.Save();
	}
}