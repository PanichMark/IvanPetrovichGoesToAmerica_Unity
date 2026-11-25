using System.Collections;
using UnityEngine;

public abstract class GameplayObjectJsonSaveLoad : MonoBehaviour, IJsonSaveLoad
{
	public int GameplayObjectIndex { get; protected set; }

	public virtual void AssignGameplayObjectIndex(int index)
	{
		GameplayObjectIndex = index;
	}

	public virtual IEnumerator LoadJsonData(JsonGameData data)
	{
		yield return null;
	}

	public virtual IEnumerator SaveJsonData(JsonGameData data)
	{
		yield return null;
	}

	private void Awake()
	{
		bool wasActive = gameObject.activeSelf;

		if (!wasActive)
		{
			gameObject.SetActive(true);
		}

		InitializeGameplayObjectJsonSaveLoad();

		if (!wasActive)
		{
			gameObject.SetActive(false);
		}
	}

	protected abstract void InitializeGameplayObjectJsonSaveLoad();
}