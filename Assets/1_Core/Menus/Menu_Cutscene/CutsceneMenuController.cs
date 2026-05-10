using UnityEngine;

public class CutsceneMenuController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasCutscene;
	private GameSceneManager _gameSceneManager;

	public void Initialize(MenuManager menuManager, GameSceneManager gameSceneManager, GameObject canvasCutscene)
	{
		_menuManager = menuManager;

		_canvasCutscene = canvasCutscene;
		_gameSceneManager = gameSceneManager;

		_menuManager.OnOpenCutsceneMenu += ShowCanvasCutscene;
		_menuManager.OnCloseCutsceneMenu += HideCanvasCutscene;

		_menuManager.OnOpenPauseMenu += HideCanvasCutscene;
		_menuManager.OnClosePauseMenu += ShowCanvasCutscene;

		_gameSceneManager.OnBeginLoadGameplayScene += HideCanvasCutscene;
		_gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasCutscene;

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
