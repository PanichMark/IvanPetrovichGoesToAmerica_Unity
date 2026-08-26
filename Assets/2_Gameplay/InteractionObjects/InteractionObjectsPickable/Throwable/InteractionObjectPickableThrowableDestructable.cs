using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableThrowableDestructable : InteractionObjectPickableThrowableAbstract, IDamageable, IBreakable
{
	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] private bool _canBeDamaged;
	[SerializeField] private float _breakingThreshold;

	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeBroken => true;

	public float CurrentHealth => _health;

	public bool CanObjectBeDamaged => _canBeDamaged;

	public GameObject Normal3Dmodel => throw new System.NotImplementedException();

	public GameObject Damaged3Dmodel => throw new System.NotImplementedException();

	public GameObject Broken3Dmodel => throw new System.NotImplementedException();

	public void TakeDamage(float amount)
	{
		if (CanObjectBeDamaged)
		{
			Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");

			_health -= amount;

			if (_health <= 0)
			{
				ObjectIsFullyDamaged();
			}
		}
	}

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

	protected override void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
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

			RigidBody.isKinematic = true;

			_isObjectDestroyed = true;
			gameObject.SetActive(false);
			Debug.Log($"{InteractionObjectNameSystem} was destroyed on impact!");
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

		var updatedItem = new ThrowableDestructableObjectData
		{
			ThrowableDestructableObjectIndex = GameplayObjectIndex,
			ThrowableDestructableObjectNameSystem = InteractionObjectNameSystem,
			ThrowableDestructableObjectPosition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			ThrowableDestructableObjectRotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			IsThrowableDestructableObjectPickedUp = IsObjectPickedUp,
			WasThrowableDestructableObjectThrown = _wasThrown,
			ThrowableDestructableObjectHealth = _health
		};

		if (data.ThrowableDestructableObjectsData == null)
		{
			data.ThrowableDestructableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<ThrowableDestructableObjectData>>();
		}
		if (!data.ThrowableDestructableObjectsData.ContainsKey(currentScene))
		{
			data.ThrowableDestructableObjectsData[currentScene] = new List<ThrowableDestructableObjectData>();
		}

		var targetList = data.ThrowableDestructableObjectsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.ThrowableDestructableObjectIndex == GameplayObjectIndex);

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

		if (data.ThrowableDestructableObjectsData == null || !data.ThrowableDestructableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.ThrowableDestructableObjectIndex == GameplayObjectIndex);
		if (savedState.Equals(default(ThrowableDestructableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsThrowableDestructableObjectPickedUp;
		_wasThrown = savedState.WasThrowableDestructableObjectThrown;

		_health = savedState.ThrowableDestructableObjectHealth;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;
			PickUpObject(true);
			_playerInteractionController?.PickUpObjectOnLoadData(gameObject);
		}
		else
		{
			transform.position = savedState.ThrowableDestructableObjectPosition;
			transform.rotation = savedState.ThrowableDestructableObjectRotation;
		}

		if (_wasThrown)
		{
			_health = 0;
		}

		if (_health <= 0)
		{
			ObjectIsFullyBroken();
		}

		yield return null;
	}
}
