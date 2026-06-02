using UnityEngine;

public class HUDhealthAndManaController : MonoBehaviour
{
    private MenuManager _menuManager;
    private GameObject _canvasHUDhealthAndMana;
    private GameSceneManager _gameSceneManager;
    private GameController _gameController;

    private GameObject _healthBar;
	private GameObject _manaBar;

	public void Initialize (
		GameController gameController,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		GameObject canvasHUDPlayerResources,
		ViewModelHUDHealthAndMana viewModelHUDHealthAndMana)
    {
        _gameSceneManager = gameSceneManager;
        _menuManager = menuManager;
        _canvasHUDhealthAndMana = canvasHUDPlayerResources;
        _healthBar = viewModelHUDHealthAndMana.HealthBar;
        _manaBar = viewModelHUDHealthAndMana.ManaBar;

		HideHealthBar();
		HideManaBar();

		_menuManager.OnOpenPauseMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenCutsceneMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseCutsceneMenu += ShowCanvasHUDhealthAndMana;
        _gameController = gameController;
        _gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDhealthAndMana;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDhealthAndMana;
        _gameController.OnPlayerEarlyDeath += HideCanvasHUDhealthAndMana;

		Debug.Log("CanvasHUDhealthAndManaController Initialized");
	}

    private void ShowCanvasHUDhealthAndMana()
    {
       if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_menuManager.IsCutsceneMenuOpened && !_gameController.IsMainMenuOpen && !_menuManager.IsMainMenuBeingLoaded)
       {

           _canvasHUDhealthAndMana.SetActive(true);
           Debug.Log("Show canvasHUDhealthAndMana");
       }
    }

	public void HideCanvasHUDhealthAndMana()
	{
		_canvasHUDhealthAndMana.SetActive(false);
		Debug.Log("Hide canvasHUDhealthAndMana");
	}

    public void ShowHealthBar()
    {
		_healthBar.SetActive(true);
    }

	public void HideHealthBar()
	{
		_healthBar.SetActive(false);
	}

	public void ShowManaBar()
	{
		_manaBar.SetActive(true);
	}

	public void HideManaBar()
	{
		_manaBar.SetActive(false);
	}
}
