using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionGraphics
{
	public GameObject TEXT_NO_GRAPHICS_SETTINGS_YET;

	public ViewModelPauseSubMenuSettingsSectionGraphics(Bootstrap bootstrap, GameObject canvas)
	{
		TEXT_NO_GRAPHICS_SETTINGS_YET = bootstrap.FindDeepGameObject(canvas, "TEXT_NO_GRAPHICS_SETTINGS_YET");
	}
}
