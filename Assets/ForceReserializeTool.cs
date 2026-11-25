#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

public static class ForceReserializeTool
{
	[MenuItem("Tools/Force Reserialize All Assets")]
	public static void ForceReserializeAll()
	{
		string[] guids = AssetDatabase.FindAssets("t:ScriptableObject t:Prefab t:Scene", new[] { "Assets" });
		string[] paths = guids.Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();

		AssetDatabase.ForceReserializeAssets(paths);
		AssetDatabase.SaveAssets();

		UnityEngine.Debug.Log($"Успешно пересохранено ассетов: {paths.Length}");
	}
}
#endif
