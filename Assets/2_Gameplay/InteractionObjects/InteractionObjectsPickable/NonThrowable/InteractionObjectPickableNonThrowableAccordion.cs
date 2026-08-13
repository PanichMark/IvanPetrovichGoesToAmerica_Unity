using System.Collections;
using UnityEngine;

public class InteractionObjectPickableNonThrowableAccordion : InteractionObjectPickableNonThrowable
{
	private const int AnimationFrames = 15;
	private const float TargetWeight = 100f;

	private SkinnedMeshRenderer _accordionSkinnedMeshRenderer;

	public override void PickUpObject()
	{
		base.PickUpObject();
		SetAccordionBlendShape(0f);
		StartCoroutine(PlayHoldShakeAnimation());
	}

	protected override IEnumerator MoveTowardsPlayer()
	{
		while (true)
		{
			Vector3 targetPosition = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
			//transform.localRotation = CachedPlayer.transform.localRotation;

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
				break;

			yield return null;
		}

		transform.position = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;
	}

	public override void DropOffObject()
	{
		StopAllCoroutines();
		SetAccordionBlendShape(0f);
		base.DropOffObject();
	}

	private IEnumerator PlayHoldShakeAnimation()
	{
		while (IsObjectPickedUp)
		{
			for (int i = 0; i <= AnimationFrames; i++)
			{
				if (!IsObjectPickedUp) yield break;
				SetAccordionBlendShape(Mathf.Lerp(0f, TargetWeight, (float)i / AnimationFrames));
				yield return null;
			}

			for (int i = AnimationFrames; i >= 0; i--)
			{
				if (!IsObjectPickedUp) yield break;
				SetAccordionBlendShape(Mathf.Lerp(0f, TargetWeight, (float)i / AnimationFrames));
				yield return null;
			}
		}
	}

	private void SetAccordionBlendShape(float weight)
	{
		if (_accordionSkinnedMeshRenderer != null)
		{
			_accordionSkinnedMeshRenderer.SetBlendShapeWeight(0, weight);
		}
	}

	protected override void InitializePickable()  
	{
		_accordionSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
	}
}