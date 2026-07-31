using TMPro;
using UnityEngine;

public class ViewModelPauseSubMenuSave
{
	public GameObject TextPauseSubMenuSave;

	public GameObject ButtonCreateNewGameFile;
	public GameObject TextButtonCreateNewGameFile;

	public GameObject[] ContainersSaveGameFile;

	public GameObject[] ButtonsRewriteGameFile;
	public GameObject[] TextGameFileMissionName;
	public GameObject[] TextGameFileSceneName;
	public GameObject[] TextGameFileDateAndTime;
	public GameObject[] ImageSceneGameFile;

	public TextMeshProUGUI[] TextGameFileSlotNumber;

	public GameObject[] ButtonsDeleteGameFile;
	public GameObject[] TextButtonsDeleteGameFile;

	public GameObject ButtonClosePauseSubMenuSave;
	public GameObject TextButtonClosePauseSubMenuSave;

	public ViewModelPauseSubMenuSave(Bootstrap bootstrap, GameObject canvas)
	{
		ContainersSaveGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];

		ButtonsRewriteGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileSceneName = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileDateAndTime = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileMissionName = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		ImageSceneGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];

		TextGameFileSlotNumber = new TextMeshProUGUI[bootstrap.GameData.NumberOfSafeFileSlots];

		ButtonsDeleteGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextButtonsDeleteGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];

		TextPauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "TextPauseSubMenuSave");

		ButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "ButtonCreateNewGameFile");
		TextButtonCreateNewGameFile = bootstrap.FindDeepGameObject(canvas, "TextCreateNewGameFile");

		string textMissionNameGameFile = "TextMissionName";
		string textSceneNameGameFile = "TextSceneName";
		string textDateAndTimeGameFile = "TextDateAndTime";
		string imageSceneGameFile = "ImageSceneGameFile";

		string textButtonDeleteGameFileName = "TextButtonDeleteGameFile";

		for (int i = 0; i < bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			string containerSaveGameFile = "ButtonSave" + (i + 1);

			string buttonRewriteGameFile = "ButtonSaveGameFile" + (i + 1);
			string buttonDeleteGameFileName = "ButtonDeleteGameFile" + (i + 1);

			ContainersSaveGameFile[i] = bootstrap.FindDeepGameObject(canvas, containerSaveGameFile);

			ButtonsRewriteGameFile[i] = bootstrap.FindDeepGameObject(canvas, buttonRewriteGameFile);
			TextGameFileSceneName[i] = bootstrap.FindDeepGameObject(ButtonsRewriteGameFile[i], textSceneNameGameFile);
			TextGameFileDateAndTime[i] = bootstrap.FindDeepGameObject(ButtonsRewriteGameFile[i], textDateAndTimeGameFile);
			TextGameFileMissionName[i] = bootstrap.FindDeepGameObject(ButtonsRewriteGameFile[i], textMissionNameGameFile);
			ImageSceneGameFile[i] = bootstrap.FindDeepGameObject(ButtonsRewriteGameFile[i], imageSceneGameFile);

			ButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(canvas, buttonDeleteGameFileName);
			TextButtonsDeleteGameFile[i] = bootstrap.FindDeepGameObject(ButtonsDeleteGameFile[i], textButtonDeleteGameFileName);

			TextGameFileSlotNumber[i] = bootstrap.FindDeepGameObject(ButtonsRewriteGameFile[i], "TextSlotNumber").GetComponent<TextMeshProUGUI>();
			TextGameFileSlotNumber[i].text = $"{i + 1}";
		}

		ButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSave");
		TextButtonClosePauseSubMenuSave = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuSave");
	}
}