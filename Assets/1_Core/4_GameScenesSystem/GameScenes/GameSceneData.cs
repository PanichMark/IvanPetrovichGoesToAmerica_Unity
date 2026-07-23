using UnityEngine;

[CreateAssetMenu(fileName = "GameSceneData", menuName = "GameScenes/GameSceneData")]
public class GameSceneData : ScriptableObject
{
	public GameScenesEnum GameScene;
	public TextAsset SceneDescription_RU;
	public TextAsset SceneDescription_EN;
	public Sprite SceneLoadingScreenImage;
}
