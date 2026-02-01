using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Unity.VisualScripting.Icons;

public class LocalizationManager
{
	private Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();

	public LanguagesEnum CurrentLanguage { get; private set; } = LanguagesEnum.English; // Начальный язык по умолчанию

	public LocalizationManager()
	{
		LoadFromCsv();
	}


	public void ChangeLanguage(LanguagesEnum language)
	{
		CurrentLanguage = language;
		Debug.Log($"Lozalization{language} Initialized");
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
					string ruValue = values[1].Trim('"'); // Удаляем возможные двойные кавычки
					string enValue = values[2].Trim('"');

					if (!_localizations.ContainsKey(key))
						_localizations[key] = new Dictionary<string, string>();

					_localizations[key]["Russian"] = ruValue;
					_localizations[key]["English"] = enValue;
				}
			}
		}
	}
}