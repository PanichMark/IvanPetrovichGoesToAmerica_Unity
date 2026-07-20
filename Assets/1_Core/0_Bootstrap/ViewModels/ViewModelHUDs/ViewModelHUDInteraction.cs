using UnityEngine;

public class ViewModelHUDInteraction
{
	public GameObject TextInteractionMessageMain;
	public GameObject TextInteractionMessageFail;
	public GameObject[] TextsGainedItems;
	public GameObject[] ImagesGainedItems;
	public GameObject TextPhraseLine;
	public GameObject HUDinteraction;
	public GameObject HUDphraseLine;

	public ViewModelHUDInteraction(Bootstrap bootstrap, GameObject canvas)
	{
		TextInteractionMessageMain = bootstrap.FindDeepGameObject(canvas, "TextMainInteraction");
		TextInteractionMessageFail = bootstrap.FindDeepGameObject(canvas, "TextFailInteraction");

		TextsGainedItems = new GameObject[]
		{
			bootstrap.FindDeepGameObject(canvas,"TextGainedItem1"),
			bootstrap.FindDeepGameObject(canvas,"TextGainedItem2"),
			bootstrap.FindDeepGameObject(canvas,"TextGainedItem3")
		};

		ImagesGainedItems = new GameObject[]
		{
			bootstrap.FindDeepGameObject(canvas,"ImageGainedItem1"),
			bootstrap.FindDeepGameObject(canvas,"ImageGainedItem2"),
			bootstrap.FindDeepGameObject(canvas,"ImageGainedItem3")
		};
		TextPhraseLine = bootstrap.FindDeepGameObject(canvas, "TextPhrase");

		HUDinteraction= bootstrap.FindDeepGameObject(canvas, "HUDinteraction");
		HUDphraseLine = bootstrap.FindDeepGameObject(canvas, "HUDphraseLine");
	}
}
