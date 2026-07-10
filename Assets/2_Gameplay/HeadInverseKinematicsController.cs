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

		var headData = _headIK.data;
		headData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_headIK.data = headData;

		var neckData = _neckIK.data;
		neckData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_neckIK.data = neckData;

		_weightedTransform.weight = 0f;
		UpdateSources();

		_rigBuilder.Build();
		_animator.Rebind();

		StartCoroutine(LerpWeight(1f));
	}

	private void StopLookingAtObject(GameObject objectToLookAt)
	{
		StartCoroutine(LerpWeight(0f));
	}

	private IEnumerator LerpWeight(float targetWeight)
	{
		float duration = 1f;
		float timeElapsed = 0f;

		float startWeight = _weightedTransform.weight;

		while (timeElapsed < duration)
		{
			timeElapsed += Time.unscaledDeltaTime;

			float t = Mathf.Clamp01(timeElapsed / duration);

			// Создаем новую структуру с новым весом
			_weightedTransform = new WeightedTransform(_weightedTransform.transform, Mathf.Lerp(startWeight, targetWeight, t));

			UpdateSources();

			yield return null;
		}

		_weightedTransform = new WeightedTransform(_weightedTransform.transform, targetWeight);
		UpdateSources();

		if (targetWeight == 0f)
		{
			_headIK.data.sourceObjects = new WeightedTransformArray();
			_neckIK.data.sourceObjects = new WeightedTransformArray();
		}
	}

	private void UpdateSources()
	{
		var tempHeadData = _headIK.data;
		tempHeadData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_headIK.data = tempHeadData;

		var tempNeckData = _neckIK.data;
		tempNeckData.sourceObjects = new WeightedTransformArray { _weightedTransform };
		_neckIK.data = tempNeckData;
	}
}