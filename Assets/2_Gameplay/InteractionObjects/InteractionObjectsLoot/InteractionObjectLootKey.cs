using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectLootKey : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectKeyData _keyData;
	private KeysManager _keysManager;
	private string _keyID;

	protected override void InitializeLootObject()
	{
		_keysManager = ServiceLocator.Resolve<KeysManager>("KeysManager");
		_keyID = _keyData.keyID.ToString();
	}

	public override void Interact()
	{
		base.Interact();

		_keysManager.AddKey(_keyID);
		Debug.Log($"Added key: {_keyID}");
	}

	public override void LoadData(GameData data)
	{
		_keysManager.RemoveKey(_keyID);

		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.LootObjectsData == null || !data.LootObjectsData.TryGetValue(currentScene, out var sourceList)) return;

		if (sourceList.Count > 0)
		{
			LootObjectData savedState = sourceList.Find(item => item.LootObjectIndex == LootObjectIndex);

			if (savedState.LootObjectIndex != 0 && savedState.IsLootObjectCollected)
			{
				WasLootItemCollected = true;

				_keysManager.AddKey(_keyID);

				Destroy(gameObject);
			}
		}
	}
}