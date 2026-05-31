using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuSettings : IViewModel
{
	private GameObject _buttonSubSettingsSectionGeneral;
	private GameObject _buttonSubSettingsSectionControls;
	private GameObject _buttonSubSettingsSectionGraphics;
	private GameObject _buttonSubSettingsSectionAudio;

	private GameObject _subSettingsSectionGeneral;
	private GameObject _subSettingsSectionControls;
	private GameObject _subSettingsSectionGraphics;
	private GameObject _subSettingsSectionAudio;

	private GameObject _imageBackgroundSectionGeneral;
	private GameObject _imageBackgroundSectionControls;
	private GameObject _imageBackgroundSectionGraphics;
	private GameObject _imageBackgroundSectionAudio;

	// Общие кнопки окна настроек
	public Button ButtonSaveGameSettings;
	public Button ButtonResetGameSettings;
	public Button ButtonClosePauseSubMenuSettings;

	// Секция General
	public GameObject SubSettingsSectionGeneral;
	public Slider SliderChangeFOV;
	public TextMeshProUGUI NumberFOV;
	public Button[] ButtonsChangeFPS = new Button[4];

	// Секция Controls
	public GameObject SubSettingsSectionControls;
	public TMP_InputField[] InputFieldsKeyRebinds = new TMP_InputField[18];

	// Секция Graphics
	public GameObject SubSettingsSectionGraphics;

	// Секция Audio
	public GameObject SubSettingsSectionAudio;
	public Button[] ButtonsChangeLanguage = new Button[2]; 

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		// --- Общие кнопки ---
		ButtonSaveGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonSaveSettings").GetComponent<Button>();
		ButtonResetGameSettings = bootstrap.FindDeepGameObject(canvas, "ButtonResetSettings").GetComponent<Button>();
		ButtonClosePauseSubMenuSettings = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuSettings").GetComponent<Button>();

		// --- Секция General ---
		SubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGeneral");
		_imageBackgroundSectionGeneral = bootstrap.FindDeepGameObject(canvas, "BackgroundGeneral");
		_buttonSubSettingsSectionGeneral = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsGeneral");
		SliderChangeFOV = bootstrap.FindDeepGameObject(canvas, "SliderChangeFOV").GetComponent<Slider>();
		NumberFOV = bootstrap.FindDeepGameObject(canvas, "NumberFOV").GetComponent<TextMeshProUGUI>();
		ButtonsChangeFPS[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_30").GetComponent<Button>();
		ButtonsChangeFPS[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_60").GetComponent<Button>();
		ButtonsChangeFPS[2] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_90").GetComponent<Button>();
		ButtonsChangeFPS[3] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeFPS_144").GetComponent<Button>();

		// --- Секция Controls ---
		SubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionControls");
		_imageBackgroundSectionControls = bootstrap.FindDeepGameObject(canvas, "BackgroundControls");
		_buttonSubSettingsSectionControls = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsControls");
		string[] keyNames = { "MoveForward", "MoveBackward", "MoveRight", "MoveLeft", "Run", "Jump", "Crouch", "Interact",
							"ChangeCameraView", "ChangeCameraShoulder", "RightHandWeaponWheel", "LeftHandWeaponWheel",
							"RightHandWeaponAttack", "LeftHandWeaponAttack", "Reload", "LegKick"};
		for (int i = 0; i < keyNames.Length; i++)
		{
			InputFieldsKeyRebinds[i] = bootstrap.FindDeepGameObject(canvas, keyNames[i]).GetComponent<TMP_InputField>();
		}

		// --- Секция Graphics ---
		SubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionGraphics");
		_imageBackgroundSectionGraphics = bootstrap.FindDeepGameObject(canvas, "BackgroundGraphics");
		_buttonSubSettingsSectionGraphics = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsGraphics");

		// --- Секция Audio ---
		SubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "SubSettingsSectionAudio");
		_imageBackgroundSectionAudio = bootstrap.FindDeepGameObject(canvas, "BackgroundAudio");
		_buttonSubSettingsSectionAudio = bootstrap.FindDeepGameObject(canvas, "ButtonSubSettingsAudio");
		ButtonsChangeLanguage[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_Russian").GetComponent<Button>();
		ButtonsChangeLanguage[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguage_English").GetComponent<Button>();
	}
}