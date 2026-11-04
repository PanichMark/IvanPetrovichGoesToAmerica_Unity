using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionGraphicsController : MonoBehaviour
{
	private LocalizationManager _localizationManager;

	private GameObject _textSectionGraphicsMessage;
	private TextMeshProUGUI _textComponentSectionGraphicsMessage;

	private GameObject _toggleTurnOnShadows;
	private TextMeshProUGUI _textToggleTurnOnShadows;
	private GameObject _toggleTurnOnSSAO;
	private TextMeshProUGUI _textTurnOnSSAO;

	private Toggle _toggleComponentTurnOnShadows;
	private Toggle _toggleComponentTurnOnSSAO;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGraphicsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGraphicsData;

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		ViewModelPauseSubMenuSettingsSectionGraphics viewModelPauseSubMenuSettingsSectionGraphics)
	{
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;

		_textSectionGraphicsMessage = viewModelPauseSubMenuSettingsSectionGraphics.TextSectionGraphicsMessage;
		_textComponentSectionGraphicsMessage = viewModelPauseSubMenuSettingsSectionGraphics.TextSectionGraphicsMessage.GetComponent<TextMeshProUGUI>();

		_toggleTurnOnShadows = viewModelPauseSubMenuSettingsSectionGraphics.ToggleTurnOnShadows;
		_toggleComponentTurnOnShadows = viewModelPauseSubMenuSettingsSectionGraphics.ToggleTurnOnShadows.GetComponent<Toggle>();
		_textToggleTurnOnShadows = viewModelPauseSubMenuSettingsSectionGraphics.TextToggleTurnOnShadows.GetComponent<TextMeshProUGUI>();
		_toggleTurnOnSSAO = viewModelPauseSubMenuSettingsSectionGraphics.ToggleTurnOnSSAO;
		_toggleComponentTurnOnSSAO = viewModelPauseSubMenuSettingsSectionGraphics.ToggleTurnOnSSAO.GetComponent<Toggle>();
		_textTurnOnSSAO = viewModelPauseSubMenuSettingsSectionGraphics.TextToggleTurnOnSSAO.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		ChangeLanguage(_localizationManager);

		_playerPrefsSettingsController.OnApplySettingsSectionGraphicsPlayerPrefs += ApplySystemLoadedSettings;

		Debug.Log("SettingsSectionGraphicsController Initialized");
	}

	public void SaveSettingsGraphics()
	{
		var currentData = new PlayerPrefsData();

		OnSaveSettingsGraphicsData?.Invoke(currentData);
	}

	public void ResetSettingsGraphics()
	{
		OnResetSettingsGraphicsData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{

		};

		OnSaveSettingsGraphicsData?.Invoke(defaultData);
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{

	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentSectionGraphicsMessage.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGraphics_Message");

		_textToggleTurnOnShadows.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGraphics_TextToggleTurnOnShadows");
		_textTurnOnSSAO.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGraphics_TextToggleTurnOnSSAO");
	}
}
