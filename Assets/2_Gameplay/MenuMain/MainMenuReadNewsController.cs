using UnityEngine;
using UnityEngine.UI;

public class MainMenuReadNewsController : MonoBehaviour
{
	private Button _buttonCloseMainMenuReadNews;
	private Button _buttonYouTube;
	private Button _buttonGitHub;
	private ViewModelMainMenuReadNews _viewModelMainMenuReadNews;

	private const string YOUTUBE_URL = "https://youtube.com/@panichmark";
	private const string GITHUB_URL = "https://github.com/PanichMark/IvanPetrovichGoesToAmerica_Unity";
	public delegate void MainMenuReadNewsHandler();
	public event MainMenuReadNewsHandler OnCloseMainMenuReadNews;
	
	public bool IsMainMenuReadNewsOpened {  get; private set; }
	private GameCanvasesList _gameCanvasesList;
	private GameObject _canvasReadNews;
	public void Initialize(
		GameCanvasesList gameCanvasesList)
	{
		_gameCanvasesList = gameCanvasesList;
		_canvasReadNews = gameCanvasesList.CanvasMainMenuReadNews;
		_viewModelMainMenuReadNews = ServiceLocator.Resolve<ViewModelMainMenuReadNews>();

		_buttonCloseMainMenuReadNews = _viewModelMainMenuReadNews.ButtonCloseMainMenuReadNews.GetComponent<Button>();
		_buttonYouTube = _viewModelMainMenuReadNews.ButtonYouTube.GetComponent<Button>();
		_buttonGitHub = _viewModelMainMenuReadNews.ButtonGitHub.GetComponent<Button>();

		_buttonCloseMainMenuReadNews.onClick.AddListener(() => HideCanvasMainMenuReadNews());

		_buttonYouTube.onClick.AddListener(() => OpenUrl(YOUTUBE_URL));
		_buttonGitHub.onClick.AddListener(() => OpenUrl(GITHUB_URL));

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
}