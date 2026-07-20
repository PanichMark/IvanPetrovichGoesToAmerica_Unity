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

	public GameObject DropdownWeaponWheelType;
	public GameObject TextDropdownWeaponWheelType;

	public GameObject SliderCameraFOV;
	public GameObject NumberSliderCameraFOV;
	public GameObject TextSliderCameraFOV;

	public GameObject SliderScreenBrightness;
	public GameObject NumberSliderScreenBrightness;
	public GameObject TextSliderScreenBrightness;

	public GameObject ButtonGameDifficulty;
	public GameObject TextButtonGameDifficulty;

	public GameObject ToggleShowIngameHints;
	public GameObject TextToggleShowIngameHints;

	public GameObject ToggleShowBlood;
	public GameObject TextToggleShowBlood;

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

		DropdownWeaponWheelType = bootstrap.FindDeepGameObject(canvas, "DropdownWeaponWheelType");
		TextDropdownWeaponWheelType = bootstrap.FindDeepGameObject(canvas, "TextWeaponWheelType");

		SliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "SliderCameraFOV");
		NumberSliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "NumberCameraFOV");
		TextSliderCameraFOV = bootstrap.FindDeepGameObject(canvas, "TextCameraFOV");

		SliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "SliderScreenBrightness");
		NumberSliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "NumberScreenBrightness");
		TextSliderScreenBrightness = bootstrap.FindDeepGameObject(canvas, "TextScreenBrightness");

		ButtonGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonGameDifficulty");
		TextButtonGameDifficulty = bootstrap.FindDeepGameObject(canvas, "TextGameDifficulty");

		ToggleShowIngameHints = bootstrap.FindDeepGameObject(canvas, "ToggleShowIngameHints");
		TextToggleShowIngameHints = bootstrap.FindDeepGameObject(canvas, "TextShowIngameHints");

		ToggleShowBlood = bootstrap.FindDeepGameObject(canvas, "ToggleShowBlood");
		TextToggleShowBlood = bootstrap.FindDeepGameObject(canvas, "TextShowBlood");
	}
}
