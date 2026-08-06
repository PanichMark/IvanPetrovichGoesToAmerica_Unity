using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class VolumeOcclusion : MonoBehaviour
{
	[SerializeField] private Collider _collider; // Коллайдер объёма
	[Range(0f, 1f)]
	public float aoMultiplier = 1f; // Степень затемнения AO

	private Dictionary<Renderer, MaterialPropertyBlock> _propertyBlocks = new();

	void OnEnable()
	{
		UpdateAffectedObjects();
	}

#if UNITY_EDITOR
	void LateUpdate()
	{
		if (!Application.isPlaying)
			UpdateAffectedObjects();
	}
#endif

	private void UpdateAffectedObjects()
	{
		var bounds = _collider.bounds;

		foreach (var renderer in FindObjectsOfType<MeshRenderer>())
		{
			bool isInside = renderer.bounds.Intersects(bounds);

			if (isInside && !_propertyBlocks.ContainsKey(renderer))
			{
				ApplyAO(renderer, aoMultiplier); // Применяем эффект через блок свойств
				Debug.Log($"Объект {renderer.name} попал под объём AO. Multiplier={aoMultiplier}");
			}
			else if (!isInside && _propertyBlocks.ContainsKey(renderer))
			{
				ResetAO(renderer); // Сбрасываем эффект
				Debug.Log($"Объект {renderer.name} вышел из объёма AO.");
			}
		}
	}

	private void ApplyAO(Renderer renderer, float value)
	{
		// Получаем или создаём блок свойств для этого рендерера
		if (!_propertyBlocks.TryGetValue(renderer, out var block))
		{
			block = new MaterialPropertyBlock();
			_propertyBlocks.Add(renderer, block);
		}

		// Устанавливаем значение AO напрямую в блоке
		block.SetFloat("_OcclusionStrength", value); // Для работы с AO
													 // Или: block.SetColor("_BaseColor", Color.Lerp(Color.white, Color.black, value)); // Для затемнения цветом
		renderer.SetPropertyBlock(block);
	}

	private void ResetAO(Renderer renderer)
	{
		if (_propertyBlocks.ContainsKey(renderer))
		{
			// Очищаем блок свойств у рендерера
			var block = _propertyBlocks[renderer];
			block.Clear(); // Удаляет все свойства блока
			renderer.SetPropertyBlock(block);

			// Можно удалить из словаря, если больше не нужен
			_propertyBlocks.Remove(renderer);
		}
	}
}