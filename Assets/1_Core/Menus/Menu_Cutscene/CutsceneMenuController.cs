using NUnit;
using UnityEngine;

public class CutsceneMenuController : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private MenuManager menuManager;
	private GameObject canvasCutscene;
	private GameSceneManager gameSceneManager;
	public void Initialize(MenuManager menuManager, GameSceneManager gameSceneManager, GameObject canvasCutscene)
	{
		this.menuManager = menuManager;

		this.canvasCutscene = canvasCutscene;
		this.gameSceneManager = gameSceneManager;

		this.menuManager.OnOpenCutsceneMenu += ShowCanvasCutscene;
		this.menuManager.OnCloseCutsceneMenu += HideCanvasCutscene;

		this.menuManager.OnOpenPauseMenu += HideCanvasCutscene;
		this.menuManager.OnClosePauseMenu += ShowCanvasCutscene;

		this.gameSceneManager.OnBeginLoadGameplayScene += HideCanvasCutscene;
		this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasCutscene;
	}

	private void ShowCanvasCutscene()
	{
		//Debug.Log("SHOWHSOWW!!!");
		if (menuManager.IsCutsceneMenuOpened)
		{

			canvasCutscene.SetActive(true);
			Debug.Log("Show CutsceneMenu");
		}
	}

	private void HideCanvasCutscene()
	{
		//Debug.Log("BRUHH!!!!");
		if (menuManager.IsCutsceneMenuOpened)
		{
			canvasCutscene.SetActive(false);
			Debug.Log("Hide CutsceneMenu");
		}
	}
}
