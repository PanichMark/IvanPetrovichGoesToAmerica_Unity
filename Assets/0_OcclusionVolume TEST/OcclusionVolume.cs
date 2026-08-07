using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class VolumeOcclusion : MonoBehaviour
{
	[Range(0f, 1f)]
	[SerializeField] private float aoMultiplier = 1f; // Степень затемнения AO

	private Dictionary<Renderer, MaterialPropertyBlock> _propertyBlocks = new();

	// Коллайдер оставляем как SerializeField для удобства настройки
	[SerializeField] private Collider _collider;

#if UNITY_EDITOR
	void OnValidate()
	{
		if (!Application.isPlaying)
			UpdateAffectedObjects();
	}
#endif

	void LateUpdate()
	{
		if (_collider != null && Application.isPlaying)
			UpdateAffectedObjects();
	}

	// Обновление при изменении положения/свойств самого объекта в редакторе
#if UNITY_EDITOR
	void OnTransformChildrenChanged()
	{
		if (!Application.isPlaying)
			UpdateAffectedObjects();
	}
#endif

	private void UpdateAffectedObjects()
	{
		if (_collider == null || !enabled) return;

		var bounds = _collider.bounds;

		foreach (var renderer in FindObjectsOfType<MeshRenderer>())
		{
			bool isInside = renderer.bounds.Intersects(bounds);

			if (isInside)
			{
				ApplyAO(renderer, aoMultiplier);
				Debug.Log($"Объект {renderer.name}: AO={aoMultiplier}");
			}
			else if (_propertyBlocks.ContainsKey(renderer))
			{
				ResetAO(renderer);
			}
		}
	}

	private void ApplyAO(Renderer renderer, float value)
	{
		if (!_propertyBlocks.TryGetValue(renderer, out var block))
		{
			block = new MaterialPropertyBlock();
			_propertyBlocks.Add(renderer, block);
		}

		block.SetFloat("_OcclusionStrength", value);
		renderer.SetPropertyBlock(block);
	}

	private void ResetAO(Renderer renderer)
	{
		if (_propertyBlocks.ContainsKey(renderer))
		{
			var block = _propertyBlocks[renderer];
			block.Clear();
			renderer.SetPropertyBlock(block);
			_propertyBlocks.Remove(renderer);
		}
	}
}