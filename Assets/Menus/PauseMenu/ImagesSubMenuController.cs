using UnityEngine;
using UnityEngine.UI;

public class ImagesSubMenuController : MonoBehaviour
{
	//InputManager playerInputsList;
	PauseMenuController pauseMenuController;

	public Canvas ImagesSubMenuCanvas;

	public Button CloseImagesSubMenuButton;
	void Start()
	{
		//playerInputsList = GetComponent<InputManager>();
		pauseMenuController = GetComponent<PauseMenuController>();

		CloseImagesSubMenuButton.onClick.AddListener(CloseImagesSubMenu);
	}

	/*
	private void Update()
	{
		if (InputManager.Instance.GetKeyPauseMenu() && ImagesSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseImagesSubMenu();
		}
	}
	*/

	public void CloseImagesSubMenu()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(false);

		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);

		Debug.Log("ImagesSubMenu closed");
	}
}


