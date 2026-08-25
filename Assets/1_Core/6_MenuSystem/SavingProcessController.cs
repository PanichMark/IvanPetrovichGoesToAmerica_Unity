using UnityEngine;

public class SavingProcessController : MonoBehaviour
{
	private JsonSaveLoadController _saveLoadController;
	private GameObject _canvasSavingProcess;
	private GameObject _gear;

	public void Initialize(
		JsonSaveLoadController saveLoadController,
		GameObject canvasSavingProcess,
		ViewModelSavingProcess viewModelSavingProcess)
	{
		_saveLoadController = saveLoadController;
		_canvasSavingProcess = canvasSavingProcess;
		_gear = viewModelSavingProcess.Gear;

		_saveLoadController.OnStartGameDataProcessForUI += ShowCanvasSavingProcess;
		_saveLoadController.OnEndGameDataProcessForUI += HideCanvasSavingProcess;
	}

	private void Update()
	{
		RotateGear(300f);
	}

	private void RotateGear(float speed)
	{
		Vector3 currentRotation = _gear.transform.localEulerAngles;
		currentRotation.z += speed * Time.unscaledDeltaTime;
		_gear.transform.localEulerAngles = currentRotation;
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