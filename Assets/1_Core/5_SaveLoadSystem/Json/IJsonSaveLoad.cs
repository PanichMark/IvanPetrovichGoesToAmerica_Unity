using System.Collections;

public interface IJsonSaveLoad
{
	IEnumerator SaveJsonData(JsonGameData data);
	IEnumerator LoadJsonData(JsonGameData data);
}