using UnityEngine;
using TMPro;

public class PauseSubMenuSettingsSectionGraphicsController : MonoBehaviour
{
	private LocalizationManager _localizationManager;

	private GameObject TEXT_NO_GRAPHICS_SETTINGS_YET;
	private TextMeshProUGUI TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGraphicsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGraphicsData;

	public void Initialize(
		LocalizationManager localizationManager,
		ViewModelPauseSubMenuSettingsSectionGraphics viewModelPauseSubMenuSettingsSectionGraphics)
	{
		_localizationManager = localizationManager;

		TEXT_NO_GRAPHICS_SETTINGS_YET = viewModelPauseSubMenuSettingsSectionGraphics.TEXT_NO_GRAPHICS_SETTINGS_YET;
		TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET = viewModelPauseSubMenuSettingsSectionGraphics.TEXT_NO_GRAPHICS_SETTINGS_YET.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		Debug.Log("SettingsSectionGraphicsInitialized");
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

		TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGraphics_NO_GRAPHICS_SETTINGS");
	}
}
