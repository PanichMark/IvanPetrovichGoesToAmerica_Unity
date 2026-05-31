using UnityEngine;

public class ViewModelPauseMenuConfirmAction : IViewModel
{
	public GameObject ButtonConfirmAction;
	public GameObject ButtonCancelAction;
	public GameObject TextConfirmActionMessage;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonConfirmAction = bootstrap.FindDeepGameObject(canvas, "ButtonConfirmAction");
		ButtonCancelAction = bootstrap.FindDeepGameObject(canvas, "ButtonCancelAction");
		TextConfirmActionMessage = bootstrap.FindDeepGameObject(canvas, "TextConfirmationMessage");
	}
}
