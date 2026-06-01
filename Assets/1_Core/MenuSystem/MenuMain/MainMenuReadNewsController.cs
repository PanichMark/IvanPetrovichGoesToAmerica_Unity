using UnityEngine;
using UnityEngine.UI;

public class MainMenuReadNewsController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private Button _buttonCloseMainMenuReadNews;
	private GameObject _canvasMainMenuReadNews;
	private Button _buttonYouTube;
	private Button _buttonGitHub;

	private const string YOUTUBE_URL = "https://youtube.com/@panichmark";
	private const string GITHUB_URL = "https://github.com/PanichMark/IvanPetrovichGoesToAmerica_Unity";

	public delegate void MainMenuReadNewsHandler();
	public event MainMenuReadNewsHandler OnCloseMainMenuReadNews;
	
	public bool IsMainMenuReadNewsOpened {  get; private set; }

	public void Initialize(
		IInputDevice inputDevice,
		GameObject canvasMainMenuReadNews,
		Button buttonCloseMainMenuReadNews,
		Button buttonYouTube,
		Button buttonGitHub)
	{
		_inputDevice = inputDevice;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_buttonCloseMainMenuReadNews = buttonCloseMainMenuReadNews;
		_buttonYouTube = buttonYouTube;
		_buttonGitHub = buttonGitHub;
		
		_buttonCloseMainMenuReadNews.onClick.AddListener(() => HideCanvasMainMenuReadNews());

		_buttonYouTube.onClick.AddListener(() => OpenUrl(YOUTUBE_URL));
		_buttonGitHub.onClick.AddListener(() => OpenUrl(GITHUB_URL));

		Debug.Log("MainMenuReadNews Initialized");
	}

	public void ShowCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = true;
		_canvasMainMenuReadNews.SetActive(true);
	}

	public void HideCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = false;
		OnCloseMainMenuReadNews?.Invoke();
		_canvasMainMenuReadNews.SetActive(false);
	}

	private void OpenUrl(string url)
	{
		Application.OpenURL(url);
	}
}