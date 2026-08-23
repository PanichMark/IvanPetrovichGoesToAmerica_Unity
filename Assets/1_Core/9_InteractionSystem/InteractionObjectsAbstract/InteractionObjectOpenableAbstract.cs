using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable, ISaveLoad
{
	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI => null;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public int OpenableObjectIndex { get; protected set; }
	public bool WasOpenableUnlocked { get; protected set; }
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	[SerializeField] protected bool _isObjectOpened;
	public virtual bool IsObjectOpened => _isObjectOpened;


	public event IInteractable.InteractableObjectHandler OnInteract;
	public void AssignOpenableObjectsIndexes(int index)
	{
		OpenableObjectIndex = index;
	}
	public abstract void Interact();

	public abstract void InteractCutscene();

	public IEnumerator SaveData(GameData data)
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

		int indexInList = targetList.FindIndex(item => item.OpenableObjectIndex == OpenableObjectIndex);

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
				OpenableObjectIndex = OpenableObjectIndex,
				OpenableObjectNameSystem = InteractionObjectNameSystem,
				IsOpenableObjectUnlocked = WasOpenableUnlocked,
				IsOpenableObjectOpened = _isObjectOpened
			});
		}

		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.OpenableObjectsData == null || !data.OpenableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.OpenableObjectIndex == OpenableObjectIndex);

		if (savedState.Equals(default(OpenableObjectData))) yield break;

		WasOpenableUnlocked = savedState.IsOpenableObjectUnlocked;
		_isObjectOpened = savedState.IsOpenableObjectOpened;

		yield return null;
	}
}