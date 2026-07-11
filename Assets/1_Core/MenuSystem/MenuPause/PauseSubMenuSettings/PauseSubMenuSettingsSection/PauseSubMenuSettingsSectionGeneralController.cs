using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionGeneralController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private MenuManager _menuManager;
	private PauseMenuController _pausedMenuController;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;

	private GameObject _dropdownScreenResolution;
	private TMP_Dropdown _dropdownComponentScreenResolution;
	private GameObject _textDropdownScreenResolution;
	private TextMeshProUGUI _textComponentDropdownScreenResolution;

	private GameObject _dropdownWindowType;
	private TMP_Dropdown _dropdownComponentWindowType;
	private GameObject _textDropdownWindowType;
	private TextMeshProUGUI _textComponentDropdownWindowType;

	private GameObject _dropdownLimitFPS;
	private TMP_Dropdown _dropdownComponentLimitFPS;
	private GameObject _textDropdownLimitFPS;
	private TextMeshProUGUI _textComponentDropdownLimitFPS;
	private int _currentFPSlimit;

	private GameObject _dropdownHUDType;
	private TMP_Dropdown _dropdownComponentHUDType;
	private GameObject _textDropdownHUDType;
	private TextMeshProUGUI _textComponentDropdownHUDType;
	public delegate void HUDtypeHandler();
	public event HUDtypeHandler OnHUDfull;
	public event HUDtypeHandler OnHUDdialoguesOnly;
	public event HUDtypeHandler OnHUDdialoguesHide;
	public event HUDtypeHandler OnHUDturnOff;

	private GameObject _dropdownWeaponWheelType;
	private TMP_Dropdown _dropdownComponentWeaponWheelType;
	private GameObject _textDropdownWeaponWheelType;
	private TextMeshProUGUI _textComponentDropdownWeaponWheelType;

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
	public delegate void CameraFOVeventHandler(float newCameraFOV, float MIN_VALUE_CAMERA_FOV, float MAX_VALUE_CAMERA_FOV);
	public event CameraFOVeventHandler OnCameraFOVchanged;

	private GameObject _sliderScreenBrightness;
	private Slider _sliderComponentScreenBrightness;
	private float _currentValueScreenBrightness;
	private const float _MIN_VALUE_SCREEN_BRIGHTNESS = 0f;
	private const float _MAX_VALUE_SCREEN_BRIGHTNESS = 100f;
	private GameObject _textNumberSliderScreenBrightness;
	private TextMeshProUGUI _textComponentNumberSliderScreenBrightness;
	private GameObject _textSliderScreenBrightness;
	private TextMeshProUGUI _textComponentSliderScreenBrightness;

	private GameObject _buttonGameDifficulty;
	private Button _buttonComponentGameDifficulty;
	private GameObject _textButtonGameDifficulty;
	private TextMeshProUGUI _textComponentButtonGameDifficulty;
	public delegate void SubMenuChooseGameDifficultyHandler();
	public event SubMenuChooseGameDifficultyHandler OnOpenSubMenuGameDifficulty;
	public event SubMenuChooseGameDifficultyHandler OnCloseSubMenuGameDifficulty;

	private GameObject _toggleShowIngameHints;
	private Toggle _toggleComponentShowIngameHints;
	private GameObject _textToggleShowIngameHints;
	private TextMeshProUGUI _textComponentToggleShowIngameHints;
	private bool _areIngameHintsEnabled;
	public delegate void IngameHintsVisibilityHandler();
	public event IngameHintsVisibilityHandler OnShowIngameHints;
	public event IngameHintsVisibilityHandler OnHideIngameHints;

	private GameObject _toggleShowBlood;
	private Toggle _toggleComponentShowBlood;
	private GameObject _textToggleShowBlood;
	private TextMeshProUGUI _textComponentToggleShowBlood;
	private bool _isBloodEnabled;
	public delegate void BloodVisibilityHandler();
	public event BloodVisibilityHandler OnShowBlood;
	public event BloodVisibilityHandler OnHideBlood;

	public delegate void SavePlayerPrefsCameraSettingsEventHandler();
	public event SavePlayerPrefsCameraSettingsEventHandler OnSaveCameraSettingsData;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGeneralData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGeneralData;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PauseMenuController pausedMenuController,
		PauseSubMenuSettingsController pauseSubMenuSettingsController,
		ViewModelPauseSubMenuSettingsSectionGeneral viewModelPauseSubMenuSettings)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_menuManager = menuManager;
		_pausedMenuController = pausedMenuController;
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetComponent<TextMeshProUGUI>();

		_dropdownScreenResolution = viewModelPauseSubMenuSettings.DropdownScreenResolution;
		_dropdownComponentScreenResolution = viewModelPauseSubMenuSettings.DropdownScreenResolution.GetComponent<TMP_Dropdown>();
		_dropdownComponentScreenResolution.onValueChanged.AddListener(SetScreenResolution);
		_textDropdownScreenResolution = viewModelPauseSubMenuSettings.TextDropdownScreenResolution;
		_textComponentDropdownScreenResolution = viewModelPauseSubMenuSettings.TextDropdownScreenResolution.GetComponent<TextMeshProUGUI>();

		_dropdownWindowType = viewModelPauseSubMenuSettings.DropdownWindowType;
		_dropdownComponentWindowType = viewModelPauseSubMenuSettings.DropdownWindowType.GetComponent<TMP_Dropdown>();
		_dropdownComponentWindowType.onValueChanged.AddListener(SetWindowType);
		_textDropdownWindowType = viewModelPauseSubMenuSettings.TextDropdownWindowType;
		_textComponentDropdownWindowType = viewModelPauseSubMenuSettings.TextDropdownWindowType.GetComponent<TextMeshProUGUI>();

		_dropdownLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS;
		_dropdownComponentLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS.GetComponent<TMP_Dropdown>();
		_dropdownComponentLimitFPS.onValueChanged.AddListener(SetFPSlimit);
		_textDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS;
		_textComponentDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS.GetComponent<TextMeshProUGUI>();

		_dropdownHUDType = viewModelPauseSubMenuSettings.DropdownHUDType;
		_dropdownComponentHUDType = viewModelPauseSubMenuSettings.DropdownHUDType.GetComponent<TMP_Dropdown>();
		_dropdownComponentHUDType.onValueChanged.AddListener(SetHUDType);
		_textDropdownHUDType = viewModelPauseSubMenuSettings.TextDropdownHUDType;
		_textComponentDropdownHUDType = viewModelPauseSubMenuSettings.TextDropdownHUDType.GetComponent<TextMeshProUGUI>();

		_dropdownWeaponWheelType = viewModelPauseSubMenuSettings.DropdownWeaponWheelType;
		_dropdownComponentWeaponWheelType = viewModelPauseSubMenuSettings.DropdownWeaponWheelType.GetComponent<TMP_Dropdown>();
		_dropdownComponentWeaponWheelType.onValueChanged.AddListener(SetWeaponWheelType);
		_textDropdownWeaponWheelType = viewModelPauseSubMenuSettings.TextDropdownWeaponWheelType;
		_textComponentDropdownWeaponWheelType = viewModelPauseSubMenuSettings.TextDropdownWeaponWheelType.GetComponent<TextMeshProUGUI>();

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

		_buttonGameDifficulty = viewModelPauseSubMenuSettings.ButtonGameDifficulty;
		_buttonComponentGameDifficulty = viewModelPauseSubMenuSettings.ButtonGameDifficulty.GetComponent<Button>();
		_buttonComponentGameDifficulty.onClick.AddListener(() => OpenSubMenuChooseGameDifficulty());
		_textButtonGameDifficulty = viewModelPauseSubMenuSettings.TextButtonGameDifficulty;
		_textComponentButtonGameDifficulty = viewModelPauseSubMenuSettings.TextButtonGameDifficulty.GetComponent<TextMeshProUGUI>();

		_toggleShowIngameHints = viewModelPauseSubMenuSettings.ToggleShowIngameHints;
		_toggleComponentShowIngameHints = viewModelPauseSubMenuSettings.ToggleShowIngameHints.GetComponent<Toggle>();
		_toggleComponentShowIngameHints.onValueChanged.AddListener(SetShowIngameHints);
		_textToggleShowIngameHints = viewModelPauseSubMenuSettings.TextToggleShowIngameHints;
		_textComponentToggleShowIngameHints = viewModelPauseSubMenuSettings.TextToggleShowIngameHints.GetComponent<TextMeshProUGUI>();

		_toggleShowBlood = viewModelPauseSubMenuSettings.ToggleShowBlood;
		_toggleComponentShowBlood = viewModelPauseSubMenuSettings.ToggleShowBlood.GetComponent<Toggle>();
		_toggleComponentShowBlood.onValueChanged.AddListener(SetShowBlood);
		_textToggleShowBlood = viewModelPauseSubMenuSettings.TextToggleShowBlood;
		_textComponentToggleShowBlood = viewModelPauseSubMenuSettings.TextToggleShowBlood.GetComponent<TextMeshProUGUI>();

		SetScreenBrightness(100);
		_sliderComponentScreenBrightness.value = 100;

		SetShowIngameHints(true);
		SetShowBlood(true);

		_gameController.OnOpenMainMenu += () => OnCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
	
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		Debug.Log("SettingsSectionGeneralController Initialized");
	}

	private void Update()
	{
		if (_inputDevice.GetKeyPauseMenu() && _menuManager.PauseMenuLevel.Count == 3 && !_menuManager.IsConfirmationOnExitToMainMenuOpened && !_pausedMenuController.IsPauseConfirmMenuOpened)
		{
			CloseSubMenuChooseGameDifficulty();
		}
	}

	public void SaveSettingsGeneral()
	{
		var currentData = new PlayerPrefsData();

		OnSaveCameraSettingsData?.Invoke();

		currentData.CameraFOV = CurrentValueCameraFOV;
		currentData.FPSlimit = _currentFPSlimit;

		OnSaveSettingsGeneralData?.Invoke(currentData);
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetFPSlimit(data, data.FPSlimit);

		SetCameraFOV(data.CameraFOV);
		_sliderComponentCameraFOV.value = data.CameraFOV;
		CurrentValueCameraFOV = data.CameraFOV;
	}

	public void ResetSettingsGeneral()
	{
		OnResetSettingsGeneralData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			FPSlimit = 60,
			CameraFOV = _MIN_VALUE_CAMERA_FOV,
		};

		OnSaveSettingsGeneralData?.Invoke(defaultData);

		SetFPSlimit(defaultData, 60);

		SetCameraFOV(_MIN_VALUE_CAMERA_FOV);
		_sliderComponentCameraFOV.value = _MIN_VALUE_CAMERA_FOV;
	}

	public void SetScreenResolution(int dropdownScreenResolutionSlot)
	{
		if (dropdownScreenResolutionSlot == 0)
		{
			Screen.SetResolution(1920, 1080, Screen.fullScreen);
		}
		else if (dropdownScreenResolutionSlot == 1)
		{
			Screen.SetResolution(2560, 1440, Screen.fullScreen);
		}
		else if (dropdownScreenResolutionSlot == 2)
		{
			Screen.SetResolution(3840, 2160, Screen.fullScreen);
		}
		else if (dropdownScreenResolutionSlot == 3)
		{
			Screen.SetResolution(1920, 1200, Screen.fullScreen);
		}
		else if (dropdownScreenResolutionSlot == 4)
		{
			Screen.SetResolution(1080, 1440, Screen.fullScreen);
		}
	}

	public void SetWindowType(int dropdownWindowTypeSlot)
	{
		if (dropdownWindowTypeSlot == 0)
		{
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		}
		else if (dropdownWindowTypeSlot == 1)
		{
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}
		else if (dropdownWindowTypeSlot == 2)
		{
			Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
		}
	}

	public void SetFPSlimit(int dropdownFPSlimitSlot)
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

		_currentFPSlimit = newFPSlimit;
		Application.targetFrameRate = newFPSlimit;
	}

	public void SetFPSlimit(PlayerPrefsData data, int newFPSlimit)
	{
		if (newFPSlimit == 30)
		{
			_dropdownComponentLimitFPS.value = 0;
		}
		if (newFPSlimit == 60)
		{
			_dropdownComponentLimitFPS.value = 1;
		}
		if (newFPSlimit == 90)
		{
			_dropdownComponentLimitFPS.value = 2;
		}
		if (newFPSlimit == 144)
		{
			_dropdownComponentLimitFPS.value = 3;
		}
		if (newFPSlimit == 999)
		{
			_dropdownComponentLimitFPS.value = 4;
		}

		_currentFPSlimit = newFPSlimit;
		Application.targetFrameRate = newFPSlimit;
	}

	public void SetHUDType(int dropdownHUDTypeSlot)
	{
		if (dropdownHUDTypeSlot == 0)
		{
			OnHUDfull?.Invoke();
		}
		else if (dropdownHUDTypeSlot == 1)
		{
			OnHUDdialoguesOnly?.Invoke();	
		}
		else if (dropdownHUDTypeSlot == 2)
		{
			OnHUDdialoguesHide?.Invoke();
		}
		else if (dropdownHUDTypeSlot == 3)
		{
			OnHUDturnOff?.Invoke();
		}
	}

	public void SetWeaponWheelType(int dropdownWeaponWheelTypeSlot)
	{
		if (dropdownWeaponWheelTypeSlot == 0)
		{
			_bootstrap.ChangeWeaponWheelType(WeaponWheelMenuTypes._2D);
		}
		else if (dropdownWeaponWheelTypeSlot == 1)
		{
			_bootstrap.ChangeWeaponWheelType(WeaponWheelMenuTypes._3D);
		}
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

	public void GetCameraCurrentFOV(float FOV)
	{
		CurrentValueCameraFOV = FOV;
	}

	public void SetScreenBrightness(float newScreenBrightness)
	{
		_currentValueScreenBrightness = newScreenBrightness;

		_textComponentNumberSliderScreenBrightness.text = ((int)newScreenBrightness).ToString();
	}

	private void OpenSubMenuChooseGameDifficulty()
	{
		Debug.Log("OPEN DIFFICULTY");
		_menuManager.PushPauseMenuLevel();
		_pauseSubMenuSettingsController.HideSettingsSubMenuCanvas();
		OnOpenSubMenuGameDifficulty?.Invoke();
	}

	private void CloseSubMenuChooseGameDifficulty()
	{
		Debug.Log("CLOSE DIFFICULTY");
		_menuManager.PopPauseMenuLevel();
		_pauseSubMenuSettingsController.ShowSettingsSubMenuCanvas();
		OnCloseSubMenuGameDifficulty?.Invoke();
	}

	public void SetShowIngameHints(bool isOn)
	{
		if (isOn)
		{
			OnShowIngameHints?.Invoke();
		}
		else
		{
			OnHideIngameHints?.Invoke();
		}

		_toggleComponentShowIngameHints.isOn = isOn;
	}

	public void SetShowBlood(bool isOn)
	{
		if (isOn)
		{
			OnShowBlood?.Invoke();
		}
		else
		{
			OnHideBlood?.Invoke();
		}

		_toggleComponentShowBlood.isOn = isOn;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentDropdownScreenResolution.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownScreenResolution");

		List<string> dropdownWindowTypelocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownWindowTypeFullscreen"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownWindowTypeBorderlessWindowed"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownWindowTypeWindowed"),
		};
		_dropdownComponentWindowType.ClearOptions();
		_dropdownComponentWindowType.AddOptions(dropdownWindowTypelocalizedOptions);
		_textComponentDropdownWindowType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownWindowType");

		List<string> dropdownLimitFPSlocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS30"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS60"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS90"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPS144"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownLimitFPSunlimited")
		};
		_dropdownComponentLimitFPS.ClearOptions();
		_dropdownComponentLimitFPS.AddOptions(dropdownLimitFPSlocalizedOptions);
		_textComponentDropdownLimitFPS.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownFPSlimit");

		List<string> dropdownHUDTypelocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownHUDTypeFull"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownHUDTypeDialoguesOnly"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownHUDTypeDialoguesTurnOff"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_DropdownHUDTypeTurnedOff")
		};
		_dropdownComponentHUDType.ClearOptions();
		_dropdownComponentHUDType.AddOptions(dropdownHUDTypelocalizedOptions);
		_textComponentDropdownHUDType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownHUDType");

		_textComponentDropdownWeaponWheelType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextDropdownWeaponWheelType");

		_textComponentSliderCameraFOV.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextSliderCameraFOV");

		_textComponentSliderScreenBrightness.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextSliderScreenBrightness");

		_textComponentButtonGameDifficulty.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextButtonShowIngameHints");

		_textComponentToggleShowIngameHints.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextToggleShowIngameHints");

		_textComponentToggleShowBlood.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsSectionGeneral_TextToggleShowBlood");
	}
}