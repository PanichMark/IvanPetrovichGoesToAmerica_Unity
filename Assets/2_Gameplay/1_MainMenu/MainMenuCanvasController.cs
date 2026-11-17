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

	private TextMeshProUGUI[] _diegeticTextsList;

	private GameObject[] _newGameCutsceneObjects;
	private TextMeshProUGUI[] _newGameCutsceneObjectsTexts;

	public void Initialize(
		MainMenuChooseMissionController mainMenuChooseMission,
		MainMenuReadNewsController mainMenuReadNews,
		GameObject canvasMainMenu,
		GameObject canvasNewGameCutscene)
    {
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		_canvasNewGameCutscene = canvasNewGameCutscene;
	
		_canvasMainMenu = canvasMainMenu;
		_mainMenuReadNews = mainMenuReadNews;
		_mainMenuChooseMission = mainMenuChooseMission;
		_menuManager = ServiceLocator.Resolve<MenuManager>();
		

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

		_newGameCutsceneObjects = new GameObject[12];

		_newGameCutsceneObjects[0] = _canvasNewGameCutscene.transform.Find("Quote ForSignal").transform.Find("Quote").gameObject;
		_newGameCutsceneObjects[1] = _canvasNewGameCutscene.transform.Find("GameBy ForSignal").transform.Find("GameBy").gameObject;
		_newGameCutsceneObjects[2] = _canvasNewGameCutscene.transform.Find("ThanksToB&D ForSignal").transform.Find("ThanksToB&D").gameObject;
		_newGameCutsceneObjects[3] = _canvasNewGameCutscene.transform.Find("Professor1 ForSignal").transform.Find("Professor1").gameObject;
		_newGameCutsceneObjects[4] = _canvasNewGameCutscene.transform.Find("Professor2 ForSignal").transform.Find("Professor2").gameObject;
		_newGameCutsceneObjects[5] = _canvasNewGameCutscene.transform.Find("Professor3 ForSignal").transform.Find("Professor3").gameObject;
		_newGameCutsceneObjects[6] = _canvasNewGameCutscene.transform.Find("Professor4 ForSignal").transform.Find("Professor4").gameObject;
		_newGameCutsceneObjects[7] = _canvasNewGameCutscene.transform.Find("Professor5 ForSignal").transform.Find("Professor5").gameObject;
		_newGameCutsceneObjects[8] = _canvasNewGameCutscene.transform.Find("Professor6 ForSignal").transform.Find("Professor6").gameObject;
		_newGameCutsceneObjects[9] = _canvasNewGameCutscene.transform.Find("Professor7 ForSignal").transform.Find("Professor7").gameObject;
		_newGameCutsceneObjects[10] = _canvasNewGameCutscene.transform.Find("Professor8 ForSignal").transform.Find("Professor8").gameObject;
		_newGameCutsceneObjects[11] = _canvasNewGameCutscene.transform.Find("GameLogo ForSignal").transform.Find("GameLogo").gameObject;

		_newGameCutsceneObjectsTexts = new TextMeshProUGUI[11];

		_newGameCutsceneObjectsTexts[0] = _newGameCutsceneObjects[0].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[1] = _newGameCutsceneObjects[1].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[2] = _newGameCutsceneObjects[2].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[3] = _newGameCutsceneObjects[3].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[4] = _newGameCutsceneObjects[4].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[5] = _newGameCutsceneObjects[5].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[6] = _newGameCutsceneObjects[6].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[7] = _newGameCutsceneObjects[7].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[8] = _newGameCutsceneObjects[8].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[9] = _newGameCutsceneObjects[9].GetComponent<TextMeshProUGUI>();
		_newGameCutsceneObjectsTexts[10] = _newGameCutsceneObjects[10].GetComponent<TextMeshProUGUI>();

		_newGameCutsceneObjectsTexts[0].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Quote");
		_newGameCutsceneObjectsTexts[1].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_GameBy");
		_newGameCutsceneObjectsTexts[2].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_ThanksToB&D");
		_newGameCutsceneObjectsTexts[3].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor1");
		_newGameCutsceneObjectsTexts[4].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor2");
		_newGameCutsceneObjectsTexts[5].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor3");
		_newGameCutsceneObjectsTexts[6].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor4");
		_newGameCutsceneObjectsTexts[7].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor5");
		_newGameCutsceneObjectsTexts[8].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor6");
		_newGameCutsceneObjectsTexts[9].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor7");
		_newGameCutsceneObjectsTexts[10].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor8");

		for (int i = 0; i < _newGameCutsceneObjects.Length; i++)
		{
			//Debug.Log(_newGameCutsceneObjects[i].name);
			_newGameCutsceneObjects[i].SetActive(false);
		}
		_canvasNewGameCutscene.SetActive(false);

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
		_canvasNewGameCutscene.SetActive(true);
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

		_newGameCutsceneObjectsTexts[0].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Quote");
		_newGameCutsceneObjectsTexts[1].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_GameBy");
		_newGameCutsceneObjectsTexts[2].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_ThanksToB&D");
		_newGameCutsceneObjectsTexts[3].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor1");
		_newGameCutsceneObjectsTexts[4].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor2");
		_newGameCutsceneObjectsTexts[5].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor3");
		_newGameCutsceneObjectsTexts[6].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor4");
		_newGameCutsceneObjectsTexts[7].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor5");
		_newGameCutsceneObjectsTexts[8].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor6");
		_newGameCutsceneObjectsTexts[9].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor7");
		_newGameCutsceneObjectsTexts[10].text = _localizationManager.GetLocalizedString("UI_Cutscene_NewGame_Professor8");
	}
}