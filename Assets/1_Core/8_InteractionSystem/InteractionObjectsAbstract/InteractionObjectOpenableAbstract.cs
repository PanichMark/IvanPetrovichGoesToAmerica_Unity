using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class InteractionObjectOpenableAbstract : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI => null;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public bool WasOpenableUnlocked { get; protected set; }
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	[SerializeField] protected bool _isObjectOpened;
	public virtual bool IsObjectOpened => _isObjectOpened;


	public event IInteractable.InteractableObjectHandler OnInteract;
	public abstract void Interact();

	public abstract void InteractCutscene();

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableObjectsData == null)
		{
			data.OpenableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<OpenableObjectData>>();
		}
		if (!data.OpenableObjectsData.ContainsKey(currentScene))
		{
			data.OpenableObjectsData[currentScene] = new List<OpenableObjectData>();
		}

		var targetList = data.OpenableObjectsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.OpenableObjectIndex == GameplayObjectIndex);

		if (indexInList != -1)
		{
			var existingItem = targetList[indexInList];

			existingItem.IsOpenableObjectUnlocked = WasOpenableUnlocked;
			existingItem.IsOpenableObjectOpened = _isObjectOpened;
			existingItem.OpenableObjectNameSystem = InteractionObjectNameSystem;

			targetList[indexInList] = existingItem;
		}
		else
		{
			targetList.Add(new OpenableObjectData
			{
				OpenableObjectIndex = GameplayObjectIndex,
				OpenableObjectNameSystem = InteractionObjectNameSystem,
				IsOpenableObjectUnlocked = WasOpenableUnlocked,
				IsOpenableObjectOpened = _isObjectOpened
			});
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableObjectsData == null || !data.OpenableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.OpenableObjectIndex == GameplayObjectIndex);

		if (savedState.Equals(default(OpenableObjectData))) yield break;

		WasOpenableUnlocked = savedState.IsOpenableObjectUnlocked;
		_isObjectOpened = savedState.IsOpenableObjectOpened;

		yield return null;
	}
}