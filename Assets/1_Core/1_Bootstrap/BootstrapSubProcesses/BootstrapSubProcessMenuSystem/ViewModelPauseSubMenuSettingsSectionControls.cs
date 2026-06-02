using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionControls
{
	public GameObject SliderMouseSensitivityX;
	public GameObject NumberSliderMouseSensitivityX;
	public GameObject TextSliderMouseSensitivityX;

	public GameObject SliderMouseSensitivityY;
	public GameObject NumberSliderMouseSensitivityY;
	public GameObject TextSliderMouseSensitivityY;

	public GameObject[] InputFieldsControls = new GameObject[16];
	public GameObject[] TextControls = new GameObject[16];

	public ViewModelPauseSubMenuSettingsSectionControls(Bootstrap bootstrap, GameObject canvas)
	{
		SliderMouseSensitivityX = bootstrap.FindDeepGameObject(canvas, "SliderMouseSensitivityX");
		NumberSliderMouseSensitivityX = bootstrap.FindDeepGameObject(canvas, "NumberMouseSensitivityX");
		TextSliderMouseSensitivityX = bootstrap.FindDeepGameObject(canvas, "TextMouseSensitivityX");

		SliderMouseSensitivityY = bootstrap.FindDeepGameObject(canvas, "SliderMouseSensitivityY");
		NumberSliderMouseSensitivityY = bootstrap.FindDeepGameObject(canvas, "NumberMouseSensitivityY");
		TextSliderMouseSensitivityY = bootstrap.FindDeepGameObject(canvas, "TextMouseSensitivityY");

		string[] nputFieldControlsNames = {
			"MoveForward",
			"MoveBackward",
			"MoveRight",
			"MoveLeft",
			"Run",
			"Jump",
			"Crouch",
			"Interact",
			"ChangeCameraView",
			"ChangeCameraShoulder",
			"WeaponWheelRightHand",
			"WeaponWheelLeftHand",
			"WeaponAttackRightHand",
			"WeaponAttackLeftHand",
			"WeaponReload",
			"LegKick"};

		for (int i = 0; i < nputFieldControlsNames.Length; i++)
		{
			InputFieldsControls[i] = bootstrap.FindDeepGameObject(canvas, $"InputFieldControl{nputFieldControlsNames[i]}");
		}

		for (int i = 0; i < nputFieldControlsNames.Length; i++)
		{
			TextControls[i] = bootstrap.FindDeepGameObject(canvas, $"TextControl{nputFieldControlsNames[i]}");
		}
	}
}
