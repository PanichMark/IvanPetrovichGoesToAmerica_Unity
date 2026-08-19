using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public void SaveData(ref GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		// Инициализируем словарь или список для текущей сцены, если их нет
		if (data.OpenableObjectsByScene == null)
		{
			data.OpenableObjectsByScene = new Dictionary<GameScenesGameplayDataEnum, List<OpenableObjectData>>();
		}
		if (!data.OpenableObjectsByScene.ContainsKey(currentScene))
		{
			data.OpenableObjectsByScene[currentScene] = new List<OpenableObjectData>();
		}

		var targetList = data.OpenableObjectsByScene[currentScene];

		int indexInList = targetList.FindIndex(item => item.OpenableObjectIndex == OpenableObjectIndex);

		if (indexInList != -1)
		{
			var existingItem = targetList[indexInList];

			existingItem.WasOpenableObjectUnlocked = WasOpenableUnlocked;
			existingItem.WasOpenableObjectOpened = _isObjectOpened;
			existingItem.OpenableObjectNameSystem = InteractionObjectNameSystem;

			targetList[indexInList] = existingItem;
		}
		else
		{
			targetList.Add(new OpenableObjectData
			{
				OpenableObjectIndex = OpenableObjectIndex,
				OpenableObjectNameSystem = InteractionObjectNameSystem,
				WasOpenableObjectUnlocked = WasOpenableUnlocked,
				WasOpenableObjectOpened = _isObjectOpened
			});
		}
	}

	public void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.OpenableObjectsByScene == null || !data.OpenableObjectsByScene.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.OpenableObjectIndex == OpenableObjectIndex);

		if (savedState.Equals(default(OpenableObjectData))) return;

		WasOpenableUnlocked = savedState.WasOpenableObjectUnlocked;
		_isObjectOpened = savedState.WasOpenableObjectOpened;
	}
}