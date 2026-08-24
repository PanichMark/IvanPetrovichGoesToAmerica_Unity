using UnityEngine;

public class InteractionObjectPickableNonThrowableUndestructable : InteractionObjectPickableNonThrowableAbstract
{
	public static InteractionObjectPickableNonThrowableUndestructable CreateWithName(GameObject obj, string interactionItemNameSystem, InteractionObjectPickableData pickableBodyData)
	{
		var component = obj.GetComponent<InteractionObjectPickableNonThrowableUndestructable>();
		if (component == null)
		{
			component = obj.AddComponent<InteractionObjectPickableNonThrowableUndestructable>();
		}
		//Debug.Log(component);

		component.SetUpPickableBody(interactionItemNameSystem, pickableBodyData);

		return component;
	}

	protected void SetUpPickableBody(string interactionObjectNameSystem, InteractionObjectPickableData pickableBodyData)
	{
		_interactionObjectPickableType = pickableBodyData;
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(interactionObjectNameSystem);


		Collider = gameObject.AddComponent<BoxCollider>();

		BoxCollider box = (BoxCollider)Collider;
		box.center = new Vector3(0f, 0.5f, 0f);
		box.size = new Vector3(0.7f, 1f, 0.7f);

		var rigidbody = GetComponent<Rigidbody>();
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		InitializePickable();
	}
}
