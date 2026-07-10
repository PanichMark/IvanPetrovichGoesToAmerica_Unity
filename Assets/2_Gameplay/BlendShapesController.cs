using UnityEngine;
using System.Collections;

public class BlendShapesController : MonoBehaviour
{
	[SerializeField] private NPCStateMachineController _NPCStateMachineController;
	[SerializeField] private NPCDialogueController _NPCDialogueController;

	private SkinnedMeshRenderer _skinnedMeshRenderer;
	private Coroutine _blinkRoutine;


	private int _blendShapeEyesClosed;
	private string[] _blendShapesFacialExpressions;
	private string[] _blendShapesPhonemes;

	private void Start()
	{
		_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

		_blendShapeEyesClosed = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("BlendShape_EyesClosed");
		Blink();

		_blendShapesFacialExpressions = new string[] {
			"BlendShape_FacialExpression_Happy",
			"BlendShape_FacialExpression_Surprised",
			"BlendShape_FacialExpression_Sad",
			"BlendShape_FacialExpression_Suspicious",
			"BlendShape_FacialExpression_Angry"
		};

		_blendShapesPhonemes = new string[] {
			"BlendShape_Phoneme_A",
			"BlendShape_Phoneme_I",
			"BlendShape_Phoneme_U",
			"BlendShape_Phoneme_E",
			"BlendShape_Phoneme_O",
			"BlendShape_Phoneme_-",
			"BlendShape_Phoneme_S"
		};

		_NPCStateMachineController.OnNPCstateDead += CloseEyes;
		_NPCStateMachineController.OnNPCstateDead += StopBlinking;
		_NPCStateMachineController.OnNPCstateDead += ResetAllBlendShapesFacialExpressions;

		if (_NPCDialogueController != null)
		{
			_NPCDialogueController.OnChangeBlendShapeFacialExpression += ChangeBlendShapeFacialExpression;
			_NPCDialogueController.OnResetAllBlendShapesFacialExpressions += ResetAllBlendShapesFacialExpressions;
			_NPCDialogueController.OnResetAllBlendShapesPhonemes += ResetAllBlendShapesPhonemes;
		}
	}

	private void OnDestroy()
	{
		_NPCStateMachineController.OnNPCstateDead -= CloseEyes;
		_NPCStateMachineController.OnNPCstateDead -= StopBlinking;
		_NPCStateMachineController.OnNPCstateDead -= ResetAllBlendShapesFacialExpressions;

		if (_NPCDialogueController != null)
		{
			_NPCDialogueController.OnChangeBlendShapeFacialExpression -= ChangeBlendShapeFacialExpression;
			_NPCDialogueController.OnResetAllBlendShapesFacialExpressions -= ResetAllBlendShapesFacialExpressions;
			_NPCDialogueController.OnResetAllBlendShapesPhonemes -= ResetAllBlendShapesPhonemes;
		}
	}

	private void Blink()
	{
		if (_blinkRoutine != null)
		{
			StopCoroutine(_blinkRoutine);
		}
		_blinkRoutine = StartCoroutine(BlinkLoop());
	}

	private IEnumerator BlinkLoop()
	{
		while (true)
		{
			CloseEyes();
			yield return new WaitForSecondsRealtime(0.1f);
			OpenEyes();

			float nextInterval = Random.Range(2f, 8f);
			yield return new WaitForSecondsRealtime(nextInterval);
		}
	}

	private void StopBlinking()
	{
		if (_blinkRoutine != null)
		{
			StopCoroutine(_blinkRoutine);
			_blinkRoutine = null;
		}
	}

	private void OpenEyes()
	{
		_skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeEyesClosed, 0f);
	}

	private void CloseEyes()
	{
		_skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeEyesClosed, 100f);
	}

	private void ChangeBlendShapeFacialExpression(string newFacialExpression)
	{
		ResetAllBlendShapesFacialExpressions();
		int index = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(newFacialExpression);
		if (index != -1)
		{
			_skinnedMeshRenderer.SetBlendShapeWeight(index, 100f);
		}
	}

	private void ResetAllBlendShapesFacialExpressions()
	{
		foreach (string shapeName in _blendShapesFacialExpressions)
		{
			int index = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);

			if (index != -1)
			{
				_skinnedMeshRenderer.SetBlendShapeWeight(index, 0f);
			}
		}
	}

	private void ResetAllBlendShapesPhonemes()
	{
		foreach (string shapeName in _blendShapesPhonemes)
		{
			int index = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);

			if (index != -1)
			{
				_skinnedMeshRenderer.SetBlendShapeWeight(index, 0f);
			}
		}
	}
}