using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionGraphics
{
	public GameObject TextSectionGraphicsMessage;

	public GameObject ToggleTurnOnShadows;
	public GameObject TextToggleTurnOnShadows;
	public GameObject ToggleTurnOnSSAO;
	public GameObject TextToggleTurnOnSSAO;

	public ViewModelPauseSubMenuSettingsSectionGraphics(Bootstrap bootstrap, GameObject canvas)
	{
		TextSectionGraphicsMessage = bootstrap.FindDeepGameObject(canvas, "TextSectionGraphicsMessage");

		ToggleTurnOnShadows = bootstrap.FindDeepGameObject(canvas, "ToggleTurnOnShadows");
		TextToggleTurnOnShadows = bootstrap.FindDeepGameObject(canvas, "TextToggleTurnOnShadows");
		ToggleTurnOnSSAO = bootstrap.FindDeepGameObject(canvas, "ToggleTurnOnSSAO");
		TextToggleTurnOnSSAO = bootstrap.FindDeepGameObject(canvas, "TextToggleTurnOnSSAO");
	}
}
