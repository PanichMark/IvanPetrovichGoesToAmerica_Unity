using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class InteractionObjectOpenableDoorDestructable : InteractionObjectOpenableDoorUndestructable, IBreakable
{
	private ObjectPoolWeaponController _bulletHoleManager;
	[SerializeField] private float _maxDurability = 100f;
	[SerializeField] private float _damageThreshold = 50f;
	[SerializeField] private GameObject _doorNormal;
	[SerializeField] private GameObject _doorDamaged;
	[SerializeField] private GameObject _doorBroken;
	private SkinnedMeshRenderer _doorBrokenSkinnedMeshRenderer;
	public float DuribilityThreshold => _damageThreshold;
	public float CurrentDurability { get; private set; }
	public bool CanObjectBeBroken => true;
	public bool IsObjectDestroyed => CurrentDurability <= 0f;

	public GameObject Normal3Dmodel => _doorNormal;

	public GameObject Damaged3Dmodel => _doorDamaged;

	public GameObject Broken3Dmodel => _doorBroken;

	private Collider _collider;

	public override void InitializeDoor()
	{
		_doorBrokenSkinnedMeshRenderer = _doorBroken.GetComponent<SkinnedMeshRenderer>();
		CurrentDurability = _maxDurability;
		_bulletHoleManager = ServiceLocator.Resolve<ObjectPoolWeaponController>("ObjectPoolWeaponController");
		_collider = GetComponent<Collider>();
	}

	public void TakeBreakDamage(float amount)
	{
		// Проверка на порог урона
		if (amount < _damageThreshold)
		{
			Debug.Log($"Недостаточно урона для break. Требуется: {_damageThreshold}, получено: {amount}");
			return;
		}

		// Нанесение урона
		CurrentDurability -= amount;
		Debug.Log($"Нанесено урона: {amount}. Осталось прочности: {CurrentDurability}");

		if (CurrentDurability <= _maxDurability / 2)
		{
			Normal3Dmodel.SetActive(false);
			Damaged3Dmodel.SetActive(true);
		}

		// Проверка на разрушение
		if (CurrentDurability <= 0f)
		{
			ObjectIsFullyBroken();
		}
	}

	public void ObjectIsFullyBroken()
	{
		_isDoorDouble = false;
		Debug.Log("Был broke!");
		_collider.enabled = false;
		Damaged3Dmodel.SetActive(false);
		Broken3Dmodel.SetActive(true);

		ReturnAttachedDecalsToPool();
		StartCoroutine(ModelBreakingAnimation());
	}

	private void ReturnAttachedDecalsToPool()
	{
		List<SpriteRenderer> decalsToReturn = new List<SpriteRenderer>();

		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);

			if (child.name.StartsWith("Pooled_Decal"))
			{
				var sr = child.GetComponent<SpriteRenderer>();
				if (sr != null)
				{
					decalsToReturn.Add(sr);
				}
			}
		}

		if (decalsToReturn.Count > 0)
		{
			_bulletHoleManager.ReturnSpecificDecalsToPool(decalsToReturn.ToArray());
		}
	}

	public IEnumerator ModelBreakingAnimation()
	{
		float duration = 0.5f;
		float elapsed = 0f;
		int index = 0;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			_doorBrokenSkinnedMeshRenderer.SetBlendShapeWeight(index, Mathf.Lerp(0f, 100f, elapsed / duration));
			yield return null;
		}

		_doorBrokenSkinnedMeshRenderer.SetBlendShapeWeight(index, 100f);
	}
}
