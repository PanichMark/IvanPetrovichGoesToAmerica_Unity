using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuSave : IViewModel
{
	public Button ButtonCreateNewGameFile;
	public Button[] ButtonsRewriteGameFile = new Button[5];
	public Button[] ButtonsDeleteGameFile = new Button[5];
	public Button ButtonClosePauseSubMenuSave;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "ButtonSaveNewGameFile").GetComponent<Button>();

		for (int i = 0; i < 5; i++)
		{
			string rewriteName = "ButtonRewriteGameFile" + (i + 1);
			string deleteName = "ButtonDeleteGameFile" + (i + 1);

			ButtonsRewriteGameFile[i] = bootstrap.FindDeepGameObject(canvas, rewriteName).GetComponent<Button>();
			ButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(canvas, deleteName).GetComponent<Button>();
		}

		ButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSave").GetComponent<Button>();
	}
}