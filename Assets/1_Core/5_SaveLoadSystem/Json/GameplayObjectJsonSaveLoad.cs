using System.Collections;
using UnityEngine;

public abstract class GameplayObjectJsonSaveLoad : MonoBehaviour, IJsonSaveLoad
{
	public int GameplayObjectIndex { get; protected set; }

	public void AssignGameplayObjectIndex(int index)
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
}