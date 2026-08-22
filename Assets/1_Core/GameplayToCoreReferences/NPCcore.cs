using UnityEngine;

public class NPCcore : MonoBehaviour, ISaveLoad
{
	public int NPCindex { get; protected set; }

	public void AssignNPCsIndexes(int index)
	{
		NPCindex = index;
	}

	public virtual void LoadData(GameData data)
	{
	}

	public virtual void SaveData(ref GameData data)
	{
	}
}
