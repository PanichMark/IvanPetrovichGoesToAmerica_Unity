using UnityEngine;

public class PauseSubMenuSettingsSectionGraphicsController : MonoBehaviour
{
	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGraphicsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGraphicsData;

	public void Initialize()
	{
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
}
