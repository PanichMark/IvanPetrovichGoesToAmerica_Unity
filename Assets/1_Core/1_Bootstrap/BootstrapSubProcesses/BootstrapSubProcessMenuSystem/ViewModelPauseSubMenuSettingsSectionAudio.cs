using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionAudio
{
	public GameObject[] ButtonsChangeLanguage = new GameObject[2];

	public ViewModelPauseSubMenuSettingsSectionAudio(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonsChangeLanguage[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_Russian");
		ButtonsChangeLanguage[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_English");
	}
}