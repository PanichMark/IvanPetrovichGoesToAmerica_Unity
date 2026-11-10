using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionMapsController : MonoBehaviour, IJsonSaveLoad
{
	private Dictionary<GameMissionsNamesEnum, List<string>> _collectedMissionMaps = new Dictionary<GameMissionsNamesEnum, List<string>>();
	private MissionsManager _missionsManager;
	private JournalMenuController _journalMenuController;
	private JournalMenuSectionMissionInfoController _journalMenuSectionMissionInfoController;
	private Bootstrap _bootstrap;
	private MissionMapsList _missionMapsList;

	public void Initialize(
		Bootstrap bootstrap,
		JournalMenuController journalMenuController,
		JournalMenuSectionMissionInfoController journalMenuSectionMissionInfoController,
		MissionsManager missionsManager)
	{
		_bootstrap = bootstrap;
		_missionMapsList = _bootstrap.GameData.MissionMapsList;
		_journalMenuController = journalMenuController;
		_journalMenuSectionMissionInfoController = journalMenuSectionMissionInfoController;
		_missionsManager = missionsManager;
		_journalMenuController.OnOpenJournaMenuSectionMissionInfo += SendMissionMapsDataToJournal;

		Debug.Log("MissionMapsController Initialized");
	}

	private void SendMissionMapsDataToJournal()
	{
		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;
		List<MissionMapData> resolvedMaps = new List<MissionMapData>();

		if (_collectedMissionMaps.TryGetValue(currentMissionKey, out var stringList))
		{
			// 1. Сортируем сырые строки по порядку их следования в MissionMapsNamesEnum
			var sortedList = stringList.OrderBy(id =>
			{
				if (System.Enum.TryParse<MissionMapsNamesEnum>(id, out var parsedEnum))
				{
					return (int)parsedEnum;
				}
				return int.MaxValue; // Если ID не найден в Enum, кидаем в конец
			}).ToList();

			// 2. Ищем ScriptableObject уже в отсортированном списке
			foreach (var mapID in sortedList)
			{
				MissionMapData foundMap = System.Array.Find(_missionMapsList.MissionMapsInOrder, x => x.MissionMapID.ToString() == mapID);
				if (foundMap != null)
				{
					resolvedMaps.Add(foundMap);
				}
			}
		}

		_journalMenuSectionMissionInfoController.ApplyMissionMapsData(resolvedMaps);
	}



	// Метод для геймплея: добавляет ID в список текущей активной миссии
	public void AddMissionMapToCollected(string mapID)
	{
		if (string.IsNullOrEmpty(mapID) || _missionsManager == null || _missionsManager.ActiveMission == null) return;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (!_collectedMissionMaps.ContainsKey(currentMissionKey))
		{
			_collectedMissionMaps[currentMissionKey] = new List<string>();
		}

		if (!_collectedMissionMaps[currentMissionKey].Contains(mapID))
		{
			_collectedMissionMaps[currentMissionKey].Add(mapID);
		}
	}

	// Метод для геймплея: удаляет ID из списка текущей активной миссии
	public void RemoveMissionMapFromCollected(string mapID)
	{
		if (string.IsNullOrEmpty(mapID) || _missionsManager == null || _missionsManager.ActiveMission == null) return;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (_collectedMissionMaps.ContainsKey(currentMissionKey))
		{
			_collectedMissionMaps[currentMissionKey].Remove(mapID);
		}
	}

	public bool HasMissionMap(string mapID)
	{
		if (_missionsManager == null || _missionsManager.ActiveMission == null) return false;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		return _collectedMissionMaps.ContainsKey(currentMissionKey) && _collectedMissionMaps[currentMissionKey].Contains(mapID);
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		if (_missionsManager == null || _missionsManager.ActiveMission == null) yield break;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		// Если текущая миссия тестовая — просто пропускаем блок с картами
		if (currentMissionKey == GameMissionsNamesEnum.Mission_Test)
		{
			yield return null;
		}
		else
		{
			if (data.MissionData.MissionMaps == null)
			{
				data.MissionData.MissionMaps = new Dictionary<GameMissionsNamesEnum, List<string>>();
			}
			if (!data.MissionData.MissionMaps.ContainsKey(currentMissionKey))
			{
				data.MissionData.MissionMaps[currentMissionKey] = new List<string>();
			}

			if (_collectedMissionMaps.ContainsKey(currentMissionKey))
			{
				data.MissionData.MissionMaps[currentMissionKey] = new List<string>(_collectedMissionMaps[currentMissionKey]);
			}
		}

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		_collectedMissionMaps.Clear();

		if (_missionsManager == null || _missionsManager.ActiveMission == null) yield break;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		// Если текущая миссия тестовая — не пытаемся читать для неё данные
		if (currentMissionKey == GameMissionsNamesEnum.Mission_Test)
		{
			yield return null;
		}
		else
		{
			if (data.MissionData.MissionMaps != null && data.MissionData.MissionMaps.TryGetValue(currentMissionKey, out var sourceList))
			{
				_collectedMissionMaps[currentMissionKey] = new List<string>(sourceList);
			}
		}

		yield return null;
	}
}