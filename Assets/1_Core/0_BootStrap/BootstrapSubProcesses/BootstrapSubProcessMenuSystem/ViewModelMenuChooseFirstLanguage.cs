using UnityEngine;

public class ViewModelMenuChooseFirstLanguage
{
	public GameObject ButtonRussianLangauge;
	public GameObject ButtonEnglishLanguage;

	public ViewModelMenuChooseFirstLanguage(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonRussianLangauge = bootstrap.FindDeepGameObject(canvas, "Russian");
		ButtonEnglishLanguage = bootstrap.FindDeepGameObject(canvas, "English");
	}
}
