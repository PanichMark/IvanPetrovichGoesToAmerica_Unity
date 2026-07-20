using UnityEngine;

public class ViewModelPauseSubMenuSettings
{
	public GameObject ButtonSaveGameSettings;
	public GameObject TextButtonSaveGameSettings;

	public GameObject ButtonResetGameSettings;
	public GameObject TextButtonResetGameSettings;

	public GameObject ButtonClosePauseSubMenuSettings;
	public GameObject TextButtonClosePauseSubMenuSettings;

	public GameObject SubSettingsSectionGeneral;
	public GameObject ImageBackgroundSectionGeneral;
	public GameObject ButtonSubSettingsSectionGeneral;
	public GameObject TextButtonSubSettingsSectionGeneral;

	public GameObject SubSettingsSectionControls;
	public GameObject ImageBackgroundSectionControls;
	public GameObject ButtonSubSettingsSectionControls;
	public GameObject TextButtonSubSettingsSectionControls;

	public GameObject SubSettingsSectionGraphics;
	public GameObject ImageBackgroundSectionGraphics;
	public GameObject ButtonSubSettingsSectionGraphics;
	public GameObject TextButtonSubSettingsSectionGraphics;

	public GameObject SubSettingsSectionAudio;
	public GameObject ImageBackgroundSectionAudio;
	public GameObject ButtonSubSettingsSectionAudio;
	public GameObject TextButtonSubSettingsSectionAudio;

	public ViewModelPauseSubMenuSettings(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonSaveGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonSaveGameSettings");
		TextButtonSaveGameSettings = bootstrap.FindDeepGameObject(canvas, "TextButtonSaveGameSettings");

		ButtonResetGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonResetGameSettings");
		TextButtonResetGameSettings = bootstrap.FindDeepGameObject(canvas, "TextButtonResetGameSettings");

		ButtonClosePauseSubMenuSettings = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSettings");
		TextButtonClosePauseSubMenuSettings = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuSettings");

		SubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGeneral");
		ImageBackgroundSectionGeneral = bootstrap.FindDeepGameObject(canvas, "ImageBackgroundSectionGeneral");
		ButtonSubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsSectionGeneral");
		TextButtonSubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "TextButtonSubSettingsSectionGeneral");

		SubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionControls");
		ImageBackgroundSectionControls = bootstrap.FindDeepGameObject(canvas, "ImageBackgroundSectionControls");
		ButtonSubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsSectionControls");
		TextButtonSubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "TextButtonSubSettingsSectionControls");

		SubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGraphics");
		ImageBackgroundSectionGraphics = bootstrap.FindDeepGameObject(canvas, "ImageBackgroundSectionGraphics");
		ButtonSubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsSectionGraphics");
		TextButtonSubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "TextButtonSubSettingsSectionGraphics");

		SubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionAudio");
		ImageBackgroundSectionAudio = bootstrap.FindDeepGameObject(canvas, "ImageBackgroundSectionAudio");
		ButtonSubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsSectionAudio");
		TextButtonSubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "TextButtonSubSettingsSectionAudio");
	}
}