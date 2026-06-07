using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuLockpickElectronic
{
	public GameObject[] ButtonsLockElectronic;
	public Button ButtonCloseMenuLockpickElectronic;

	public ViewModelMenuLockpickElectronic(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonsLockElectronic = new GameObject[]
		{
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic1"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic2"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic3"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic4"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic5"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic6"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic7"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic8"),
			bootstrap.FindDeepGameObject(canvas, "ButtonLockElectronic9")
		};
		ButtonCloseMenuLockpickElectronic = canvas.transform.Find("ButtonExitLockpickElectronicMenu").GetComponent<Button>();
	}
}
