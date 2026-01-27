using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager
{
	private Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();

	public string CurrentLanguage { get; private set; } = "English"; // Начальный язык по умолчанию

	public LocalizationManager()
	{
		LoadFromCsv();
	}

	public void ChangeLanguage(string language)
	{
		CurrentLanguage = language;
	}

	public string GetLocalizedString(string key)
	{
		if (_localizations.TryGetValue(key, out var translations) &&
			translations.TryGetValue(CurrentLanguage, out var translation))
		{
			return translation;
		}
		return $"Unknown key '{key}' or no translation available.";
	}

	private void LoadFromCsv()
	{
		TextAsset csvFile = Resources.Load("Lozalization/IvanPetrovichGoesToAmerica_Localization") as TextAsset;
		if (csvFile == null)
		{
			Debug.LogError("Could not load CSV file.");
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
					string enValue = values[1].Trim('"'); // Удаляем возможные двойные кавычки
					string ruValue = values[2].Trim('"');

					if (!_localizations.ContainsKey(key))
						_localizations[key] = new Dictionary<string, string>();

					_localizations[key]["English"] = enValue;
					_localizations[key]["Russian"] = ruValue;
				}
			}
		}
	}
}