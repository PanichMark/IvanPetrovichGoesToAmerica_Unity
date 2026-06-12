using UnityEngine;

public class ViewModelMenuLockpickMechanical
{
	public GameObject ButtonCloseMenuLockpickMechanical;
	public GameObject TextButtonCloseMenuLockpickMechanical;

	public ViewModelMenuLockpickMechanical(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMenuLockpickMechanical = bootstrap.FindDeepGameObject(canvas, "ButtonExitLockpickMechanicalMenu");
		TextButtonCloseMenuLockpickMechanical = bootstrap.FindDeepGameObject(canvas, "TextButtonExitLockpickMechanicalMenu");
	}
}