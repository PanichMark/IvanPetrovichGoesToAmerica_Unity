using UnityEngine;

public class ViewModelHUDHealthAndMana
{
	public GameObject HealthBar;
	public GameObject SliderHealthBar;
	public GameObject ManaBar;
	public GameObject SliderManaBar;
	public GameObject SliderManaBarFillArea;
	

	public ViewModelHUDHealthAndMana(Bootstrap bootstrap, GameObject canvas)
	{
		HealthBar = bootstrap.FindDeepGameObject(canvas, "HealthBar");
		SliderHealthBar = bootstrap.FindDeepGameObject(canvas, "SliderHealthBar");

		ManaBar = bootstrap.FindDeepGameObject(canvas, "ManaBar");
		SliderManaBar = bootstrap.FindDeepGameObject(canvas, "SliderManaBar");
		SliderManaBarFillArea = bootstrap.FindDeepGameObject(canvas, "SliderManaBarFillArea");
	}
}