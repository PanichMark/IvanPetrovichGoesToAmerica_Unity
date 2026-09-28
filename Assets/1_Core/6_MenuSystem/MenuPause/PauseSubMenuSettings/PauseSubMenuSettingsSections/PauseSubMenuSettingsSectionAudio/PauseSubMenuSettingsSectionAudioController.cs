using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class PauseSubMenuSettingsSectionAudioController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;
	private AudioMixer _audioMixer;

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
	private const float _DEFAULT_VALUE_VOLUME = 50f;

	/*
	public delegate void VolumeEventHandle(float newVolumeValue, float MIN_VALUE_VOLUME, float MAX_VALUE_VOLUME);
	public event VolumeEventHandle OnVolumeGeneralChanged;
	public event VolumeEventHandle OnVolumeEnvironmentChanged;
	public event VolumeEventHandle OnVolumeEffectsChanged;
	public event VolumeEventHandle OnVolumeVoicesChanged;
	public event VolumeEventHandle OnVolumeMusicAmbienceChanged;
	public event VolumeEventHandle OnVolumeMusicIngameChanged;
	*/

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionAudio viewModelPauseSubMenuSettingsAudio,
		AudioMixer audioMixer)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;
		_pauseMenuController = pauseMenuController;
		_audioMixer = audioMixer;

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

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_playerPrefsSettingsController.OnApplySettingsSectionGeneralPlayerPrefs += ApplySystemLoadedSettings;

		Debug.Log("SettingsSectionAudioController Initialized");
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetVolumeGeneral(data.VolumeGeneral);
		_sliderComponentVolumeGeneral.value = data.VolumeGeneral;

		SetVolumeEnvironment(data.VolumeEnvironment);
		_sliderComponentVolumeEnvironment.value = data.VolumeEnvironment;

		SetVolumeEffects(data.VolumeEffects);
		_sliderComponentVolumeEffects.value = data.VolumeEffects;

		SetVolumeVoices(data.VolumeVoices);
		_sliderComponentVolumeVoices.value = data.VolumeVoices;

		SetVolumeMusicAmbience(data.VolumeMusicAmbience);
		_sliderComponentVolumeMusicAmbience.value = data.VolumeMusicAmbience;

		SetVolumeMusicIngame(data.VolumeMusicIngame);
		_sliderComponentVolumeMusicIngame.value = data.VolumeMusicIngame;
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
		ApplyMixerVolume(AudioMixerGroupsEnum.Master, newVolumeGeneral);
		//OnVolumeGeneralChanged?.Invoke(newVolumeGeneral, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEnvironment(float newVolumeEnvironment)
	{
		_currentValueVolumeEnvironment = newVolumeEnvironment;
		_textComponentNumberSliderVolumeEnvironment.text = ((int)newVolumeEnvironment).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeEnvironment, newVolumeEnvironment);
		//OnVolumeEnvironmentChanged?.Invoke(newVolumeEnvironment, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEffects(float newVolumeEffects)
	{
		_currentValueVolumeEffects = newVolumeEffects;
		_textComponentNumberSliderVolumeEffects.text = ((int)newVolumeEffects).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeEffects, newVolumeEffects);
		//OnVolumeEffectsChanged?.Invoke(newVolumeEffects, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeVoices(float newVolumeVoices)
	{
		_currentValueVolumeVoices = newVolumeVoices;
		_textComponentNumberSliderVolumeVoices.text = ((int)newVolumeVoices).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeVoices, newVolumeVoices);
		//OnVolumeVoicesChanged?.Invoke(newVolumeVoices, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicAmbience(float newVolumeMusicAmbience)
	{
		_currentValueVolumeMusicAmbience = newVolumeMusicAmbience;
		_textComponentNumberSliderVolumeMusicAmbience.text = ((int)newVolumeMusicAmbience).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeMusicAmbience, newVolumeMusicAmbience);
		//OnVolumeMusicAmbienceChanged?.Invoke(newVolumeMusicAmbience, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicIngame(float newVolumeMusicIngame)
	{
		_currentValueVolumeMusicIngame = newVolumeMusicIngame;
		_textComponentNumberSliderVolumeMusicIngame.text = ((int)newVolumeMusicIngame).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeMusicIngame, newVolumeMusicIngame);
		//OnVolumeMusicIngameChanged?.Invoke(newVolumeMusicIngame, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SaveSettingsAudio()
	{
		var currentData = new PlayerPrefsData();

		currentData.Language = _localizationManager.CurrentLanguage.ToString();
		currentData.VolumeGeneral = (int)_currentValueVolumeGeneral;
		currentData.VolumeEnvironment = (int)_currentValueVolumeEnvironment;
		currentData.VolumeEffects = (int)_currentValueVolumeEffects;
		currentData.VolumeVoices = (int)_currentValueVolumeVoices;
		currentData.VolumeMusicAmbience = (int)_currentValueVolumeMusicAmbience;
		currentData.VolumeMusicIngame = (int)_currentValueVolumeMusicIngame;

		_playerPrefsSettingsController.SaveSettingsAudio(currentData);
	}

	public void ResetSettingsAudio()
	{
		_playerPrefsSettingsController.ResetSettingsAudio();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			Language = _localizationManager.CurrentLanguage.ToString(),
			VolumeGeneral = (int)_DEFAULT_VALUE_VOLUME,
			VolumeEnvironment = (int)_DEFAULT_VALUE_VOLUME,
			VolumeEffects = (int)_DEFAULT_VALUE_VOLUME,
			VolumeVoices = (int)_DEFAULT_VALUE_VOLUME,
			VolumeMusicAmbience = (int)_DEFAULT_VALUE_VOLUME,
			VolumeMusicIngame = (int)_DEFAULT_VALUE_VOLUME,
		};

		_playerPrefsSettingsController.SaveSettingsAudio(defaultData);

		_sliderComponentVolumeGeneral.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeEffects.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeVoices.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.value = _DEFAULT_VALUE_VOLUME;

		SetVolumeGeneral(_DEFAULT_VALUE_VOLUME);
		SetVolumeEnvironment(_DEFAULT_VALUE_VOLUME);
		SetVolumeEffects(_DEFAULT_VALUE_VOLUME);
		SetVolumeVoices(_DEFAULT_VALUE_VOLUME);
		SetVolumeMusicAmbience(_DEFAULT_VALUE_VOLUME);
		SetVolumeMusicIngame(_DEFAULT_VALUE_VOLUME);
	}

	private void ApplyMixerVolume(AudioMixerGroupsEnum group, float value)
	{
		float normalized = Mathf.InverseLerp(_MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME, value);
		float db = (normalized > Mathf.Epsilon) ? Mathf.Log10(normalized) * 20f : -80f;
		_audioMixer.SetFloat(group.ToString(), db);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentChangeLanguage.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextChangeLanguage");

		_textComponentSliderVolumeGeneral.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeGeneral");
		_textComponentSliderVolumeEnvironment.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeEnvironment");
		_textComponentSliderVolumeEffects.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeEffects");
		_textComponentSliderVolumeVoices.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeVoices");
		_textComponentSliderVolumeMusicAmbience.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeMusicAmbience");
		_textComponentSliderVolumeMusicIngame.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeMusicIngame");
	}
}