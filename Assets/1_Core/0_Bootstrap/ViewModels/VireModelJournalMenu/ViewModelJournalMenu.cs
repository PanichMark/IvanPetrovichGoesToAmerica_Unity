using UnityEngine;

public class ViewModelJournalMenu
{
	public GameObject ButtonCloseJournalMenu;
	public GameObject TextButtonCloseJournalMenu;

	public GameObject JournalSectionMissionInfo;
	public GameObject ButtonJournalSectionMissionInfo;
	public GameObject TextButtonJournalSectionMissionInfo;

	public GameObject JournalSectionSkillTree;
	public GameObject ButtonJournalSectionSkillTree;
	public GameObject TextButtonJournalSectionSkillTree;

	public GameObject JournalSectionInventory;
	public GameObject ButtonJournalSectionInventory;
	public GameObject TextButtonJournalSectionInventory;

	public GameObject JournalSectionStatistics;
	public GameObject ButtonJournalSectionStatistics;
	public GameObject TextButtonJournalSectionStatistics;

	public ViewModelJournalMenu(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseJournalMenu = bootstrap.FindDeepGameObject(canvas, "ButtonCloseJournalMenu");
		TextButtonCloseJournalMenu = bootstrap.FindDeepGameObject(canvas, "TextButtonCloseJournalMenu");

		JournalSectionMissionInfo = bootstrap.FindDeepGameObject(canvas, "JournalSubSectionMissionInfo");
		ButtonJournalSectionMissionInfo = bootstrap.FindDeepGameObject(canvas, "ButtonJournalSubSectionMissionInfo");
		TextButtonJournalSectionMissionInfo = bootstrap.FindDeepGameObject(canvas, "TextButtonJournalSubSectionMissionInfo");

		JournalSectionSkillTree = bootstrap.FindDeepGameObject(canvas, "JournalSubSectionMissionSkillTree");
		ButtonJournalSectionSkillTree = bootstrap.FindDeepGameObject(canvas, "ButtonJournalSubSectionMissionSkillTree");
		TextButtonJournalSectionSkillTree = bootstrap.FindDeepGameObject(canvas, "TextButtonJournalSubSectionMissionSkillTree");

		JournalSectionInventory = bootstrap.FindDeepGameObject(canvas, "JournalSubSectionMissionInventory");
		ButtonJournalSectionInventory = bootstrap.FindDeepGameObject(canvas, "ButtonJournalSubSectionMissionInventory");
		TextButtonJournalSectionInventory = bootstrap.FindDeepGameObject(canvas, "TextButtonJournalSubSectionMissionInventory");

		JournalSectionStatistics = bootstrap.FindDeepGameObject(canvas, "JournalSubSectionMissionStatistics");
		ButtonJournalSectionStatistics = bootstrap.FindDeepGameObject(canvas, "ButtonJournalSubSectionMissionStatistics");
		TextButtonJournalSectionStatistics = bootstrap.FindDeepGameObject(canvas, "TextButtonJournalSubSectionMissionStatistics");
	}
}