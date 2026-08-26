using UnityEngine;

[CreateAssetMenu(fileName = "InteractionObjectChangeScene", menuName = "InteractionObjects/ChangeScene/")]
public class InteractionObjectChangeScene : ScriptableObject
{
	public GameScenesGameplayDataEnum SceneToLoad;
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}
