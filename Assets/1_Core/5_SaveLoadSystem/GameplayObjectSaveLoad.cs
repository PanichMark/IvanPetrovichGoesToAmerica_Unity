using System.Collections;
using UnityEngine;

public abstract class GameplayObjectSaveLoad : MonoBehaviour, ISaveLoad
{
	public int GameplayObjectIndex { get; protected set; }

	public void AssignGameplayObjectIndex(int index)
	{
		GameplayObjectIndex = index;
	}

	public virtual IEnumerator LoadData(GameData data)
	{
		yield return null;
	}

	public virtual IEnumerator SaveData(GameData data)
	{
		yield return null;
	}
}