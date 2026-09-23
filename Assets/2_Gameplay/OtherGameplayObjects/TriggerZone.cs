using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerZone : GameplayObjectJsonSaveLoad
{
	[SerializeField] private InteractionObjectNote _noteObject;
	[SerializeField] private CutsceneController _cutscene;
	private Collider _triggerZone;
	private bool _wasHintMessageShown;
	private GameObject _playerCollider;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController; 

	private void Start()
	{
_pauseSubMenuSettingsSectionGeneralController = ServiceLocator.Resolve<PauseSubMenuSettingsSectionGeneralController>();
_playerCollider = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCollider);

		_triggerZone = GetComponent<Collider>();

		_triggerZone.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
	
		

		if (_cutscene != null)
		{
			Debug.Log("CUTSCENE!!!!");
			_cutscene.TriggerCutscene(null);
		}

		if (_pauseSubMenuSettingsSectionGeneralController.AreIngameTutorialsEnabled)
		{
			if (_noteObject != null)
			{
				if (_wasHintMessageShown == false)
				{
					if (other.gameObject == _playerCollider)
					{
						//Debug.Log("SHOW HINT!");

						_noteObject.Interact();
					}
				}
			}
		}

		_wasHintMessageShown = true;
		_triggerZone.enabled = false;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.TriggerZonesData == null)
		{
			data.TriggerZonesData = new Dictionary<GameScenesGameplayEnum, List<TriggerZoneData>>();
		}
		if (!data.TriggerZonesData.ContainsKey(currentScene))
		{
			data.TriggerZonesData[currentScene] = new List<TriggerZoneData>();
		}

		var targetList = data.TriggerZonesData[currentScene];

		int indexInList = targetList.FindIndex(item => item.TriggerZoneIndex == GameplayObjectIndex);
		//Debug.Log(GameplayObjectIndex);
		//Debug.Log(_noteObject.name);
		//Debug.Log(_wasHintMessageShown);
		var updatedItem = new TriggerZoneData
		{
			TriggerZoneIndex = GameplayObjectIndex,
			TriggerZoneNameSystem = gameObject.name,
			WasZoneTriggered = _wasHintMessageShown
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		//Debug.Log("BRUH!!!!");

		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayEnum currentScene)) yield break;

		if (data.TriggerZonesData == null || !data.TriggerZonesData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.TriggerZoneIndex == GameplayObjectIndex);

		if (savedState.Equals(default(TriggerZoneData))) yield break;
		
		_wasHintMessageShown = savedState.WasZoneTriggered;
		Debug.Log(_wasHintMessageShown);
		yield return null;
	}
}