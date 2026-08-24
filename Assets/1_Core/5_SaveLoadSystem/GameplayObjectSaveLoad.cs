using System.Collections;
using UnityEngine;

public class GameplayObjectSaveLoad : MonoBehaviour, ISaveLoad
{
	public int GameplayObjectIndex { get; private set; }

	public IEnumerator LoadData(GameData data)
	{
		yield return null;
	}

	public IEnumerator SaveData(GameData data)
	{
		yield return null;
	}

	public void AssignGameplayObjectIndex(int index)
	{
		GameplayObjectIndex = index;
	}
}