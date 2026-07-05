using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class AutoRigLinker : MonoBehaviour
{
	[SerializeField] private GameObject parentRoot;
	[SerializeField] private List<GameObject> childRoots;

	private void Start()
	{
		Debug.Log("AutoRigLinker: Начало процесса связывания.");

		if (parentRoot == null)
		{
			Debug.LogError("AutoRigLinker: Parent Root не назначен!");
			return;
		}

		var parentBoneDict = GetAllChildrenDictionary(parentRoot.transform);

		for (int rootIndex = 0; rootIndex < childRoots.Count; rootIndex++)
		{
			var rootObj = childRoots[rootIndex];
			if (rootObj == null) continue;

			var bones = new List<Transform>();
			GetAllChildren(rootObj.transform, bones);

			Debug.Log($"AutoRigLinker: Обработка '{rootObj.name}'. Найдено костей: {bones.Count}.");

			foreach (var bone in bones)
			{
				Transform sourceBone = FindMatchingParentBone(bone.name, parentBoneDict);

				if (sourceBone == null)
				{
					Debug.LogWarning($"AutoRigLinker: Для кости '{bone.name}' не найдена соответствующая кость в Parent Root '{parentRoot.name}'. Компонент OverrideTransform добавлен не будет.");
					continue;
				}

				try
				{
					var existingComp = bone.GetComponent<OverrideTransform>();
					OverrideTransform overrideComp;

					if (existingComp != null)
					{
						overrideComp = existingComp;
					}
					else
					{
						overrideComp = bone.gameObject.AddComponent<OverrideTransform>();
					}

					overrideComp.data.constrainedObject = bone;
					overrideComp.data.sourceObject = sourceBone;
					overrideComp.data.positionWeight = 1;
					overrideComp.data.rotationWeight = 1;

					Debug.Log($"AutoRigLinker: Успешно привязано на '{bone.name}': Source -> '{sourceBone.name}'");
				}
				catch (System.Exception e)
				{
					Debug.LogError($"AutoRigLinker: Критическая ошибка при создании компонента на кости '{bone.name}'. Ошибка: {e.Message}");
				}
			}
		}

		Debug.Log("AutoRigLinker: Процесс завершен.");
	}

	private Dictionary<string, Transform> GetAllChildrenDictionary(Transform current)
	{
		var dict = new Dictionary<string, Transform>();
		GetAllChildrenRecursive(current, dict);
		return dict;
	}

	private void GetAllChildrenRecursive(Transform current, Dictionary<string, Transform> dict)
	{
		dict[current.name] = current;
		foreach (Transform child in current)
		{
			GetAllChildrenRecursive(child, dict);
		}
	}

	private Transform FindMatchingParentBone(string boneName, Dictionary<string, Transform> parentDict)
	{
		if (parentDict.TryGetValue(boneName, out Transform foundBone))
		{
			return foundBone;
		}
		return null;
	}

	private void GetAllChildren(Transform current, List<Transform> list)
	{
		list.Add(current);
		foreach (Transform child in current)
		{
			GetAllChildren(child, list);
		}
	}
}