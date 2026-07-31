using UnityEngine;
using TMPro;

public class ViewModelPauseSubMenuLoad
{
	public GameObject TextPauseSubMenuLoad;

	public GameObject[] ButtonsLoadGameFile;
	public GameObject[] TextGameFileMissionName;
	public GameObject[] TextGameFileSceneName;
	public GameObject[] TextGameFileDateAndTime;
	public GameObject[] ImageSceneGameFile;
	public TextMeshProUGUI[] TextGameFileSlotNumber;

	public GameObject ButtonClosePauseSubMenuLoad;
	public GameObject TextButtonClosePauseSubMenuLoad;

	public ViewModelPauseSubMenuLoad(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonsLoadGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileMissionName = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileSceneName = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileDateAndTime = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];
		ImageSceneGameFile = new GameObject[bootstrap.GameData.NumberOfSafeFileSlots];

		TextGameFileSlotNumber = new TextMeshProUGUI[bootstrap.GameData.NumberOfSafeFileSlots];

		TextPauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "TextPauseSubMenuLoad");

		for (int i = 0; i < bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			string slotRootName = "ButtonLoadGameFile" + (i + 1);

			ButtonsLoadGameFile[i] = bootstrap.FindDeepGameObject(canvas, $"{slotRootName}");
			Debug.Log(ButtonsLoadGameFile[i]);
			TextGameFileMissionName[i] = bootstrap.FindDeepGameObject(ButtonsLoadGameFile[i], "TextMissionName");
			TextGameFileSceneName[i] = bootstrap.FindDeepGameObject(ButtonsLoadGameFile[i], "TextSceneName");
			TextGameFileDateAndTime[i] = bootstrap.FindDeepGameObject(ButtonsLoadGameFile[i], "TextDateAndTime");
			ImageSceneGameFile[i] = bootstrap.FindDeepGameObject(ButtonsLoadGameFile[i], "ImageSceneGameFile");
			TextGameFileSlotNumber[i] = bootstrap.FindDeepGameObject(ButtonsLoadGameFile[i], "TextSlotNumber").GetComponent<TextMeshProUGUI>();
			TextGameFileSlotNumber[i].text = $"{i + 1}";
		}

		ButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuLoad");
		TextButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuLoad");
	}
}