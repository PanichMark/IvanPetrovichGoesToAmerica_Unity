using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		var updatedItem = new NonThrowableUndestructableObjectData
		{
			NonThrowableUndestructableObjectIndex = GameplayObjectIndex,
			NonThrowableUndestructableObjectNameSystem = InteractionObjectNameSystem,
			NonThrowableUndestructableObjectPosition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			NonThrowableUndestructableObjectRotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			IsNonThrowableUndestructableObjectPickedUp = IsObjectPickedUp
		};

		if (data.NonThrowableUndestructableObjectsData == null)
		{
			data.NonThrowableUndestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<NonThrowableUndestructableObjectData>>();
		}
		if (!data.NonThrowableUndestructableObjectsData.ContainsKey(currentScene))
		{
			data.NonThrowableUndestructableObjectsData[currentScene] = new List<NonThrowableUndestructableObjectData>();
		}

		var targetList = data.NonThrowableUndestructableObjectsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.NonThrowableUndestructableObjectIndex == GameplayObjectIndex);

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.NonThrowableUndestructableObjectsData == null || !data.NonThrowableUndestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.NonThrowableUndestructableObjectIndex == GameplayObjectIndex);
		if (savedState.Equals(default(NonThrowableUndestructableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsNonThrowableUndestructableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;
			PickUpObject(true);
			_playerInteractionController?.PickUpObjectOnLoadData(gameObject);
		}
		else
		{
			transform.position = savedState.NonThrowableUndestructableObjectPosition;
			transform.rotation = savedState.NonThrowableUndestructableObjectRotation;
		}

		yield return null;
	}
}
