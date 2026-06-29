using UnityEngine;

public class BlenderShapesResetter : MonoBehaviour
{
	private SkinnedMeshRenderer _skinnedMeshRenderer;
	private NPCDialogueController _NPCDialogueController;

	private void Start()
	{
		_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		_NPCDialogueController = transform.parent.GetComponent<NPCDialogueController>();

		_NPCDialogueController.OnResetBlenderShapes += ResetAllBlenderShapes;
	}

	private void OnDestroy()
	{
		_NPCDialogueController.OnResetBlenderShapes -= ResetAllBlenderShapes;
	}

	public void ResetAllBlenderShapes()
	{
		if (_skinnedMeshRenderer.sharedMesh.blendShapeCount == 0) return;

		for (int i = 0; i < _skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
		}
	}
}