using UnityEngine;

public class HUDmissionsController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasHUDmissions;
	private GameSceneManager _gameSceneManager;
	private GameController _gameController;

	public void Initialize(
		GameController gameController,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		GameObject canvasHUDmissions)
	{
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_canvasHUDmissions = canvasHUDmissions;

		_menuManager.OnOpenPauseMenu += HideCanvasHUDmissions;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenCutsceneMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseCutsceneMenu += ShowCanvasHUDmissions;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDmissions;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDmissions;

		_gameController.OnPlayerEarlyDeath += HideCanvasHUDmissions;
	}

	private void ShowCanvasHUDmissions()
	{
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOpen && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded)
		{
			_canvasHUDmissions.SetActive(true);

			Debug.Log("Show canvasMissions");
		}
	}

	private void HideCanvasHUDmissions()
	{
		_canvasHUDmissions.SetActive(false);

		Debug.Log("Hide canvasMissions");
	}
}
