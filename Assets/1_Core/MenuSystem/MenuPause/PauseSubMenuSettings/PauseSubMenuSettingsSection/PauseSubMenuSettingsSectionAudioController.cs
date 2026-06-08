using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseSubMenuSettingsSectionAudioController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private GameObject[] _buttonsChangeLanguage;
	private Button[] _buttonsComponentsChangeLanguage;
	private GameObject _textChangeLanguage;
	private TextMeshProUGUI _textComponentChangeLanguage;

	private GameObject _sliderVolumeGeneral;
	private Slider _sliderComponentVolumeGeneral;
	private float _currentValueVolumeGeneral;
	private GameObject _textNumberSliderVolumeGeneral;
	private TextMeshProUGUI _textComponentNumberSliderVolumeGeneral;
	private GameObject _textSliderVolumeGeneral;
	private TextMeshProUGUI _textComponentSliderVolumeGeneral;

	private GameObject _sliderVolumeEnvironment;
	private Slider _sliderComponentVolumeEnvironment;
	private float _currentValueVolumeEnvironment;
	private GameObject _textNumberSliderVolumeEnvironment;
	private TextMeshProUGUI _textComponentNumberSliderVolumeEnvironment;
	private GameObject _textSliderVolumeEnvironment;
	private TextMeshProUGUI _textComponentSliderVolumeEnvironment;

	private GameObject _sliderVolumeEffects;
	private Slider _sliderComponentVolumeEffects;
	private float _currentValueVolumeEffects;
	private GameObject _textNumberSliderVolumeEffects;
	private TextMeshProUGUI _textComponentNumberSliderVolumeEffects;
	private GameObject _textSliderVolumeEffects;
	private TextMeshProUGUI _textComponentSliderVolumeEffects;

	private GameObject _sliderVolumeVoices;
	private Slider _sliderComponentVolumeVoices;
	private float _currentValueVolumeVoices;
	private GameObject _textNumberSliderVolumeVoices;
	private TextMeshProUGUI _textComponentNumberSliderVolumeVoices;
	private GameObject _textSliderVolumeVoices;
	private TextMeshProUGUI _textComponentSliderVolumeVoices;

	private GameObject _sliderVolumeMusicAmbience;
	private Slider _sliderComponentVolumeMusicAmbience;
	private float _currentValueVolumeMusicAmbience;
	private GameObject _textNumberSliderVolumeMusicAmbience;
	private TextMeshProUGUI _textComponentNumberSliderVolumeMusicAmbience;
	private GameObject _textSliderVolumeMusicAmbience;
	private TextMeshProUGUI _textComponentSliderVolumeMusicAmbience;

	private GameObject _sliderVolumeMusicIngame;
	private Slider _sliderComponentVolumeMusicIngame;
	private float _currentValueVolumeMusicIngame;
	private GameObject _textNumberSliderVolumeMusicIngame;
	private TextMeshProUGUI _textComponentNumberSliderVolumeMusicIngame;
	private GameObject _textSliderVolumeMusicIngame;
	private TextMeshProUGUI _textComponentSliderVolumeMusicIngame;

	private const float _MIN_VALUE_VOLUME = 0f;
	private const float _MAX_VALUE_VOLUME = 100f;

	public delegate void VolumeEventHandle(float newVolumeValue, float MIN_VALUE_VOLUME, float MAX_VALUE_VOLUME);
	public event VolumeEventHandle OnVolumeGeneralChanged;
	public event VolumeEventHandle OnVolumeEnvironmentChanged;
	public event VolumeEventHandle OnVolumeEffectsChanged;
	public event VolumeEventHandle OnVolumeVoicesChanged;
	public event VolumeEventHandle OnVolumeMusicAmbienceChanged;
	public event VolumeEventHandle OnVolumeMusicIngameChanged;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsAudioData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsAudioData;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionAudio viewModelPauseSubMenuSettingsAudio)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;

		_buttonsChangeLanguage = new GameObject[viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage.Length];
		_buttonsComponentsChangeLanguage = new Button[viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage.Length];
		for (int i = 0; i < _buttonsChangeLanguage.Length; i++)
		{
			_buttonsComponentsChangeLanguage[i] = viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage[i].GetComponent<Button>();
		}
		_buttonsComponentsChangeLanguage[0].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.Russian));
		_buttonsComponentsChangeLanguage[1].onClick.AddListener(() => ChangeLanguage(LanguagesEnum.English));
		_textChangeLanguage = viewModelPauseSubMenuSettingsAudio.TextChangeLanguage;
		_textComponentChangeLanguage = viewModelPauseSubMenuSettingsAudio.TextChangeLanguage.GetComponent<TextMeshProUGUI>();

		_sliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.SliderVolumeGeneral;
		_sliderComponentVolumeGeneral = viewModelPauseSubMenuSettingsAudio.SliderVolumeGeneral.GetComponent<Slider>();
		_sliderComponentVolumeGeneral.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeGeneral.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeGeneral.onValueChanged.AddListener(SetVolumeGeneral);
		_textNumberSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeGeneral;
		_textComponentNumberSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeGeneral.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeGeneral;
		_textComponentSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeGeneral.GetComponent<TextMeshProUGUI>();

		_sliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.SliderVolumeEnvironment;
		_sliderComponentVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.SliderVolumeEnvironment.GetComponent<Slider>();
		_sliderComponentVolumeEnvironment.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.onValueChanged.AddListener(SetVolumeEnvironment);
		_textNumberSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEnvironment;
		_textComponentNumberSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEnvironment.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEnvironment;
		_textComponentSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEnvironment.GetComponent<TextMeshProUGUI>();

		_sliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.SliderVolumeEffects;
		_sliderComponentVolumeEffects = viewModelPauseSubMenuSettingsAudio.SliderVolumeEffects.GetComponent<Slider>();
		_sliderComponentVolumeEffects.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeEffects.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEffects.onValueChanged.AddListener(SetVolumeEffects);
		_textNumberSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEffects;
		_textComponentNumberSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEffects.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEffects;
		_textComponentSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEffects.GetComponent<TextMeshProUGUI>();

		_sliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.SliderVolumeVoices;
		_sliderComponentVolumeVoices = viewModelPauseSubMenuSettingsAudio.SliderVolumeVoices.GetComponent<Slider>();
		_sliderComponentVolumeVoices.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeVoices.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeVoices.onValueChanged.AddListener(SetVolumeVoices);
		_textNumberSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeVoices;
		_textComponentNumberSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeVoices.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeVoices;
		_textComponentSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeVoices.GetComponent<TextMeshProUGUI>();

		_sliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicAmbience;
		_sliderComponentVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicAmbience.GetComponent<Slider>();
		_sliderComponentVolumeMusicAmbience.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.onValueChanged.AddListener(SetVolumeMusicAmbience);
		_textNumberSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicAmbience;
		_textComponentNumberSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicAmbience.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicAmbience;
		_textComponentSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicAmbience.GetComponent<TextMeshProUGUI>();

		_sliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicIngame;
		_sliderComponentVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicIngame.GetComponent<Slider>();
		_sliderComponentVolumeMusicIngame.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.onValueChanged.AddListener(SetVolumeMusicIngame);
		_textNumberSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicIngame;
		_textComponentNumberSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicIngame.GetComponent<TextMeshProUGUI>();
		_textSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicIngame;
		_textComponentSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicIngame.GetComponent<TextMeshProUGUI>();

		SetVolumeGeneral(_MAX_VALUE_VOLUME);
		SetVolumeEnvironment(_MAX_VALUE_VOLUME);
		SetVolumeEffects(_MAX_VALUE_VOLUME);
		SetVolumeVoices(_MAX_VALUE_VOLUME);
		SetVolumeMusicAmbience(_MAX_VALUE_VOLUME);
		SetVolumeMusicIngame(_MAX_VALUE_VOLUME);

		_sliderComponentVolumeGeneral.value = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.value = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEffects.value = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeVoices.value = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.value = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.value = _MAX_VALUE_VOLUME;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionAudioController Initialized");
	}

	private void DisableButtons()
	{
		foreach (var button in _buttonsComponentsChangeLanguage)
		{
			button.interactable = false;
		}
	}

	private void EnableButtons()
	{
		foreach (var button in _buttonsComponentsChangeLanguage)
		{
			button.interactable = true;
		}
	}

	private void ChangeLanguage(LanguagesEnum language)
	{
		_bootstrap.ChangeLanguage(language);
		Debug.Log("Changed Language to: " + language);
	}

	public void SetVolumeGeneral(float newVolumeGeneral)
	{
		_currentValueVolumeGeneral = newVolumeGeneral;

		_textComponentNumberSliderVolumeGeneral.text = ((int)newVolumeGeneral).ToString();

		OnVolumeGeneralChanged?.Invoke(newVolumeGeneral, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEnvironment(float newVolumeEnvironment)
	{
		_currentValueVolumeEnvironment = newVolumeEnvironment;

		_textComponentNumberSliderVolumeEnvironment.text = ((int)newVolumeEnvironment).ToString();

		OnVolumeEnvironmentChanged?.Invoke(newVolumeEnvironment, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEffects(float newVolumeEffects)
	{
		_currentValueVolumeEffects = newVolumeEffects;

		_textComponentNumberSliderVolumeEffects.text = ((int)newVolumeEffects).ToString();

		OnVolumeEffectsChanged?.Invoke(newVolumeEffects, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeVoices(float newVolumeVoices)
	{
		_currentValueVolumeVoices = newVolumeVoices;

		_textComponentNumberSliderVolumeVoices.text = ((int)newVolumeVoices).ToString();

		OnVolumeVoicesChanged?.Invoke(newVolumeVoices, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicAmbience(float newVolumeMusicAmbience)
	{
		_currentValueVolumeMusicAmbience = newVolumeMusicAmbience;

		_textComponentNumberSliderVolumeMusicAmbience.text = ((int)newVolumeMusicAmbience).ToString();

		OnVolumeMusicAmbienceChanged?.Invoke(newVolumeMusicAmbience, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicIngame(float newVolumeMusicIngame)
	{
		_currentValueVolumeMusicIngame = newVolumeMusicIngame;

		_textComponentNumberSliderVolumeMusicIngame.text = ((int)newVolumeMusicIngame).ToString();

		OnVolumeMusicIngameChanged?.Invoke(newVolumeMusicIngame, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SaveSettingsAudio()
	{
		var currentData = new PlayerPrefsData();

		currentData.CurrentLanguage = _localizationManager.CurrentLanguage.ToString();

		OnSaveSettingsAudioData?.Invoke(currentData);
	}

	public void ResetSettingsAudio()
	{
		OnResetSettingsAudioData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			CurrentLanguage = _localizationManager.CurrentLanguage.ToString(),
		};

		OnSaveSettingsAudioData?.Invoke(defaultData);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentChangeLanguage.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextChangeLanguage");

		_textComponentSliderVolumeGeneral.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeGeneral");
		_textComponentSliderVolumeEnvironment.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeEnvironment");
		_textComponentSliderVolumeEffects.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeEffects");
		_textComponentSliderVolumeVoices.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeVoices");
		_textComponentSliderVolumeMusicAmbience.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeMusicAmbience");
		_textComponentSliderVolumeMusicIngame.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionAudio_TextSliderVolumeMusicIngame");
	}
}
