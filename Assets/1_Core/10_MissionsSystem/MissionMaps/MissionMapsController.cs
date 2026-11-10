using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMapsController : MonoBehaviour, IJsonSaveLoad
{
	private List<MissionMap> _collectedMissionMaps = new List<MissionMap>();

	public List<MissionMap> CollectedMissionMaps => _collectedMissionMaps;

	public void Initialize()
	{
		Debug.Log("MissionMapsController Initialized");
	}

	// Метод для геймплея: просто передаете ID
	public void AddMissionMapToCollected(string mapID)
	{
		if (string.IsNullOrEmpty(mapID)) return;
		if (_collectedMissionMaps.Exists(x => x.MissionMapID == mapID)) return;

		_collectedMissionMaps.Add(new MissionMap { MissionMapID = mapID });
	}

	public void RemoveMissionMapFromCollected(string mapID)
	{
		_collectedMissionMaps.RemoveAll(x => x.MissionMapID == mapID);
	}

	public bool HasMissionMap(string mapID) => _collectedMissionMaps.Exists(x => x.MissionMapID == mapID);

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		// Определяем, в какую миссию (из вашего enum) мы сохраняем карты
		GameMissionsNamesEnum currentMissionKey = GameMissionsNamesEnum.Mission_Test;

		if (data.MissionData.MissionMaps == null)
		{
			data.MissionData.MissionMaps = new Dictionary<GameMissionsNamesEnum, List<MissionMap>>();
		}
		if (!data.MissionData.MissionMaps.ContainsKey(currentMissionKey))
		{
			data.MissionData.MissionMaps[currentMissionKey] = new List<MissionMap>();
		}

		// Присваиваем наш список объектов напрямую в словарь
		data.MissionData.MissionMaps[currentMissionKey] = new List<MissionMap>(_collectedMissionMaps);

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		_collectedMissionMaps.Clear();

		GameMissionsNamesEnum currentMissionKey = GameMissionsNamesEnum.Mission_Test;

		if (data.MissionData.MissionMaps != null && data.MissionData.MissionMaps.TryGetValue(currentMissionKey, out var sourceList))
		{
			_collectedMissionMaps = new List<MissionMap>(sourceList);
		}

		yield return null;
	}
}