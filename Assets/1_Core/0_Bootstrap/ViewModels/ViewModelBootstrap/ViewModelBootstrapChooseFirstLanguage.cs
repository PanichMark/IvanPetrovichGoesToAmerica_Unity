using UnityEngine;

public class ViewModelBootstrapChooseFirstLanguage
{
	public GameObject ButtonRussianLangauge;
	public GameObject ButtonEnglishLanguage;

	public ViewModelBootstrapChooseFirstLanguage(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonRussianLangauge = bootstrap.FindDeepGameObject(canvas, "Russian");
		ButtonEnglishLanguage = bootstrap.FindDeepGameObject(canvas, "English");
	}
}
