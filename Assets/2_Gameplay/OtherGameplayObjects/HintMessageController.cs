using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HintMessageController : GameplayObjectJsonSaveLoad
{
	[SerializeField] private InteractionObjectNote _noteObject;
	private Collider _triggerZone;
	private bool _wasHintMessageShown;
	private GameObject _playerCollider;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController; 

	private void Awake()
	{
_pauseSubMenuSettingsSectionGeneralController = ServiceLocator.Resolve<PauseSubMenuSettingsSectionGeneralController>();
_playerCollider = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCollider);

		_triggerZone = GetComponent<Collider>();

		_triggerZone.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_pauseSubMenuSettingsSectionGeneralController.AreIngameTutorialsEnabled)
		{
			if (_wasHintMessageShown == false)
			{
				if (other.gameObject == _playerCollider)
				{
					Debug.Log("SHOW HINT!");

					_noteObject.Interact();
				}
			}
		}

		_wasHintMessageShown = true;
		_triggerZone.enabled = false;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.HintMessagesData == null)
		{
			data.HintMessagesData = new Dictionary<GameScenesGameplayDataEnum, List<HintMessageData>>();
		}
		if (!data.HintMessagesData.ContainsKey(currentScene))
		{
			data.HintMessagesData[currentScene] = new List<HintMessageData>();
		}

		var targetList = data.HintMessagesData[currentScene];

		int indexInList = targetList.FindIndex(item => item.HintMessageIndex == GameplayObjectIndex);

		var updatedItem = new HintMessageData
		{
			HintMessageIndex = GameplayObjectIndex,
			HintMessageSystem = _noteObject.name,
			WasHintMessageShown = _wasHintMessageShown
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
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.HintMessagesData == null || !data.HintMessagesData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.HintMessageIndex == GameplayObjectIndex);

		if (savedState.Equals(default(HintMessageData))) yield break;

		_wasHintMessageShown = savedState.WasHintMessageShown;

		yield return null;
	}
}