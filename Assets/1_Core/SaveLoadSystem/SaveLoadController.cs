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

	private GameSceneManager _gameSceneManager;
	private string _fileSaveDataTEMP = "";
	private string _fileSaveDataName1 = "";
	private string _fileSaveDataName2 = "";
	private string _fileSaveDataName3 = "";
	private string _fileSaveDataName4 = "";
	private string _fileSaveDataName5 = "";

	private GameData _gameData;
	public bool IsSavingFinished { get; private set; }

	private List<ISaveLoad> _persistentSaveLoadObjects;
	private List<ISaveLoad> _gameplaySaveLoadObjects;
	private FileDataHandler _fileDataHandler;
	private GameController _gameController;
	public void Initialize(GameSceneManager gameSceneManager, GameController gameController)
	{
		_gameSceneManager = gameSceneManager;
		_gameController = gameController;
		_fileSaveDataTEMP = "SaveGameTEMP.json";
		_fileSaveDataName1 = "SaveGame1.json";
		_fileSaveDataName2 = "SaveGame2.json";
		_fileSaveDataName3 = "SaveGame3.json";
		_fileSaveDataName4 = "SaveGame4.json";
		_fileSaveDataName5 = "SaveGame5.json";

		_gameSceneManager.OnEndLoadingGameplayScene += () => StartCoroutine(OnSceneLoadUpdateGameplayObjects());
		
		Debug.Log("SaveLoadController Initialized");
	}
	private void AssignGameplayObjectIndex()
	{
		InteractionObjectLootAbstract[] lootItems = FindObjectsOfType<InteractionObjectLootAbstract>();

		Array.Sort(lootItems, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < lootItems.Length; index++)
		{
			lootItems[index].AssignLootObjectsIndex(index);
		}
	}

	public IEnumerator NewGame()
	{
		_persistentSaveLoadObjects = FindAllPersistentSaveLoadObjects();
		_gameData = new GameData();

		_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataTEMP);

		_fileDataHandler.Save(_gameData);

		foreach (ISaveLoad saveLoadObj in _persistentSaveLoadObjects)
		{
			saveLoadObj.LoadData(_gameData);
		}

		Debug.Log("--- New Game Started ---");
		yield break;
	}

	public IEnumerator SaveGame(int saveSlotNumber)
	{
		IsSavingFinished = false;

		if (saveSlotNumber == -1)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataTEMP);

			if (_gameData == null)
			{
				Debug.Log("NO GAMEDATA TO SAVE");
				yield break;
			}
		}
		
		if (saveSlotNumber == 1)
        {
             _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName1);
        }
		else if (saveSlotNumber == 2)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName2);
		}
		else if (saveSlotNumber == 3)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName3);
		}
		else if (saveSlotNumber == 4)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName4);
		}
		else if (saveSlotNumber == 5)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName5);
		}

		foreach (ISaveLoad saveLoadObj in _persistentSaveLoadObjects)
		{
			saveLoadObj.SaveData(ref _gameData);
		}

		foreach (ISaveLoad saveLoadObj in _gameplaySaveLoadObjects)
		{
			saveLoadObj.SaveData(ref _gameData);
		}

		_fileDataHandler.Save(_gameData);

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
		if (_gameData == null)
		{
			Debug.Log("NO GAMEDATA TO LOAD");
			yield break;
		}

		OnSafeFileLoad?.Invoke();
		_gameController.CloseMainMenu();
		
		if (loadSlotNumber == 1)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName1);
		
		}
		else if (loadSlotNumber == 2)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName2);
		
		}
		else if (loadSlotNumber == 3)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName3);
			
		}
		else if (loadSlotNumber == 4)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName4);
			
		}
		else if (loadSlotNumber == 5)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileSaveDataName5);
			
		}

		_gameData = _fileDataHandler.Load();

		SceneNameToLoad = _gameData.CurrentSceneNameSystem;

		foreach (ISaveLoad persistentLoadObj in _persistentSaveLoadObjects)
		{
			persistentLoadObj.LoadData(_gameData);
		}
		yield return StartCoroutine(_gameSceneManager.LoadGameplayScene((GameScenesEnum)Enum.Parse(typeof(GameScenesEnum), SceneNameToLoad)));

		yield break;
	}

	IEnumerator OnSceneLoadUpdateGameplayObjects()
	{
		yield return StartCoroutine(UpdateGameplaySaveLoadObjects());

		foreach (ISaveLoad gameplayLoadObj in _gameplaySaveLoadObjects)
		{
			gameplayLoadObj.LoadData(_gameData);
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
			fullPath = Path.Combine(Application.persistentDataPath, _fileSaveDataName1);
		}
		else if (deleteSlotNumber == 2)
		{
			fullPath = Path.Combine(Application.persistentDataPath, _fileSaveDataName2);
		}
		else if (deleteSlotNumber == 3)
		{
			fullPath = Path.Combine(Application.persistentDataPath, _fileSaveDataName3);
		}
		else if (deleteSlotNumber == 4)
		{
			fullPath = Path.Combine(Application.persistentDataPath, _fileSaveDataName4);
		}
		else if (deleteSlotNumber == 5)
		{
			fullPath = Path.Combine(Application.persistentDataPath, _fileSaveDataName5);
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

	public (string SavefileDateAndTime, string SafefileSceneNameSystem)[] GetExtendedSaveInfo()
	{
		var extendedInfo = new List<(string DateAndTime, string SceneNameSystem)>();

		extendedInfo.Add(GetExtendedSaveDataForFile(_fileSaveDataName1));
		extendedInfo.Add(GetExtendedSaveDataForFile(_fileSaveDataName2));
		extendedInfo.Add(GetExtendedSaveDataForFile(_fileSaveDataName3));
		extendedInfo.Add(GetExtendedSaveDataForFile(_fileSaveDataName4));
		extendedInfo.Add(GetExtendedSaveDataForFile(_fileSaveDataName5));

		return extendedInfo.ToArray();
	}

	private (string SavefileDateAndTime, string SafefileSceneNameSystem) GetExtendedSaveDataForFile(string fileName)
	{
		try
		{
			GameData gameData = _fileDataHandler.LoadFromFile(fileName);
			if (gameData != null)
			{
				return (
					gameData.CurrentDateAndTime,
					gameData.CurrentSceneNameSystem
				);
			}
			else
			{
				return (null, null);
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"Ошибка при чтении файла '{fileName}'\n{e.Message}");
			return (null, null);
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
		if (_gameplaySaveLoadObjects != null)
		_gameplaySaveLoadObjects.Clear();

		AssignGameplayObjectIndex();

		_gameplaySaveLoadObjects = FindAllGameplaySaveLoadObjects();

		yield break;
	}
}