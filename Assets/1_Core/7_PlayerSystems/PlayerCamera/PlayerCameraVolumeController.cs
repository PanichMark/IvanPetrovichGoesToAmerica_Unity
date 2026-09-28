using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCameraVolumeController : MonoBehaviour
{
	private MenuManager _menuManager;
	private Volume _thirdPersonCameraVolumeBlur;
	private Volume _thirdPersonCameraVolumeBrightness;
	private Volume _firstPersonCameraVolumeBlur;
	private Volume _firstPersonCameraVolumeBrightness;
	private GameObject _playerCameraFirstPerson;

	// Ссылка на контроллер настроек
	private PauseSubMenuSettingsSectionGeneralController _generalSettingsController;

	public void Initialize(
		MenuManager manager,
		PauseSubMenuSettingsSectionGeneralController generalSettingsController,
		GameObject playerCameraFirstPerson) // Принимаем контроллер
	{
		_menuManager = manager;
		_playerCameraFirstPerson = playerCameraFirstPerson;
		_generalSettingsController = generalSettingsController; // Сохраняем ссылку

		var thirdPersonVolumes = GetComponents<Volume>();
		_thirdPersonCameraVolumeBlur = thirdPersonVolumes[0];
		_thirdPersonCameraVolumeBrightness = thirdPersonVolumes[1];

		var firstPersonVolumes = _playerCameraFirstPerson.GetComponents<Volume>();
		_firstPersonCameraVolumeBlur = firstPersonVolumes[0];
		_firstPersonCameraVolumeBrightness = firstPersonVolumes[1];

		_menuManager.OnOpenAnyMenu += ActivateCameraBlur;
		_menuManager.OnCloseAnyMenu += DeactivateCameraBlur;
		_menuManager.OnOpenPauseMenu += ActivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedDialogueMenu += DeactivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedCutsceneMenu += DeactivateCameraBlur;

		// Подписываемся на событие яркости
		_generalSettingsController.OnScreenBrightnessChanged += ChangeCameraBrightness;

		Debug.Log("PlayerCameraBlurFilter Initialized");
	}

	public void ActivateCameraBlur()
	{
		_thirdPersonCameraVolumeBlur.enabled = true;
		_firstPersonCameraVolumeBlur.enabled = true;
		Debug.Log("Active CameraBlur");
	}

	public void DeactivateCameraBlur()
	{
		_thirdPersonCameraVolumeBlur.enabled = false;
		_firstPersonCameraVolumeBlur.enabled = false;
		Debug.Log("Deactive CameraBlur");
	}

	public void ChangeCameraBrightness(int value)
	{
		// 1. Нормализуем значение слайдера (0-100) в диапазон от -1 до 1
		//    При value = 50, t будет равен 0.
		float t = (value / 100f) * 2f - 1f;

		// 2. Применяем смещенную экспоненту. 
		//    Это дает очень плавный ход в центре и контролируемый по краям.
		//    1.5f - это коэффициент "кривизны". Можешь менять 1.2f - 2f для настройки чувствительности.
		float exposure = Mathf.Sign(t) * (Mathf.Pow(Mathf.Abs(t) + 1f, 1.5f) - 1f);

		// 3. Ограничиваем диапазон, чтобы не уйти в абсолютную тьму или пересвет
		//    (опционально, но полезно)
		exposure = Mathf.Clamp(exposure, -3f, 3f);

		_thirdPersonCameraVolumeBrightness.profile.TryGet(out ColorAdjustments thirdPersonColor);
		_firstPersonCameraVolumeBrightness.profile.TryGet(out ColorAdjustments firstPersonColor);

		if (thirdPersonColor != null) thirdPersonColor.postExposure.value = exposure;
		if (firstPersonColor != null) firstPersonColor.postExposure.value = exposure;
	}
}