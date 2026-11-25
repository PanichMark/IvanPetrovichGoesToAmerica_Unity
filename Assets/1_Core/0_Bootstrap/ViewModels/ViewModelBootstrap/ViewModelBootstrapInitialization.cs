using UnityEngine;

public class ViewModelBootstrapInitialization
{
	public GameObject BootstrapInitializationPart1;
	public GameObject BootstrapInitializationPart2;
	public GameObject BootstrapInitializationPart3;

	public GameObject TextSavingProcessIcon;
	public GameObject Gear;

	public ViewModelBootstrapInitialization(Bootstrap bootstrap, GameObject canvas)
	{
		BootstrapInitializationPart1 = bootstrap.FindDeepGameObject(canvas, "BootstrapInitializationPart1");
		BootstrapInitializationPart2 = bootstrap.FindDeepGameObject(canvas, "BootstrapInitializationPart2");
		BootstrapInitializationPart3 = bootstrap.FindDeepGameObject(canvas, "BootstrapInitializationPart3");

		TextSavingProcessIcon = bootstrap.FindDeepGameObject(canvas, "TextSavingProcessIcon");
		Gear = bootstrap.FindDeepGameObject(canvas, "Gear");
	}

}