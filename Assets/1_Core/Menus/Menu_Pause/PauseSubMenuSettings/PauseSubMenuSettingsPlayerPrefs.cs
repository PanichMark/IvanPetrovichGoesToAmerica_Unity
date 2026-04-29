// Файл: PauseSubMenuSettingsPlayerPrefs.cs

using System;
using System.Collections.Generic;
using UnityEngine;

public static class PauseSubMenuSettingsPlayerPrefs
{
	// Ключи для PlayerPrefs
	private const string KEY_LANGUAGE = "Language";
	private const string KEY_FOV = "FOV";
	private const string KEY_FPS_LIMIT = "FPSLimit";
	private const string PREFIX_KEYBINDING = "KeyBinding_";

	// НОВЫЙ КЛЮЧ (если вы добавляли чувствительность мыши)
	// private const string KEY_MOUSE_SENSITIVITY = "MouseSensitivity"; 

	// Сохранение всех настроек из объекта SettingsData
	public static void SaveSettings(SettingsData data)
	{
		PlayerPrefs.SetInt(KEY_LANGUAGE, (int)data.Language);
		PlayerPrefs.SetFloat(KEY_FOV, data.FOV);
		PlayerPrefs.SetInt(KEY_FPS_LIMIT, data.FPSLimit);
		// PlayerPrefs.SetFloat(KEY_MOUSE_SENSITIVITY, data.MouseSensitivity); 
		PlayerPrefs.Save(); // Сохраняем основные параметры

		foreach (var binding in data.KeyBindings)
		{
			PlayerPrefs.SetString(PREFIX_KEYBINDING + binding.Key, binding.Value.ToString());
		}
		PlayerPrefs.Save(); // Сохраняем биндинги клавиш
	}

	// Загрузка всех настроек в новый объект SettingsData
	public static SettingsData LoadSettings()
	{
		var data = new SettingsData();

		if (PlayerPrefs.HasKey(KEY_LANGUAGE))
			data.Language = (LanguagesEnum)PlayerPrefs.GetInt(KEY_LANGUAGE);

		if (PlayerPrefs.HasKey(KEY_FOV))
			data.FOV = PlayerPrefs.GetFloat(KEY_FOV);

		if (PlayerPrefs.HasKey(KEY_FPS_LIMIT))
			data.FPSLimit = PlayerPrefs.GetInt(KEY_FPS_LIMIT);

		// if (PlayerPrefs.HasKey(KEY_MOUSE_SENSITIVITY))
		//     data.MouseSensitivity = PlayerPrefs.GetFloat(KEY_MOUSE_SENSITIVITY);


		// --- ИСПРАВЛЕННЫЙ ЦИКЛ ---
		// Получаем строку со всеми ключами
		string allKeysString = PlayerPrefs.GetString("");

		// Разделяем её на массив по нулевому символу
		string[] allKeysArray = allKeysString.Split('\0');

		// Проходим по каждому ключу
		foreach (string key in allKeysArray)
		{
			// Проверяем, является ли ключ нашим биндингом
			if (!string.IsNullOrEmpty(key) && key.StartsWith(PREFIX_KEYBINDING))
			{
				string actionName = key.Substring(PREFIX_KEYBINDING.Length);
				if (Enum.TryParse<KeyCode>(PlayerPrefs.GetString(key), out var keyCode))
				{
					data.KeyBindings[actionName] = keyCode;
				}
			}
		}

		return data;
	}

	// Полное удаление всех наших настроек из PlayerPrefs
	public static void DeleteAllSettings()
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