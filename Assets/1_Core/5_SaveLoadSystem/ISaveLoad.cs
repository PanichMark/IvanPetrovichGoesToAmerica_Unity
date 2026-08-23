using System.Collections;

public interface ISaveLoad
{
	IEnumerator SaveData(GameData data);
	IEnumerator LoadData(GameData data);
}