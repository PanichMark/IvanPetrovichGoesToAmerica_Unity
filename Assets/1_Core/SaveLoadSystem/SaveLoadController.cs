using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController : MonoBehaviour
{
	public string SceneNameToLoad {  get; private set; }

	public delegate void GameSafeFileHandler();
	public event GameSafeFileHandler OnSafeFileDelete;
	public event GameSafeFileHandler OnSafeFileLoad;
	public event GameSafeFileHandler OnSafeFileSaved;

	private GameSceneManager gameSceneManager;
	private string fileSaveDataTEMP = "";
	private string fileSaveDataName1 = "";
	private string fileSaveDataName2 = "";
	private string fileSaveDataName3 = "";
	private string fileSaveDataName4 = "";
	private string fileSaveDataName5 = "";

	private GameData gameData;
	public bool IsSavingFinished { get; private set; }

	private List<ISaveLoad> persistentSaveLoadObjects;
	private List<ISaveLoad> gameplaySaveLoadObjects;
	private FileDataHandler fileDataHandler;
	private GameController gameController;
	public void Initialize(GameSceneManager gameSceneManager, GameController gameController)
	{
		this.gameSceneManager = gameSceneManager;
		this.gameController = gameController;
		fileSaveDataTEMP = "SaveGameTEMP.json";
		fileSaveDataName1 = "SaveGame1.json";
		fileSaveDataName2 = "SaveGame2.json";
		fileSaveDataName3 = "SaveGame3.json";
		fileSaveDataName4 = "SaveGame4.json";
		fileSaveDataName5 = "SaveGame5.json";

		this.gameSceneManager.OnEndLoadGameplayScene += () => StartCoroutine(OnSceneLoadUpdateGameplayObjects());
		
		Debug.Log("SaveLoadController Initialized");
	}
	private void AssignGameplayObjectIndex()
	{
		InteractionObjectLootAbstract[] lootItems = FindObjectsOfType<InteractionObjectLootAbstract>();

		Array.Sort(lootItems, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < lootItems.Length; index++)
		{
			lootItems[index].AssignLootItemIndex(index);
		}
	}

	public IEnumerator NewGame()
	{
		this.persistentSaveLoadObjects = FindAllPersistentSaveLoadObjects();
		this.gameData = new GameData();

		this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataTEMP);

		fileDataHandler.Save(this.gameData);

		foreach (ISaveLoad saveLoadObj in persistentSaveLoadObjects)
		{
			saveLoadObj.LoadData(this.gameData);
		}

		Debug.Log("--- New Game Started ---");
		yield break;
	}

	public IEnumerator SaveGame(int saveSlotNumber)
	{
		IsSavingFinished = false;

		if (saveSlotNumber == -1)
		{
			this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileSaveDataTEMP);

			if (this.gameData == null)
			{
				Debug.Log("NO GAMEDATA TO SAVE");
				yield break;
			}
		}
		
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

		foreach (ISaveLoad saveLoadObj in persistentSaveLoadObjects)
		{
			saveLoadObj.SaveData(ref gameData);
		}

		foreach (ISaveLoad saveLoadObj in gameplaySaveLoadObjects)
		{
			saveLoadObj.SaveData(ref gameData);
		}

		fileDataHandler.Save(gameData);

		if (saveSlotNumber != -1)
		{
			OnSafeFileSaved?.Invoke();

			Debug.Log("Data saved to slot " + saveSlotNumber);
		}

		IsSavingFinished = true;
		yield break;
	}

	public IEnumerator LoadGame(int loadSlotNumber)
	{
		if (this.gameData == null)
		{
			Debug.Log("NO GAMEDATA TO LOAD");
			yield break;
		}

		OnSafeFileLoad?.Invoke();
		gameController.CloseMainMenu();
		
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

		SceneNameToLoad = gameData.CurrentSceneNameSystem;

		foreach (ISaveLoad persistentLoadObj in persistentSaveLoadObjects)
		{
			persistentLoadObj.LoadData(gameData);
		}
		yield return StartCoroutine(gameSceneManager.LoadScene((GameScenesEnum)Enum.Parse(typeof(GameScenesEnum), SceneNameToLoad)));

		yield break;
	}

	IEnumerator OnSceneLoadUpdateGameplayObjects()
	{
		yield return StartCoroutine(UpdateGameplaySaveLoadObjects());

		foreach (ISaveLoad gameplayLoadObj in gameplaySaveLoadObjects)
		{
			gameplayLoadObj.LoadData(gameData);
		}

		yield return StartCoroutine(SaveGame(-1));
		yield break;
	}

	public void DeleteGame(int deleteSlotNumber)
	{
		if (deleteSlotNumber <= 0)
		{
			Debug.LogError("Invalid slot number for deletion.");
			return;
		}

		string fullPath = "";

		if (deleteSlotNumber == 1)
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileSaveDataName1);
		}
		else if (deleteSlotNumber == 2)
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileSaveDataName2);
		}
		else if (deleteSlotNumber == 3)
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileSaveDataName3);
		}
		else if (deleteSlotNumber == 4)
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileSaveDataName4);
		}
		else if (deleteSlotNumber == 5)
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileSaveDataName5);
		}

		try
		{
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
				OnSafeFileDelete?.Invoke();
				Debug.Log("Deleted game from slot " + deleteSlotNumber);
			}
			else
			{
				Debug.LogWarning("No save file exists at slot " + deleteSlotNumber);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error deleting the save file: " + ex.Message);
		}
	}

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
				return new Tuple<string, string, string>(null, null, null); 
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"Ошибка при чтении файла '{fileName}'\n{e.Message}");
			return new Tuple<string, string, string>(null, null, null); 
		}
	}

	private List<ISaveLoad> FindAllPersistentSaveLoadObjects()
	{
		IEnumerable<ISaveLoad> saveLoadObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoad>();

		return new List<ISaveLoad>(saveLoadObjects);
	}

	private List<ISaveLoad> FindAllGameplaySaveLoadObjects()
	{
		IEnumerable<ISaveLoad> gameplaySceneObjects = SceneManager.GetSceneAt(1).GetRootGameObjects()
																  .SelectMany(go => go.GetComponentsInChildren<MonoBehaviour>())
																  .OfType<ISaveLoad>();

		return new List<ISaveLoad>(gameplaySceneObjects);
	}

	public IEnumerator UpdateGameplaySaveLoadObjects()
	{
		if (gameplaySaveLoadObjects != null)
		gameplaySaveLoadObjects.Clear();

		AssignGameplayObjectIndex();

		gameplaySaveLoadObjects = FindAllGameplaySaveLoadObjects();

		yield break;
	}
}