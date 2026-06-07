using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelHUDInteraction
{
	public TextMeshProUGUI TextInteractionMessageMain;
	public TextMeshProUGUI TextInteractionMessageFail;
	public TextMeshProUGUI[] TextsGainedItems;
	public Image[] ImagesGainedItems;
	public TextMeshProUGUI TextPhraseLine;

	public ViewModelHUDInteraction(Bootstrap bootstrap, GameObject canvas)
	{
		TextInteractionMessageMain = canvas.transform.Find("TextMainInteraction").GetComponent<TextMeshProUGUI>();
		TextInteractionMessageFail = canvas.transform.Find("TextAdditionalInteraction").GetComponent<TextMeshProUGUI>();
		TextsGainedItems = new TextMeshProUGUI[]
		{
			canvas.transform.Find("TextGainedItem1").GetComponent<TextMeshProUGUI>(),
			canvas.transform.Find("TextGainedItem2").GetComponent<TextMeshProUGUI>(),
			canvas.transform.Find("TextGainedItem3").GetComponent<TextMeshProUGUI>()
		};

		ImagesGainedItems = new Image[]
		{
			canvas.transform.Find("ImageGainedItem1").GetComponent<Image>(),
			canvas.transform.Find("ImageGainedItem2").GetComponent<Image>(),
			canvas.transform.Find("ImageGainedItem3").GetComponent<Image>()
		};
		TextPhraseLine = canvas.transform.Find("TextPhrase").GetComponent<TextMeshProUGUI>();
	}
}
