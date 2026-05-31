using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuSettings
{
	public GameObject ButtonSubSettingsSectionGeneral;
	public GameObject ButtonSubSettingsSectionControls;
	public GameObject ButtonSubSettingsSectionGraphics;
	public GameObject ButtonSubSettingsSectionAudio;

	public GameObject ImageBackgroundSectionGeneral;
	public GameObject ImageBackgroundSectionControls;
	public GameObject ImageBackgroundSectionGraphics;
	public GameObject ImageBackgroundSectionAudio;

	// Общие кнопки окна настроек
	public GameObject ButtonSaveGameSettings;
	public GameObject ButtonResetGameSettings;
	public GameObject ButtonClosePauseSubMenuSettings;

	// Секция General
	public GameObject SubSettingsSectionGeneral;
	public GameObject SliderChangeFOV;
	public GameObject NumberFOV;
	public GameObject[] ButtonsChangeFPS = new GameObject[4];

	// Секция Controls
	public GameObject SubSettingsSectionControls;
	public GameObject[] InputFieldsKeyRebinds = new GameObject[16];

	// Секция Graphics
	public GameObject SubSettingsSectionGraphics;

	// Секция Audio
	public GameObject SubSettingsSectionAudio;
	public GameObject[] ButtonsChangeLanguage = new GameObject[2]; 

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
		SliderChangeFOV = bootstrap.FindDeepGameObject(canvas, "SliderChangeFOV");
		NumberFOV = bootstrap.FindDeepGameObject(canvas, "NumberFOV");
		ButtonsChangeFPS[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_30");
		ButtonsChangeFPS[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_60");
		ButtonsChangeFPS[2] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_90");
		ButtonsChangeFPS[3] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_144");

		// --- Секция Controls ---
		SubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionControls");
		ImageBackgroundSectionControls = bootstrap.FindDeepGameObject(canvas, "BackgroundControls");
		ButtonSubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsControls");

		string[] keyNames = { "MoveForward", "MoveBackward", "MoveRight", "MoveLeft", "Run", "Jump", "Crouch", "Interact",
							"ChangeCameraView", "ChangeCameraShoulder", "RightHandWeaponWheel", "LeftHandWeaponWheel",
							"RightHandWeaponAttack", "LeftHandWeaponAttack", "Reload", "LegKick"};
		for (int i = 0; i < keyNames.Length; i++)
		{
			InputFieldsKeyRebinds[i] = bootstrap.FindDeepGameObject(canvas, keyNames[i]);
		}

		// --- Секция Graphics ---
		SubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGraphics");
		ImageBackgroundSectionGraphics = bootstrap.FindDeepGameObject(canvas, "BackgroundGraphics");
		ButtonSubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsGraphics");

		// --- Секция Audio ---
		SubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionAudio");
		ImageBackgroundSectionAudio = bootstrap.FindDeepGameObject(canvas, "BackgroundAudio");
		ButtonSubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsAudio");
		ButtonsChangeLanguage[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_Russian");
		ButtonsChangeLanguage[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_English");
	}
}