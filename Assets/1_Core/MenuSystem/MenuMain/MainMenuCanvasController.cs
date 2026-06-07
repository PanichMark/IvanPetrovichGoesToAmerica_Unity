using UnityEngine;
using TMPro;

public class MainMenuCanvasController : MonoBehaviour
{
    private LocalizationManager _localizationManager;
	private GameObject _CanvasDiegeticText;
	private MainMenuReadNewsController _mainMenuReadNews;
	private MenuManager _menuManager;

	private TextMeshProUGUI[] _diegeticTextsList;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        _localizationManager = ServiceLocator.Resolve<LocalizationManager > ("LocalizationManager");
       
		_CanvasDiegeticText = GameObject.Find("CanvasMainMenu");
		_mainMenuReadNews = ServiceLocator.Resolve<MainMenuReadNewsController>("MainMenuReadNews");
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
	

		_diegeticTextsList = new[]
		{
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextNewGame").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextTestScene").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextLoadGame1").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextLoadGame2").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextSettings").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextReadNews").GetComponent<TextMeshProUGUI>(),
			GameObject.Find("CanvasMainMenu").transform.Find("CanvasDiegeticTexts").transform.Find("TextExitGame").GetComponent<TextMeshProUGUI>(),
		};

		_diegeticTextsList[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextNewGame");
		_diegeticTextsList[1].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextTestScene");
		_diegeticTextsList[2].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGame1");
		_diegeticTextsList[3].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGame2");
		_diegeticTextsList[4].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextSettings");
		_diegeticTextsList[5].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextReadNews");
		_diegeticTextsList[6].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextExitGame");

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_mainMenuReadNews.OnCloseMainMenuReadNews += ShowMainMenuCanvas;
		_menuManager.OnCloseAnyMenu += ShowMainMenuCanvas;
	}

    public void ShowMainMenuCanvas()
    {
		_CanvasDiegeticText.SetActive(true);
	}

	public void HideMainMenuCanvas()
	{
		_CanvasDiegeticText.SetActive(false);
	}

	private void OnDestroy()
	{
		_mainMenuReadNews.OnCloseMainMenuReadNews -= ShowMainMenuCanvas;
		_menuManager.OnCloseAnyMenu -= ShowMainMenuCanvas;
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;

		_diegeticTextsList[0].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextNewGame");
		_diegeticTextsList[1].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextTestScene");
		_diegeticTextsList[2].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGame1");
		_diegeticTextsList[3].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextLoadGame2");
		_diegeticTextsList[4].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextSettings");
		_diegeticTextsList[5].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextReadNews");
		_diegeticTextsList[6].text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_TextExitGame");
	}
}
