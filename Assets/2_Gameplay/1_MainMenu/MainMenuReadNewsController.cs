using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenuReadNewsController : MonoBehaviour
{
	private Button _buttonCloseMainMenuReadNews;
	private Button _buttonYouTube;
	private Button _buttonGitHub;
	private Button _buttonDTF;
	private ViewModelMainMenuReadNews _viewModelMainMenuReadNews;

	private const string YOUTUBE_URL = "https://youtube.com/@panichmark";
	private const string GITHUB_URL = "https://github.com/PanichMark/IvanPetrovichGoesToAmerica_Unity";
	private const string DTF_URL = "https://dtf.ru/id3315912";
	public delegate void MainMenuReadNewsHandler();
	public event MainMenuReadNewsHandler OnCloseMainMenuReadNews;
	private LocalizationManager _localizationManager;

	private TextMeshProUGUI _textComponentButtonCloseMainMenuReadNews;

	private TextMeshProUGUI _textReadNews;
	private Bootstrap _bootstrap;

	public bool IsMainMenuReadNewsOpened {  get; private set; }
	private GameObject _canvasReadNews;
	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_bootstrap = bootstrap;

		_canvasReadNews = _bootstrap._canvasMainMenuReadNews;
		_viewModelMainMenuReadNews = ServiceLocator.Resolve<ViewModelMainMenuReadNews>();

		_textComponentButtonCloseMainMenuReadNews = _viewModelMainMenuReadNews.TextButtonCloseMainMenuReadNews.GetComponent<TextMeshProUGUI>();

	   _buttonCloseMainMenuReadNews = _viewModelMainMenuReadNews.ButtonCloseMainMenuReadNews.GetComponent<Button>();
		_buttonYouTube = _viewModelMainMenuReadNews.ButtonYouTube.GetComponent<Button>();
		_buttonGitHub = _viewModelMainMenuReadNews.ButtonGitHub.GetComponent<Button>();
		_buttonDTF = _viewModelMainMenuReadNews.ButtonDTF.GetComponent<Button>();

		_buttonCloseMainMenuReadNews.onClick.AddListener(() => HideCanvasMainMenuReadNews());
		_textReadNews = _viewModelMainMenuReadNews.TextReadNews.GetComponent<TextMeshProUGUI>();
		_buttonYouTube.onClick.AddListener(() => OpenUrl(YOUTUBE_URL));
		_buttonGitHub.onClick.AddListener(() => OpenUrl(GITHUB_URL));
		_buttonDTF.onClick.AddListener(() => OpenUrl(DTF_URL));

		ChangeLanguage(_localizationManager);

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		Debug.Log("MainMenuReadNewsController Initialized");
	}

	public void ShowCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = true;
		_canvasReadNews.SetActive(true);

		Debug.Log("Show ReadNews");
	}

	public void HideCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = false;
		OnCloseMainMenuReadNews?.Invoke();
		_canvasReadNews.SetActive(false);

		Debug.Log("Hide ReadNews");
	}

	private void OpenUrl(string url)
	{
		Application.OpenURL(url);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			_textReadNews.text = _bootstrap.GameData.GameNews.GameNews_RU.text;
		}
		else
		{
			_textReadNews.text = _bootstrap.GameData.GameNews.GameNews_EN.text;
		}

		_textComponentButtonCloseMainMenuReadNews.text = _localizationManager.GetLocalizedString("UI_Menu_MainMenu_ReadNews_ButtonCloseReadNews");
	}
}