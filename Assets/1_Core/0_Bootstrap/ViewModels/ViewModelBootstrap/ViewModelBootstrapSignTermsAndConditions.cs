using UnityEngine;

public class ViewModelBootstrapSignTermsAndConditions
{
	public GameObject TextHeaderTermsAndConditions;
	public GameObject TextTermsAndConditions;

	public GameObject ButtonSign;
	public GameObject TextButtonSign;
	public GameObject ButtonRefuse;
	public GameObject TextButtonRefuse;

	public GameObject ToggleAgreeWithTerms;
	public GameObject TextToggleAgreeWithTerms;

	public ViewModelBootstrapSignTermsAndConditions(Bootstrap bootstrap, GameObject canvas)
	{
		TextHeaderTermsAndConditions = bootstrap.FindDeepGameObject(canvas, "TextHeaderTermsAndConditions");
		TextTermsAndConditions = bootstrap.FindDeepGameObject(canvas, "TextTermsAndConditions");

		ButtonSign = bootstrap.FindDeepGameObject(canvas, "ButtonSign");
		TextButtonSign = bootstrap.FindDeepGameObject(canvas, "TextButtonSign");
		ButtonRefuse = bootstrap.FindDeepGameObject(canvas, "ButtonRefuse");
		TextButtonRefuse = bootstrap.FindDeepGameObject(canvas, "TextButtonRefuse");

		ToggleAgreeWithTerms = bootstrap.FindDeepGameObject(canvas, "ToggleAgreeWithTerms");
		TextToggleAgreeWithTerms = bootstrap.FindDeepGameObject(canvas, "TextToggleAgreeWithTerms");
	}
}
