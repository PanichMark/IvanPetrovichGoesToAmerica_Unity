using UnityEngine;


public class CanvasHUDPlayerResourcesController : MonoBehaviour
{
    private MenuManager menuManager;
    private GameObject canvasHUDPlayerResources;
    private GameSceneManager gameSceneManager;

    public void Initialize (GameSceneManager gameSceneManager, MenuManager menuManager, GameObject canvasHUDPlayerResources)
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

        this.gameSceneManager.OnLoadMainMenuScene += HideCanvasHUDPlayerResources;
		this.gameSceneManager.OnLoadGameplayScene += ShowCanvasHUDPlayerResources;
	}
    private void ShowCanvasHUDPlayerResources()
    {
       if (!menuManager.IsReadNoteMenuOpened && !menuManager.IsLockpickMenuOpened)
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
