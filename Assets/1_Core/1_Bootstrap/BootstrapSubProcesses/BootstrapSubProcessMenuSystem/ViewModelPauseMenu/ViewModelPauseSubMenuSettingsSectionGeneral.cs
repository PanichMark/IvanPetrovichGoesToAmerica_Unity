using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionGeneral
{
	public GameObject DropdownScreenResolution;
	public GameObject TextDropdownScreenResolution;

	public GameObject DropdownWindowType;
	public GameObject TextDropdownWindowType;

	public GameObject DropdownLimitFPS;
	public GameObject TextDropdownLimitFPS;

	public GameObject DropdownHUDType;
	public GameObject TextDropdownHUDType;

	public GameObject SliderCameraFOV;
	public GameObject NumberSliderCameraFOV;
	public GameObject TextSliderCameraFOV;

	public GameObject SliderScreenBrightness;
	public GameObject NumberSliderScreenBrightness;
	public GameObject TextSliderScreenBrightness;

	public GameObject DropdownWeaponWheelType;
	public GameObject TextDropdownWeaponWheelType;

	public GameObject DropdownShowBlood;
	public GameObject TextDropdownShowBlood;

	public ViewModelPauseSubMenuSettingsSectionGeneral(Bootstrap bootstrap, GameObject canvas)
	{
		DropdownScreenResolution = bootstrap.FindDeepGameObject(canvas, "DropdownScreenResolution");
		TextDropdownScreenResolution = bootstrap.FindDeepGameObject(canvas, "TextScreenResolution");

		DropdownWindowType = bootstrap.FindDeepGameObject(canvas, "DropdownWindowType");
		TextDropdownWindowType = bootstrap.FindDeepGameObject(canvas, "TextWindowType");

		DropdownLimitFPS = bootstrap.FindDeepGameObject(canvas, "DropdownLimitFPS");
		TextDropdownLimitFPS = bootstrap.FindDeepGameObject(canvas, "TextLimitFPS");

		DropdownHUDType = bootstrap.FindDeepGameObject(canvas, "DropdownHUDtype");
		TextDropdownHUDType = bootstrap.FindDeepGameObject(canvas, "TextHUDtype");

		SliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "SliderCameraFOV");
		NumberSliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "NumberCameraFOV");
		TextSliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "TextCameraFOV");

		SliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "SliderScreenBrightness");
		NumberSliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "NumberScreenBrightness");
		TextSliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "TextScreenBrightness");

		DropdownWeaponWheelType = bootstrap.FindDeepGameObject(canvas, "DropdownWeaponWheelType");
		TextDropdownWeaponWheelType = bootstrap.FindDeepGameObject(canvas, "TextWeaponWheelType");

		DropdownShowBlood = bootstrap.FindDeepGameObject(canvas, "DropdownShowBlood");
		TextDropdownShowBlood = bootstrap.FindDeepGameObject(canvas, "TextShowBlood");
	}
}
