using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionAudioController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private Button[] _buttonsChangeLanguage;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsAudioData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsAudioData;


	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		GameObject[] buttonsChangeLanguage)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;

		_buttonsChangeLanguage = new Button[buttonsChangeLanguage.Length];

		for (int i = 0; i < _buttonsChangeLanguage.Length; i++)
		{
			_buttonsChangeLanguage[i] = buttonsChangeLanguage[i].GetComponent<Button>();
		}

		_buttonsChangeLanguage[0].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.Russian));
		_buttonsChangeLanguage[1].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.English));

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionAudio Initialized");
	}


	private void DisableButtons()
	{
		foreach (var button in _buttonsChangeLanguage)
		{
			if (button != null) button.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var button in _buttonsChangeLanguage)
		{
			if (button != null) button.interactable = true;
		}
	}


	private void ChangeLanguage(LanguagesEnum language)
	{
		_bootstrap.ChangeLanguage(language);
		Debug.Log("Changed Language to: " + language);
	}

	public void SaveSettingsAudio()
	{
		var currentData = new PlayerPrefsData();

		currentData.Language = _localizationManager.CurrentLanguage.ToString();

		OnSaveSettingsAudioData?.Invoke(currentData);
	}



	public void ResetSettingsAudio()
	{
		OnResetSettingsAudioData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			Language = _localizationManager.CurrentLanguage.ToString(),
		};

		OnSaveSettingsAudioData?.Invoke(defaultData);
	}
}
