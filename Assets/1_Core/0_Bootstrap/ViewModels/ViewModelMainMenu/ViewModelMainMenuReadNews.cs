using UnityEngine;

public class ViewModelMainMenuReadNews
{
	public GameObject ButtonCloseMainMenuReadNews;
	public GameObject ButtonYouTube;
	public GameObject ButtonGitHub;
	public GameObject ButtonDTF;

	public ViewModelMainMenuReadNews(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMainMenuReadNews = bootstrap.FindDeepGameObject(canvas, "ButtonCloseMainMenuReadNews");
		ButtonYouTube = bootstrap.FindDeepGameObject(canvas, "YouTube");
		ButtonGitHub = bootstrap.FindDeepGameObject(canvas, "GitHub");
		ButtonDTF = bootstrap.FindDeepGameObject(canvas, "DTF");
	}
}
