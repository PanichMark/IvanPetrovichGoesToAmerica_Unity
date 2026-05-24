using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class LocalizationManager
{
	private Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();

	public LanguagesEnum CurrentLanguage { get; private set; }
	public delegate void ChangeLanguageEvent();
	public event ChangeLanguageEvent OnLanguageChanged;

	public LocalizationManager()
	{
		LoadFromLocalizationCSV();
	
		Debug.Log("Localization Manager Initialized");
	}

	public void ChangeLanguage(LanguagesEnum language)
	{
		CurrentLanguage = language;
		OnLanguageChanged?.Invoke();
	}

	public string GetLocalizedString(string key)
	{
		if (_localizations.TryGetValue(key, out var translations) &&
			translations.TryGetValue(CurrentLanguage.ToString(), out var translation))
		{
			return translation;
		}
		else
		{
			Debug.LogError($"Key \"{key}\" not found!");
			return null;
		}
	}

	private void LoadFromLocalizationCSV()
	{
		TextAsset csvFile = Resources.Load("Lozalization/IvanPetrovichGoesToAmerica_Localization") as TextAsset;
		if (csvFile == null)
		{
			Debug.LogError("Could not load CSV file");
			return;
		}

		using (StreamReader sr = new StreamReader(new MemoryStream(csvFile.bytes)))
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				string[] values = line.Split(',');
				if (values.Length >= 3)
				{
					string key = values[0];
					string ruValue = values[1].Trim('"'); 
					string enValue = values[2].Trim('"');

					if (!_localizations.ContainsKey(key))
						_localizations[key] = new Dictionary<string, string>();

					_localizations[key]["Russian"] = ruValue;
					_localizations[key]["English"] = enValue;
				}
			}
		}
	}

	public string GetLanguageSuffix()
	{
		if (CurrentLanguage == LanguagesEnum.Russian)
		{
			return "RU";
		}
		if (CurrentLanguage == LanguagesEnum.English)
		{
			return "EN";
		}
		else
		{
			throw new NotImplementedException("This language NOT supported");
		}
	}

	public string GetLanguageSuffix(InteractionObjectNoteData noteData)
	{
		if (noteData == null)
			return string.Empty;

		// Получаем суффикс языка
		string suffix = GetLanguageSuffix();

		// Формируем имя поля, которое хотим найти
		string fieldName = "NoteText_" + suffix;

		// Используем отражение, чтобы получить значение поля по имени
		FieldInfo fieldInfo = noteData.GetType().GetField(fieldName,
			BindingFlags.Public | BindingFlags.Instance);

		if (fieldInfo != null)
		{
			TextAsset textAsset = fieldInfo.GetValue(noteData) as TextAsset;
			if (textAsset != null)
			{
				return textAsset.text;
			}
		}

		// Если поле не найдено или данные некорректны
		Debug.LogError($"{suffix} text not found in {noteData.name}");
		return string.Empty;
	}

}