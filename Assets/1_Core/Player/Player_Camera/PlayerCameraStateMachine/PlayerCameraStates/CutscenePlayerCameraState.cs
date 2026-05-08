using UnityEngine;

public class CutscenePlayerCameraState : AbstractPlayerCameraState


{
	private Vector3 position;
	private Vector3 eulerAngles;

	public CutscenePlayerCameraState()
	{
	
		
		
		//	Debug.Log("POSITION "+ this.position);
	}

	
	public override void Update()
	{
		//playerCamera.CutsceneCameraTransform(new Vector3(0, 5, -7));
		//playerCamera.CutsceneCameraTransform(position);
	}
	

}


