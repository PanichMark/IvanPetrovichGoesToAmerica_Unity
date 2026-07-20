using UnityEngine;

public class HUDhealthAndManaController : MonoBehaviour
{
    private MenuManager _menuManager;
    private GameObject _canvasHUDhealthAndMana;
    private GameScenesManager _gameSceneManager;
    private GameController _gameController;

	private GameObject _HUDhealthAndManaBars;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private GameObject _healthBar;
	private GameObject _manaBar;

	public void Initialize (
		GameController gameController,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameObject canvasHUDPlayerResources,
		ViewModelHUDHealthAndMana viewModelHUDHealthAndMana)
    {
        _gameSceneManager = gameSceneManager;
        _menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
        _canvasHUDhealthAndMana = canvasHUDPlayerResources;
        _healthBar = viewModelHUDHealthAndMana.HealthBar;
        _manaBar = viewModelHUDHealthAndMana.ManaBar;
		_HUDhealthAndManaBars = viewModelHUDHealthAndMana.HUDhealthAndManaBars;

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

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDhealthAndManaBars;

		Debug.Log("HUDhealthAndManaController Initialized");
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

	private void ShowHUDhealthAndManaBars()
	{
		_HUDhealthAndManaBars.SetActive(true);
	}

	private void HideHUDhealthAndManaBars()
	{
		_HUDhealthAndManaBars.SetActive(false);
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
