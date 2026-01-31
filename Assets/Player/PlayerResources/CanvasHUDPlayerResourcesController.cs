using UnityEngine;


public class CanvasHUDPlayerResourcesController : MonoBehaviour
{
    private MenuManager menuManager;
    private GameObject canvasHUDPlayerResources;

    public void Initialize (MenuManager menuManager, GameObject canvasHUDPlayerResources)
    {
        this.menuManager = menuManager;
        this.canvasHUDPlayerResources = canvasHUDPlayerResources;

		this.menuManager.OnOpenPauseMenu += HideCanvasHUDPlayerResources;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDPlayerResources;
		this.menuManager.OnOpenInteractionMenu += HideCanvasHUDPlayerResources;
		this.menuManager.OnCloseInteractionMenu += ShowCanvasHUDPlayerResources;
        Debug.Log("CanvasHUDPlayerResourcesController Initialized");
	}
    private void ShowCanvasHUDPlayerResources()
    {
       //if (!menuManager.IsInteractionMenuOpened)
      // {

        canvasHUDPlayerResources.SetActive(true);
        Debug.Log("Show canvasHUDPlayerResources");
       // }
    }
	private void HideCanvasHUDPlayerResources()
	{
		canvasHUDPlayerResources.SetActive(false);
		Debug.Log("Hide canvasHUDPlayerResources");
	}
}
