using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class HeadInverseKinematicsController : MonoBehaviour
{
	[SerializeField] private MultiAimConstraint _headIK;
	[SerializeField] private MultiAimConstraint _neckIK;
	[SerializeField] private NPCPhrasesController _NPCPhrasesController;
	[SerializeField] private NPCDialogueController _NPCDialogueController;

	private WeightedTransform _weightedTransform;
	[SerializeField] private Animator _animator;
	[SerializeField] private RigBuilder _rigBuilder;
	

	private void Start()
	{
		_NPCPhrasesController.OnStartLookingAtObject += StartLookingAtObject;
		_NPCPhrasesController.OnStopLookingAtObject += StopLookingAtObject;

		if (_NPCDialogueController != null)
		{
			_NPCDialogueController.OnStartLookingAtObject += StartLookingAtObject;
			_NPCDialogueController.OnStopLookingAtObject += StopLookingAtObject;
		}

		_weightedTransform = new WeightedTransform();
	}

	private void OnDestroy()
	{
		_NPCPhrasesController.OnStartLookingAtObject -= StartLookingAtObject;
		_NPCPhrasesController.OnStopLookingAtObject -= StopLookingAtObject;

		if (_NPCDialogueController != null)
		{
			_NPCDialogueController.OnStartLookingAtObject -= StartLookingAtObject;
			_NPCDialogueController.OnStopLookingAtObject -= StopLookingAtObject;
		}
	}

	private void StartLookingAtObject(GameObject objectToLookAt)
	{
		_weightedTransform.transform = objectToLookAt.transform;
		_weightedTransform.weight = 1f;

		var headData = _headIK.data;
		headData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_headIK.data = headData;

		var neckData = _neckIK.data;
		neckData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_neckIK.data = neckData;

		_headIK.weight = 0f;
		_neckIK.weight = 0f;

		_rigBuilder.Build();
		_animator.Rebind();

		StartCoroutine(LerpWeight(1f));
	}

	private void StopLookingAtObject(GameObject objectToLookAt)
	{
		StartCoroutine(LerpWeight(0f));

		// i need to delete Head and Neck from Source Objects list upon StopLookingAt
	}

	private IEnumerator LerpWeight(float targetWeight)
	{
		float duration = 1f;
		float timeElapsed = 0f;

		float startHead = _headIK.weight;
		float startNeck = _neckIK.weight;

		while (timeElapsed < duration)
		{
			timeElapsed += Time.unscaledDeltaTime;

			float t = Mathf.Clamp01(timeElapsed / duration);

			_headIK.weight = Mathf.Lerp(startHead, targetWeight, t);
			_neckIK.weight = Mathf.Lerp(startNeck, targetWeight, t);

			yield return null;
		}

		_headIK.weight = targetWeight;
		_neckIK.weight = targetWeight;
	}
}