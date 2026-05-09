#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoadAttribute]
public static class LoadBootstrapSceneBeforeAnyEditScene
{
	static LoadBootstrapSceneBeforeAnyEditScene()
	{
		EditorApplication.playModeStateChanged += HandlePlayModeChange;
	}

	static void HandlePlayModeChange(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredPlayMode)
		{
			EditorSceneManager.LoadScene(0);
		}
	}
}
#endif