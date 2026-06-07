using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuLockpickMechanical
{
	public Button ButtonCloseMenuLockpickMechanical;

	public ViewModelMenuLockpickMechanical(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMenuLockpickMechanical = canvas.transform.Find("ButtonExitLockpickMechanicalMenu").GetComponent<Button>();
	}
}
