using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuAppearance
{
	public GameObject ButtonClosePauseSubMenuAppearance;

	public ViewModelPauseSubMenuAppearance(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonClosePauseSubMenuAppearance = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuAppearance");
	}
}