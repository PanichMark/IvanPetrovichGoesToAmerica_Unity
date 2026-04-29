

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

	// Загрузка всех настроек в новый объект SettingsData
	public SettingsData LoadSettings()
	{
		var data = new SettingsData();

		// --- 1. ЗАГРУЗКА FOV ---
		if (PlayerPrefs.HasKey(KEY_FOV))
		{
			data.FOV = PlayerPrefs.GetFloat(KEY_FOV);
			Debug.Log($"[Загрузка] FOV успешно загружен: {data.FOV}");
		}
		else
		{
			Debug.Log("[Загрузка] Ключ FOV не найден в PlayerPrefs. Используется значение по умолчанию.");
		}

		// --- 2. ЗАГРУЗКА БИНДИНГОВ ---
		// Получаем строку со всеми ключами, хранящимися в PlayerPrefs
		string allKeysString = PlayerPrefs.GetString("");
		string[] allKeysArray = allKeysString.Split('\0'); // Разделяем на массив

		// Проходим по каждому найденному ключу
		foreach (string key in allKeysArray)
		{
			// Проверяем, является ли ключ нашим биндингом и не является ли он пустой строкой
			if (!string.IsNullOrEmpty(key) && key.StartsWith(PREFIX_KEYBINDING))
			{
				// Извлекаем имя действия из ключа.
				// Например, из "KeyBinding_Jump" получаем "Jump"
				string actionName = key.Substring(PREFIX_KEYBINDING.Length);

				// Получаем строковое значение клавиши, которое мы сохранили ранее
				string savedValueStr = PlayerPrefs.GetString(key);
				KeyCode parsedKeyCode;

				// --- ИСПРАВЛЕННЫЙ ПАРСИНГ KEYCODE ---
				// Пробуем распарсить строку напрямую
				if (Enum.TryParse<KeyCode>(savedValueStr, out parsedKeyCode))
				{
					// Если получилось, добавляем в словарь
					data.KeyBindings[actionName] = parsedKeyCode;
					Debug.Log($"[Загрузка] УСПЕШНО распарсен биндинг: {actionName} = {parsedKeyCode}");
				}
				else
				{
					// Если не получилось (например, строка "KeyCode.Space"), пробуем обрезать префикс
					if (savedValueStr.StartsWith("KeyCode."))
					{
						string trimmedValue = savedValueStr.Substring(8); // "KeyCode.Space" -> "Space"
						if (Enum.TryParse<KeyCode>(trimmedValue, out parsedKeyCode))
						{
							data.KeyBindings[actionName] = parsedKeyCode;
							Debug.Log($"[Загрузка] УСПЕШНО распарсен биндинг (с обрезкой префикса): {actionName} = {parsedKeyCode}");
						}
						else
						{
							Debug.LogError($"[Загрузка] ОШИБКА: Не удалось распарсить значение '{savedValueStr}' для действия '{actionName}'");
						}
					}
					else
					{
						Debug.LogError($"[Загрузка] ОШИБКА: Неизвестный формат значения '{savedValueStr}' для действия '{actionName}'");
					}
				}
				// -----------------------------------
			}
		}

		// Лог для проверки, сколько ключей мы в итоге загрузили
		if (data.KeyBindings.Count > 0)
		{
			Debug.Log($"[Загрузка] Всего успешно загружено биндингов: {data.KeyBindings.Count}");
		}
		else
		{
			Debug.LogWarning("[Загрузка] Словарь биндингов остался пустым. Проверьте формат сохраненных данных.");
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