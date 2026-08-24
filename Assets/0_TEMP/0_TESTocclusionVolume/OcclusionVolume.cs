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
		UpdateAffectedObjects();
	}
#endif

#if UNITY_EDITOR
	Vector3 _lastPosition;
	Quaternion _lastRotation;
	Vector3 _lastScale;

	void Awake()
	{
		_lastPosition = transform.position;
		_lastRotation = transform.rotation;
		_lastScale = transform.localScale;
	}

	void Update() // Только в редакторе!
	{
		if (_collider == null || !enabled) return;

		bool changed =
			transform.position != _lastPosition ||
			transform.rotation != _lastRotation ||
			transform.localScale != _lastScale;

		if (changed)
		{
			UpdateAffectedObjects();

			_lastPosition = transform.position;
			_lastRotation = transform.rotation;
			_lastScale = transform.localScale;
		}
	}


#endif

	private void UpdateAffectedObjects()
	{
		var bounds = _collider.bounds;

		foreach (var renderer in FindObjectsOfType<MeshRenderer>())
		{
			bool isInside = renderer.bounds.Intersects(bounds);

			if (isInside)
			{
				ApplyAO(renderer, aoMultiplier);
				//Debug.Log($"Объект {renderer.name}: AO={aoMultiplier}");
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