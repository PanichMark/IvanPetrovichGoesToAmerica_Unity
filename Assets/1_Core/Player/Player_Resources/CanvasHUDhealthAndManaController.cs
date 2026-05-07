using UnityEngine;


public class CanvasHUDhealthAndManaController : MonoBehaviour
{
    private MenuManager menuManager;
    private GameObject canvasHUDhealthAndMana;
    private GameSceneManager gameSceneManager;
    private GameController gameController;

    public void Initialize (GameSceneManager gameSceneManager, GameController gameController, MenuManager menuManager, GameObject canvasHUDPlayerResources)
    {
        this.gameSceneManager = gameSceneManager;
        this.menuManager = menuManager;
        this.canvasHUDhealthAndMana = canvasHUDPlayerResources;

		this.menuManager.OnOpenPauseMenu += HideCanvasHUDhealthAndMana;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDhealthAndMana;
		this.menuManager.OnOpenInteractionMenu += HideCanvasHUDhealthAndMana;
		this.menuManager.OnCloseInteractionMenu += ShowCanvasHUDhealthAndMana;
		this.menuManager.OnOpenDialogueMenu += HideCanvasHUDhealthAndMana;
		this.menuManager.OnCloseDialogueMenu += ShowCanvasHUDhealthAndMana;
		this.menuManager.OnOpenCutsceneMenu += HideCanvasHUDhealthAndMana;
		this.menuManager.OnCloseCutsceneMenu += ShowCanvasHUDhealthAndMana;
		Debug.Log("CanvasHUDhealthAndMana Initialized");
        this.gameController = gameController;
        this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasHUDhealthAndMana;
		this.gameSceneManager.OnBeginLoadGameplayScene += ShowCanvasHUDhealthAndMana;
	}
    private void ShowCanvasHUDhealthAndMana()
    {
       if (!menuManager.IsInteractionMenuOpened && !menuManager.IsDialogueMenuOpened && !gameController.IsMainMenuOpen)
       {

           canvasHUDhealthAndMana.SetActive(true);
           Debug.Log("Show canvasHUDhealthAndMana");
       }
    }
	public void HideCanvasHUDhealthAndMana()
	{
		canvasHUDhealthAndMana.SetActive(false);
		Debug.Log("Hide canvasHUDhealthAndMana");
	}
}
