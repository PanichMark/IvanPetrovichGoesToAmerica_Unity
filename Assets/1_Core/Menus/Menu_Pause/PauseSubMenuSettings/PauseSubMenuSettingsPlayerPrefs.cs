

using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseSubMenuSettingsPlayerPrefs: MonoBehaviour
{
	// Ключи для PlayerPrefs
	// В теле класса
	public string KEY_FOV { get; private set; } = "FOV";
	public string PREFIX_KEYBINDING  { get; private set; } = "KeyBinding_";

	// НОВЫЙ КЛЮЧ (если вы добавляли чувствительность мыши)
	// private const string KEY_MOUSE_SENSITIVITY = "MouseSensitivity"; 

	// Сохранение всех настроек из объекта SettingsData
	public void SaveSettings(SettingsData data)
	{

		PlayerPrefs.SetFloat(KEY_FOV, data.FOV);

		// PlayerPrefs.SetFloat(KEY_MOUSE_SENSITIVITY, data.MouseSensitivity); 
		PlayerPrefs.Save(); // Сохраняем основные параметры

		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(PREFIX_KEYBINDING + binding.Key, binding.Value.ToString());
		}
		PlayerPrefs.Save(); // Сохраняем биндинги клавиш
	}

	// В классе PauseSubMenuSettingsPlayerPrefs

	// Убираем поле _inputDevice и метод Initialize

	// Переписываем метод LoadSettings
	public SettingsData LoadSettings(List<string> actionNamesToLoad)
	{
		var data = new SettingsData();

		// --- 1. ЗАГРУЗКА FOV ---
		if (PlayerPrefs.HasKey(KEY_FOV))
		{
			data.FOV = PlayerPrefs.GetFloat(KEY_FOV);
			Debug.Log($"[Загрузка] FOV успешно загружен: {data.FOV}");
		}

		// --- 2. ЗАГРУЗКА БИНДИНГОВ ---
		// Принимаем список имен действий, которые нужно загрузить
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

			if (loadedBindingsCount > 0)
			{
				Debug.Log($"[Загрузка] Всего успешно загружено биндингов: {loadedBindingsCount}");
			}
			else
			{
				Debug.LogWarning("[Загрузка] Словарь биндингов остался пустым. Проверьте формат сохраненных данных.");
			}
		}

		return data;
	}
	// Полное удаление всех наших настроек из PlayerPrefs
	public void ResetSettings()
	{
		//PlayerPrefs.DeleteKey(KEY_LANGUAGE);
		PlayerPrefs.DeleteKey(KEY_FOV);
		//PlayerPrefs.DeleteKey(KEY_FPS_LIMIT);
		// PlayerPrefs.DeleteKey(KEY_MOUSE_SENSITIVITY);

		// --- ИСПРАВЛЕННЫЙ ЦИКЛ ---
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