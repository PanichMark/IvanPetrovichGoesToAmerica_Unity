using UnityEngine;

public class ViewModelPauseSubMenuSettingsGameDifficultyController
{
	public GameObject ImageGameDifficulty;
	public GameObject TextGameDifficulty;

	public GameObject ButtonNextGameDifficulty;
	public GameObject ButtonPreviousGameDifficulty;

	public GameObject ButtonCloseSettingsGameDifficulty;
	public GameObject TextButtonCloseSettingsGameDifficulty;

	public ViewModelPauseSubMenuSettingsGameDifficultyController(Bootstrap bootstrap, GameObject canvas)
	{
		ImageGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ImageGameDifficulty");
		TextGameDifficulty = bootstrap.FindDeepGameObject(canvas, "TextGameDifficulty");

		ButtonNextGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonNextGameDifficulty");
		ButtonPreviousGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousGameDifficulty");

		ButtonCloseSettingsGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonCloseSettingsGameDifficulty");
		TextButtonCloseSettingsGameDifficulty = bootstrap.FindDeepGameObject(canvas, "TextButtonCloseSettingsGameDifficulty");
	}
}