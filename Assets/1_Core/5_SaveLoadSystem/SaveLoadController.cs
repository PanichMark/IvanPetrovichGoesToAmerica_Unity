using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;

	private FileDataHandler _fileDataHandler;
	private GameData _gameData;
	private int LoadedObjectsCount;
	private List<ISaveLoad> _persistentSaveLoadObjects;
	private List<ISaveLoad> _gameplaySaveLoadObjects;

	private const string _SAFE_FILE_DATA_TEMP = "SafeFile_TEMP.json";

	private string[] _saveFilePaths;
	private const string _SAVE_SLOT_PREFIX = "SafeFileSlot_";
	private const string _SAVE_SLOT_SUFFIX = ".json";

	public delegate void GameSaveProcessHandler();
	public event GameSaveProcessHandler OnStartGameDataProcessForUI;
	public event GameSaveProcessHandler OnEndGameDataProcessForUI;

	public string SceneNameToLoad { get; private set; }
	public bool IsSavingFinished { get; private set; }

	public delegate void GameSafeFileHandler();
	public event GameSafeFileHandler OnSafeFileDelete;
	public event GameSafeFileHandler OnSafeFileLoad;
	public event GameSafeFileHandler OnSafeFileSaved;

	public void Initialize(
		Bootstrap bootstrap,
		GameScenesManager gameSceneManager,
		GameController gameController)
	{
		_bootstrap = bootstrap;
		_gameSceneManager = gameSceneManager;
		_gameController = gameController;

		_saveFilePaths = new string[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			_saveFilePaths[i] = $"{_SAVE_SLOT_PREFIX}{i + 1}{_SAVE_SLOT_SUFFIX}";
		}

		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			if (IsSavingFinished == false)
			{
				StartCoroutine(OnSceneLoadUpdateGameplayObjects());
			}
		};

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => StartCoroutine(NewGame());
		
		Debug.Log("SaveLoadController Initialized");
	}

	public IEnumerator NewGame()
	{
		_persistentSaveLoadObjects = FindAllPersistentSaveLoadObjects();

		_gameData = new GameData();

		_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _SAFE_FILE_DATA_TEMP);
		_fileDataHandler.Save(_gameData);

		foreach (ISaveLoad saveLoadObj in _persistentSaveLoadObjects)
		{
			saveLoadObj.LoadData(_gameData);
		}

		Debug.Log("### New Game Started ###");
		yield break;
	}

	public IEnumerator SaveGame(int saveSlotNumber)
	{
		IsSavingFinished = false;
		_fileDataHandler = null;

		if (saveSlotNumber == -1)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _SAFE_FILE_DATA_TEMP);

			if (_gameData == null)
			{
				Debug.Log("NO GAMEDATA TO SAVE");
				yield break;
			}
		}
		else
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _saveFilePaths[saveSlotNumber - 1]);
		}

		OnStartGameDataProcessForUI?.Invoke();

		foreach (ISaveLoad saveLoadObj in _persistentSaveLoadObjects)
		{
			yield return saveLoadObj.SaveData(_gameData);
		}

		foreach (ISaveLoad saveLoadObj in _gameplaySaveLoadObjects)
		{
			yield return saveLoadObj.SaveData(_gameData);
		}

		_fileDataHandler.Save(_gameData);

		if (saveSlotNumber != -1)
		{
			OnSafeFileSaved?.Invoke();

			Debug.Log("Data saved to slot " + saveSlotNumber);
		}

		OnEndGameDataProcessForUI?.Invoke();

		IsSavingFinished = true;
		yield break;
	}

	public IEnumerator LoadGame(int loadSlotNumber)
	{
		Debug.Log("LOADING 1111111");
		if (_gameData == null)
		{
			Debug.Log("NO GAMEDATA TO LOAD");
			yield break;
		}

		OnSafeFileLoad?.Invoke();
		_gameController.CloseMainMenu();

		_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _saveFilePaths[loadSlotNumber - 1]);
		_gameData = _fileDataHandler.Load();

		SceneNameToLoad = _gameData.Scene;

		// Запускаем сцену (она сама заполнит шкалу до 0.667)
		StartCoroutine(_gameSceneManager.LoadGameplayScene((GameScenesSystemEnum)Enum.Parse(typeof(GameScenesSystemEnum), SceneNameToLoad)));

		// Ждем, пока ASYNC SCENE LOAD не закончится
		yield return new WaitWhile(() => _gameSceneManager.HasLoadedGameplayScene == false);

		Debug.Log("LOADING 222222222222222222"); // СЦЕНА ЗАГРУЖЕНА

		int totalObjects = _persistentSaveLoadObjects.Count;
		LoadedObjectsCount = 0;



		// Грузим персистентные объекты (инвентарь игрока)
		foreach (ISaveLoad persistentLoadObj in _persistentSaveLoadObjects)
		{
			yield return persistentLoadObj.LoadData(_gameData);

			LoadedObjectsCount++;
			float progress = Mathf.Lerp(0.5f, 0.75f, (float)LoadedObjectsCount / totalObjects);

			// --- ОБНОВЛЕНИЕ СЛАЙДЕРА ---

			_gameSceneManager.SetLoadingSliderValue(progress);
			
		}
		Debug.Log("LOADING 333333333333333");

		// ОБНОВЛЯЕМ СПИСОК НОВЫХ ОБЪЕКТОВ СРАЗУ ПОСЛЕ ЗАГРУЗКИ СЦЕНЫ
		yield return UpdateGameplaySaveLoadObjects();
		Debug.Log("LOADING 444444444444");

		totalObjects += _gameplaySaveLoadObjects.Count;

		// ТЕПЕРЬ грузим в них данные
		for (int i = _gameplaySaveLoadObjects.Count - 1; i >= 0; i--)
		{
			yield return _gameplaySaveLoadObjects[i].LoadData(_gameData);

			LoadedObjectsCount++;
			float progress = Mathf.Lerp(0.75f, 1f, (float)LoadedObjectsCount / totalObjects);

			// --- ОБНОВЛЕНИЕ СЛАЙДЕРА ---
			_gameSceneManager.SetLoadingSliderValue(progress);

		}
		Debug.Log("LOADING 5555555555555555");

		// Сохраняем актуальное состояние во временный слот (-1)
		yield return SaveGame(-1);
		Debug.Log("LOADING 66666666666666666");

		// Говорим менеджеру: "Данные применены, можно выключать UI"
		_gameSceneManager.ApplyGameplayDataFinished();

		Debug.Log("LOADING 77777777777777777");
	}

	public void DeleteGame(int deleteSlotNumber)
	{
		if (deleteSlotNumber <= 0)
		{
			Debug.LogError("Invalid slot number for deletion.");
			return;
		}

		string fullPath = Path.Combine(Application.persistentDataPath, _saveFilePaths[deleteSlotNumber - 1]);

		_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _saveFilePaths[deleteSlotNumber - 1]);

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

	IEnumerator OnSceneLoadUpdateGameplayObjects()
	{
		OnStartGameDataProcessForUI?.Invoke();

		yield return StartCoroutine(UpdateGameplaySaveLoadObjects());

		foreach (ISaveLoad gameplayLoadObj in _gameplaySaveLoadObjects)
		{
			gameplayLoadObj.LoadData(_gameData);
		}

		yield return StartCoroutine(SaveGame(-1));

		OnEndGameDataProcessForUI?.Invoke();

		yield break;
	}

	private void AssignGameplayObjectIndexes()
	{
		AssignNPCsIndexes();
		AssignLootObjectsIndexes();
		AssignPickableObjectsIndexes();
		AssignOpenableObjectsIndexes();
		AssignTVsIndexes();
		AssignLightsIndexes();
	}

	private void AssignLootObjectsIndexes()
	{
		InteractionObjectLootAbstract[] lootObjects = FindObjectsOfType<InteractionObjectLootAbstract>();

		Array.Sort(lootObjects, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < lootObjects.Length; index++)
		{
			lootObjects[index].AssignLootObjectsIndexes(index);
		}
	}

	private void AssignPickableObjectsIndexes()
	{
		InteractionObjectPickableAbstract[] pickableObjects = FindObjectsOfType<InteractionObjectPickableAbstract>();

		Array.Sort(pickableObjects, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < pickableObjects.Length; index++)
		{
			pickableObjects[index].AssignPickableObjectsIndexes(index);
		}
	}

	private void AssignOpenableObjectsIndexes()
	{
		InteractionObjectOpenableAbstract[] openableObjects = FindObjectsOfType<InteractionObjectOpenableAbstract>();

		Array.Sort(openableObjects, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < openableObjects.Length; index++)
		{
			openableObjects[index].AssignOpenableObjectsIndexes(index);
		}
	}

	private void AssignNPCsIndexes()
	{
		NPCcore[] TVs = FindObjectsOfType<NPCcore>();

		Array.Sort(TVs, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < TVs.Length; index++)
		{
			TVs[index].AssignNPCsIndexes(index);
		}
	}

	private void AssignTVsIndexes()
	{
		InteractionObjectTVabstract[] TVs = FindObjectsOfType<InteractionObjectTVabstract>();

		Array.Sort(TVs, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < TVs.Length; index++)
		{
			TVs[index].AssignTVsIndexes(index);
		}
	}

	private void AssignLightsIndexes()
	{
		InteractionObjectLightAbstract[] lights = FindObjectsOfType<InteractionObjectLightAbstract>();

		Array.Sort(lights, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

		for (int index = 0; index < lights.Length; index++)
		{
			lights[index].AssignLightsIndexes(index);
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

		AssignGameplayObjectIndexes();

		_gameplaySaveLoadObjects = FindAllGameplaySaveLoadObjects();

		yield break;
	}

	public (string SavefileDateAndTime, string SafeFileMissionNameSystem, string SafefileSceneNameSystem)[] GetExtendedSaveInfo()
	{
		var extendedInfo = new List<(string DateAndTime, string MissionNAmeSystem, string SceneNameSystem)>();

		for (int i = 0; i < _saveFilePaths.Length; i++)
		{
			extendedInfo.Add(GetExtendedSaveDataForFile(_saveFilePaths[i]));
		}

		return extendedInfo.ToArray();
	}

	private (string SavefileDateAndTime, string SafeFileMissionNameSystem, string SafefileSceneNameSystem) GetExtendedSaveDataForFile(string fileName)
	{
		try
		{
			GameData gameData = _fileDataHandler.LoadFromFile(fileName);
			if (gameData != null)
			{
				return (
					gameData.SafeFileDateAndTime,
					gameData.MissionData.Mission,
					gameData.Scene
				);
			}
			else
			{
				return (null, null, null);
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"Ошибка при чтении файла '{fileName}'\n{e.Message}");
			return (null, null, null);
		}
	}
}