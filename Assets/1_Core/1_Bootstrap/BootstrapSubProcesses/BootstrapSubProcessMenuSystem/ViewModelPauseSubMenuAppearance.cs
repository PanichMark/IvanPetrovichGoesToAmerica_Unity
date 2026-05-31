using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuAppearance : IViewModel
{
	public GameObject ButtonClosePauseSubMenuAppearance;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonClosePauseSubMenuAppearance = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuAppearance");
	}
}