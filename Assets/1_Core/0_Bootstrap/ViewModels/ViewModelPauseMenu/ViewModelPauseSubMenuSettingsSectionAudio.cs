using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionAudio
{
	public GameObject[] ButtonsChangeLanguage = new GameObject[2];
	public GameObject TextChangeLanguage;

	public GameObject SliderVolumeGeneral;
	public GameObject NumberSliderVolumeGeneral;
	public GameObject TextSliderVolumeGeneral;

	public GameObject SliderVolumeEnvironment;
	public GameObject NumberSliderVolumeEnvironment;
	public GameObject TextSliderVolumeEnvironment;

	public GameObject SliderVolumeEffects;
	public GameObject NumberSliderVolumeEffects;
	public GameObject TextSliderVolumeEffects;

	public GameObject SliderVolumeVoices;
	public GameObject NumberSliderVolumeVoices;
	public GameObject TextSliderVolumeVoices;

	public GameObject SliderVolumeMusicAmbience;
	public GameObject NumberSliderVolumeMusicAmbience;
	public GameObject TextSliderVolumeMusicAmbience;

	public GameObject SliderVolumeMusicIngame;
	public GameObject NumberSliderVolumeMusicIngame;
	public GameObject TextSliderVolumeMusicIngame;

	public ViewModelPauseSubMenuSettingsSectionAudio(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonsChangeLanguage[0] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguageRussian");
		ButtonsChangeLanguage[1] = bootstrap.FindDeepGameObject(canvas, "ButtonChangeLanguageEnglish");

		TextChangeLanguage = bootstrap.FindDeepGameObject(canvas, "TextChangeLanguage");

		SliderVolumeGeneral = bootstrap.FindDeepGameObject(canvas, "SliderVolumeGeneral");
		NumberSliderVolumeGeneral = bootstrap.FindDeepGameObject(canvas, "NumberVolumeGeneral");
		TextSliderVolumeGeneral = bootstrap.FindDeepGameObject(canvas, "TextVolumeGeneral");

		SliderVolumeEnvironment = bootstrap.FindDeepGameObject(canvas, "SliderVolumeEnvironment");
		NumberSliderVolumeEnvironment = bootstrap.FindDeepGameObject(canvas, "NumberVolumeEnvironment");
		TextSliderVolumeEnvironment = bootstrap.FindDeepGameObject(canvas, "TextVolumeEnvironment");

		SliderVolumeEffects = bootstrap.FindDeepGameObject(canvas, "SliderVolumeEffects");
		NumberSliderVolumeEffects = bootstrap.FindDeepGameObject(canvas, "NumberVolumeEffects");
		TextSliderVolumeEffects = bootstrap.FindDeepGameObject(canvas, "TextVolumeEffects");

		SliderVolumeVoices = bootstrap.FindDeepGameObject(canvas, "SliderVolumeVoices");
		NumberSliderVolumeVoices = bootstrap.FindDeepGameObject(canvas, "NumberVolumeVoices");
		TextSliderVolumeVoices = bootstrap.FindDeepGameObject(canvas, "TextVolumeVoices");

		SliderVolumeMusicAmbience = bootstrap.FindDeepGameObject(canvas, "SliderVolumeMusicAmbience");
		NumberSliderVolumeMusicAmbience = bootstrap.FindDeepGameObject(canvas, "NumberVolumeMusicAmbience");
		TextSliderVolumeMusicAmbience = bootstrap.FindDeepGameObject(canvas, "TextVolumeMusicAmbience");

		SliderVolumeMusicIngame = bootstrap.FindDeepGameObject(canvas, "SliderVolumeMusicIngame");
		NumberSliderVolumeMusicIngame = bootstrap.FindDeepGameObject(canvas, "NumberVolumeMusicIngame");
		TextSliderVolumeMusicIngame = bootstrap.FindDeepGameObject(canvas, "TextVolumeMusicIngame");
	}
}