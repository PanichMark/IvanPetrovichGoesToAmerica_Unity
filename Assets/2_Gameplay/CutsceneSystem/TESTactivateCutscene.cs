using UnityEngine;

public class TESTactivateCutscene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CutsceneController controller;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.M) && !controller.IsCutscenePlaying)
        {
            controller.TriggerCutscene();
        }
	}
}
