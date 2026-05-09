using UnityEngine;

public class TESTactivateCutscene : MonoBehaviour
{
    public CutsceneController controller;
 
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.M) && !controller.IsCutscenePlaying)
        {
            controller.TriggerCutscene();
        }
	}
}
