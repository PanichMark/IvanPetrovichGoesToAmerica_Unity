using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
	private string dataDirPath = "";

	private string dataFileName = "";

	public FileDataHandler(string dataDirPath, string dataFileName)
	{
		this.dataDirPath = dataDirPath;
		this.dataFileName = dataFileName;
	}

	public GameData Load()
	{
		string fullPath = Path.Combine(dataDirPath, dataFileName);
		GameData loadedData = null;
		if (File.Exists(fullPath))
		{
			try
			{
				string dataToLoad = "";
				using (FileStream stream = new FileStream(fullPath, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						dataToLoad = reader.ReadToEnd();
					}
				}
				loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

			}
			catch (Exception e)
			{
				Debug.LogError("Loading error: " + fullPath + "/n" + e);
			}
		}
		return loadedData;
	}

	public void Save(GameData data)
	{
		string fullPath = Path.Combine(dataDirPath, dataFileName);
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

			// --- НОВЫЙ КОД ДЛЯ СОРТИРОВКИ ---
			// Создаем копии списков и сортируем их по LootObjectIndex
			// Это нужно, чтобы не менять исходные данные в GameData во время игры
			if (data.LootObjects_Scene_0_Test != null)
			{
				var sortedScene0 = new List<LootObjectData>(data.LootObjects_Scene_0_Test);
				sortedScene0.Sort((a, b) => a.LootObjectIndex.CompareTo(b.LootObjectIndex));
				data.LootObjects_Scene_0_Test = sortedScene0;
			}

			if (data.LootObjects_Scene_1_StreetMain != null)
			{
				var sortedScene1 = new List<LootObjectData>(data.LootObjects_Scene_1_StreetMain);
				sortedScene1.Sort((a, b) => a.LootObjectIndex.CompareTo(b.LootObjectIndex));
				data.LootObjects_Scene_1_StreetMain = sortedScene1;
			}
			// --- КОНЕЦ НОВОГО КОДА ---

			// Теперь сериализуем уже отсортированные данные
			string dataToStore = JsonUtility.ToJson(data, true);

			using (FileStream stream = new FileStream(fullPath, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.Write(dataToStore);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Saving error: " + fullPath + "\n" + e);
		}
	}

	public GameData LoadFromFile(string fileName)
	{
		string fullPath = Path.Combine(dataDirPath, fileName);
		GameData loadedData = null;
		if (File.Exists(fullPath))
		{
			try
			{
				string dataToLoad = "";
				using (FileStream stream = new FileStream(fullPath, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						dataToLoad = reader.ReadToEnd();
					}
				}
				loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
			}
			catch (Exception e)
			{
				Debug.LogError("Loading error: " + fullPath + "\n" + e);
			}
		}
		return loadedData;
	}
}