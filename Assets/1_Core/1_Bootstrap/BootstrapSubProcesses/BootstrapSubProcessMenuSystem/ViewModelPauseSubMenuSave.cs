using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuSave
{
	public GameObject ButtonCreateNewGameFile;
	public GameObject[] ButtonsRewriteGameFile = new GameObject[5];
	public GameObject[] ButtonsDeleteGameFile = new GameObject[5];
	public GameObject ButtonClosePauseSubMenuSave;

	public ViewModelPauseSubMenuSave(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "ButtonSaveNewGameFile");

		for (int i = 0; i < 5; i++)
		{
			string rewriteName = "ButtonRewriteGameFile" + (i + 1);
			string deleteName = "ButtonDeleteGameFile" + (i + 1);

			ButtonsRewriteGameFile[i] = bootstrap.FindDeepGameObject(canvas, rewriteName);
			ButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(canvas, deleteName);
		}

		ButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSave");
	}
}