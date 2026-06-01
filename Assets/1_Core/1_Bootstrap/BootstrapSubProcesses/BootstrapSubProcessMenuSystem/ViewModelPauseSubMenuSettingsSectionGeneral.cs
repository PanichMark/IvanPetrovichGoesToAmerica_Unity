using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionGeneral : MonoBehaviour
{
	public GameObject SliderChangeFOV;
	public GameObject NumberFOV;
	public GameObject[] ButtonsChangeFPS = new GameObject[4];

	public ViewModelPauseSubMenuSettingsSectionGeneral(Bootstrap bootstrap, GameObject canvas)
	{
		SliderChangeFOV = bootstrap.FindDeepGameObject(canvas, "SliderChangeFOV");
		NumberFOV = bootstrap.FindDeepGameObject(canvas, "NumberFOV");
		ButtonsChangeFPS[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_30");
		ButtonsChangeFPS[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_60");
		ButtonsChangeFPS[2] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_90");
		ButtonsChangeFPS[3] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_144");
	}
}
