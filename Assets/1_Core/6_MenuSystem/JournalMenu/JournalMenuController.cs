using UnityEngine;

public class JournalMenuController : MonoBehaviour
{
	private GameObject _canvasJournalMenu;
	private ViewModelJournalMenu _viewModelJournalMenu;

	public void Initialize(
		GameObject canvasJournalMenu,
		ViewModelJournalMenu viewModelJournalMenu)
	{
		_canvasJournalMenu = canvasJournalMenu;
		_viewModelJournalMenu = viewModelJournalMenu;
	}

	public void ShowJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(true);
	}
	public void HideJournalMenu()
	{
		_canvasJournalMenu.gameObject.SetActive(false);
	}
}
