using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System;

public class DataPersistenceManager : MonoBehaviour
{
	private string fileSaveDataTEMP = "";
	private string fileSaveDataName1 = "";
	private string fileSaveDataName2 = "";
	private string fileSaveDataName3 = "";
	private string fileSaveDataName4 = "";
	private string fileSaveDataName5 = "";

	private GameData gameData;
	public bool IsSavingFinished { get; private set; }

	private List<IDataPersistence> dataPersistenceObjects;
	private FileDataHandler fileDataHandler;

	public void Initialize()
	{
		fileSaveDataTEMP = "SaveGameTEMP.json";
		fileSaveDataName1 = "SaveGame1.json";
		fileSaveDataName2 = "SaveGame2.json";
		fileSaveDataName3 = "SaveGame3.json";
		fileSaveDataName4 = "SaveGame4.json";
		fileSaveDataName5 = "SaveGame5.json";

		this.dataPersistenceObjects = FindAllDataPersistenceObjects();

		InteractionObjectLootAbstract[] lootItems = FindObjectsOfType<InteractionObjectLootAbstract>();
		for (int i = 0; i < lootItems.Length; i++)
		{
			lootItems[i].AssignLootItemIndex(i);
		}

		NewGame();


	}















	/*
	public void NewGame()
	{
		this.gameData = new GameData();
		//Debug.Log(gameData.HealingItems);
		whatSaveNumberWasLoaded = 0;
		SaveGame(-1);


		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.LoadData(gameData);
		}
	}
	*/
	public void NewGame()
	{
		// Шаг 1: Создание нового объекта GameData с начальными значениями
		this.gameData = new GameData();

		// Шаг 2: Настройка обработки файла для временного хранилища
		this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataTEMP);

		// Шаг 3: Сохранение текущего состояния в временный файл
		fileDataHandler.Save(this.gameData);

		// Шаг 4: Теперь обновляем каждый объект, реализующий IDataPersistence,
		// используя ранее созданные данные из временного файла
		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.LoadData(this.gameData);
		}

		Debug.Log("A new game has been started!");
	}



	public void SaveGame(int saveSlotNumber)
	{
		IsSavingFinished = false;

		if (saveSlotNumber == -1)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataTEMP);

			if (this.gameData == null)
			{
				this.gameData = new GameData();
			}
			//else this.gameData = fileDataHandler.Load();

		}
		/*
		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.SaveData(ref gameData);
		}
		

		fileDataHandler.Save(gameData);
		*/
		
		
		if (saveSlotNumber == 1)
        {
             this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName1);
        }
		else if (saveSlotNumber == 2)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName2);
		}
		else if (saveSlotNumber == 3)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName3);
		}
		else if (saveSlotNumber == 4)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName4);
		}
		else if (saveSlotNumber == 5)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName5);
		}

		foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
		{
			dataPersistenceObj.SaveData(ref gameData);
		}

		fileDataHandler.Save(gameData);

		if (saveSlotNumber != -1)
		{
			Debug.Log("Data saved to slot " + saveSlotNumber);
		}

	


		IsSavingFinished = true;
	}

	public void LoadGame(int loadSlotNumber)
	{
		
		if (loadSlotNumber == 1)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName1);
		
		}
		else if (loadSlotNumber == 2)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName2);
		
		}
		else if (loadSlotNumber == 3)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName3);
			
		}
		else if (loadSlotNumber == 4)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName4);
			
		}
		else if (loadSlotNumber == 5)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataName5);
			
		}

		
		this.gameData = fileDataHandler.Load();



		if (this.gameData == null)
		{
			Debug.Log("No data to load found in slot " + loadSlotNumber);
			


			return;
		}
		else
		{

			string sceneName = gameData.CurrentSceneNameSystem;

			SceneManager.LoadSceneAsync(sceneName);
			Debug.Log($"Scene {sceneName} loaded");
		}
	}




	// Расширенный метод, возвращающий имя уровня, сумму денег и имя сцены
	public Tuple<string, string, string>[] GetExtendedSaveInfo()
	{
		List<Tuple<string, string, string>> extendedInfo = new List<Tuple<string, string, string>>();

		extendedInfo.Add(GetExtendedSaveDataForFile(fileSaveDataName1));
		extendedInfo.Add(GetExtendedSaveDataForFile(fileSaveDataName2));
		extendedInfo.Add(GetExtendedSaveDataForFile(fileSaveDataName3));
		extendedInfo.Add(GetExtendedSaveDataForFile(fileSaveDataName4));
		extendedInfo.Add(GetExtendedSaveDataForFile(fileSaveDataName5));

		return extendedInfo.ToArray();
	}

	// Вспомогательный метод для получения расширённой информации
	private Tuple<string, string, string> GetExtendedSaveDataForFile(string fileName)
	{
		try
		{
			GameData gameData = fileDataHandler.LoadFromFile(fileName);
			if (gameData != null)
			{
				return new Tuple<string, string, string>(

					gameData.CurrentDateAndTime, 
					gameData.CurrentSceneNameUI,       
					gameData.CurrentSceneNameSystem  
					

				);
			}
			else
			{
				return new Tuple<string, string, string>(null, null, null); // Значения по умолчанию, если данных нет
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"Ошибка при чтении файла '{fileName}'\n{e.Message}");
			return new Tuple<string, string, string>(null, null, null); // Безопасное значение по умолчанию
		}
	}


	private List<IDataPersistence> FindAllDataPersistenceObjects()
	{
		IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

		return new List<IDataPersistence>(dataPersistenceObjects);
	}
}


