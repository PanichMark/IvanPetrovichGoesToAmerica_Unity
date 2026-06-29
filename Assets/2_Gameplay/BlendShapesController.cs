using UnityEngine;

public class BlendShapesController : MonoBehaviour
{
	private SkinnedMeshRenderer _skinnedMeshRenderer;
	private NPCDialogueController _NPCDialogueController;

	private void Start()
	{
		_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		_NPCDialogueController = transform.parent.GetComponent<NPCDialogueController>();
		_NPCDialogueController.OnResetPhonemesBlendShapes += ResetPhonemesBlendShapes;
	}

	private void OnDestroy()
	{
		_NPCDialogueController.OnResetPhonemesBlendShapes -= ResetPhonemesBlendShapes;
	}

	public void ResetPhonemesBlendShapes()
	{
		string[] phonemeShapeNames = {
			"BlenderShape_Phoneme_A",
			"BlenderShape_Phoneme_I",
			"BlenderShape_Phoneme_U",
			"BlenderShape_Phoneme_E",
			"BlenderShape_Phoneme_O",
			"BlenderShape_Phoneme_-",
			"BlenderShape_Phoneme_S"
		};

		foreach (string shapeName in phonemeShapeNames)
		{
			int index = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);
			if (index != -1)
				_skinnedMeshRenderer.SetBlendShapeWeight(index, 0f);
		}
	}
}