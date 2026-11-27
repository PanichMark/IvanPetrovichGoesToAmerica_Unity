using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalMenuController : MonoBehaviour
{
	private GameObject _canvasJournalMenu;
	private ViewModelJournalMenu _viewModelJournalMenu;
	private MenuManager _menuManager;

	public delegate void JournalMenuSectionEventHandler();
	public event JournalMenuSectionEventHandler OnOpenJournaMenuSectionMissionInfo;
	public event JournalMenuSectionEventHandler OnOpenJournaMenuSectionInventory;

	public void ShowJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(true);
	}
	public void HideJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(false);
	}

	private LocalizationManager _localizationManager;

	private GameObject _buttonCloseJournalMenu;
	private Button _buttonComponentCloseJournalMenu;
	private GameObject _textButtonCloseJournalMenu;
	private TextMeshProUGUI _textComponentButtonCloseJournalMenu;

	private GameObject _journalSectionMissionInfo;
	private GameObject _buttonJournalSectionMissionInfo;
	private Button _buttonComponentJournalSectionMissionInfo;
	private GameObject _textButtonJournalSectionMissionInfo;
	private TextMeshProUGUI _textComponentButtonJournalSectionMissionInfo;

	private GameObject _journalSectionSkillTree;
	private GameObject _buttonJournalSectionSkillTree;
	private Button _buttonComponentJournalSectionSkillTree;
	private GameObject _textButtonJournalSectionSkillTree;
	private TextMeshProUGUI _textComponentButtonJournalSectionSkillTree;

	private GameObject _journalSectionInventory;
	private GameObject _buttonJournalSectionInventory;
	private Button _buttonComponentJournalSectionInventory;
	private GameObject _textButtonJournalSectionInventory;
	private TextMeshProUGUI _textComponentButtonJournalSectionInventory;

	private GameObject _journalSectionStatistics;
	private GameObject _buttonJournalSectionStatistics;
	private Button _buttonComponentJournalSectionStatistics;
	private GameObject _textButtonJournalSectionStatistics;
	private TextMeshProUGUI _textComponentButtonJournalSectionStatistics;

	private string _currentOpenedJournalSubSection;
	public void Initialize(
		LocalizationManager localizationManager,
		MenuManager menuManager,
		GameObject canvasJournalMenu,
		ViewModelJournalMenu viewModelJournalMenu)
	{
		_localizationManager = localizationManager;
		_menuManager = menuManager;
		_canvasJournalMenu = canvasJournalMenu;
		_viewModelJournalMenu = viewModelJournalMenu;

		_buttonCloseJournalMenu = viewModelJournalMenu.ButtonCloseJournalMenu;
		_buttonComponentCloseJournalMenu = viewModelJournalMenu.ButtonCloseJournalMenu.GetComponent<Button>();
		_buttonComponentCloseJournalMenu.onClick.AddListener(() => _menuManager.CloseJournalMenu());
		_textButtonCloseJournalMenu = viewModelJournalMenu.TextButtonCloseJournalMenu;
		_textComponentButtonCloseJournalMenu = viewModelJournalMenu.TextButtonCloseJournalMenu.GetComponent<TextMeshProUGUI>();

		_journalSectionMissionInfo = viewModelJournalMenu.JournalSectionMissionInfo;
		_buttonJournalSectionMissionInfo = viewModelJournalMenu.ButtonJournalSectionMissionInfo;
		_buttonComponentJournalSectionMissionInfo = viewModelJournalMenu.ButtonJournalSectionMissionInfo.GetComponent<Button>();
		_buttonComponentJournalSectionMissionInfo.onClick.AddListener(() => OpenJournalSubSection(_journalSectionMissionInfo));
		_textButtonJournalSectionMissionInfo = viewModelJournalMenu.TextButtonJournalSectionMissionInfo;
		_textComponentButtonJournalSectionMissionInfo = viewModelJournalMenu.TextButtonJournalSectionMissionInfo.GetComponent<TextMeshProUGUI>();

		_journalSectionSkillTree = viewModelJournalMenu.JournalSectionSkillTree;
		_buttonJournalSectionSkillTree = viewModelJournalMenu.ButtonJournalSectionSkillTree;
		_buttonComponentJournalSectionSkillTree = viewModelJournalMenu.ButtonJournalSectionSkillTree.GetComponent<Button>();
		_buttonComponentJournalSectionSkillTree.onClick.AddListener(() => OpenJournalSubSection(_journalSectionSkillTree));
		_textButtonJournalSectionSkillTree = viewModelJournalMenu.TextButtonJournalSectionSkillTree;
		_textComponentButtonJournalSectionSkillTree = viewModelJournalMenu.TextButtonJournalSectionSkillTree.GetComponent<TextMeshProUGUI>();

		_journalSectionInventory = viewModelJournalMenu.JournalSectionInventory;
		_buttonJournalSectionInventory = viewModelJournalMenu.ButtonJournalSectionInventory;
		_buttonComponentJournalSectionInventory = viewModelJournalMenu.ButtonJournalSectionInventory.GetComponent<Button>();
		_buttonComponentJournalSectionInventory.onClick.AddListener(() => OpenJournalSubSection(_journalSectionInventory));
		_textButtonJournalSectionInventory = viewModelJournalMenu.TextButtonJournalSectionInventory;
		_textComponentButtonJournalSectionInventory = viewModelJournalMenu.TextButtonJournalSectionInventory.GetComponent<TextMeshProUGUI>();

		_journalSectionStatistics = viewModelJournalMenu.JournalSectionStatistics;
		_buttonJournalSectionStatistics = viewModelJournalMenu.ButtonJournalSectionStatistics;
		_buttonComponentJournalSectionStatistics = viewModelJournalMenu.ButtonJournalSectionStatistics.GetComponent<Button>();
		_buttonComponentJournalSectionStatistics.onClick.AddListener(() => OpenJournalSubSection(_journalSectionStatistics));
		_textButtonJournalSectionStatistics = viewModelJournalMenu.TextButtonJournalSectionStatistics;
		_textComponentButtonJournalSectionStatistics = viewModelJournalMenu.TextButtonJournalSectionStatistics.GetComponent<TextMeshProUGUI>();

		_menuManager.OnOpenJournalMenu += () => OpenJournalSubSection(_journalSectionMissionInfo);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_menuManager.OnOpenJournalMenu += ShowJournalMenu;
		_menuManager.OnCloseJournalMenu += HideJournalMenu;

		Debug.Log("MenuJournalController Initialized");
	}

	private void OpenJournalSubSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(true);

		if (subSettingsSection == _journalSectionMissionInfo)
		{
			OnOpenJournaMenuSectionMissionInfo?.Invoke();

			CloseJournalSubSection(_journalSectionSkillTree);
			CloseJournalSubSection(_journalSectionInventory);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionMissionInfo.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.MissionInfo.ToString();
		}
		if (subSettingsSection == _journalSectionSkillTree)
		{
			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionInventory);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionSkillTree.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.SkillTree.ToString();
		}
		if (subSettingsSection == _journalSectionInventory)
		{
			OnOpenJournaMenuSectionInventory?.Invoke();

			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionSkillTree);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionInventory.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.Inventory.ToString();
		}
		if (subSettingsSection == _journalSectionStatistics)
		{
			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionSkillTree);
			CloseJournalSubSection(_journalSectionInventory);

			_buttonComponentJournalSectionStatistics.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.Statistics.ToString();
		}
	}

	private void CloseJournalSubSection(GameObject subSettingsSection)
	{
		subSettingsSection.SetActive(false);

		if (subSettingsSection == _journalSectionMissionInfo)
		{
			_buttonComponentJournalSectionMissionInfo.interactable = true;
		}
		if (subSettingsSection == _journalSectionSkillTree)
		{
			//_buttonComponentJournalSectionAbilities.interactable = true;
		}
		if (subSettingsSection == _journalSectionInventory)
		{
			_buttonComponentJournalSectionInventory.interactable = true;
		}
		if (subSettingsSection == _journalSectionStatistics)
		{
			//_buttonComponentJournalSectionStatistics.interactable = true;
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentButtonCloseJournalMenu.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonCloseJournalMenu");

		_textComponentButtonJournalSectionMissionInfo.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionMissionInfo");
		_textComponentButtonJournalSectionSkillTree.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionSkillTree");
		_textComponentButtonJournalSectionInventory.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionInventory");
		_textComponentButtonJournalSectionStatistics.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionStatistics");
	}
}