using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SaveLoadController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;

	private FileDataHandler _fileDataHandler;
	private GameData _gameData;
	private int LoadedObjectsCount;
	private List<ISaveLoad> _coreSaveLoadObjects;
	private List<ISaveLoad> _gameplaySaveLoadObjects;

	private const string _SAFE_FILE_DATA_TEMP = "SafeFile_TEMP.json";

	private string[] _saveFilePaths;
	private const string _SAVE_SLOT_PREFIX = "SafeFileSlot_";
	private const string _SAVE_SLOT_SUFFIX = ".json";
	private bool _WasSavedToTEMPbeforeLoadingNewScene;
	public delegate void GameSaveProcessHandler();
	public event GameSaveProcessHandler OnStartGameDataProcessForUI;
	public event GameSaveProcessHandler OnEndGameDataProcessForUI;

	public string SceneNameToLoad { get; private set; }
	public bool IsSavingFinished { get; private set; }

	public delegate void GameSafeFileHandler();
	public event GameSafeFileHandler OnSafeFileDelete;
	public event GameSafeFileHandler OnSafeFileLoad;
	public event GameSafeFileHandler OnSafeFileSaved;
	private bool _isLoadingFromSaveFile;

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

		_gameSceneManager.OnBeginLoadingGameplayScene += () =>
		{
			if (_gameSceneManager.WasInitialGameplaySceneLoaded)
			{
				StartCoroutine(OnBeforeSceneUnloadedSaveGameplayObjects());
			}
		};
		
		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			//if (IsSavingFinished == false)
			//{
			if (!_isLoadingFromSaveFile)
			{
				StartCoroutine(OnAfterSceneLoadedUpdateAndLoadUpdateGameplayObjects());
			}
				//StartCoroutine(LoadGame(-1));
			//}
		};

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => StartCoroutine(NewGame());
		
		Debug.Log("SaveLoadController Initialized");
	}

	public IEnumerator NewGame()
	{
		_coreSaveLoadObjects = FindAllCoreSaveLoadObjects();
		
		_gameData = new GameData();
		
		
		_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _SAFE_FILE_DATA_TEMP);
		_fileDataHandler.Save(_gameData);
		
		//yield return StartCoroutine(SaveGame(-1));


		foreach (ISaveLoad saveLoadObj in _coreSaveLoadObjects)
		{
			yield return saveLoadObj.LoadData(_gameData);
		}
		

		Debug.Log("### New Game Started ###");
		yield break;
	}

	private void Update()
	{
		//Debug.Log(IsSavingFinished);
	}

	public IEnumerator SaveGame(int saveSlotNumber)
	{
		IsSavingFinished = false;
		//_fileDataHandler = null;

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

		if (saveSlotNumber != -1)
		{
			foreach (ISaveLoad saveLoadObj in _coreSaveLoadObjects)
			{
				yield return saveLoadObj.SaveData(_gameData);
			}
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

		if (_gameSceneManager.WasInitialGameplaySceneLoaded)
		{
			_gameSceneManager.SavedOldGameplayData();
		}

		OnEndGameDataProcessForUI?.Invoke();

		IsSavingFinished = true;
		yield break;
	}

	public IEnumerator LoadGame(int loadSlotNumber)
	{


		Debug.Log($"LoadGame_1 Started loading Slot {loadSlotNumber}");
		if (_gameData == null)
		{
			Debug.Log("NO GAMEDATA TO LOAD");
			yield break;
		}

		OnSafeFileLoad?.Invoke();

		if (_gameController.IsMainMenuOpen)
		{
			_gameController.CloseMainMenu();
		}

		if (loadSlotNumber == -1)
		{
			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _SAFE_FILE_DATA_TEMP);
			Debug.Log($"LoadGame is TEMPslot");
			if (_gameData == null)
			{
				Debug.Log("NO GAMEDATA TO SAVE");
				yield break;
			}
		}
		else
		{
			_isLoadingFromSaveFile = true;

			_fileDataHandler = new FileDataHandler(Application.persistentDataPath, _saveFilePaths[loadSlotNumber - 1]);
			Debug.Log($"LoadGame is {loadSlotNumber} slot");
		}

		_gameData = _fileDataHandler.Load();

		SceneNameToLoad = _gameData.Scene;

		// Запускаем сцену (она сама заполнит шкалу до 0.667)
		if (loadSlotNumber != -1)
		{
			Debug.Log($"LoadGame_2 loading new scene");

			StartCoroutine(_gameSceneManager.LoadGameplayScene((GameScenesSystemEnum)Enum.Parse(typeof(GameScenesSystemEnum), SceneNameToLoad)));

			// Ждем, пока ASYNC SCENE LOAD не закончится
			yield return new WaitWhile(() => _gameSceneManager.HasLoadedGameplayScene == false);
		}

		int totalObjects = _coreSaveLoadObjects.Count;
		LoadedObjectsCount = 0;


		if (loadSlotNumber != -1)
		{
			Debug.Log($"LoadGame_3 Started loading CoreObjects");
			// Грузим персистентные объекты (инвентарь игрока)
			foreach (ISaveLoad coreLoadObj in _coreSaveLoadObjects)
			{
				yield return coreLoadObj.LoadData(_gameData);

				LoadedObjectsCount++;
				float progress = Mathf.Lerp(0.5f, 0.75f, (float)LoadedObjectsCount / totalObjects);

				// --- ОБНОВЛЕНИЕ СЛАЙДЕРА ---

				_gameSceneManager.SetLoadingSliderValue(progress);

			}
			Debug.Log($"LoadGame_4 Ended loading CoreObjects");


			Debug.Log($"LoadGame_5 Started update and load GameplayObjects");
			// ОБНОВЛЯЕМ СПИСОК НОВЫХ ОБЪЕКТОВ СРАЗУ ПОСЛЕ ЗАГРУЗКИ СЦЕНЫ
			yield return UpdateAndLoadGameplaySaveLoadObjects();
			Debug.Log($"LoadGame_6 Ended update and load GameplayObjects");


		}

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
		Debug.Log("LOADING 777777777777");

		// Сохраняем актуальное состояние во временный слот (-1)
		//yield return StartCoroutine(SaveGame(-1));
		Debug.Log("LOADING 888888888888");

		// Говорим менеджеру: "Данные применены, можно выключать UI"
		_gameSceneManager.ApplyGameplayDataLoadingFinished();
		_isLoadingFromSaveFile = false;

		// *** НАЧАЛО НОВОЙ ЛОГИКИ КОПИРОВАНИЯ ***
		if (loadSlotNumber != -1)
		{
			Debug.Log($"Starting copy of slot {loadSlotNumber} to TEMP...");

			// Сбрасываем флаг перед запуском

			// Запускаем начало процесса для UI
			//OnStartGameDataProcessForUI?.Invoke();

			// Запускаем саму копию как отдельную корутину
			StartCoroutine(CopyCurrentSlotToTempAsync(loadSlotNumber));

			// ЖДЕМ, пока копия не завершится. 
			// Это удержит очередь loadgame от вызова OnEnd до завершения копирования.
			//yield return new WaitWhile(() => _isCopyingToTemp);

			// Копирование завершено, вызываем финал для UI
			
		}
		else
		{
			OnEndGameDataProcessForUI?.Invoke();
		}
			// *** КОНЕЦ НОВОЙ ЛОГИКИ ***


			Debug.Log("LOADING 999999999999");
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

	private IEnumerator OnBeforeSceneUnloadedSaveGameplayObjects()
	{
		if (_WasSavedToTEMPbeforeLoadingNewScene == false)
		{
			yield return StartCoroutine(SaveGame(-1));
		}
		else
		{
			//Debug.Log("WAS TEMPED!!!");
			_gameSceneManager.SkipWaitingDueToTEMPcopied();
			_WasSavedToTEMPbeforeLoadingNewScene = false;
		}
		//Debug.Log("WAS TEMPED!!!");
		yield return null;
	}


	private IEnumerator OnAfterSceneLoadedUpdateAndLoadUpdateGameplayObjects()
	{
		Debug.Log("OnSceneLoadUpdateGameplayObjects Started");

		OnStartGameDataProcessForUI?.Invoke();

		yield return StartCoroutine(UpdateAndLoadGameplaySaveLoadObjects());

		yield return StartCoroutine(LoadGame(-1));

		//OnEndGameDataProcessForUI?.Invoke();

		Debug.Log("OnSceneLoadUpdateGameplayObjects Ended");

		yield return null;
	}

	private void AssignGameplayObjectIndexes()
	{
		/*
		AssignNPCsIndexes();
		AssignLootObjectsIndexes();
		AssignPickableObjectsIndexes();
		AssignOpenableObjectsIndexes();
		AssignTVsIndexes();
		AssignLightsIndexes();
		*/
	}

	/*
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
	*/

	private List<ISaveLoad> FindAllCoreSaveLoadObjects()
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

	public IEnumerator UpdateAndLoadGameplaySaveLoadObjects()
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

	// Вспомогательный адаптер для запуска Task внутри IEnumerator
	private IEnumerator RunTask(Task task)
	{
		// 1. Блок ожидания вынесен отдельно (вне try-catch)
		yield return new WaitWhile(() => !task.IsCompleted);

		// 2. Теперь проверяем результат в обычном потоке C#
		try
		{
			// Пробрасываем ошибку, если она была. 
			// Обращение к свойству .Exception само по себе не выбрасывает AggregateException,
			// поэтому используем конструкцию ниже для явного выброса или просто читаем InnerExceptions.
			if (task.Exception != null)
			{
				throw task.Exception;
			}

			Debug.Log($"Async operation completed successfully.");
		}
		catch (AggregateException ae)
		{
			// Task.Run оборачивает любые ошибки в AggregateException
			foreach (var ex in ae.InnerExceptions)
			{
				Debug.LogError($"Async operation failed: {ex.Message}");
			}
			// Повторно выбрасываем, чтобы вызывающая корутина могла узнать о сбое
			throw;
		}
		catch (Exception ex)
		{
			Debug.LogError($"Async operation failed: {ex.Message}");
			throw;
		}
	}

	public IEnumerator CopyCurrentSlotToTempAsync(int sourceSlotNumber)
	{
		string sourcePath = Path.Combine(Application.persistentDataPath, _saveFilePaths[sourceSlotNumber - 1]);
		string destPath = Path.Combine(Application.persistentDataPath, _SAFE_FILE_DATA_TEMP);

		if (!File.Exists(sourcePath))
		{
			Debug.LogError($"Source file not found for TEMP copy: {sourcePath}");
			OnEndGameDataProcessForUI?.Invoke();
			yield break;
		}

		// Запускаем асинхронную копию
		Task copyTask = Task.Run(() => File.Copy(sourcePath, destPath, true));

		// Ждем завершения задачи (вызов StartCoroutine тоже содержит yield, 
		// поэтому он обязан быть вне try-catch)
		yield return StartCoroutine(RunTask(copyTask));

		// Этот вызов сработает только если RunTask завершился без исключения.
		// Если же "throw" из RunTask дошел сюда, выполнение метода прервется.
		_WasSavedToTEMPbeforeLoadingNewScene = true;
		OnEndGameDataProcessForUI?.Invoke();
	}
}