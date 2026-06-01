using UnityEngine;

public class ViewModelPauseSubMenuSave
{
	public GameObject TextPauseSubMenuSave;

	public GameObject ButtonCreateNewGameFile;
	public GameObject TextButtonCreateNewGameFile;

	public GameObject[] ButtonsRewriteGameFile = new GameObject[5];
	public GameObject[] TextGameFileDateAndTime = new GameObject[5];
	public GameObject[] TextGameFileSceneName = new GameObject[5];
	public GameObject[] ImageSceneGameFile = new GameObject[5];

	public GameObject[] ButtonsDeleteGameFile = new GameObject[5];
	public GameObject[] TextButtonsDeleteGameFile = new GameObject[5];

	public GameObject ButtonClosePauseSubMenuSave;
	public GameObject TextButtonClosePauseSubMenuSave;

	public ViewModelPauseSubMenuSave(Bootstrap bootstrap, GameObject canvas)
	{
		TextPauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "TextPauseSubMenuSave");

		ButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "ButtonCreateNewGameFile");
		TextButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "TextCreateNewGameFile");

		for (int i = 0; i < ButtonsRewriteGameFile.Length; i++)
		{
			string buttonRewriteGameFile = "ButtonRewriteGameFile" + (i + 1);
			string textDateAndTimeGameFile = "TextDateAndTimeGameFile" + (i + 1);
			string textSceneNameGameFile = "TextSceneNameGameFile" + (i + 1);
			string imageSceneGameFile = "ImageSceneGameFile" + (i + 1);

			string buttonDeleteGameFileName = "ButtonDeleteGameFile" + (i + 1);
			string textButtonDeleteGameFileName = "TextButtonDeleteGameFile" + (i + 1);

			ButtonsRewriteGameFile[i] = bootstrap.FindDeepGameObject(canvas, buttonRewriteGameFile);
			TextGameFileDateAndTime[i] = bootstrap.FindDeepGameObject(canvas, textDateAndTimeGameFile);
			TextGameFileSceneName[i] = bootstrap.FindDeepGameObject(canvas, textSceneNameGameFile);
			ImageSceneGameFile[i] = bootstrap.FindDeepGameObject(canvas, imageSceneGameFile);

			ButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(canvas, buttonDeleteGameFileName);
			TextButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(canvas, textButtonDeleteGameFileName);
		}

		ButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSave");
		TextButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuSave");
	}
}