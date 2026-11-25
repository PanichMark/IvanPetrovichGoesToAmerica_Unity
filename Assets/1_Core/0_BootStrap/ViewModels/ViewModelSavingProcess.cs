using UnityEngine;

public class ViewModelSavingProcess
{
	public GameObject Gear;

	public ViewModelSavingProcess(Bootstrap bootstrap, GameObject canvas)
	{
		Gear = bootstrap.FindDeepGameObject(canvas, "Gear");
	}
}
