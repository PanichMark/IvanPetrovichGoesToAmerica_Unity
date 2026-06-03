using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionGeneralController : MonoBehaviour
{
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;










	private GameObject _dropdownLimitFPS;
	private TMP_Dropdown _dropdownComponentLimitFPS;
	private GameObject _textDropdownLimitFPS;
	private TextMeshProUGUI _textComponentDropdownLimitFPS;

	private GameObject _sliderCameraFOV;
	private Slider _sliderComponentCameraFOV;
	public float CurrentValueCameraFOV { get; private set; }
	private const float _MIN_VALUE_CAMERA_FOV = 60f;
	public float MIN_VALUE_CAMERA_FOV => _MIN_VALUE_CAMERA_FOV;
	private const float _MAX_VALUE_CAMERA_FOV = 120f;
	public float MAX_VALUE_CAMERA_FOV => _MAX_VALUE_CAMERA_FOV;
	private GameObject _textNumberSliderCameraFOV;
	private TextMeshProUGUI _textComponentNumberSliderCameraFOV;
	private GameObject _textSliderCameraFOV;
	private TextMeshProUGUI _textComponentSliderCameraFOV;

	private GameObject _sliderScreenBrightness;
	private Slider _sliderComponentScreenBrightness;
	private float _currentValueScreenBrightness;
	private const float _MIN_VALUE_SCREEN_BRIGHTNESS = 0f;
	private const float _MAX_VALUE_SCREEN_BRIGHTNESS = 100f;
	private GameObject _textNumberSliderScreenBrightness;
	private TextMeshProUGUI _textComponentNumberSliderScreenBrightness;
	private GameObject _textSliderScreenBrightness;
	private TextMeshProUGUI _textComponentSliderScreenBrightness;








	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGeneralData;

	public delegate void CameraFOVeventHandler(float newCameraFOV, float MIN_VALUE_CAMERA_FOV, float MAX_VALUE_CAMERA_FOV);
	public event CameraFOVeventHandler OnCameraFOVchanged;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGeneralData;

	public delegate void SavePlayerPrefsCameraSettingsEventHandler();
	public event SavePlayerPrefsCameraSettingsEventHandler OnSaveCameraSettingsData;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionGeneral viewModelPauseSubMenuSettings)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetComponent<TextMeshProUGUI>();


		_dropdownLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS;
		_dropdownComponentLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS.GetComponent<TMP_Dropdown>();
		_dropdownComponentLimitFPS.onValueChanged.AddListener(SetFPSLimit);
		_textDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS;
		_textComponentDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS.GetComponent<TextMeshProUGUI>();

		_sliderCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV;
		_sliderComponentCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV.GetComponent<Slider>();
		_sliderComponentCameraFOV.minValue = _MIN_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.maxValue = _MAX_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.onValueChanged.AddListener(SetCameraFOV);
		_textNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetComponent<TextMeshProUGUI>();
		_textSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV;
		_textComponentSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV.GetComponent<TextMeshProUGUI>();

		_sliderScreenBrightness = viewModelPauseSubMenuSettings.SliderScreenBrightness;
		_sliderComponentScreenBrightness = viewModelPauseSubMenuSettings.SliderScreenBrightness.GetComponent<Slider>();
		_sliderComponentScreenBrightness.minValue = _MIN_VALUE_SCREEN_BRIGHTNESS;
		_sliderComponentScreenBrightness.maxValue = _MAX_VALUE_SCREEN_BRIGHTNESS;
		_sliderComponentScreenBrightness.onValueChanged.AddListener(SetScreenBrightness);
		_textNumberSliderScreenBrightness = viewModelPauseSubMenuSettings.NumberSliderScreenBrightness;
		_textComponentNumberSliderScreenBrightness = viewModelPauseSubMenuSettings.NumberSliderScreenBrightness.GetComponent<TextMeshProUGUI>();
		_textSliderScreenBrightness = viewModelPauseSubMenuSettings.TextSliderScreenBrightness;
		_textComponentSliderScreenBrightness = viewModelPauseSubMenuSettings.TextSliderScreenBrightness.GetComponent<TextMeshProUGUI>();











		SetFPSLimit(1);

		SetScreenBrightness(100);
		_sliderComponentScreenBrightness.value = 100;

		_gameController.OnOpenMainMenu += () => OnCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
	
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionGeneral Initialized");
	}

	private void DisableButtons()
	{
		_sliderComponentCameraFOV.interactable = false;
	}

	private void EnableButtons()
	{
		 _sliderComponentCameraFOV.interactable = true;
	}

	public void SaveSettingsGeneral()
	{
		var currentData = new PlayerPrefsData();

		OnSaveCameraSettingsData?.Invoke();

		currentData.FOV = CurrentValueCameraFOV;

		OnSaveSettingsGeneralData?.Invoke(currentData);
	}

	public void GetCameraCurrentFOV(float FOV)
	{
		CurrentValueCameraFOV = FOV;
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetCameraFOV(data.FOV);
		_sliderComponentCameraFOV.value = data.FOV;
		CurrentValueCameraFOV = data.FOV;
	}

	public void ResetSettingsGeneral()
	{
		OnResetSettingsGeneralData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			FOV = _MIN_VALUE_CAMERA_FOV,
		};

		OnSaveSettingsGeneralData?.Invoke(defaultData);

		SetCameraFOV(_MIN_VALUE_CAMERA_FOV);
		_sliderComponentCameraFOV.value = _MIN_VALUE_CAMERA_FOV;
	}

	public void SetFPSLimit(int dropdownFPSlimitSlot)
	{
		int newFPSlimit;

		if (dropdownFPSlimitSlot == 0)
		{
			newFPSlimit = 30;
		}
		else if (dropdownFPSlimitSlot == 1)
		{
			newFPSlimit = 60;
		}
		else if (dropdownFPSlimitSlot == 2)
		{
			newFPSlimit = 90;
		}
		else if (dropdownFPSlimitSlot == 3)
		{
			newFPSlimit = 144;
		}
		else
		{
			newFPSlimit = 999;
		}

		Application.targetFrameRate = newFPSlimit;
	}

	public void SetCameraFOV(float newCameraFOV)
	{
		CurrentValueCameraFOV = newCameraFOV;

		_textComponentNumberSliderCameraFOV.text = ((int)newCameraFOV).ToString();

		if (!_gameController.IsMainMenuOpen)
		{
			OnCameraFOVchanged?.Invoke(newCameraFOV, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
		else
		{
			OnCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
	}

	public void SetScreenBrightness(float newScreenBrightness)
	{
		_currentValueScreenBrightness = newScreenBrightness;

		_textComponentNumberSliderScreenBrightness.text = ((int)newScreenBrightness).ToString();
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentSliderCameraFOV.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextSliderCameraFOV");
		_textComponentSliderScreenBrightness.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextSliderScreenBrightness");

		List<string> DropdownLimitFPSlocalizedOptions = new List<string>
		{
		_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS30"),
		_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS60"),
		_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS90"),
		_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS144"),
		_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPSunlimited")
		};
		_dropdownComponentLimitFPS.ClearOptions();
		_dropdownComponentLimitFPS.AddOptions(DropdownLimitFPSlocalizedOptions);
		_textComponentDropdownLimitFPS.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownFPSlimit");
	}
}
