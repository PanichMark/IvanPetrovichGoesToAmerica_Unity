using UnityEngine;


public class CanvasHUDPlayerResourcesController : MonoBehaviour
{
    private MenuManager menuManager;
    private GameObject canvasHUDPlayerResources;
    private GameSceneManager gameSceneManager;
    private GameController gameController;

    public void Initialize (GameSceneManager gameSceneManager, GameController gameController, MenuManager menuManager, GameObject canvasHUDPlayerResources)
    {
        this.gameSceneManager = gameSceneManager;
        this.menuManager = menuManager;
        this.canvasHUDPlayerResources = canvasHUDPlayerResources;

		this.menuManager.OnOpenPauseMenu += HideCanvasHUDPlayerResources;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDPlayerResources;
		this.menuManager.OnOpenReadNoteMenu += HideCanvasHUDPlayerResources;
		this.menuManager.OnCloseReadNoteMenu += ShowCanvasHUDPlayerResources;
		this.menuManager.OnOpenLockpickMenu += HideCanvasHUDPlayerResources;
		this.menuManager.OnCloseLockpickMenu += ShowCanvasHUDPlayerResources;
		Debug.Log("CanvasHUDPlayerResourcesController Initialized");
        this.gameController = gameController;
        this.gameSceneManager.OnLoadMainMenuScene += HideCanvasHUDPlayerResources;
		this.gameSceneManager.OnLoadGameplayScene += ShowCanvasHUDPlayerResources;
	}
    private void ShowCanvasHUDPlayerResources()
    {
       if (!menuManager.IsReadNoteMenuOpened && !menuManager.IsLockpickMenuOpened && !gameController.IsMainMenuOpen)
       {

           canvasHUDPlayerResources.SetActive(true);
           Debug.Log("Show canvasHUDPlayerResources");
       }
    }
	public void HideCanvasHUDPlayerResources()
	{
		canvasHUDPlayerResources.SetActive(false);
		Debug.Log("Hide canvasHUDPlayerResources");
	}
}
