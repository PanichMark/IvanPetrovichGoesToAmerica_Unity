using UnityEngine;
using UnityEngine.UI;

public class MainMenuReadNewsController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private Button _buttonCloseMainMenuReadNews;
	private GameObject _canvasMainMenuReadNews;

	public delegate void MainMenuReadNewsHandler();
	public event MainMenuReadNewsHandler OnCloseMainMenuReadNews;
	
	public bool IsMainMenuReadNewsOpened {  get; private set; }

	public void Initialize(IInputDevice inputDevice, GameObject canvasMainMenuReadNews, Button buttonCloseMainMenuReadNews)
	{
		_inputDevice = inputDevice;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_buttonCloseMainMenuReadNews = buttonCloseMainMenuReadNews;
		
		_buttonCloseMainMenuReadNews.onClick.AddListener(() => HideCanvasMainMenuReadNews());
	
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
}