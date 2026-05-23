using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsSectionGeneralController : MonoBehaviour
{
	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGeneralData;

	public delegate void MainCameraFOVeventHandle(float newFov, float MIN_FOV_VALUE, float MAX_FOV_VALUE);
	public event MainCameraFOVeventHandle OnMainCameraFOVchanged;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGeneralData;

	public delegate void SavePlayerPrefsCameraSettingsEventHandler();
	public event SavePlayerPrefsCameraSettingsEventHandler OnSaveCameraSettingsData;
	private PauseMenuController _pauseMenuController;
	public float CurrentFOV {  get; private set; }
	private const float _MIN_FOV_VALUE = 60f;
	public float MIN_FOV_VALUE => _MIN_FOV_VALUE;
	private const float _MAX_FOV_VALUE = 120f;
	public float MAX_FOV_VALUE => _MAX_FOV_VALUE;
	private Button[] _FPSbuttons;
	private int _currentFrameRateLimit = 60;
	private GameObject _sliderFOVgameObject;
	private Slider _sliderFOV;
	private TextMeshProUGUI _fovDisplayText;

	private Color _activeColor = Color.green;
	private Color _normalColor = Color.white;

	private GameController _gameController;

	public void Initialize(
		GameController  gameController,
		PauseMenuController pauseMenuController,
		GameObject fovDisplayText,
		GameObject FOVSlider,
		GameObject[] FPSbuttons)
	{
		_gameController = gameController;
		_pauseMenuController = pauseMenuController;
		_fovDisplayText = fovDisplayText.GetComponent<TextMeshProUGUI>();

		_sliderFOVgameObject = FOVSlider;

		_sliderFOV = _sliderFOVgameObject.GetComponent<Slider>();

		_sliderFOV.minValue = _MIN_FOV_VALUE;
		_sliderFOV.maxValue = _MAX_FOV_VALUE;
		_sliderFOV.onValueChanged.AddListener(SetFOV);

		_FPSbuttons = new Button[FPSbuttons.Length];

		_gameController.OnOpenMainMenu += () =>
		{
			OnMainCameraFOVchanged?.Invoke(60, _MIN_FOV_VALUE, _MAX_FOV_VALUE);
		};

		for (int i = 0; i < _FPSbuttons.Length; i++)
		{
			_FPSbuttons[i] = FPSbuttons[i].GetComponent<Button>();
		}

		ApplyFPSbuttonColors(_currentFrameRateLimit);
		ChangeFrameRateLimit(60);

		_FPSbuttons[0].onClick.AddListener(() => ChangeFrameRateLimit(30));
		_FPSbuttons[1].onClick.AddListener(() => ChangeFrameRateLimit(60));
		_FPSbuttons[2].onClick.AddListener(() => ChangeFrameRateLimit(90));
		_FPSbuttons[3].onClick.AddListener(() => ChangeFrameRateLimit(144));

		_pauseMenuController.OnOpenConfirmMenu += DisableButtons;
		_pauseMenuController.OnCloseConfirmMenu += EnableButtons;

		Debug.Log("SettingsSectionGeneral Initialized");
	}

	private void HighlightFPSbutton(Button button)
	{
		button.image.color = _activeColor;
	}

	private void DisableButtons()
	{
		foreach (var button in _FPSbuttons)
		{
			if (button != null) button.interactable = false;
		}

		if (_sliderFOV != null) _sliderFOV.interactable = false;
	}

	private void EnableButtons()
	{
		foreach (var button in _FPSbuttons)
		{
			if (button != null) button.interactable = true;
		}

		if (_sliderFOV != null) _sliderFOV.interactable = true;
	}

	public void SaveSettingsGeneral()
	{
		var currentData = new PlayerPrefsData();

		OnSaveCameraSettingsData?.Invoke();

		currentData.FOV = CurrentFOV;

		OnSaveSettingsGeneralData?.Invoke(currentData);
	}

	private void ApplyFPSbuttonColors(int activeFrameRate)
	{
		ResetFPSbuttons();

		if (activeFrameRate == 30)
		{
			HighlightFPSbutton(_FPSbuttons[0]);
		}
		if (activeFrameRate == 60)
		{
			HighlightFPSbutton(_FPSbuttons[1]);
		}
		if (activeFrameRate == 90)
		{
			HighlightFPSbutton(_FPSbuttons[2]);
		}
		if (activeFrameRate == 144)
		{
			HighlightFPSbutton(_FPSbuttons[3]);
		}
	}

	public void GetCameraCurrentFOV(float FOV)
	{
		CurrentFOV = FOV;
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetFOV(data.FOV);
		_sliderFOV.value = data.FOV;
		CurrentFOV = data.FOV;
	}

	private void ResetFPSbuttons()
	{
		_FPSbuttons[0].image.color = _normalColor;
		_FPSbuttons[1].image.color = _normalColor;
		_FPSbuttons[2].image.color = _normalColor;
		_FPSbuttons[3].image.color = _normalColor;
	}

	public void ResetSettingsGeneral()
	{
		OnResetSettingsGeneralData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			FOV = _MIN_FOV_VALUE,
		};

		OnSaveSettingsGeneralData?.Invoke(defaultData);

		SetFOV(_MIN_FOV_VALUE);
		_sliderFOV.value = _MIN_FOV_VALUE;
	}

	public void SetFOV(float newFov)
	{
		CurrentFOV = newFov;

		_fovDisplayText.text = ((int)newFov).ToString();

		if (!_gameController.IsMainMenuOpen)
		{
			OnMainCameraFOVchanged?.Invoke(newFov, _MIN_FOV_VALUE, _MAX_FOV_VALUE);
		}
		else
		{
			
			OnMainCameraFOVchanged?.Invoke(60, _MIN_FOV_VALUE, _MAX_FOV_VALUE);
		}
	}

	private void ChangeFrameRateLimit(int frameRate)
	{
		Application.targetFrameRate = frameRate;
		_currentFrameRateLimit = frameRate;
		ApplyFPSbuttonColors(frameRate);
		Debug.Log($"Frame rate limit set to {frameRate}");
	}
}
