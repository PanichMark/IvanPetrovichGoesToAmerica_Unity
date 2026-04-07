using UnityEngine;
using UnityEngine.UI;

public class MainMenuReadNews : MonoBehaviour
{
	private IInputDevice inputDevice;
	private Button buttonCloseMainMenuReadNews;
	private GameObject canvasMainMenuReadNews;

	public delegate void MainMenuReadNewsHandler();
	public event MainMenuReadNewsHandler OnCloseMainMenuReadNews;
	
	public bool IsMainMenuReadNewsOpened {  get; private set; }

	public void Initialize(IInputDevice inputDevice, GameObject canvasMainMenuReadNews, Button buttonCloseMainMenuReadNews)
	{
		this.inputDevice = inputDevice;
		this.canvasMainMenuReadNews = canvasMainMenuReadNews;
		this.buttonCloseMainMenuReadNews = buttonCloseMainMenuReadNews;
		

		this.buttonCloseMainMenuReadNews.onClick.AddListener(() => HideCanvasMainMenuReadNews());
	


		Debug.Log("MainMenuReadNews Initialized");
	}

	public void ShowCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = true;
		canvasMainMenuReadNews.SetActive(true);
	}

	public void HideCanvasMainMenuReadNews()
	{
		IsMainMenuReadNewsOpened = false;
		OnCloseMainMenuReadNews?.Invoke();
		canvasMainMenuReadNews.SetActive(false);
	}

}
