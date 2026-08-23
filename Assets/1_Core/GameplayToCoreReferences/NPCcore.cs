using UnityEngine;
using System.Collections;

public class NPCcore : MonoBehaviour, ISaveLoad
{
	public int NPCindex { get; protected set; }

	public void AssignNPCsIndexes(int index)
	{
		NPCindex = index;
	}

	public virtual IEnumerator SaveData(GameData data)
	{
		yield return null;
	}

	public virtual IEnumerator LoadData(GameData data)
	{
		yield return null;
	}
}
