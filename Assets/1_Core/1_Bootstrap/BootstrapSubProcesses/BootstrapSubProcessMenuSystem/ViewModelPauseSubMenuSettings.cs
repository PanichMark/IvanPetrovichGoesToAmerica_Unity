using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuSettings
{
	public GameObject ButtonSaveGameSettings;
	public GameObject ButtonResetGameSettings;
	public GameObject ButtonClosePauseSubMenuSettings;

	public GameObject ButtonSubSettingsSectionGeneral;
	public GameObject ButtonSubSettingsSectionControls;
	public GameObject ButtonSubSettingsSectionGraphics;
	public GameObject ButtonSubSettingsSectionAudio;

	public GameObject ImageBackgroundSectionGeneral;
	public GameObject ImageBackgroundSectionControls;
	public GameObject ImageBackgroundSectionGraphics;
	public GameObject ImageBackgroundSectionAudio;

	public GameObject SubSettingsSectionGeneral;
	public GameObject SubSettingsSectionControls;
	public GameObject SubSettingsSectionGraphics;
	public GameObject SubSettingsSectionAudio;
	

	public ViewModelPauseSubMenuSettings(Bootstrap bootstrap, GameObject canvas)
	{
		// --- Общие кнопки ---
		ButtonSaveGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonSaveSettings");
		ButtonResetGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonResetSettings");
		ButtonClosePauseSubMenuSettings = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSettings");

		// --- Секция General ---
		SubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGeneral");
		ImageBackgroundSectionGeneral = bootstrap.FindDeepGameObject(canvas, "BackgroundGeneral");
		ButtonSubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsGeneral");

		// --- Секция Controls ---
		SubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionControls");
		ImageBackgroundSectionControls = bootstrap.FindDeepGameObject(canvas, "BackgroundControls");
		ButtonSubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsControls");

		// --- Секция Graphics ---
		SubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGraphics");
		ImageBackgroundSectionGraphics = bootstrap.FindDeepGameObject(canvas, "BackgroundGraphics");
		ButtonSubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsGraphics");

		// --- Секция Audio ---
		SubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionAudio");
		ImageBackgroundSectionAudio = bootstrap.FindDeepGameObject(canvas, "BackgroundAudio");
		ButtonSubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsAudio");
	}
}