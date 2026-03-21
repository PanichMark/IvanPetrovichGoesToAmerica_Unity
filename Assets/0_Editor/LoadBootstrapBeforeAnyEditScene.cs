#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class LoadBootstrapBeforeAnyEditScene
{
	static LoadBootstrapBeforeAnyEditScene()
	{
		EditorApplication.playModeStateChanged += HandlePlayModeChange;
	}

	static void HandlePlayModeChange(PlayModeStateChange state)
	{
		/*
		if (state == PlayModeStateChange.ExitingEditMode)
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); // Сохраняем текущую сцену, если нужно
		}
		*/

		if (state == PlayModeStateChange.EnteredPlayMode)
		{
			// Первая загрузка сцены Bootstrap (индекс 0)
			EditorSceneManager.LoadScene(0);

			
		}
	}
}
#endif