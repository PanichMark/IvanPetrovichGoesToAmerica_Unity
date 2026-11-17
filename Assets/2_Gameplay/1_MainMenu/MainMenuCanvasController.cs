using TMPro;
using UnityEngine;

public class MainMenuCanvasController : MonoBehaviour
{
    private LocalizationManager _localizationManager;
	private GameObject _canvasMainMenu;
	private MainMenuReadNewsController _mainMenuReadNews;
	private MainMenuChooseMissionController _mainMenuChooseMission;
	private MenuManager _menuManager;
	private GameObject _canvasNewGameCutscene;

	private GameObject _gameVersion;

	private TextMeshProUGUI[] _diegeticTextsList;
	
	public void Initialize(
		MainMenuChooseMissionController mainMenuChooseMission,
		MainMenuReadNewsController mainMenuReadNews,
		GameObject canvasMainMenu,
		GameObject canvasNewGameCutscene)
    {
      _localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_canvasMainMenu = canvasMainMenu;
		_mainMenuReadNews = mainMenuReadNews;
		_mainMenuChooseMission = mainMenuChooseMission;
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_canvasNewGameCutscene = canvasNewGameCutscene;

		_gameVersion = _canvasMainMenu.transform.Find("GameVersion").gameObject;

		_diegeticTextsList = new TextMeshProUGUI[9];

		_diegeticTextsList[0] = _canvasMainMenu.transform.Find("TextNewGame").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[1] = _canvasMainMenu.transform.Find("TextLoadGamePart1").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[2] = _canvasMainMenu.transform.Find("TextLoadGamePart2").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[3] = _canvasMainMenu.transform.Find("TextChooseMission").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[4] = _canvasMainMenu.transform.Find("TextReadNews").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[5] = _canvasMainMenu.transform.Find("TextEndGameTitles").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[6] = _canvasMainMenu.transform.Find("TextTestScene").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[7] = _canvasMainMenu.transform.Find("TextSettings").GetComponent<TextMeshProUGUI>();
		_diegeticTextsList[8] = _canvasMainMenu.transform.Find("TextExitGame").GetComponent<TextMeshProUGUI>();

		_diegeticTextsList[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextNewGame");
		_diegeticTextsList[1].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGamePart1");
		_diegeticTextsList[2].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGamePart2");
		_diegeticTextsList[3].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextChooseMission");
		_diegeticTextsList[4].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextReadNews");
		_diegeticTextsList[5].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextEndGameTitles");
		_diegeticTextsList[6].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextTestScene");
		_diegeticTextsList[7].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextSettings");
		_diegeticTextsList[8].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextExitGame");

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_mainMenuReadNews.OnCloseMainMenuReadNews += ShowCanvasMainMenu;
		_mainMenuChooseMission.OnCloseMainMenuChooseMission += ShowCanvasMainMenu;
		_menuManager.OnCloseAnyMenu += ShowCanvasMainMenu;

		Debug.Log("MainMenuCanvasController Initialized");
	}

    public void ShowCanvasMainMenu()
    {
		_canvasMainMenu.SetActive(true);
	}

	public void HideCanvasMainMenu()
	{
		_canvasMainMenu.SetActive(false);
	}

	public void ShowCanvasNewGameCutscene()
	{

	}

	private void OnDestroy()
	{
		_mainMenuReadNews.OnCloseMainMenuReadNews -= ShowCanvasMainMenu;
		_mainMenuChooseMission.OnCloseMainMenuChooseMission -= ShowCanvasMainMenu;
		_menuManager.OnCloseAnyMenu -= ShowCanvasMainMenu;
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;

		_diegeticTextsList[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextNewGame");
		_diegeticTextsList[1].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGamePart1");
		_diegeticTextsList[2].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGamePart2");
		_diegeticTextsList[3].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextChooseMission");
		_diegeticTextsList[4].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextReadNews");
		_diegeticTextsList[5].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextEndGameTitles");
		_diegeticTextsList[6].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextTestScene");
		_diegeticTextsList[7].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextSettings");
		_diegeticTextsList[8].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextExitGame");
	}
}