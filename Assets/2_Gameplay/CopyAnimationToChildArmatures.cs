using System.Collections.Generic;
using UnityEngine;

public class CopyAnimationToChildArmatures : MonoBehaviour
{
	public Transform parentArmatureRoot; // Родительская арматура
	public List<Transform> childArmatureRoots = new List<Transform>(); // Список дочерних арматур

	// Для каждого дочернего корня храним свою карту костей
	private List<Dictionary<string, Transform>> childBonesMaps = new List<Dictionary<string, Transform>>();

	void Start()
	{
		BuildChildBonesMaps();
	}

	void LateUpdate()
	{
		if (!parentArmatureRoot || childArmatureRoots.Count == 0)
			return;

		// Получаем все кости родительской арматуры
		Transform[] parentBones = parentArmatureRoot.GetComponentsInChildren<Transform>(true);

		// Проходим по каждому дочернему корню
		for (int i = 0; i < childArmatureRoots.Count; i++)
		{
			Transform childRoot = childArmatureRoots[i];
			Dictionary<string, Transform> childBonesMap = childBonesMaps[i];

			if (!childRoot)
				continue;

			// Копируем позицию и поворот корня
			childRoot.position = parentArmatureRoot.position;
			childRoot.rotation = parentArmatureRoot.rotation;

			// Синхронизируем каждую кость по имени
			foreach (Transform bone in parentBones)
			{
				string name = bone.name;
				if (childBonesMap.TryGetValue(name, out var childBone))
				{
					childBone.localPosition = bone.localPosition;
					childBone.localRotation = bone.localRotation;
					// childBone.localScale = bone.localScale; // Если нужно копировать масштаб
				}
			}
		}
	}

	private void BuildChildBonesMaps()
	{
		childBonesMaps.Clear();
		foreach (var childRoot in childArmatureRoots)
		{
			if (childRoot == null) continue;

			var bonesMap = new Dictionary<string, Transform>();
			foreach (var transform in childRoot.GetComponentsInChildren<Transform>(true))
			{
				bonesMap[transform.name] = transform;
			}
			childBonesMaps.Add(bonesMap);
		}
	}
}