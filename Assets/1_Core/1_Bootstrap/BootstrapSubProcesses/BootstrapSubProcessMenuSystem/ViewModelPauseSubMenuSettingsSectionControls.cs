using UnityEngine;

public class ViewModelPauseSubMenuSettingsSectionControls : MonoBehaviour
{
	public GameObject[] InputFieldsKeyRebinds = new GameObject[16];

	public ViewModelPauseSubMenuSettingsSectionControls(Bootstrap bootstrap, GameObject canvas)
	{
		string[] keyNames = { "MoveForward", "MoveBackward", "MoveRight", "MoveLeft", "Run", "Jump", "Crouch", "Interact",
							"ChangeCameraView", "ChangeCameraShoulder", "RightHandWeaponWheel", "LeftHandWeaponWheel",
							"RightHandWeaponAttack", "LeftHandWeaponAttack", "Reload", "LegKick"};
		for (int i = 0; i < keyNames.Length; i++)
		{
			InputFieldsKeyRebinds[i] = bootstrap.FindDeepGameObject(canvas, keyNames[i]);
		}
	}
}
