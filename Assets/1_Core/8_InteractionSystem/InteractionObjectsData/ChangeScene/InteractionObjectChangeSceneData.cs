using UnityEngine;

[CreateAssetMenu(fileName = "InteractionObjectChangeSceneData", menuName = "InteractionObjects/ChangeScene/")]
public class InteractionObjectChangeSceneData : ScriptableObject
{
	public GameScenesGameplayEnum SceneToLoad;
	public Vector3 PlayerPosition;
	public int PlayerRotationY;
}
