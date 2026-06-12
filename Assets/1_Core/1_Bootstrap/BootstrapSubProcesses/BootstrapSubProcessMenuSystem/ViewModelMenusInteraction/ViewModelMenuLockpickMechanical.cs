using UnityEngine;

public class ViewModelMenuLockpickMechanical
{
	public GameObject ButtonCloseMenuLockpickMechanical;
	public GameObject TextButtonCloseMenuLockpickMechanical;
	public GameObject ButtonMoveLockMechanismUp;
	public GameObject ButtonMoveLockMechanismDown;
	public GameObject ButtonMoveLockMechanismRight;
	public GameObject ButtonMoveLockMechanismLeft;


	public ViewModelMenuLockpickMechanical(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMenuLockpickMechanical = bootstrap.FindDeepGameObject(canvas, "ButtonExitLockpickMechanicalMenu");
		TextButtonCloseMenuLockpickMechanical = bootstrap.FindDeepGameObject(canvas, "TextButtonExitLockpickMechanicalMenu");

		ButtonMoveLockMechanismUp = bootstrap.FindDeepGameObject(canvas, "ButtonUp");
		ButtonMoveLockMechanismDown = bootstrap.FindDeepGameObject(canvas, "ButtonDown");
		ButtonMoveLockMechanismRight = bootstrap.FindDeepGameObject(canvas, "ButtonRight");
		ButtonMoveLockMechanismLeft = bootstrap.FindDeepGameObject(canvas, "ButtonLeft");
	}
}