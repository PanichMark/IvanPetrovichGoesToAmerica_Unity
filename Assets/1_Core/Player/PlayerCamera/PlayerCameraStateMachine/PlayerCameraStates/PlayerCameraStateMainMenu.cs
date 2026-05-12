using UnityEngine;

public class PlayerCameraStateMainMenu : PlayerCameraStateAbstract
{
	private Vector3 _position;
	private Vector3 _eulerAngles;

	public PlayerCameraStateMainMenu(PlayerCameraController playerCam, Vector3 position, Vector3 eulerAngles)
	{
		_playerCamera = playerCam;
		_position = position;
		_eulerAngles = eulerAngles;
		_playerCamera.SetCameraMainMenuPosition(this._position);
		var quaternionRotation = Quaternion.Euler(_eulerAngles);
		_playerCamera.SetCameraMainMenuRotation(quaternionRotation);
	}

	public override void Update()
	{

	}
}