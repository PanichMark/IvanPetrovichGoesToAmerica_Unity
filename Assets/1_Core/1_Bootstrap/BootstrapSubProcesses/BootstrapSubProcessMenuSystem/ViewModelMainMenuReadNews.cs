using UnityEngine;
using UnityEngine.UI;

public class ViewModelMainMenuReadNews
{
	public Button ButtonCloseMainMenuReadNews;
	public Button ButtonYouTube;
	public Button ButtonGitHub;

	public ViewModelMainMenuReadNews(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMainMenuReadNews = canvas.transform.Find("ButtonCloseMainMenuReadNews").GetComponent<Button>();
		ButtonYouTube = canvas.transform.Find("YouTube").GetComponent<Button>();
		ButtonGitHub = canvas.transform.Find("GitHub").GetComponent<Button>();
	}
}
