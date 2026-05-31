using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelHUDHealthAndMana
{
	public GameObject HealthBar;
	public Slider SliderHealthBar;
	public Slider SliderManaBar;
	public GameObject ManaBar;

	public ViewModelHUDHealthAndMana(Bootstrap bootstrap, GameObject canvas)
	{
		HealthBar = bootstrap.FindDeepGameObject(canvas, "HealthBar");
		SliderHealthBar = bootstrap.FindDeepGameObject(canvas, "SliderHealthBar").GetComponent<Slider>();

		ManaBar = bootstrap.FindDeepGameObject(canvas, "ManaBar");
		SliderManaBar = bootstrap.FindDeepGameObject(canvas, "SliderManaBar").GetComponent<Slider>();
	}
}
