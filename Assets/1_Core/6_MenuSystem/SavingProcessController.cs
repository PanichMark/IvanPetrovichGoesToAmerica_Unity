using UnityEngine;

public class SavingProcessController : MonoBehaviour
{
	private SaveLoadController _saveLoadController;
	private GameObject _canvasSavingProcess;
	public void Initialize(
		SaveLoadController saveLoadController,
		GameObject canvasSavingProcess)
	{
		_saveLoadController = saveLoadController;
		_canvasSavingProcess = canvasSavingProcess;

		_saveLoadController.OnStartSavingProcess += ShowCanvasSavingProcess;
		_saveLoadController.OnEndSavingProcess += HideCanvasSavingProcess;
	}

	private void ShowCanvasSavingProcess()
	{
		Debug.Log("ShowCanvasSavingProcess");
		_canvasSavingProcess.SetActive(true);
	}

	private void HideCanvasSavingProcess()
	{
		Debug.Log("HideCanvasSavingProcess");
		_canvasSavingProcess.SetActive(false);
	}
}
