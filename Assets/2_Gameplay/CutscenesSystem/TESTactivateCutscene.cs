using UnityEngine;

public class TESTactivateCutscene : MonoBehaviour
{
    public CutsceneController _CutsceneController;
 
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.M) && !_CutsceneController.IsCutscenePlaying)
        {
            _CutsceneController.TriggerCutscene();
        }
	}
}
