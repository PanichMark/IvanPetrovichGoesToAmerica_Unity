using UnityEngine;

public class ViewModelMainMenuReadNews
{
	public GameObject ButtonCloseMainMenuReadNews;
	public GameObject TextButtonCloseMainMenuReadNews;

	public GameObject ButtonYouTube;
	public GameObject ButtonGitHub;
	public GameObject ButtonDTF;

	public GameObject TextReadNews;

	public ViewModelMainMenuReadNews(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMainMenuReadNews = bootstrap.FindDeepGameObject(canvas, "ButtonCloseMainMenuReadNews");
		TextButtonCloseMainMenuReadNews = bootstrap.FindDeepGameObject(canvas, "TextButtonCloseMainMenuReadNews");

		ButtonYouTube = bootstrap.FindDeepGameObject(canvas, "YouTube");
		ButtonGitHub = bootstrap.FindDeepGameObject(canvas, "GitHub");
		ButtonDTF = bootstrap.FindDeepGameObject(canvas, "DTF");

		TextReadNews = bootstrap.FindDeepGameObject(canvas, "TextReadNews");
	}
}
