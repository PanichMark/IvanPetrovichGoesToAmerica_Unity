using UnityEngine;

public class ViewModelPauseMenuConfirmAction
{
	public GameObject TextActionMessage;

	public GameObject ButtonConfirmAction;
	public GameObject TextButtonConfirmAction;

	public GameObject ButtonCancelAction;
	public GameObject TextButtonCancelAction;


	public ViewModelPauseMenuConfirmAction(Bootstrap bootstrap, GameObject canvas)
	{
		TextActionMessage = bootstrap.FindDeepGameObject(canvas, "TextActionMessage");

		ButtonConfirmAction = bootstrap.FindDeepGameObject(canvas, "ButtonConfirmAction");
		TextButtonConfirmAction = bootstrap.FindDeepGameObject(canvas, "TextButtonConfirmAction");

		ButtonCancelAction = bootstrap.FindDeepGameObject(canvas, "ButtonCancelAction");
		TextButtonCancelAction = bootstrap.FindDeepGameObject(canvas, "TextButtonCancelAction");
	}
}
