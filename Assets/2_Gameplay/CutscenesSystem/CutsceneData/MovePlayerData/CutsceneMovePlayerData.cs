using UnityEngine;

[CreateAssetMenu(fileName = "CutsceneMovePlayerData", menuName = "Cutscenes/CutsceneMovePlayerData")]
public class CutsceneMovePlayerData : ScriptableObject
{
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}
