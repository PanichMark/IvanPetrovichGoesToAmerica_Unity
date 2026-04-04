using System.Collections.Generic;
using UnityEngine;

public class CopyAnimation : MonoBehaviour
{
	public Transform parentArmatureRoot; // Родительская арматура
	public Transform childArmatureRoot; // Дочерняя арматура

	private Dictionary<string, Transform> childBonesMap = new Dictionary<string, Transform>();

	void Start()
	{
		BuildChildBonesMap();
	}

	void LateUpdate()
	{
		if (!parentArmatureRoot || !childArmatureRoot)
			return;

		// Применяем позицию и поворот только для корня
		childArmatureRoot.position = parentArmatureRoot.position;
		childArmatureRoot.rotation = parentArmatureRoot.rotation;

		// Синхронизация всех остальных костей
		foreach (Transform bone in parentArmatureRoot.GetComponentsInChildren<Transform>(true))
		{
			string name = bone.name;

			if (childBonesMap.TryGetValue(name, out var childBone))
			{
				childBone.localPosition = bone.localPosition;
				childBone.localRotation = bone.localRotation;

				// Если возникает проблема с масштабом, попробуйте отключить следующие строки:
				// childBone.localScale = bone.localScale;
			}
		}
	}

	private void BuildChildBonesMap()
	{
		foreach (var transform in childArmatureRoot.GetComponentsInChildren<Transform>(true))
		{
			childBonesMap.Add(transform.name, transform);
		}
	}
}