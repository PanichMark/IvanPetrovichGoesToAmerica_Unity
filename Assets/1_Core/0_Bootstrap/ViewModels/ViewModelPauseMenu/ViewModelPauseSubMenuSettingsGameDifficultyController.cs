using UnityEngine;

public class ViewModelPauseSubMenuSettingsGameDifficultyController
{
	public GameObject ImageGameDifficulty;
	public GameObject TextGameDifficultyHeader;
	public GameObject TextGameDifficultyDescription;

	public GameObject ButtonNextGameDifficulty;
	public GameObject ButtonPreviousGameDifficulty;

	public GameObject ButtonCloseSettingsGameDifficulty;
	public GameObject TextButtonCloseSettingsGameDifficulty;

	public GameObject DifficultyNotAvailable;
	public GameObject TextDifficultyNotAvailable;

	public ViewModelPauseSubMenuSettingsGameDifficultyController(Bootstrap bootstrap, GameObject canvas)
	{
		ImageGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ImageGameDifficulty");
		TextGameDifficultyHeader = bootstrap.FindDeepGameObject(canvas, "TextGameDifficultyHeader");
		TextGameDifficultyDescription = bootstrap.FindDeepGameObject(canvas, "TextGameDifficultyDescription");

		ButtonNextGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonNextGameDifficulty");
		ButtonPreviousGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousGameDifficulty");

		ButtonCloseSettingsGameDifficulty = bootstrap.FindDeepGameObject(canvas, "ButtonCloseSettingsGameDifficulty");
		TextButtonCloseSettingsGameDifficulty = bootstrap.FindDeepGameObject(canvas, "TextButtonCloseSettingsGameDifficulty");

		DifficultyNotAvailable = bootstrap.FindDeepGameObject(canvas, "DifficultyNotAvailable");
		TextDifficultyNotAvailable = bootstrap.FindDeepGameObject(canvas, "TextDifficultyNotAvailable");
	}
}