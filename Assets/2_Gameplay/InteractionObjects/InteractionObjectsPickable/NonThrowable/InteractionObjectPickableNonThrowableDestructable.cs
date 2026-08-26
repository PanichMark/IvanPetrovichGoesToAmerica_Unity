using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableNonThrowableDestructable : InteractionObjectPickableNonThrowableAbstract, IBreakable
{
	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] private float _breakingThreshold;

	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeBroken => true;

	public GameObject Normal3Dmodel => throw new System.NotImplementedException();

	public GameObject Damaged3Dmodel => throw new System.NotImplementedException();

	public GameObject Broken3Dmodel => throw new System.NotImplementedException();

	public virtual void TakeBreakDamage(float amount)
	{
		if (CanObjectBeBroken)
		{
			if (amount >= DuribilityThreshold)
			{
				_health -= amount;

				if (_health <= 0)
				{
					ObjectIsFullyBroken();
				}
			}
		}
	}

	public void ObjectIsFullyBroken()
	{
		_isObjectDestroyed = true;

		gameObject.SetActive(false);
	}

	public IEnumerator ModelBreakingAnimation()
	{
		throw new System.NotImplementedException();
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		var updatedItem = new NonThrowableDestructableObjectData
		{
			NonThrowableDestructableObjectIndex = GameplayObjectIndex,
			NonThrowableDestructableObjectNameSystem = InteractionObjectNameSystem,
			NonThrowableDestructableObjectPosition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			NonThrowableDestructableObjectRotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			IsNonThrowableDestructableObjectPickedUp = IsObjectPickedUp,
			NonThrowableDestructableObjectHealth = _health
		};

		if (data.NonThrowableDestructableObjectsData == null)
		{
			data.NonThrowableDestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<NonThrowableDestructableObjectData>>();
		}
		if (!data.NonThrowableDestructableObjectsData.ContainsKey(currentScene))
		{
			data.NonThrowableDestructableObjectsData[currentScene] = new List<NonThrowableDestructableObjectData>();
		}

		var targetList = data.NonThrowableDestructableObjectsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.NonThrowableDestructableObjectIndex == GameplayObjectIndex);

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

		if (data.NonThrowableDestructableObjectsData == null || !data.NonThrowableDestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.NonThrowableDestructableObjectIndex == GameplayObjectIndex);
		if (savedState.Equals(default(NonThrowableDestructableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsNonThrowableDestructableObjectPickedUp;
		_health = savedState.NonThrowableDestructableObjectHealth;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;
			PickUpObject(true);
			_playerInteractionController?.PickUpObjectOnLoadData(gameObject);
		}
		else
		{
			transform.position = savedState.NonThrowableDestructableObjectPosition;
			transform.rotation = savedState.NonThrowableDestructableObjectRotation;
		}

		if (_health <= 0)
		{
			ObjectIsFullyBroken();
		}

		yield return null;
	}
}
