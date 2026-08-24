using System.Collections;
using UnityEngine;

public class InteractionObjectPickableNonThrowableAccordion : InteractionObjectPickableNonThrowableAbstract
{
	private const float AnimationDuration = 0.604f;
	private const float TargetWeight = 100f;

	private SkinnedMeshRenderer _accordionSkinnedMeshRenderer;
	private float _animationStartTime;

	public override void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		base.PickUpObject(isPickedUpByLoadSafeFile);
		_animationStartTime = Time.time;
		StartCoroutine(PlayAccordionBlendShapeAnimation());
	}

	public override void DropOffObject()
	{
		StopAllCoroutines();
		SetAccordionBlendShape(0f);
		base.DropOffObject();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy(); // Затем вызовется логика родителя
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
		base.InitializePickable();

		_accordionSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
	}
}