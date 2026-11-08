using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalMenuController : MonoBehaviour
{
	private GameObject _canvasJournalMenu;
	private ViewModelJournalMenu _viewModelJournalMenu;
	private MenuManager _menuManager;

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

	private GameObject _journalSectionAbilities;
	private GameObject _buttonJournalSectionAbilities;
	private Button _buttonComponentJournalSectionAbilities;
	private GameObject _textButtonJournalSectionAbilities;
	private TextMeshProUGUI _textComponentButtonJournalSectionAbilities;

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

		_journalSectionAbilities = viewModelJournalMenu.JournalSectionAbilities;
		_buttonJournalSectionAbilities = viewModelJournalMenu.ButtonJournalSectionAbilities;
		_buttonComponentJournalSectionAbilities = viewModelJournalMenu.ButtonJournalSectionAbilities.GetComponent<Button>();
		_buttonComponentJournalSectionAbilities.onClick.AddListener(() => OpenJournalSubSection(_journalSectionAbilities));
		_textButtonJournalSectionAbilities = viewModelJournalMenu.TextButtonJournalSectionAbilities;
		_textComponentButtonJournalSectionAbilities = viewModelJournalMenu.TextButtonJournalSectionAbilities.GetComponent<TextMeshProUGUI>();

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
			CloseJournalSubSection(_journalSectionAbilities);
			CloseJournalSubSection(_journalSectionInventory);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionMissionInfo.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.MissionInfo.ToString();
		}
		if (subSettingsSection == _journalSectionAbilities)
		{
			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionInventory);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionAbilities.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.Abilities.ToString();
		}
		if (subSettingsSection == _journalSectionInventory)
		{
			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionAbilities);
			CloseJournalSubSection(_journalSectionStatistics);

			_buttonComponentJournalSectionInventory.interactable = false;

			_currentOpenedJournalSubSection = JournalMenuSectionTypes.Inventory.ToString();
		}
		if (subSettingsSection == _journalSectionStatistics)
		{
			CloseJournalSubSection(_journalSectionMissionInfo);
			CloseJournalSubSection(_journalSectionAbilities);
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
		if (subSettingsSection == _journalSectionAbilities)
		{
			_buttonComponentJournalSectionAbilities.interactable = true;
		}
		if (subSettingsSection == _journalSectionInventory)
		{
			_buttonComponentJournalSectionInventory.interactable = true;
		}
		if (subSettingsSection == _journalSectionStatistics)
		{
			_buttonComponentJournalSectionStatistics.interactable = true;
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		/*

		_textComponentButtonCloseJournalMenu.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonCloseJournalMenu");

		_textComponentButtonJournalSectionMissionInfo.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionMissionInfo");
		_textComponentButtonJournalSectionAbilities.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionMissionAbilities");
		_textComponentButtonJournalSectionInventory.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionMissionInventory");
		_textComponentButtonJournalSectionStatistics.text = _localizationManager.GetLocalizedString("UI_Menu_Journal_ButtonJournalSectionMissionStatistics");
		*/
	}
}