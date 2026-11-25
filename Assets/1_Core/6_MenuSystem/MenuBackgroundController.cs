using UnityEngine;

public class MenuBackgroundController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasMenuBackground;

	public void Initialize(
		MenuManager menuManager,
		GameObject canvasMenuBackground)
	{
		_menuManager = menuManager;
		_canvasMenuBackground = canvasMenuBackground;

		_menuManager.OnOpenMenuBackground += ShowCanvasMenuBackground;
		_menuManager.OnCloseMenuBackground += HideCanvasMenuBackground;
	}

	public void ShowCanvasMenuBackground()
	{
		_canvasMenuBackground.SetActive(true);

		Debug.Log("Show MenuBackground");
	}

	public void HideCanvasMenuBackground()
	{
		_canvasMenuBackground.SetActive(false);

		Debug.Log("Hide MenuBackground");
	}
}
