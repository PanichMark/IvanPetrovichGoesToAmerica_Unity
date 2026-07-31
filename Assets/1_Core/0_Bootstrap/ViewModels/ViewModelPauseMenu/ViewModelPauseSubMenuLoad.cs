using UnityEngine;
using TMPro;

public class ViewModelPauseSubMenuLoad
{
	private int _numberOfSlots = 5;

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
		ButtonsLoadGameFile = new GameObject[_numberOfSlots];
		TextGameFileMissionName = new GameObject[_numberOfSlots];
		TextGameFileSceneName = new GameObject[_numberOfSlots];
		TextGameFileDateAndTime = new GameObject[_numberOfSlots];
		ImageSceneGameFile = new GameObject[_numberOfSlots];

		TextGameFileSlotNumber = new TextMeshProUGUI[_numberOfSlots];

		TextPauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "TextPauseSubMenuLoad");

		for (int i = 0; i < 5; i++)
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