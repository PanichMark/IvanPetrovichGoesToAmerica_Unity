using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMapsController : MonoBehaviour, IJsonSaveLoad
{
	private Dictionary<GameMissionsNamesEnum, List<MissionMap>> _collectedMissionMaps = new Dictionary<GameMissionsNamesEnum, List<MissionMap>>();
	private MissionsManager _missionsManager;
	private JournalMenuController _journalMenuController;
	private JournalMenuSectionMissionInfoController _journalMenuSectionMissionInfoController;

	public void Initialize(
		JournalMenuController journalMenuController,
		JournalMenuSectionMissionInfoController journalMenuSectionMissionInfoController,
		MissionsManager missionsManager)
	{
		_journalMenuController = journalMenuController;
		_journalMenuSectionMissionInfoController = journalMenuSectionMissionInfoController;	
		_missionsManager = missionsManager;
		_journalMenuController.OnOpenJournaMenuSectionMissionInfo += SendMissionMapsDataToJournal;

		Debug.Log("MissionMapsController Initialized");
	}

	private void SendMissionMapsDataToJournal()
	{
		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (_collectedMissionMaps.TryGetValue(currentMissionKey, out var currentMissionMapsList))
		{
			_journalMenuSectionMissionInfoController.ReceiveMissionMapsData(currentMissionMapsList);
		}
		else
		{
			// Если данных по текущей миссии нет, отправляем пустой список, чтобы UI не упал
			_journalMenuSectionMissionInfoController.ReceiveMissionMapsData(new List<MissionMap>());
		}
	}

	// Метод для геймплея: добавляет ID в список текущей активной миссии
	public void AddMissionMapToCollected(string mapID)
	{
		if (string.IsNullOrEmpty(mapID) || _missionsManager == null || _missionsManager.ActiveMission == null) return;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (!_collectedMissionMaps.ContainsKey(currentMissionKey))
		{
			_collectedMissionMaps[currentMissionKey] = new List<MissionMap>();
		}

		if (!_collectedMissionMaps[currentMissionKey].Exists(x => x.MissionMapID == mapID))
		{
			_collectedMissionMaps[currentMissionKey].Add(new MissionMap { MissionMapID = mapID });
		}
	}

	// Метод для геймплея: удаляет ID из списка текущей активной миссии
	public void RemoveMissionMapFromCollected(string mapID)
	{
		if (string.IsNullOrEmpty(mapID) || _missionsManager == null || _missionsManager.ActiveMission == null) return;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (_collectedMissionMaps.ContainsKey(currentMissionKey))
		{
			_collectedMissionMaps[currentMissionKey].RemoveAll(x => x.MissionMapID == mapID);
		}
	}

	public bool HasMissionMap(string mapID)
	{
		if (_missionsManager == null || _missionsManager.ActiveMission == null) return false;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		return _collectedMissionMaps.ContainsKey(currentMissionKey) && _collectedMissionMaps[currentMissionKey].Exists(x => x.MissionMapID == mapID);
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		if (_missionsManager == null || _missionsManager.ActiveMission == null) yield break;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (data.MissionData.MissionMaps == null)
		{
			data.MissionData.MissionMaps = new Dictionary<GameMissionsNamesEnum, List<MissionMap>>();
		}
		if (!data.MissionData.MissionMaps.ContainsKey(currentMissionKey))
		{
			data.MissionData.MissionMaps[currentMissionKey] = new List<MissionMap>();
		}

		// Сохраняем только данные по текущей активной миссии
		data.MissionData.MissionMaps[currentMissionKey] = new List<MissionMap>(_collectedMissionMaps[currentMissionKey]);

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		_collectedMissionMaps.Clear();

		if (_missionsManager == null || _missionsManager.ActiveMission == null) yield break;

		GameMissionsNamesEnum currentMissionKey = _missionsManager.ActiveMission.MissionName;

		if (data.MissionData.MissionMaps != null && data.MissionData.MissionMaps.TryGetValue(currentMissionKey, out var sourceList))
		{
			_collectedMissionMaps[currentMissionKey] = new List<MissionMap>(sourceList);
		}

		yield return null;
	}
}