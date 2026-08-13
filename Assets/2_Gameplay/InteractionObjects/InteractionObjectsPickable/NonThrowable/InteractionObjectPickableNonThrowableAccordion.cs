using System.Collections;
using UnityEngine;

public class InteractionObjectPickableNonThrowableAccordion : InteractionObjectPickableNonThrowable
{
	private const float AnimationDuration = 0.604f;
	private const float TargetWeight = 100f;

	private SkinnedMeshRenderer _accordionSkinnedMeshRenderer;
	private float _animationStartTime;

	public override void PickUpObject()
	{
		base.PickUpObject();
		_animationStartTime = Time.time;
		StartCoroutine(PlayAccordionBlendShapeAnimation());
	}

	protected override IEnumerator MoveTowardsPlayer()
	{
		while (true)
		{
			Vector3 targetPosition = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.3f + Vector3.up * 0.9f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			Quaternion targetRotation = Quaternion.LookRotation(-CachedPlayer.transform.forward, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
				break;

			yield return null;
		}

		transform.position = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.3f + Vector3.up * 0.9f;
		transform.rotation = Quaternion.LookRotation(-CachedPlayer.transform.forward, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
	}

	public override void DropOffObject()
	{
		StopAllCoroutines();
		SetAccordionBlendShape(0f);
		base.DropOffObject();
	}

	private IEnumerator PlayAccordionBlendShapeAnimation()
	{
		while (IsObjectPickedUp)
		{
			float phase = Mathf.PingPong(Time.time - _animationStartTime, AnimationDuration) / AnimationDuration;
			SetAccordionBlendShape(Mathf.Lerp(0f, TargetWeight, phase));
			yield return null;
		}
	}

	private void SetAccordionBlendShape(float weight)
	{
		_accordionSkinnedMeshRenderer.SetBlendShapeWeight(0, weight);
	}

	protected override void InitializePickable()
	{
		_accordionSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
	}
}