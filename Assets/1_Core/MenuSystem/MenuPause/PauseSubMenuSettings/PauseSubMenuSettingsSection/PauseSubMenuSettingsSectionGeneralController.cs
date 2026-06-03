using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionGeneralController : MonoBehaviour
{
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

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




	private Button[] _FPSbuttons;
	private int _currentFrameRateLimit = 60;


	private Color _activeColor = Color.green;
	private Color _normalColor = Color.white;



	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGeneralData;

	public delegate void MainCameraFOVeventHandle(float newCameraFOV, float MIN_VALUE_CAMERA_FOV, float MAX_VALUE_CAMERA_FOV);
	public event MainCameraFOVeventHandle OnMainCameraFOVchanged;

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

		_sliderCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV;
		_sliderComponentCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV.GetComponent<Slider>();
		_sliderComponentCameraFOV.minValue = _MIN_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.maxValue = _MAX_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.onValueChanged.AddListener(SetFOV);
		_textNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetComponent<TextMeshProUGUI>();
		_textSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV;
		_textComponentSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV.GetComponent<TextMeshProUGUI>();

		/*
		_FPSbuttons = new Button[viewModelPauseSubMenuSettings.ButtonsChangeFPS.Length];

	

		for (int i = 0; i < _FPSbuttons.Length; i++)
		{
			_FPSbuttons[i] = viewModelPauseSubMenuSettings.ButtonsChangeFPS[i].GetComponent<Button>();
		}
		

		ApplyFPSbuttonColors(_currentFrameRateLimit);
		ChangeFrameRateLimit(60);


		_FPSbuttons[0].onClick.AddListener(() => ChangeFrameRateLimit(30));
		_FPSbuttons[1].onClick.AddListener(() => ChangeFrameRateLimit(60));
		_FPSbuttons[2].onClick.AddListener(() => ChangeFrameRateLimit(90));
		_FPSbuttons[3].onClick.AddListener(() => ChangeFrameRateLimit(144));
		*/

		_gameController.OnOpenMainMenu += () =>
		{
			OnMainCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		};

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
		SetFOV(data.FOV);
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

		SetFOV(_MIN_VALUE_CAMERA_FOV);
		_sliderComponentCameraFOV.value = _MIN_VALUE_CAMERA_FOV;
	}

	public void SetFOV(float newFov)
	{
		CurrentValueCameraFOV = newFov;

		_textComponentNumberSliderCameraFOV.text = ((int)newFov).ToString();

		if (!_gameController.IsMainMenuOpen)
		{
			OnMainCameraFOVchanged?.Invoke(newFov, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
		else
		{
			
			OnMainCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	}
}
