using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectPickableThrowableUndestructable : InteractionObjectPickableThrowableAbstract
{
	protected override void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
			gameObject.tag = "Interactable";
		
			var damageable = collision.gameObject.GetComponent<IDamageable>();
			if (damageable != null && damageable.CanObjectBeDamaged)
			{
				damageable.TakeDamage(_damage);
			}

			if (_canDamageBreakable)
			{
				var breakable = collision.gameObject.GetComponent<IBreakable>();
				if (breakable != null && breakable.CanObjectBeBroken && !breakable.IsObjectDestroyed)
				{
					breakable.TakeBreakDamage(_damage);
				}
			}

		}
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		var updatedItem = new ThrowableUndestructableObjectData
		{
			ThrowableUndestructableObjectIndex = GameplayObjectIndex,
			ThrowableUndestructableObjectNameSystem = InteractionObjectNameSystem,
			ThrowableUndestructableObjectPosition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			ThrowableUndestructableObjectRotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			IsThrowableUndestructableObjectPickedUp = IsObjectPickedUp,
			WasThrowableUndestructableObjectThrown = _wasThrown,
		};

		if (data.ThrowableUndestructableObjectsData == null)
		{
			data.ThrowableUndestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<ThrowableUndestructableObjectData>>();
		}
		if (!data.ThrowableUndestructableObjectsData.ContainsKey(currentScene))
		{
			data.ThrowableUndestructableObjectsData[currentScene] = new List<ThrowableUndestructableObjectData>();
		}

		var targetList = data.ThrowableUndestructableObjectsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.ThrowableUndestructableObjectIndex == GameplayObjectIndex);

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

		if (data.ThrowableUndestructableObjectsData == null || !data.ThrowableUndestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.ThrowableUndestructableObjectIndex == GameplayObjectIndex);
		if (savedState.Equals(default(ThrowableUndestructableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsThrowableUndestructableObjectPickedUp;
		_wasThrown = savedState.WasThrowableUndestructableObjectThrown;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;
			PickUpObject(true);
			_playerInteractionController?.PickUpObjectOnLoadData(gameObject);
		}
		else
		{
			transform.position = savedState.ThrowableUndestructableObjectPosition;
			transform.rotation = savedState.ThrowableUndestructableObjectRotation;
		}

		yield return null;
	}
}