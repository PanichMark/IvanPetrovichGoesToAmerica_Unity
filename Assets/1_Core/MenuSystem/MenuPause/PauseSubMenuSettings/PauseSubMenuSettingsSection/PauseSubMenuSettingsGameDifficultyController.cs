using UnityEngine;

public class PauseSubMenuSettingsGameDifficultyController : MonoBehaviour
{
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private GameObject _canvasGameDifficulty;

	public void Initialize(
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameObject canvasGameDifficulty,
		ViewModelPauseSubMenuSettingsGameDifficultyController viewModelPauseSubMenuSettingsGameDifficultyController)
	{
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasGameDifficulty = canvasGameDifficulty;

		_pauseSubMenuSettingsSectionGeneralController.OnOpenSubMenuGameDifficulty += ShowMenuGameDifficulty;
		_pauseSubMenuSettingsSectionGeneralController.OnCloseSubMenuGameDifficulty += HideMenuGameDifficulty;
	}

	private void ShowMenuGameDifficulty()
	{
		_canvasGameDifficulty.SetActive(true);
	}

	private void HideMenuGameDifficulty()
	{
		_canvasGameDifficulty.SetActive(false);
	}
}
