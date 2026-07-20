using UnityEngine;

public class CutsceneMenuController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasCutscene;
	private GameScenesManager _gameSceneManager;

	public void Initialize(GameScenesManager gameSceneManager, MenuManager menuManager, GameObject canvasCutscene)
	{
		_menuManager = menuManager;

		_canvasCutscene = canvasCutscene;
		_gameSceneManager = gameSceneManager;

		_menuManager.OnOpenCutsceneMenu += ShowCanvasCutscene;
		_menuManager.OnCloseCutsceneMenu += HideCanvasCutscene;

		_menuManager.OnOpenPauseMenu += HideCanvasCutscene;
		_menuManager.OnClosePauseMenu += ShowCanvasCutscene;

		_gameSceneManager.OnBeginLoadingGameplayScene += HideCanvasCutscene;
		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasCutscene;

		Debug.Log("CutsceneMenuController Initialized");
	}

	private void ShowCanvasCutscene()
	{
		if (_menuManager.IsCutsceneMenuOpened)
		{
			_canvasCutscene.SetActive(true);
			Debug.Log("Show CutsceneMenu");
		}
	}

	private void HideCanvasCutscene()
	{
		if (_menuManager.IsCutsceneMenuOpened)
		{
			_canvasCutscene.SetActive(false);
			Debug.Log("Hide CutsceneMenu");
		}
	}
}
